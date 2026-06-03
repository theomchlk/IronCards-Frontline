using System.Collections;
using UnityEngine;

public abstract class Soldier : MonoBehaviour
{
    [Header("Data & Stats")]
    [SerializeField] private CardsSO data;
    [SerializeField] private HealthBar healthBar;
    public int ownerId;
    private int netId = -1;
    private float health;
    private Soldier target;
    private float activationTime;
    private float lastActionTime;
    private Animator animator;
    private Rigidbody[] ragdollRigidbodies;
    private Rigidbody mainRigidbody;
    private Collider[] ragdollColliders;
    private float lastMaterialChangeTime = 0f;
    private Color _playerColor = Color.white;
    private Material _materialInstance;
    private int groupId = -1;
    private SoldierState state = SoldierState.Idle;
    private Vector3 destination;
    private bool movementRequested;
    private bool isOwnerPlayer = false;

    private float aiSyncTimer;
    private const float aiSyncInterval = 0.1f;
    private Vector3 lastSentDestination;
    private bool lastSentMoving;

    private float actionCooldown;
    private float positionSyncTimer;
    private FightManager fightManager;

    // ==========================================
    // GETTERS
    // ==========================================

    public string GetName() => data.cardName;
    public float GetMaxHealth() => data.health;
    public float GetMoveSpeed() => data.movementSpeed;
    public float GetDamage() => data.damage;
    public float GetAttackSpeed() => data.attackSpeed;
    public float GetRange() => data.range;
    public Vector3 GetPosition() => transform.position;
    public AudioClip GetSound() => data.sound;
    public float GetHealth() => health;
    public float GetLastActionTime() => lastActionTime;
    public int GetOwnerId() => ownerId;
    public int GetNetId() => netId;
    public bool IsAlive() => health > 0;
    public Animator GetAnimator() => animator;
    public CombatActionSO GetCombatAction() => data.combatAction;
    public Soldier GetTarget() => target;
    public float GetSoundVolume() => data.soundVolume;
    public float GetArmorProtection() => data.armorProtection;
    public AudioClip GetProtectionSound() => data.protectionSound;

    public SoldierState GetState() => state;
    public bool IsOwnerPlayer => isOwnerPlayer;
    public FightManager GetFightManager() => fightManager;
    public int GetGroupId() => groupId;

    // ==========================================
    // ABSTRACT METHODS
    // ==========================================
    public abstract bool ChangeTargetCondition();

    public virtual bool CompareOwnerId(Soldier other)
    {
        return GetOwnerId() != other.GetOwnerId();
    }

    // ==========================================
    // SETTERS
    // ==========================================

    public void SetGroupId(int id) => groupId = id;

    public void SetFightManager(FightManager fm) => fightManager = fm;

    public void SetHealth(float value) => health = value;
    public void SetLastActionTime(float value) => lastActionTime = value;

    private void ConfigureAudio(AudioSource audioSource, AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.volume = GetSoundVolume();
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = 1.5f;
        audioSource.maxDistance = Mathf.Max(15f, GetRange() * 4f);
    }

    private void SetRagdollState(bool enabled)
    {
        foreach (Rigidbody rrb in ragdollRigidbodies)
        {
            rrb.isKinematic = !enabled;
        }

        foreach (Collider col in ragdollColliders)
        {
            if (col.gameObject != gameObject)
                col.enabled = enabled;
        }

        if (mainRigidbody != null)
            mainRigidbody.isKinematic = enabled;

        if (GetComponent<Collider>() != null)
            GetComponent<Collider>().enabled = !enabled;
    }

    private Material GetOrCreateMaterialInstance()
    {
        if (_materialInstance != null) return _materialInstance;
        SkinnedMeshRenderer r = GetComponentInChildren<SkinnedMeshRenderer>();
        if (r == null) return null;
        _materialInstance = new Material(r.sharedMaterial);
        r.material = _materialInstance;
        return _materialInstance;
    }

    public void SetPlayerColor(Color color)
    {
        _playerColor = color;
        SetDefaultMaterial();
    }

    public void SetDefaultMaterial()
    {
        Material mat = GetOrCreateMaterialInstance();
        if (mat == null) return;
        mat.color = _playerColor;
        mat.DisableKeyword("_EMISSION");
        lastMaterialChangeTime = Time.time;
    }

    public void SetBloomMaterial()
    {
        Material mat = GetOrCreateMaterialInstance();
        if (mat == null) return;
        mat.color = _playerColor;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", _playerColor * Mathf.Pow(2f, 4f));
        lastMaterialChangeTime = Time.time;
    }

    public void SetNetId(int id) => netId = id;

    public void SetOwnerId(int id)
    {
        ownerId = id;
        SetDefaultMaterial();
    }

    public void SetIsControlledByPlayer(bool value)
    {
        if (value)
        {
            state = SoldierState.PlayerControlled;
            StopMovementRigidbody();
            target = null;
        }
        else
        {
            state = SoldierState.Idle;
            StopMovementRigidbody();
        }
    }

    public void SetTarget(Soldier newTarget)
    {
        target = newTarget;
    }

    public void SetIsOwnerPlayer(bool value)
    {
        isOwnerPlayer = value;
    }

    // ==========================================
    // MÉTHODES UNITY
    // ==========================================

    private void Awake()
    {
        if (SoldierRegistry.Instance != null)
            SoldierRegistry.Instance.Register(this);

        activationTime = Time.time + 2f;
        animator = GetComponent<Animator>();
        health = GetMaxHealth();

        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        mainRigidbody = GetComponent<Rigidbody>();

        SetRagdollState(false);

        if (animator != null)
            animator.SetFloat("AttackSpeedMultiplier", 1/GetAttackSpeed());

        SetDefaultMaterial();
        RefreshHealthBar();
    }

    private void Start()
    {
        if (animator != null)
            animator.SetFloat("AttackSpeedMultiplier", 1f / GetAttackSpeed());
    }

    void Update()
    {
        if (lastMaterialChangeTime + 0.2f < Time.time) 
            SetDefaultMaterial();

        if (Time.time < activationTime || !IsAlive())
            return;

        if (!isOwnerPlayer) return;

        positionSyncTimer += Time.deltaTime;
        if (positionSyncTimer >= 2f)
        {
            positionSyncTimer = 0f;
            fightManager?.CmdSyncPosition(netId, transform.position, transform.rotation);
        }

        if (state == SoldierState.PlayerControlled)
            HandlePlayerBehavior();
        else
            HandleAIBehavior();
    }

    private void FixedUpdate()
    {
        if (Time.time < activationTime || !IsAlive() || !movementRequested || mainRigidbody == null)
            return;

        Vector3 direction = destination - transform.position;
        if (direction.sqrMagnitude < 0.0001f)
            return;

        direction.Normalize();
        mainRigidbody.MovePosition(mainRigidbody.position + direction * GetMoveSpeed() * Time.fixedDeltaTime);
        transform.LookAt(transform.position + direction);
    }

    // ==========================================
    // MÉTHODES
    // ==========================================

    private void HandleAIBehavior()
    {
        if (!ChangeTargetCondition())
        {
            RequestStop();
            RequestAction(target);
        }
        else
        {
            target = GetNearestTarget();
            if (target != null)
                RequestMoveTo(target.GetPosition());
            else
                RequestStop();
        }
    }

    private void RequestMoveTo(Vector3 dest)
    {
        aiSyncTimer += Time.deltaTime;
        bool destinationChanged = Vector3.Distance(dest, lastSentDestination) > 0.3f;
        bool intervalElapsed    = aiSyncTimer >= aiSyncInterval;

        if (!lastSentMoving || destinationChanged || intervalElapsed)
        {
            fightManager?.CmdMoveSoldier(netId, dest);
            lastSentDestination = dest;
            lastSentMoving      = true;
            aiSyncTimer         = 0f;
        }
    }

    private void RequestStop()
    {
        if (!lastSentMoving) return;
        fightManager?.CmdStopSoldier(netId);
        lastSentMoving = false;
        aiSyncTimer    = 0f;
    }

    private void HandlePlayerBehavior()
    {
        if (target == null)
        {
            float remainingDistance = Vector3.Distance(transform.position, destination);
            if (remainingDistance < 0.1f && movementRequested)
            {
                movementRequested = false;
                fightManager?.CmdStopSoldier(netId);
            }
        }
        else
        {
            if (IsInRange(target))
            {
                RequestStop();
                RequestAction(target);
            }
            else
            {
                RequestMoveTo(target.GetPosition());
            }
        }
    }

    private void RequestAction(Soldier target)
    {
        if (target == null || !target.IsAlive()) return;
        if (Time.time - actionCooldown < GetAttackSpeed()) return;
        actionCooldown = Time.time;
        fightManager?.CmdRequestAction(netId, target.GetNetId());
    }

    public void ExecuteNetworkAction(Soldier target)
    {
        if (!IsAlive()) return;
        SetLastActionTime(Time.time);

        StartCoroutine(DelayedActionTrigger());
        StartCoroutine(DelayedSound());

        StartCoroutine(DelayedVisualAction(target));

        if (isOwnerPlayer)
            StartCoroutine(DelayedNetworkAction(target));
    }

    private IEnumerator DelayedActionTrigger()
    {
        yield return null;
        if (!IsAlive()) yield break;

        if (movementRequested || lastSentMoving) yield break;

        Animator anim = GetAnimator();
        if (anim == null) yield break;

        anim.SetTrigger("Action");

        yield return new WaitForSeconds(GetAttackSpeed() + 0.1f);
        if (!IsAlive() || !movementRequested) yield break;

    }

    private IEnumerator DelayedVisualAction(Soldier target)
    {
        yield return new WaitForSeconds(GetAttackSpeed() - 0.1f);
        CombatActionSO combatAction = GetCombatAction();
        if (combatAction != null)
            combatAction.Execute(this.gameObject, target.gameObject);
    }

    protected virtual IEnumerator DelayedNetworkAction(Soldier target)
    {
        yield return new WaitForSeconds(GetAttackSpeed() - 0.1f);
        if (target == null || !target.IsAlive()) yield break;
        target.TakeDamage(this.gameObject, GetDamage());
    }

    public void SnapToPosition(Vector3 pos, Quaternion rot)
    {
        if (mainRigidbody == null || mainRigidbody.isKinematic) return;
        mainRigidbody.MovePosition(pos);
        mainRigidbody.MoveRotation(rot);
    }

    public void ApplyHealLocal(float amount)
    {
        if (!IsAlive()) return;
        SetHealth(Mathf.Min(GetHealth() + amount, GetMaxHealth()));
        RefreshHealthBar();
    }

    public virtual Soldier GetNearestTarget()
    {
        if (SoldierRegistry.Instance != null)
            return SoldierRegistry.Instance.GetNearestTarget(this);

        return null;
    }

    public bool IsInRange(Soldier target)
    {
        return Vector3.Distance(transform.position, target.GetPosition()) <= GetRange();
    }

    public void HandleMovementRigidbody(Vector3 destination)
    {
        if (!IsAlive())
            return;

        this.destination = destination;
        movementRequested = true;

        Vector3 direction = destination - transform.position;
        if (direction.sqrMagnitude > 0.0001f)
            direction.Normalize();

        animator.SetFloat("MoveX", Mathf.Abs(direction.x));
        animator.SetFloat("MoveZ", Mathf.Abs(direction.z));

        if (GetMoveSpeed() > 2f) {
            animator.SetBool("Running", true);
        } else {
            animator.SetBool("Walking", true);
        }
    }

    public void StopMovementRigidbody()
    {
        movementRequested = false;
        if (mainRigidbody != null && !mainRigidbody.isKinematic)
            mainRigidbody.linearVelocity = Vector3.zero;
        destination = transform.position;

        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveZ", 0);
        animator.SetBool("Walking", false);
        animator.SetBool("Running", false);
    }

    public void TakeDamage(GameObject source, float damage)
    {
        if (!source.GetComponent<Soldier>().IsOwnerPlayer) return;
        fightManager?.CmdApplyDamage(netId, damage);
    }

    public void ApplyDamageLocal(float damage, bool blocked)
    {
        if (blocked)
        {
            GameObject tempAudioObject = new GameObject("Protection Sound");
            tempAudioObject.transform.SetParent(transform);
            tempAudioObject.transform.position = transform.position;
            AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();
            ConfigureAudio(audioSource, GetProtectionSound());
            audioSource.Play();
            Destroy(tempAudioObject, GetProtectionSound().length / audioSource.pitch);
            return;
        }

        SetHealth(GetHealth() - damage);
        RefreshHealthBar();
        if (GetHealth() <= 0)
        {
            health = 0;
            DieLocal();
        }
    }

    private void DieLocal()
    {
        if (state == SoldierState.Dead) return;
        state = SoldierState.Dead;

        if (animator != null)
            animator.enabled = false;

        SetRagdollState(true);
        healthBar?.gameObject.SetActive(false);

        if (SoldierRegistry.Instance != null)
            SoldierRegistry.Instance.Unregister(this);

        fightManager.RemoveSoldierFromGroupId(groupId, this);
        Destroy(gameObject, 4f);
    }

    public void Heal(float amount)
    {
        if (!IsAlive())
            return;

        SetHealth(Mathf.Min(GetHealth() + amount, GetMaxHealth()));
        RefreshHealthBar();
    }

    private void RefreshHealthBar() => healthBar?.SetHealth(health, GetMaxHealth());

    public IEnumerator DelayedSound()
    {
        yield return new WaitForSeconds(GetAttackSpeed() - GetSound().length * GetAttackSpeed());

        GameObject tempAudioObject = new GameObject("Action Sound");
        tempAudioObject.transform.SetParent(transform);
        tempAudioObject.transform.position = transform.position;
        AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();
        audioSource.pitch = 1 / GetAttackSpeed();
        ConfigureAudio(audioSource, GetSound());
        audioSource.Play();

        Destroy(tempAudioObject, GetSound().length / audioSource.pitch);
    }

}
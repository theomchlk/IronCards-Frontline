using System.Collections;
using UnityEngine;

public abstract class Soldier : MonoBehaviour
{
    [Header("Data & Stats")]
    [SerializeField] private CardsSO data;
    public int ownerId;
    private float health;
    private Soldier target;
    private float activationTime;
    private float lastActionTime;
    private Animator animator;
    private Rigidbody[] ragdollRigidbodies;
    private Rigidbody mainRigidbody;
    private Collider[] ragdollColliders;
    private float lastMaterialChangeTime = 0f;
    private SoldierState state = SoldierState.Idle;
    private Vector3 destination;
    private bool movementRequested;

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
    public bool IsAlive() => health > 0;
    public Animator GetAnimator() => animator;
    public CombatActionSO GetCombatAction() => data.combatAction;
    public Soldier GetTarget() => target;
    public float GetSoundVolume() => data.soundVolume;
    public float GetArmorProtection() => data.armorProtection;
    public AudioClip GetProtectionSound() => data.protectionSound;
    private FightColorsSO GetMaterials() => data.fightColors;
    public SoldierState GetState() => state;

    // ==========================================
    // ABSTRACT METHODS
    // ==========================================
    public abstract void Action(Soldier target);
    public abstract bool ChangeTargetCondition();

    public virtual bool CompareOwnerId(Soldier other)
    {
        return GetOwnerId() != other.GetOwnerId();
    }

    // ==========================================
    // SETTERS
    // ==========================================

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

    public void SetMaterial(Material mat)
    {
        SkinnedMeshRenderer renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (renderer != null)
        {
            renderer.material = mat;
            lastMaterialChangeTime = Time.time;
        }
    }

    public void SetDefaultMaterial()
    {
        if (GetMaterials() != null && GetMaterials().colors.Length > ownerId)
        {
            SetMaterial(GetMaterials().colors[ownerId].material);
        }
    }

    public void SetBloomMaterial()
    {
        if (GetMaterials() != null && GetMaterials().colors.Length > ownerId)
        {
            SetMaterial(GetMaterials().colors[ownerId].bloomMaterial);
        }
    }

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
        }
    }

    public void SetTarget(Soldier newTarget)
    {
        target = newTarget;
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
        {
            animator.SetFloat("AttackSpeedMultiplier", 1 / GetAttackSpeed());
        }

        SetDefaultMaterial();
    }

    void Update()
    {
        if (lastMaterialChangeTime + 0.2f < Time.time) 
            SetDefaultMaterial();

        if (Time.time < activationTime || !IsAlive())
            return;

        if (state == SoldierState.PlayerControlled)
        {
            HandlePlayerBehavior();
        }
        else
        {
            HandleAIBehavior();
        }
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
            StopMovementRigidbody();
            Action(target);
        }
        else
        {
            target = GetNearestTarget(); 
            if (target != null)
                HandleMovementRigidbody(target.GetPosition());
            else 
                StopMovementRigidbody();
        }
    }

    private void HandlePlayerBehavior()
    {
        if (target == null)
        {
            float remainingDistance = Vector3.Distance(transform.position, destination);
            if (remainingDistance < 0.1f)
            {
                StopMovementRigidbody();
            } else {
                HandleMovementRigidbody(destination);
            }
        } 
        else
        {
            if (IsInRange(target))
            {
                StopMovementRigidbody();
                Action(target);
            } 
            else
            {
                HandleMovementRigidbody(target.GetPosition());
            }
        }
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
        if (mainRigidbody != null)
            mainRigidbody.linearVelocity = Vector3.zero;
        destination = transform.position;

        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveZ", 0);
        animator.SetBool("Walking", false);
        animator.SetBool("Running", false);
    }

    public void TakeDamage(Soldier source, float damage) {
        if (Random.value < GetArmorProtection())
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
        if (GetHealth() <= 0) {
            health = 0;
            Die();
        }
    }

    public void Die()
    {
        state = SoldierState.Dead;

        if (animator != null)
            animator.enabled = false;

        SetRagdollState(true);

        if (SoldierRegistry.Instance != null)
            SoldierRegistry.Instance.Unregister(this);

        Destroy(gameObject, 4f);
    }

    public void Heal(float amount)
    {
        if (!IsAlive())
            return;

        SetHealth(Mathf.Min(GetHealth() + amount, GetMaxHealth()));
    }

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
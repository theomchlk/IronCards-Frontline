using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class Soldier : MonoBehaviour
{
    [Header("Data & Stats")]
    [SerializeField] private CardsSO data;
    public int ownerId;
    private float health;
    private Soldier target;
    private float activationTime;
    private float lastActionTime;
    private Rigidbody rb;
    private Animator animator;
    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;
    private NavMeshAgent agent;
    private float lastDestinationUpdateTime = 0f;

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

    private void SetRagdollState(bool state)
    {
        foreach (Rigidbody rrb in ragdollRigidbodies)
        {
            rrb.isKinematic = !state;
        }

        foreach (Collider col in ragdollColliders)
        {
            if (col.gameObject != gameObject)
                col.enabled = state;
        }

        if (GetComponent<Rigidbody>() != null)
            GetComponent<Rigidbody>().isKinematic = state;

        if (GetComponent<Collider>() != null)
            GetComponent<Collider>().enabled = !state;
    }

    // ==========================================
    // MÉTHODES UNITY
    // ==========================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        activationTime = Time.time + 2f;
        animator = GetComponent<Animator>();
        health = GetMaxHealth();

        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        SetRagdollState(false);

        if (agent != null)
        {
            agent.speed = GetMoveSpeed();
            agent.stoppingDistance = GetRange();
            agent.updateRotation = true; 
        }

        if (animator != null)
        {
            animator.SetFloat("AttackSpeedMultiplier", 1 / GetAttackSpeed());
        }
    }

    void Update()
    {
        if (Time.time < activationTime || !IsAlive())
            return;

        if (!ChangeTargetCondition())
        {
            StopMovement();
            Action(target);
        }
        else
        {
            target = GetNearestTarget();
        }
    }

    void FixedUpdate()
    {
        if (rb != null && target != null && !IsInRange(target))
        {
            HandleMovement();
            return;
        }

        StopMovement();
    }

    // ==========================================
    // MÉTHODES
    // ==========================================

    public Soldier GetNearestTarget()
    {
        Soldier[] soldiers = FindObjectsByType<Soldier>(FindObjectsSortMode.None);
        Soldier nearestTarget = null;
        float shortestDistance = float.MaxValue;

        foreach (Soldier s in soldiers)
        {
            if (s != this && s.IsAlive() && CompareOwnerId(s))
            {
                float distance = Vector3.Distance(transform.position, s.GetPosition());
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestTarget = s;
                }
            }
        }

        return nearestTarget;
    }

    public bool IsInRange(Soldier target)
    {
        return Vector3.Distance(transform.position, target.GetPosition()) <= GetRange();
    }

    private void HandleMovement()
    {
        if (target != null && target.IsAlive())
        {
            if (lastDestinationUpdateTime + 0.5f > Time.time)
                return;

            agent.SetDestination(target.GetPosition());
            lastDestinationUpdateTime = Time.time;

            if (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
            {
                Vector3 direction = (target.GetPosition() - transform.position).normalized;
                animator.SetFloat("MoveX", Mathf.Abs(direction.x));
                animator.SetFloat("MoveZ", Mathf.Abs(direction.z));
                if (GetMoveSpeed() > 2f) {
                    animator.SetBool("Running", true);
                } else {
                    animator.SetBool("Walking", true);
                }

            }
            else if (!agent.pathPending)
            {
                StopMovement();
            }
        }
        else StopMovement();
    }

    private void StopMovement()
    {
        if (agent.isOnNavMesh)
        {
            agent.ResetPath();
        }
        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveZ", 0);
        animator.SetBool("Walking", false);
        animator.SetBool("Running", false);
    }

    public void TakeDamage(Soldier source, float damage) {
        if (Random.value < GetArmorProtection())
        {
            GameObject tempAudioObject = new GameObject("Protection Sound");
            tempAudioObject.transform.position = transform.position;
            AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();
            ConfigureAudio(audioSource, GetProtectionSound());
            audioSource.Play();
            Destroy(tempAudioObject, GetProtectionSound().length / audioSource.pitch);
            return;
        }

        SetHealth(GetHealth() - damage);
        if (GetHealth() <= 0) {
            Die();
        }
    }

    public void Die()
    {
        if (animator != null)
            animator.enabled = false;

        SetRagdollState(true);
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
        tempAudioObject.transform.position = transform.position;
        AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();
        audioSource.pitch = 1 / GetAttackSpeed();
        ConfigureAudio(audioSource, GetSound());
        audioSource.Play();

        Destroy(tempAudioObject, GetSound().length / audioSource.pitch);
    }
}
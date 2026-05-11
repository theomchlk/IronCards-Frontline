using System.Collections;
using System.Collections.Generic;
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
    private Animator animator;
    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;
    private NavMeshAgent agent;
    private float lastMaterialChangeTime = 0f;
    private bool isControlledByPlayer = false;
    private static List<Soldier> allSoldiers = new List<Soldier>();
    private float nextPathUpdateTime = 0f;

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
        isControlledByPlayer = value;
        if (isControlledByPlayer)
        {
            StopMovement();
            target = null;
            agent.stoppingDistance = 0.1f;
        } else {
            agent.stoppingDistance = GetRange() * 0.8f;
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
        allSoldiers.Add(this);

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
            agent.stoppingDistance = GetRange() * 0.8f;
            agent.updateRotation = false; 
        }

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

        if (isControlledByPlayer)
        {
            HandlePlayerBehavior();
        }
        else
        {
            HandleAIBehavior();
        }
    }

    // ==========================================
    // MÉTHODES
    // ==========================================

    private void HandleAIBehavior()
    {
        if (!ChangeTargetCondition())
        {
            StopMovement();
            Action(target);
        }
        else
        {
            target = GetNearestTarget(); 
            if (target != null)
                HandleMovement(target.GetPosition());
            else 
                StopMovement();
        }
    }

    private void HandlePlayerBehavior()
    {
        if (target == null)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
                StopMovement();
        } 
        else
        {
            if (IsInRange(target))
            {
                StopMovement();
                Action(target);
            } 
            else
            {
                HandleMovement(target.GetPosition());
            }
        }
    }

    public Soldier GetNearestTarget()
    {
        Soldier nearestTarget = null;
        float shortestDistance = float.MaxValue;

        foreach (Soldier s in allSoldiers)
        {
            if (s != this && s.IsAlive() && CompareOwnerId(s))
            {
                float distance = (transform.position - s.GetPosition()).sqrMagnitude;
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

    public void HandleMovement(Vector3 destination)
    {
        agent.SetDestination(destination);

        Vector3 direction = (destination - transform.position).normalized;
        
        transform.LookAt(transform.position + direction);

        animator.SetFloat("MoveX", Mathf.Abs(direction.x));
        animator.SetFloat("MoveZ", Mathf.Abs(direction.z));
        
        if (GetMoveSpeed() > 2f) {
            animator.SetBool("Running", true);
        } else {
            animator.SetBool("Walking", true);
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StopMovement();
        }
    }

    public void StopMovement()
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
            health = 0;
            Die();
        }
    }

    public void Die()
    {
        if (animator != null)
            animator.enabled = false;

        if (agent != null)
            agent.enabled = false;

        SetRagdollState(true);
        allSoldiers.Remove(this);
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
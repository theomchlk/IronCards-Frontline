using System.Collections;
using UnityEngine;

public abstract class Soldier : MonoBehaviour
{
    [Header("Data & Stats")]
    [SerializeField] private CardsSO data;
    [SerializeField] private int ownerId;    
    private float health;
    private Soldier target;
    private float activationTime;
    private float lastActionTime;
    
    private Animator animator;
    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;

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

    // ==========================================
    // SETTERS
    // ==========================================

    public void SetHealth(float value) => health = value;
    public void SetLastActionTime(float value) => lastActionTime = value;

    private void SetRagdollState(bool state)
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = !state;
        }
        foreach (Collider col in ragdollColliders)
        {
            if (col.gameObject != gameObject) 
                col.enabled = state;
        }

        // Gestion du Rigidbody et Collider principaux
        if (GetComponent<Rigidbody>() != null)
            GetComponent<Rigidbody>().isKinematic = state;
        
        // Note: Assure-toi d'avoir un BoxCollider ou CapsuleCollider sur le root
        if (GetComponent<Collider>() != null)
            GetComponent<Collider>().enabled = !state;
    }

    // ==========================================
    // MÉTHODES DE LOGIQUE & UNITY
    // ==========================================

    private void Awake()
    {
        activationTime = Time.time + 2f;
        animator = GetComponent<Animator>();
        health = GetMaxHealth();

        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        SetRagdollState(false);
    }

    void Update()
    {
        if (Time.time < activationTime || !IsAlive())
            return;

        if (target != null && IsInRange(target)) 
        {
            if (animator != null) 
            {
                animator.SetFloat("MoveX", 0);
                animator.SetFloat("MoveZ", 0);
                animator.SetBool("Walking", false);
            }
            Action(target);
        } 
        else 
        {
            target = GetNearestTarget();
            if (target != null) 
            {
                Move(target.GetPosition());
            }
        }
    }

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

    public virtual bool CompareOwnerId(Soldier other)
    {
        return GetOwnerId() != other.GetOwnerId();
    }

    public bool IsInRange(Soldier target)
    {
        return Vector3.Distance(transform.position, target.GetPosition()) <= GetRange();
    }

    public abstract void Action(Soldier target);

    public abstract void TakeDamage(Soldier source, float damage);

    public void Move(Vector3 destination) 
    {
        Vector3 direction = (destination - transform.position).normalized;
        Vector3 flatDir = new Vector3(direction.x, 0f, direction.z);

        if (flatDir.sqrMagnitude > 0.0001f) 
        {
            transform.rotation = Quaternion.LookRotation(flatDir);
        }

        if (animator != null) 
        {
            animator.SetFloat("MoveX", Mathf.Abs(direction.x));
            animator.SetFloat("MoveZ", Mathf.Abs(direction.z));
            animator.SetBool("Walking", true);
        }

        transform.position += direction * GetMoveSpeed() * Time.deltaTime;
    }

    public void Die() 
    {
        if (animator != null) 
            animator.enabled = false;

        SetRagdollState(true);
        Destroy(gameObject, 10f);
    }

    public void Heal(float amount) 
    {
        if (!IsAlive())
            return;

        SetHealth(Mathf.Min(GetHealth() + amount, GetMaxHealth()));
    }

    public IEnumerator DelayedSound() {
        yield return new WaitForSeconds(GetAttackSpeed() - GetSound().length*GetAttackSpeed());
        
        GameObject tempAudioObject = new GameObject("TempAudio");
        tempAudioObject.transform.position = transform.position;
        AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();
        audioSource.clip = GetSound();
        audioSource.pitch = 1/GetAttackSpeed();
        audioSource.Play();
        
        Destroy(tempAudioObject, GetSound().length / audioSource.pitch);
    }
}
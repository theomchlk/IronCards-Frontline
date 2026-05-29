using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LobbySoldier : MonoBehaviour
{
    private CardsSO data;
    public int ownerId;
    private float health;
    private LobbySoldier target;
    private float activationTime;
    private float lastActionTime;
    private Animator animator;
    private Rigidbody[] ragdollRigidbodies;
    private Rigidbody mainRigidbody;
    private Collider[] ragdollColliders;
    private float lastMaterialChangeTime = 0f;
    private static List<LobbySoldier> allSoldiers = new List<LobbySoldier>();
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
    public LobbySoldier GetTarget() => target;
    public float GetSoundVolume() => data.soundVolume;
    public float GetArmorProtection() => data.armorProtection;
    public AudioClip GetProtectionSound() => data.protectionSound;
    private FightColorsSO GetMaterials() => data.fightColors;

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

        if (mainRigidbody != null)
            mainRigidbody.isKinematic = state;

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

    public void SetTarget(LobbySoldier newTarget)
    {
        target = newTarget;
    }

    public void bind(CardsSO data)
    {
        this.data = data;
    
        allSoldiers.Add(this);

        activationTime = Time.time + 1f;
        animator = GetComponent<Animator>();
        health = GetMaxHealth();

        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        mainRigidbody = GetComponent<Rigidbody>();

        SetRagdollState(false);

        SetDefaultMaterial();
    }

    // ==========================================
    // MÉTHODES UNITY
    // ==========================================

    void Start()
    {
        if (animator != null)
        {
            animator.SetFloat("AttackSpeedMultiplier", 1 / GetAttackSpeed());
        }
    }

    void Update()
    {
        if (lastMaterialChangeTime + 0.2f < Time.time) 
            SetDefaultMaterial();

        if (Time.time < activationTime || !IsAlive())
            return;

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
        if (ActionOnTargetConditions())
        {

            StopMovementRigidbody();
            Action(target);
        }
        else
        {
            if (target == null || !target.IsAlive())
                target = GetNearestTarget();
            
            if (target != null && target.IsAlive())
                HandleMovementRigidbody(target.GetPosition());
            else 
                StopMovementRigidbody();
        }
    }

    public LobbySoldier GetNearestTarget()
    {
        LobbySoldier nearestTarget = null;
        float shortestDistance = float.MaxValue;

        foreach (LobbySoldier s in allSoldiers)
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

    public bool IsInRange(LobbySoldier target)
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

    public void TakeDamage(LobbySoldier source, float damage) {
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
        if (animator != null)
            animator.enabled = false;

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
        tempAudioObject.transform.SetParent(transform);
        tempAudioObject.transform.position = transform.position;
        AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();
        audioSource.pitch = 1 / GetAttackSpeed();
        ConfigureAudio(audioSource, GetSound());
        audioSource.Play();

        Destroy(tempAudioObject, GetSound().length / audioSource.pitch);
    }

    public void Action(LobbySoldier target) {
        if (target == null || !target.IsAlive() || !IsInRange(target) || GetOwnerId() == target.GetOwnerId())
            return;
        
        if (IsInRange(target) && target.IsAlive() && Time.time >= GetLastActionTime() + GetAttackSpeed()) {
            SetLastActionTime(Time.time);

            Animator animator = GetAnimator();
            if (animator != null) {
                animator.SetTrigger("Action");
            }


            StartCoroutine(DelayedSound());
            StartCoroutine(DelayedDamage(target));
        }
    }

    private IEnumerator DelayedDamage(LobbySoldier target) {
        yield return new WaitForSeconds(GetAttackSpeed()-0.1f);
        
        CombatActionSO action = GetCombatAction();
        if (action != null) {
            action.Execute(gameObject, target.gameObject);
        }
        if (IsInRange(target))
        {
            target.TakeDamage(this, GetDamage());
        }
    }

    public bool ActionOnTargetConditions()
    {
        return GetTarget() != null && GetTarget().IsAlive() && IsInRange(GetTarget());
    }

    public bool CompareOwnerId(LobbySoldier other)
    {
        return GetOwnerId() != other.GetOwnerId();
    }
    
}

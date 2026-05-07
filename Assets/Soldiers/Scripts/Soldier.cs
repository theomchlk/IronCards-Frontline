using Unity.VisualScripting;
using UnityEngine;

public abstract class Soldier : MonoBehaviour
{
    [SerializeField] private CardsSO data;
    private float health;
    private Soldier target;
    private float activationTime;
    private float lastActionTime;
    [SerializeField] private int ownerId;
    [SerializeField] private AudioClip sound;
    private Animator animator;




    // Getters
    public string GetName() { return data.cardName; }
    public float GetMaxHealth() { return data.health; }
    public float GetMoveSpeed() { return data.movementSpeed; }
    public float GetDamage() { return data.damage; }
    public float GetAttackSpeed() { return data.attackSpeed; }
    public float GetRange() { return data.range; }
    public Vector3 GetPosition() { return transform.position; }


    public float GetHealth() { return health; }
    public void SetHealth(float value) { health = value; }

    public float GetLastActionTime() { return lastActionTime; }
    public void SetLastActionTime(float value) { lastActionTime = value; }

    public Soldier GetNearestTarget()
    {
        Soldier[] soldiers = FindObjectsByType<Soldier>(FindObjectsSortMode.None);
        Soldier nearestTarget = null;
        float shortestDistance = float.MaxValue;

        foreach (Soldier s in soldiers)
        {
            if (s != this && s.CompareOwnerId(this))
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


    public int GetOwnerId() { return ownerId; }

    public abstract bool CompareOwnerId(Soldier other);
   

    // Setters
    public void SetPosition(Vector3 position) { 
        transform.position = position; 
    }


    public abstract void TakeDamage(Soldier source, float damage);


    public void Heal(float amount) {
        health = Mathf.Min(health + amount, GetMaxHealth());
    }


    // Methods
    public bool IsInRange(Soldier target) {
        return Vector3.Distance(transform.position, target.GetPosition()) <= GetRange();
    }


    public abstract void Action(Soldier target);


    public void Move(Vector3 destination) {
        Vector3 direction = (destination - transform.position).normalized;

        Vector3 flatDir = new Vector3(direction.x, 0f, direction.z);
        if (flatDir.sqrMagnitude > 0.0001f) {
            transform.rotation = Quaternion.LookRotation(flatDir);
        }

        if (animator != null) {
            animator.SetFloat("MoveX", Mathf.Abs(direction.x));
            animator.SetFloat("MoveZ", Mathf.Abs(direction.z));
            animator.SetBool("Walking", true);
        }

        transform.position += direction * GetMoveSpeed() * Time.deltaTime;
    }


    public void Die() {
        Destroy(gameObject);
    }


    private void Awake()
    {
        activationTime = Time.time + 2f;
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        if (Time.time < activationTime)
            return;

        if (target != null && IsInRange(target)) {
            if (animator != null) {
                animator.SetFloat("MoveX", 0);
                animator.SetFloat("MoveZ", 0);
                animator.SetBool("Walking", false);
            }
            Action(target);
        } else {
            target = GetNearestTarget();
            if (target != null) {
                Move(target.GetPosition());
            }
        }
    }

}

using UnityEngine;

public class Attacker : Soldier
{
    // Attributes
    [SerializeField] private string soldierName;
    [SerializeField] private int maxHealth;
    [SerializeField] private int health;
    [SerializeField] private double moveSpeed;
    [SerializeField] private int damage;
    [SerializeField] private double attackSpeed;
    [SerializeField] private double range;
    [SerializeField] private TargetType targetType;
    [SerializeField] private AudioClip sound;

    private Soldier target;

    // Getters
    public override string GetName() { return soldierName; }

    public override int GetMaxHealth() { return maxHealth; }

    public override int GetHealth() { return health; }

    public override double GetMoveSpeed() { return moveSpeed; }

    public override int GetDamage() { return damage; }

    public override double GetAttackSpeed() { return attackSpeed; }

    public override double GetRange() { return range; }

    public override bool IsAlive() { return health > 0; }

    public override Soldier GetTarget() { return target; }

    public override Vector3 GetPosition() { return transform.position; }

    public override TargetType GetTargetType() { return targetType; }

    public override Soldier GetNearestTarget()
    {
        if (targetType == TargetType.Enemy)
        {
            //Trouver un moyen de récupérer l'ennemi le plus proche (Surement en utilisant une liste des ennemis dans la scène)
        }
        else
        {
            //Trouver un moyen de récupérer l'allié le plus proche (Surement en utilisant une liste des alliés dans la scène)
        }
        return null; //temporaire
    }


    // Setters
    public override void SetPosition(Vector3 position) { 
        transform.position = position; 
    }

    public override void TakeDamage(Soldier source, int damage) {
        health -= damage;
        if (health <= 0) {
            Die();
        }
    }

    public override void Heal(int amount) {
        health = Mathf.Min(health + amount, maxHealth);
    }

    // Methods
    public override bool IsInRange(Soldier target) {
        return Vector3.Distance(transform.position, target.GetPosition()) <= range;
    }

    public override void Action(Soldier target) {
        if (IsInRange(target)) {
            AudioSource.PlayClipAtPoint(sound, transform.position);
            target.TakeDamage(this, damage);
        }
    }


    public override void Move(Vector3 destination) {
        Vector3 direction = (destination - transform.position).normalized;
        transform.position += direction * (float)moveSpeed * Time.deltaTime;
    }

    public override void Die() {
        //Detruire l'objet ou le faire ragdoll
    }

    void Update()
    {
        if (target != null && IsInRange(target)) {
            Action(target);
        } else {
            target = GetNearestTarget();
            if (target != null) {
                Move(target.GetPosition());
            }
        }
    }
}
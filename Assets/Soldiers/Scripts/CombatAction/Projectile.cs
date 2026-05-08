using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Soldier target;
    private Soldier source;
    private float speed;
    private float lifetime;

    public void Launch(Soldier target, Soldier source, float speed, float lifetime)
    {
        this.target = target;
        this.source = source;
        this.speed = speed;
        this.lifetime = lifetime;
        transform.LookAt(target.GetPosition() + Vector3.up); 
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (target == null) { Destroy(gameObject); return; }

        Vector3 targetPos = target.GetPosition() + Vector3.up; 
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.2f)
        {
            Destroy(gameObject);
        }
    }
}
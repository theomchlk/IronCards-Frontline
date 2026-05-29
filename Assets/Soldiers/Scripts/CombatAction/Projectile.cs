using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject target;
    private float speed;
    private float lifetime;

    public void Launch(GameObject target, float speed, float lifetime)
    {
        this.target = target;
        this.speed = speed;
        this.lifetime = lifetime;
        transform.LookAt(target.transform.position + Vector3.up); 
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (target == null) { Destroy(gameObject); return; }

        Vector3 targetPos = target.transform.position + Vector3.up; 
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        transform.LookAt(targetPos);

        if (Vector3.Distance(transform.position, targetPos) < 0.2f)
        {
            Destroy(gameObject);
        }
    }
}
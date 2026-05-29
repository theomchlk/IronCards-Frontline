using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Action", menuName = "Fight/Projectile Action")]
public class ProjectileActionSO : CombatActionSO
{
    public GameObject projectilePrefab;
    public float launchHeight = 1.5f;
    public float projectileSpeed;
    public float projectileLifetime;

    public override void Execute(GameObject source, GameObject target)
    {
        if (projectilePrefab == null) return;

        Vector3 spawnPos = source.transform.position + Vector3.up * launchHeight;
        GameObject projGo = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        
        Projectile proj = projGo.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.Launch(target.gameObject, projectileSpeed, projectileLifetime);
        }
    }

}
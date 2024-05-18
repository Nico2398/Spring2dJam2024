using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyProjectileThrower : MonoBehaviour
{
    [SerializeField] float cooldown = 1.5f;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform firePosition;

    float currentCooldown = 0f;

    private void FixedUpdate()
    {
        currentCooldown = Mathf.Max(currentCooldown - Time.fixedDeltaTime, 0f);
    }

    public void Fire(Vector3 targetPosition)
    {
        if (currentCooldown > 0f)
            return;

        Vector2 direction = targetPosition - transform.position;
        GameObject projectile = Instantiate(projectilePrefab, firePosition.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null )
            rb.velocity = direction.normalized * projectileSpeed;
        currentCooldown = cooldown;
    }
}

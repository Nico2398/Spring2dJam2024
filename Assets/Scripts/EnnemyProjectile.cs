using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class EnnemyProjectile : MonoBehaviour
{
    public float knockbackFactor = .5f;
    public float duration = 10f;
    private CircleCollider2D col;

    private void Awake()
    {
        col = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null && rb.velocity.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void FixedUpdate()
    {
        duration -= Time.deltaTime;
        if (duration <= 0f)
            DestroyProjectile();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController controller = collision.gameObject.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.ReceiveDamages(transform.position, knockbackFactor);
            }
            DestroyProjectile();
        }
        else if (collision.gameObject.CompareTag("GroundOrWall"))
        {
            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        col.enabled = false;
        GameObject.Destroy(gameObject, 0f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class EnnemyProjectile : MonoBehaviour
{
    public float knockbackFactor = .5f;
    private CircleCollider2D col;

    private void Awake()
    {
        col = GetComponent<CircleCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController controller = collision.gameObject.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.ReceiveDamages(transform.position, knockbackFactor);
            }
        }
        col.enabled = false;
        GameObject.Destroy(gameObject, 0f);
    }
}

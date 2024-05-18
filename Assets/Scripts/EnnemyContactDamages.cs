using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyContactDamages : MonoBehaviour
{
    [SerializeField][Range(0f, 1f)] float knockbackFactor= 1.0f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController controller = collision.gameObject.GetComponent<PlayerController>();
            if (controller != null)
                controller.ReceiveDamages(transform.position, knockbackFactor);
        }
    }
}

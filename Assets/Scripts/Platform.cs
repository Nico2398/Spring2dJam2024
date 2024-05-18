using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Platform : MonoBehaviour
{
    [SerializeField] BoxCollider2D boxCollider;

    public void SetEnabledPlatform(bool enabled)
    {
        boxCollider.enabled = enabled;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Platform collision enter");
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController controller = collision.gameObject.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.SetPlatform(this);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController controller = gameObject.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.SetPlatform(null);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SetEnabledPlatform(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SetEnabledPlatform(true);
        }
    }
}

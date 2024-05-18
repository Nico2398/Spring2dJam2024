using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sunbath : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController controller = collision.gameObject.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.SetSunbathAvailable(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController controller = collision.gameObject.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.SetSunbathAvailable(false);
            }
        }
    }
}

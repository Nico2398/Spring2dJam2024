using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxGlove : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController controller = collision.gameObject.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.UnlockPunch();
                gameObject.SetActive(false);
            }
        }
    }
}

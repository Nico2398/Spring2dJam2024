using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPunch : MonoBehaviour
{
    private int countdown;

    public void Punch()
    {
        gameObject.SetActive(true);
        countdown = 1;
    }

    private void FixedUpdate()
    {
        // This object should be enabled only for one fixed update, to inflict damages only once
        if (countdown == 0)
        {
            gameObject.SetActive (false);
        }
        countdown--;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ennemy"))
        {
            EnnemyHealth ennemy = collision.gameObject.GetComponent<EnnemyHealth>();
            if (ennemy != null)
            {
                ennemy.ReceiveDamage();
            }
        }
    }
}

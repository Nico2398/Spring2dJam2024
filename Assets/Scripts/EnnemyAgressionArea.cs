using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyAgressionArea : MonoBehaviour
{
    [SerializeField] EnnemyProjectileThrower thrower;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && thrower != null)
        {
            thrower.Fire(collision.transform.position);
        }
            
    }
}

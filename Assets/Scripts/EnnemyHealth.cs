using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class EnnemyHealth : MonoBehaviour
{
    [SerializeField] int defaultHealth = 3;

    int health = 3;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Init();
    }

    public void ReceiveDamage()
    {
        health -= 1;
        if (rb != null)
            rb.velocity = Vector2.zero;
        if (health <= 0)
            gameObject.SetActive(false);
    }

    public void Init()
    {
        health = defaultHealth;
        gameObject.SetActive(true);
    }
}

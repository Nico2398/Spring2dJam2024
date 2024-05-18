using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnnemyMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed = 10f;
    [SerializeField] float areaWidth = 5f;

    Rigidbody2D rb;
    float direction = 1f;

    float maxX = 0f;
    float minX = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        maxX = transform.position.x + areaWidth / 2;
        minX = transform.position.x - areaWidth / 2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.x > maxX)
            direction = -1f;
        else if (transform.position.x < minX)
            direction = 1f;

        Vector2 velocity = rb.velocity;
        velocity.x = direction * movementSpeed;
        rb.velocity = velocity;
    }
}

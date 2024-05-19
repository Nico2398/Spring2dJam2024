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
    Vector3 initialScale = Vector3.zero;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        maxX = transform.position.x + areaWidth / 2;
        minX = transform.position.x - areaWidth / 2;
        initialScale = transform.localScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.x > maxX)
            direction = -1f;
        else if (transform.position.x < minX)
            direction = 1f;

        transform.localScale = Vector3.Scale(initialScale, new Vector3(-direction, 1, 1));
        Vector2 velocity = rb.velocity;
        velocity.x = direction * movementSpeed;
        rb.velocity = velocity;
    }
}

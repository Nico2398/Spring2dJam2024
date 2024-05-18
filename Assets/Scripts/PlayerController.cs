using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // Components
    Rigidbody2D rb;

    // Fields
    [SerializeField] float movementSpeed = 10f;
    [SerializeField] float jumpVelocity = 20f;
    [SerializeField] float knockbackVelocity = 20f;
    [SerializeField] float knockbackMaximumVerticalVelocity = 10f;
    [SerializeField][Range(0.95f, 1f)] float movementLagGround = 0.995f;
    [SerializeField][Range(0.95f, 1f)] float movementLagAir = 0.999f;
    [SerializeField][Range(0f, 1f)] float gravityReductionOnJump = .25f;
    [SerializeField][Range(0f, 1f)] float gravityReductionOnPushingWall = .8f;
    [SerializeField][Range(0f, 1f)] float gravityAugmentOnDown = .25f;
    [SerializeField] int defaultHealth = 3;
    [SerializeField][Range(0f, 3f)] float invincibilityDuration = 1f;

    // Player inputs
    float movement = 0f;
    bool holdingJump = false;
    bool pendingJump = false;
    bool sunbath = false;
    bool down = false;
    bool punch = false;
    bool fire = false;

    // Gameplay
    bool isOnGround = false;
    float initialGravityScale = 1f;
    int health = 1;
    float invincibilityCooldown = 0f;
    bool isPushingWallLeft = false;
    bool isPushingWallRight = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        initialGravityScale = rb.gravityScale;
        health = defaultHealth;
    }

    void FixedUpdate()
    {
        Debug.Log("" + isPushingWallLeft + isPushingWallRight);
        UpdateVelocity(Time.fixedDeltaTime);

        bool isPushingWall = isPushingWallLeft || isPushingWallRight;
        float gravityFactor =
            isPushingWall ? 1 - gravityReductionOnPushingWall : 
            holdingJump ? 1f - gravityReductionOnJump : 
            down ? 1f + gravityAugmentOnDown :
                1f;
        rb.gravityScale = initialGravityScale * gravityFactor;

        invincibilityCooldown = Mathf.Max(invincibilityCooldown - Time.fixedDeltaTime, 0f);

        if (rb.velocity.magnitude > .0001f)
            isOnGround = false;
        isPushingWallLeft = false;
        isPushingWallRight = false;
    }

    private void UpdateVelocity(float deltaTime)
    {
        float movementLag = isOnGround ? movementLagGround : movementLagAir;

        // Horizontal velocity follows an arithmetico-geometric sequence formula Un+1 = a*Un+b, which converges towards r
        // See more here (explainations in french): https://www.desmos.com/calculator/4gbjauqhf7
        Vector2 velocity = rb.velocity;

        float r = movement * movementSpeed;
        float a = Mathf.Pow(movementLag, 1f / deltaTime);
        float b = r * (1 - a);

        velocity.x = a * velocity.x + b;
        if (isOnGround && pendingJump)
        {
            velocity.y = jumpVelocity;
            pendingJump = false;

            if (isPushingWallRight)
                velocity.x = -movementSpeed;
            else if (isPushingWallLeft)
                velocity.x = movementSpeed;
        }

        rb.velocity = velocity;
    }

    public void Move(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<float>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && isOnGround)
            pendingJump = true;

        holdingJump = context.ReadValue<float>() > 0f;
    }

    public void Sunbath(InputAction.CallbackContext context)
    {
        sunbath = context.ReadValue<float>() > 0f;
    }

    public void Down(InputAction.CallbackContext context)
    {
        down = context.ReadValue<float>() > 0f;
    }

    public void Punch(InputAction.CallbackContext context)
    {
        punch = context.ReadValue<float>() > 0f;
    }

    public void Fire(InputAction.CallbackContext context)
    {
        fire = context.ReadValue<float>() > 0f;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Note that OnCollisionXX is called once per FixedUpdate per collider (called after FixedUpdate).
        // See https://docs.unity3d.com/Manual/ExecutionOrder.html

        if (collision.gameObject.CompareTag("GroundOrWall") && rb.velocity.y <= 0.001)
        {
            isOnGround = true;
            float colNormalX = collision.GetContact(0).normal.x;
            // if collision pushes player horizontally and is opposite to player direction
            if (Mathf.Abs(colNormalX) > .5f && movement * colNormalX < 0)
            {
                if (movement > 0)
                    isPushingWallRight = true;
                else if (movement < 0)
                    isPushingWallLeft = true;
            }
        }
    }

    public void ReceiveDamages(Vector3 damageSourcePosition, float knockbackFactor)
    {
        if (invincibilityCooldown > 0f)
            return;
        health -= 1;
        Debug.Log("lifes: " + health);
        invincibilityCooldown = invincibilityDuration;
        Vector2 knockbackDirection = transform.position - damageSourcePosition;
        Vector2 knockback = knockbackDirection.normalized * knockbackVelocity * knockbackFactor;
        knockback.y = Mathf.Min(knockbackMaximumVerticalVelocity, knockback.y);
        rb.velocity = knockback;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField] PlayerPunch playerPunch;
    [SerializeField] Animator animator;
    [SerializeField] List<GameObject> UiHealthImages;
    [SerializeField] List<GameObject> UiMaxHealthImages;

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
    bool isSunbathAvailable = false;
    Platform platform = null;
    int maxHealth = 1;
    Vector3 respawnLocation = Vector3.zero;
    bool isPunchUnlocked = false;
    Vector3 initalScale = Vector3.zero;
    float direction = 1f;

    private bool isPushingWall => isPushingWallRight || isPushingWallLeft;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        initialGravityScale = rb.gravityScale;
        maxHealth = defaultHealth;
        health = defaultHealth;
        respawnLocation = transform.position;
        initalScale = transform.localScale;
        UpdateUI();
    }

    private void Update()
    {
        animator.SetBool("Jump", !isOnGround);
        animator.SetBool("Wall", isPushingWall);
        animator.SetBool("Run", Mathf.Abs(rb.velocity.x) > .05);
        animator.SetBool("Sunbath", sunbath && isOnGround && Mathf.Abs(rb.velocity.x) < .05);
        transform.localScale = Vector3.Scale(initalScale, new Vector3(direction, 1, 1));
    }

    void FixedUpdate()
    {
        UpdateVelocity(Time.fixedDeltaTime);
        UpdateGravity();
        UpdateActions();

        if (punch && playerPunch != null && isPunchUnlocked)
            Punch();

        invincibilityCooldown = Mathf.Max(invincibilityCooldown - Time.fixedDeltaTime, 0f);
        if (rb.velocity.magnitude != 0f)
            isOnGround = false;
        isPushingWallLeft = false;
        isPushingWallRight = false;

        if (rb.velocity.x > 0f)
            direction = 1f;
        else if (rb.velocity.x < 0f)
            direction = -1f;
    }

    private void UpdateGravity()
    {
        float gravityFactor =
            isPushingWall ? 1 - gravityReductionOnPushingWall :
            holdingJump ? 1f - gravityReductionOnJump :
            down ? 1f + gravityAugmentOnDown :
                1f;
        rb.gravityScale = initialGravityScale * gravityFactor;
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
        if (pendingJump)
        {
            pendingJump = false;

            if (isOnGround || isPushingWall)
                velocity.y = jumpVelocity;

            if (isPushingWallRight && !isOnGround)
                velocity.x = -movementSpeed;
            else if (isPushingWallLeft && !isOnGround)
                velocity.x = movementSpeed;
        }

        rb.velocity = velocity;
    }

    private void UpdateActions()
    {
        if (sunbath && isSunbathAvailable)
        {
            TakeSunbath();
        }

        if (down && platform != null)
        {
            GoDown();
        }
    }

    private void TakeSunbath()
    {
        health = maxHealth;
        respawnLocation = transform.position;
        UpdateUI();
    }

    private void GoDown()
    {
        if (down && platform != null)
        {
            platform.SetEnabledPlatform(false);
            platform = null;
        }
    }

    private void Respawn()
    {
        transform.position = respawnLocation;
        health = maxHealth;
        foreach (EnnemyHealth ennemy in FindObjectsOfType< EnnemyHealth>(true))
            ennemy.Init();
        UpdateUI();
    }

    private void Punch()
    {
        if (punch && playerPunch != null && isPunchUnlocked)
            playerPunch.Punch();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < UiHealthImages.Count; i++)
        {
            UiHealthImages[i].SetActive(i < health);
        }

        for (int i = 0; i < UiMaxHealthImages.Count; i++)
        {
            UiMaxHealthImages[i].SetActive(i < maxHealth);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<float>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && (isOnGround || isPushingWall))
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
            Vector2 normal = collision.GetContact(0).normal;
            if (Mathf.Abs(normal.x) > .5f)
            {
                if (normal.x < 0)
                    isPushingWallRight = true;
                else if (normal.x > 0)
                    isPushingWallLeft = true;
            }
            else if (normal.y > .5f)
            {
                isOnGround = true;
            }
        }
    }

    public void ReceiveDamages(Vector3 damageSourcePosition, float knockbackFactor)
    {
        if (invincibilityCooldown > 0f)
            return;
        health -= 1;
        if (health <= 0)
        {
            Respawn();
            return;
        }
        Debug.Log("lifes: " + health);
        invincibilityCooldown = invincibilityDuration;
        Vector2 knockbackDirection = transform.position - damageSourcePosition;
        Vector2 knockback = knockbackDirection.normalized * knockbackVelocity * knockbackFactor;
        knockback.y = Mathf.Min(knockbackMaximumVerticalVelocity, knockback.y);
        rb.velocity = knockback;
        UpdateUI();
    }

    public void SetSunbathAvailable(bool isAvailable)
    {
        isSunbathAvailable = isAvailable;
    }

    public void SetPlatform(Platform p)
    {
        Debug.Log("SetPlaform: " + p);
        platform = p;
    }

    public void PowerUp()
    {
        maxHealth += 1;
        UpdateUI();
    }

    public void UnlockPunch()
    {
        isPunchUnlocked = true;
    }
}

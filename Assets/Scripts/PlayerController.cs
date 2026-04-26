using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Directional Animation Rows")]
    public Sprite[] southFrames;    
    public Sprite[] swFrames;       
    public Sprite[] westFrames;     
    public Sprite[] nwFrames;       
    public Sprite[] northFrames;    

    [Header("Animation")]
    public float frameRate = 8f;    

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 moveInput;
    private bool isDead = false;

    // Animation state
    private Sprite[] currentFrames;
    private int currentFrame = 0;
    private float frameTimer = 0f;
    private bool isMoving = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentFrames = southFrames;
    }

    void Update()
    {
        if (isDead) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float horizontal = 0f;
        float vertical = 0f;

        if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed)  horizontal -= 1f;
        if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed) horizontal += 1f;
        if (keyboard.downArrowKey.isPressed || keyboard.sKey.isPressed)  vertical -= 1f;
        if (keyboard.upArrowKey.isPressed || keyboard.wKey.isPressed)    vertical += 1f;

        moveInput = new Vector2(horizontal, vertical).normalized;

        isMoving = moveInput.sqrMagnitude > 0.01f;

        if (isMoving)
            UpdateDirection();

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (isDead) return;
        rb.linearVelocity = moveInput * moveSpeed;
    }

    void UpdateDirection()
    {
        float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        sr.flipX = false;

        if (angle >= 255f && angle < 285f)
        {
            
            currentFrames = southFrames;
        }
        else if (angle >= 285f && angle < 345f)
        {
            
            currentFrames = swFrames;
            sr.flipX = true;
        }
        else if (angle >= 345f || angle < 15f)
        {
            
            currentFrames = westFrames;
            sr.flipX = true;
        }
        else if (angle >= 15f && angle < 75f)
        {
            
            currentFrames = nwFrames;
            sr.flipX = true;
        }
        else if (angle >= 75f && angle < 105f)
        {
            
            currentFrames = northFrames;
        }
        else if (angle >= 105f && angle < 165f)
        {
           
            currentFrames = nwFrames;
        }
        else if (angle >= 165f && angle < 210f)
        {
            
            currentFrames = westFrames;
        }
        else if (angle >= 210f && angle < 255f)
        {
           
            currentFrames = swFrames;
        }
    }

    void UpdateAnimation()
    {
        if (!isMoving)
        {
            
            if (currentFrames != null && currentFrames.Length > 0)
                sr.sprite = currentFrames[0];
            currentFrame = 0;
            frameTimer = 0f;
            return;
        }

        frameTimer += Time.deltaTime;
        if (frameTimer >= 1f / frameRate)
        {
            frameTimer = 0f;
            currentFrame = (currentFrame + 1) % currentFrames.Length;
            sr.sprite = currentFrames[currentFrame];
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        GameManager.Instance.GameOver();
    }

    public void SetDead(bool value)
    {
        isDead = value;
        if (isDead) rb.linearVelocity = Vector2.zero;
    }
}
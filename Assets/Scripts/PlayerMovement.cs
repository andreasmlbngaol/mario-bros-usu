using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    private Vector2 velocity;
    private new Collider2D collider;
    
    private float inputAxis;
    public float moveSpeed = 8f;
    public float maxJumpHeight = 5f;

    // SAFETY: Default 1f prevents DivideByZero (Infinite Gravity) error
    public float maxJumpTime = 1f; 

    public float jumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    public float gravity => (-2f * maxJumpHeight) / Mathf.Pow((maxJumpTime / 2f), 2);

    public bool grounded { get; private set; }
    public bool jumping { get; private set; }
    public bool running => Mathf.Abs(velocity.x) > 0.25 || Mathf.Abs(inputAxis) > 0.25f;
    public bool sliding => (inputAxis > 0f && velocity.x < 0f) || (inputAxis < 0f && velocity.x > 0f);

    private new Camera camera;
    private AudioSource audioSource;
    public AudioClip jumpSound;
    
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        camera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // FIX: Reset rotation so "Flip via Scale" works correctly
        transform.rotation = Quaternion.identity;
    }

    private void OnEnable()
    {
        rigidbody.isKinematic = false;
        collider = GetComponent<Collider2D>();
        if(collider != null) collider.enabled = true;

        velocity = Vector2.zero;
        jumping = false;
    }
    
    private void OnDisable()
    {
        rigidbody.isKinematic = true;
        if(collider != null) collider.enabled = false;
        velocity = Vector2.zero;
        jumping = false;
    }

    private void Update()
    {
        HorizontalMovement();
        grounded = rigidbody.Raycast(Vector2.down);

        if (grounded)
        {
            GroundedMovement();
        }

        ApplyGravity();
    }

    // --- ASSIGNMENT REQUIREMENT: MANUAL MATH ---
    // Replaces Mathf.MoveTowards
    float MoveTowardsTransform(float current, float target, float delta)
    {
        float diff = target - current;
        if (Mathf.Abs(diff) <= delta)
            return target;
        
        return current + Mathf.Sign(diff) * delta;
    }

    private void HorizontalMovement()
    {
        inputAxis = Input.GetAxis("Horizontal");

        float targetVelocity = inputAxis * moveSpeed;
        float delta = moveSpeed * Time.deltaTime;
        

        // Apply Manual Math for Acceleration
        velocity.x = MoveTowardsTransform(
            velocity.x,
            targetVelocity,
            delta
        );
        // FIX: REMOVED Raycast(Vector2.right) to solve "Toe Stubbing" error.
        // The Rigidbody handles wall collisions automatically.

        // Flip Logic (Manual Transformation via Scale)
        if (velocity.x > 0f)
        {
            Flip(1); // Face Right
        }
        else if (velocity.x < 0f)
        {
            Flip(-1); // Face Left
        }
    }

    private void Flip(float direction)
    {
        Vector3 currentScale = transform.localScale;
        
        // Only apply if we need to change direction
        if (Mathf.Sign(currentScale.x) != direction)
        {
            // Set X to (Current Absolute Size * Direction)
            // This ensures we don't accidentally shrink a Big Mario
            transform.localScale = new Vector3(Mathf.Abs(currentScale.x) * direction, currentScale.y, currentScale.z);
        }
    }

    private void GroundedMovement()
    {
        velocity.y = Mathf.Max(velocity.y, 0f);
        jumping = velocity.y > 0f;

        if (Input.GetButtonDown("Jump"))
        {
            velocity.y = jumpForce;
            jumping = true;
            PlayJumpSound();
        }
    }

    private void ApplyGravity()
    {
        bool falling = velocity.y < 0f || !Input.GetButton("Jump");
        float multiplier = falling ? 2f : 1f;

        velocity.y += gravity * multiplier * Time.deltaTime;
        velocity.y = Mathf.Max(velocity.y, gravity / 2f);
    }

    private void FixedUpdate()
    {
        Vector2 velocityVector = velocity * Time.fixedDeltaTime;
        Vector2 nextPosition = rigidbody.position + velocityVector;
        
        // Manual Camera Clamp
        if (camera != null)
        {
            Vector2 leftEdge = camera.ScreenToWorldPoint(Vector2.zero);
            Vector2 rightEdge = camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        
            nextPosition.x = Mathf.Clamp(nextPosition.x, leftEdge.x + 0.5f, rightEdge.x - 0.5f);
        }
        
        rigidbody.position = nextPosition; 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Case 1: Hit enemy from above (Mario kills Enemy)
            if (transform.DotTest(collision.transform, Vector2.down))
            {
                velocity.y = jumpForce / 2f;
                velocity.x = 0f; // ADD THIS - Reset horizontal momentum
                PlayJumpSound();
            }
            else 
            {
                var player = GetComponent<Player>();
                if (player != null)
                {
                    velocity.x = 0f; // ADD THIS - Reset momentum saat hit dari samping
                    player.Hit();
                }
            }
        }
        else if (collision.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
        {
            if (transform.DotTest(collision.transform, Vector2.up))
            {
                velocity.y = 0f;
            }
        }
    }
    
    private void PlayJumpSound()
    {
        if (jumpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
   private Rigidbody2D rigidbody;
   private Vector2 velocity;
   private float inputAxis;
   public float moveSpeed = 12f;
   public float maxJumpHeight = 5f;

   public float maxJumpTime = 1f;

   public float jumpForce => (2f * maxJumpHeight) / (maxJumpTime/2f);

  public float gravity => 
    -(2f * maxJumpHeight) / Mathf.Pow((maxJumpTime / 2f), 2);

    
   public bool grounded { get; private set;} 

   public bool jumping { get; private set; } 


   private Camera camera;

   private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        camera = Camera.main; 
    }

    private void Update()
{
    HorizontalMovement();
    
    // Pastikan raycast mendeteksi ground dengan benar
    grounded = rigidbody.Raycast(Vector2.down);
    
    if (grounded)
    {
        GroundedMovement();
    }
    
    ApplyGravity();
    
    // Debug untuk mengecek status
    Debug.Log($"Grounded: {grounded}, Velocity.y: {velocity.y}, Position.y: {transform.position.y}");
}

    // ii transformasi manual itu 
    float MoveTowardsTransform(float current, float target, float delta)
{
    float transform = target - current;

    if (transform >  delta)
        transform =  delta;
    else if (transform < -delta)
        transform = -delta;

    return current + transform;
}


   private void HorizontalMovement()
{
    inputAxis = Input.GetAxis("Horizontal");

    float targetVelocity = inputAxis * moveSpeed;
    float delta = moveSpeed * Time.deltaTime;

    velocity.x = MoveTowardsTransform(
        velocity.x,
        targetVelocity,
        delta
    );
}

private void GroundedMovement()
{
    // Reset velocity.y ketika di ground
    if (!jumping)
    {
        velocity.y = 0f;
    }
    
    jumping = velocity.y > 0f;
    
    if (Input.GetButtonDown("Jump"))
    {
        velocity.y = jumpForce;
        jumping = true;
        Debug.Log("Jump pressed! Jump force: " + jumpForce);
    }
}

private void ApplyGravity()
    {
        bool falling = velocity.y < 0f || !Input.GetButton("Jump");
        float multiplier = falling ? 2f : 1f;

       velocity.y += gravity * multiplier * Time.deltaTime;
       velocity.y = Mathf.Max(velocity.y, -Mathf.Abs(gravity));

    }
    private void FixedUpdate()
    {
        Vector2 position = rigidbody.position;
        position+= velocity * Time.fixedDeltaTime;
        Vector2 leftEdge = camera.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        position.x = Mathf.Clamp(position.x, leftEdge.x + 0.5f, rightEdge.x-0.5f);


        rigidbody.MovePosition(position);   
    } 




}

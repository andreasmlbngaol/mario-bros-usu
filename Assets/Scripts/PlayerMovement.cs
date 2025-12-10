using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
   private Rigidbody2D playerRigidbody;
   private Vector2 velocity;
   private float inputAxis;
   public float moveSpeed = 12f;
   public float maxJumpHeight = 5f;
   private Camera playerCamera;

   private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCamera = Camera.main; 
    }

    private void Update()
    //ini untuk input move nya 
    {
        HorizontalMovement();    
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


    private void FixedUpdate()
    {
        Vector2 position = playerRigidbody.position;
        position+= velocity * Time.fixedDeltaTime;
        Vector2 leftEdge = playerCamera.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = playerCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        position.x = Mathf.Clamp(position.x, leftEdge.x + 0.5f, rightEdge.x-0.5f);


        playerRigidbody.MovePosition(position);   
    } 


}

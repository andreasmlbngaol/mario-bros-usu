using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
   private Rigidbody2D rigidbody;
   private Vector2 velocity;
   private float inputAxis;
   public float moveSpeed = 8f;

   private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
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
        Vector2 position = rigidbody.position;
        position+= velocity * Time.fixedDeltaTime;
        rigidbody.MovePosition(position);   
    } 




}

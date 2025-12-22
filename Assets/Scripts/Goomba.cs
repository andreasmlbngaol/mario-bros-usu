using System;
using System.Collections;
using UnityEngine;

public class Goomba : MonoBehaviour
{
    public Sprite flatSprite;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player.starpower)
            {
                HitWithStarpower();
            }
            else if (collision.transform.DotTest(transform, Vector2.down))
            {
                Flatten();
                Debug.Log("Flattened");
            }
            else
            {
                player.Hit();
                Debug.Log("Hit");
            }
        }
    }
    
    

    private void Flatten()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<EntityMovement>().enabled = false;
        GetComponent<AnimatedSprite>().enabled = false;
        GetComponent<SpriteRenderer>().sprite = flatSprite;
        Destroy(gameObject, 0.5f);
    }

    private void Hit()
    {
        GetComponent<AnimatedSprite>().enabled = false;
        GetComponent<DeathAnimation>().enabled = true;
        Destroy(gameObject, 3f);
    }
    
    private void HitWithStarpower()
    {
        GetComponent<AnimatedSprite>().enabled = false;
        GetComponent<DeathAnimation>().enabled = true;
        
        // ASSIGNMENT REQUIREMENT: MANUAL ROTATION LOGIC
        StartCoroutine(RotateAndFall());
        
        Destroy(gameObject, 3f);
    }

    private IEnumerator RotateAndFall()
    {
        float elapsed = 0f;
        float duration = 3f;
        float targetRotation = 720f; // 2 full rotations
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Manual rotation interpolation
            float currentZ = Mathf.Lerp(0f, targetRotation, t);
            transform.rotation = Quaternion.Euler(0, 0, currentZ);
            
            yield return null;
        }
        
        transform.rotation = Quaternion.Euler(0, 0, targetRotation);
    }
}

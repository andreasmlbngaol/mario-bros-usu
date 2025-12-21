using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerSpriteRenderer activeRenderer; 
    private DeathAnimation deathAnimation;
    private CapsuleCollider2D capsuleCollider;

    // Defines base sizes (Absolute values)
    private Vector3 baseSmallScale = Vector3.one; 
    private Vector3 baseBigScale = new Vector3(1.5f, 1.5f, 1f); 

    // Logic checks using Absolute values so facing direction (-1 scale) doesn't break logic
    public bool big => Mathf.Abs(transform.localScale.x) > 1.1f;
    public bool small => Mathf.Abs(transform.localScale.x) <= 1.1f;
    public bool dead => deathAnimation.enabled; 
    public bool starpower { get; private set; }

    private void Awake()
    {
        deathAnimation = GetComponent<DeathAnimation>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        
        // FIX: Ensure we start with a clean slate
        transform.localScale = baseSmallScale;
        transform.rotation = Quaternion.identity; // Reset rotation to prevent invisibility
    }

    public void Hit()
    {
        if (!dead && !starpower)
        {
            if (big)
            {
                Shrink();
            }
            else
            {
                Death();
            }
        }
    }

    public void Grow()
    {
        // REVISI: Manual Transform Animation
        StopAllCoroutines(); 
        
        // Check which way we are currently facing (1 or -1)
        float currentDirection = Mathf.Sign(transform.localScale.x);
        
        // Calculate target vector manually, preserving direction
        Vector3 target = new Vector3(baseBigScale.x * currentDirection, baseBigScale.y, baseBigScale.z);
        
        StartCoroutine(AnimateScale(target));
    }

    private void Shrink()
    {
        StopAllCoroutines();
        
        float currentDirection = Mathf.Sign(transform.localScale.x);
        Vector3 target = new Vector3(baseSmallScale.x * currentDirection, baseSmallScale.y, baseSmallScale.z);
        
        StartCoroutine(AnimateScale(target));
    }

    // --- ASSIGNMENT REQUIREMENT: MANUAL MATH ---
    // Replaces Vector3.Lerp
    private Vector3 ManualLerp(Vector3 start, Vector3 target, float t)
    {
        // Formula: P(t) = P0 + (P1 - P0) * t
        float x = start.x + (target.x - start.x) * t;
        float y = start.y + (target.y - start.y) * t;
        float z = start.z + (target.z - start.z) * t;
        return new Vector3(x, y, z);
    }

    private IEnumerator AnimateScale(Vector3 targetScale)
    {
        float elapsed = 0f;
        float duration = 0.5f;
        
        Vector3 startScale = transform.localScale;
        Vector3 startPosition = transform.localPosition;

        // Calculate height difference to keep feet on the ground
        // We use Mathf.Abs to get the raw height regardless of flipping
        float heightDifference = (Mathf.Abs(targetScale.y) - Mathf.Abs(startScale.y));
        Vector3 targetPositionAdjusted = startPosition + new Vector3(0, heightDifference * 0.5f, 0);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Clamp t manually
            if (t > 1f) t = 1f;

            // 1. Manual Scale Interpolation
            transform.localScale = ManualLerp(startScale, targetScale, t);

            // 2. Manual Position Interpolation (Adjusting Y)
            transform.localPosition = ManualLerp(startPosition, targetPositionAdjusted, t);

            yield return null;
        }

        transform.localScale = targetScale;
        transform.localPosition = targetPositionAdjusted;
    }

    public void Starpower(float duration = 10f)
    {
        StartCoroutine(StarpowerAnimation(duration));
    }

    private IEnumerator StarpowerAnimation(float duration)
    {
        starpower = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (Time.frameCount % 4 == 0)
            {
                activeRenderer.spriteRenderer.color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
            }
            yield return null;
        }

        activeRenderer.spriteRenderer.color = Color.white;
        starpower = false;
    }

    private void Death()
    {
        // FIX: Do NOT disable activeRenderer here. 
        // DeathAnimation.cs handles the sprite swap.
        
        deathAnimation.enabled = true; // This activates the script that disables physics
        
        GameManager.Instance.ResetLevel(3f);
    }
}
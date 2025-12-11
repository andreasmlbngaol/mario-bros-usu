using System.Collections;
using UnityEngine;

public class BlockHit : MonoBehaviour
{
    public GameObject item;
    public Sprite emptyBlock;
    public int maxHits = -1;

    private bool animating;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!animating && maxHits != 0 && collision.gameObject.CompareTag("Player"))
        {
            // This relies on Extension.cs being in your project!
            if (collision.transform.DotTest(transform, Vector2.up))
            {
                Hit();
            }
        }
    }

    private void Hit()
    {
        maxHits--;
        if (maxHits == 0)
        {
            spriteRenderer.sprite = emptyBlock;
        }
        
        StartCoroutine(Animate());

        if (item != null)
        {
            Instantiate(item, transform.position, Quaternion.identity);
        }
        
        // Triggers the custom shader flash
        StartCoroutine(FlashShader());
    }

    private IEnumerator Animate()
    {
        animating = true;

        Vector3 restingPosition = transform.localPosition;
        Vector3 animatedPosition = restingPosition + Vector3.up * 0.5f;

        yield return MoveManual(restingPosition, animatedPosition);
        yield return MoveManual(animatedPosition, restingPosition);

        animating = false;
    }

    // Manual Math calculation (No built-in MoveTowards)
    private IEnumerator MoveManual(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        float duration = 0.125f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // Manual linear interpolation formula: P = A + (B - A) * t
            transform.localPosition = from + (to - from) * t;
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = to;
    }

    private IEnumerator FlashShader()
    {
        // Ensure your Material has a float property named "_HitEffect"
        if (spriteRenderer != null && spriteRenderer.material.HasProperty("_HitEffect"))
        {
            spriteRenderer.material.SetFloat("_HitEffect", 1f);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.material.SetFloat("_HitEffect", 0f);
        }
    }
}
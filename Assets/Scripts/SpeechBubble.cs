using System.Collections;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    // Declare spriteRenderer
    private SpriteRenderer spriteRenderer;

    private float bubbleTime = 3f;

    private void Start()
    {
        // Initialize the spriteRenderer
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void ChangeBubble(Sprite bubble, float time)
    {
        // Exit function early if player already has a speech bubble
        if (spriteRenderer.sprite != null) return;

        spriteRenderer.sprite = bubble;

        bubbleTime = time;

        StartCoroutine("BubbleNone");
    }

    private IEnumerator BubbleNone()
    {
        yield return new WaitForSeconds(bubbleTime);
        spriteRenderer.sprite = null;
    }

    // Flip Speech Bubble so text is legible
    public void FlipSprite()
    {
        spriteRenderer.flipX = true;
    }

    // Flip Speech Bubble back so text is legible
    public void DefaultSprite()
    {
        spriteRenderer.flipX = false;
    }
}
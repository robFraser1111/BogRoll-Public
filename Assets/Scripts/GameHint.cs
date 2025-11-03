using System.Collections;
using UnityEngine;

public class GameHint : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;

    [SerializeField] private float bubbleTime = 6f;

    [SerializeReference] private float bubbleDelay = 0f;

    // Speech Bubble that shows when picking up object
    [SerializeField] private Sprite speechBubble = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Start the speech bubble and audio
        StartCoroutine("Bubble");
    }

    private IEnumerator Bubble()
    {
        // Suspend execution
        yield return new WaitForSeconds(bubbleDelay);

        // Add speech bubble
        FindObjectOfType<SpeechBubble>().ChangeBubble(speechBubble, bubbleTime);

        // Play audio
        if (audioClip) AudioSource.PlayClipAtPoint(audioClip, transform.position);
    }
}
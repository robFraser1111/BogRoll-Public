using UnityEngine;
using UnityEngine.Audio;

public class CoinPickup : MonoBehaviour
{
    // Variables
    [SerializeField] private AudioClip coinPickUpSFX;

    [SerializeField] private int pointsForCoinPickup = 100;

    [SerializeField] private float bubbleTime = 3f;

    // Speech Bubble that shows when picking up object
    [SerializeField] private Sprite speechBubble = null;

    private bool triggering = false;

    private AudioMixer audioMixer;

    private float masterVolume;

    private void Start()
    {
        audioMixer = Resources.Load<AudioMixer>("MainAudioMixer");
        audioMixer.GetFloat("masterVolume", out masterVolume);
    }

    // Handle pickups
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only add coin if collided with capsule
        if (collision is BoxCollider2D) return;

        FindObjectOfType<SpeechBubble>().ChangeBubble(speechBubble, bubbleTime);

        if (gameObject.name.Contains("Shungite"))
        {
            FindObjectOfType<GameSession>().AddLife();
            FindObjectOfType<GameSession>().AddToShungiteScore();
        }

        if (pointsForCoinPickup > 0) FindObjectOfType<GameSession>().AddToScore(pointsForCoinPickup);

        if (coinPickUpSFX) AudioSource.PlayClipAtPoint(coinPickUpSFX, transform.position, (masterVolume + 80f) * 0.01f);

        Destroy(gameObject);
    }
}
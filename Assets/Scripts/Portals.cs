using System.Collections;
using Map;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class Portals : MonoBehaviour
{
    // Scores
    [SerializeField] private ScoreTracker scoreTracker;

    [SerializeField] private AudioClip loadLevelSFX;

    [SerializeField] private Sprite mapFinishedSprite;

    [SerializeField] private Sprite mapGoldSprite;

    // Where the portal is taking you
    [SerializeField] private string mapExit;

    // Variables
    private readonly float loadTime = 2f;

    private AudioMixer audioMixer;

    private string currentScene;

    private GameObject gameSession;

    private float masterVolume;

    private GameObject player;
    private bool portal;

    private SpriteRenderer spriteRenderer;

    // Awake
    private void Awake()
    {
        currentScene = SceneManager.GetActiveScene().name;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    // Start 
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameSession = GameObject.FindGameObjectWithTag("GameSession");

        // Set map portal sprite
        SetMapPortalSprite(gameObject.name);

        // Get master volume
        audioMixer = Resources.Load<AudioMixer>("MainAudioMixer");
        audioMixer.GetFloat("masterVolume", out masterVolume);
    }

    // Update
    private void Update()
    {
        PortalKey();
    }

    // Enter Portal
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger)
        {
            gameSession.GetComponent<GameSession>().SetTriggered(true);

            if (collision.tag == "Player")
            {
                // Show level score on trigger for map
                if (currentScene == "Map")
                    // !! Make dynamic depending on map
                    gameSession.GetComponent<GameSession>().ShowMapScore(gameObject.name);

                portal = true;
            }
        }
    }

    // Exit Portal
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.isTrigger)
        {
            gameSession.GetComponent<GameSession>().SetTriggered(false);

            if (collision.tag == "Player")
            {
                // Hide level score on exit trigger for map
                if (currentScene == "Map") gameSession.GetComponent<GameSession>().ResetScore();

                portal = false;
            }
        }
    }

    // Set map portal sprite
    private void SetMapPortalSprite(string map)
    {
        // Return if finished sprites haven't been set
        if (!mapFinishedSprite || !mapGoldSprite) return;

        // Apply finished or 100% finished sprite
        if (scoreTracker.Scores[map] >= 100 && scoreTracker.ShungiteScores[map] >= 3)
            spriteRenderer.sprite = mapGoldSprite;
        else if (scoreTracker.Scores[map] > 0) spriteRenderer.sprite = mapFinishedSprite;
    }

    // Handle level change
    private void PortalKey()
    {
        if (CrossPlatformInputManager.GetButtonDown("Submit") && portal)
        {
            if (currentScene == "sa") FindObjectOfType<Steam>().StopTimer();

            // Update level score on exit
            gameSession.GetComponent<GameSession>().UpdateHiScore(mapExit);
            StartCoroutine("LoadLevel");
        }
    }

    // Load level
    private IEnumerator LoadLevel()
    {
        player.GetComponent<Player>().DisableInput(loadTime);

        FindObjectOfType<Overlay>().FadeOut(loadTime);

        AudioSource.PlayClipAtPoint(loadLevelSFX, transform.position, (masterVolume + 80f) * 0.01f);

        yield return new WaitForSeconds(loadTime);

        SceneManager.LoadScene(gameObject.name);
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    // Variables
    [SerializeField] private int playerLives = 3;

    // Scores
    [SerializeField] private ScoreTracker scoreTracker;

    [SerializeField] private int score;

    [SerializeField] private Text scoreText;

    [SerializeField] private int shungiteScore;

    [SerializeField] private Text shungiteText;

    // Setting UI Canvas as the Parent for the hearts
    [SerializeField] private GameObject heartParent;

    // Adding the heart prefab image
    [SerializeField] private Image heartPrefab;

    // Adding the heart sprite
    [SerializeField] private Sprite heartSprite;

    [SerializeField] private GameObject levelButtons;

    [SerializeField] private GameObject levelSettingsMenu;

    // Creating a list of heart images
    private readonly List<Image> hearts = new List<Image>();

    // x value of heart
    private int heartx = 50;

    private int maxLives;

    private bool paused;

    private GameObject player;

    private GameObject scoreUi;

    // Bool for checking if Player is on a portal
    private bool triggered;

    private GameObject ui;

    // Awake
    private void Awake()
    {
        var numGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numGameSessions > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    // Start
    private void Start()
    {
        ui = GameObject.Find("UI Canvas");
        scoreUi = GameObject.Find("Score");
        player = GameObject.Find("Player");
        paused = false;

        scoreText.text = score.ToString();
        shungiteText.text = shungiteScore.ToString();
        maxLives = playerLives;
    }

    private void Update()
    {
        HandlePause();
    }

    // On Enable
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // On Disable
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void HandlePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name == "Main Menu") return;

            paused = !paused;
            levelButtons.SetActive(paused);
            Time.timeScale = paused ? 0.001f : 1;
        }
    }

    public void HandleLevelSettingsMenu()
    {
        levelSettingsMenu.SetActive(true);
    }

    public void ReturnToGame()
    {
        Time.timeScale = 1;
        levelButtons.SetActive(false);
        paused = false;
    }

    public void HandleExit()
    {
        if (SceneManager.GetActiveScene().name == "Map")
        {
            SaveSystem.SaveRolls(scoreTracker.Scores);
            SaveSystem.SaveShungites(scoreTracker.ShungiteScores);
            ReturnToMainScreen();
        }
        else
        {
            ReturnToMap();
        }
    }

    private void ReturnToMainScreen()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
        levelButtons.SetActive(false);
        paused = false;
    }

    private void ReturnToMap()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1f;
        levelButtons.SetActive(false);
        paused = false;
    }

    // On Scene Load
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check level and reset stats
        CheckLevel();

        // Load Hearts into the UI
        CreateHearts();
    }


    private void CheckLevel()
    {
        ResetScore();
        ResetHealth();
    }


    // Updates bool if Player has walked in or out of a trigger
    public void SetTriggered(bool x)
    {
        triggered = x;
    }

    // Returns the triggered bool
    public bool GetTriggered()
    {
        return triggered;
    }

    // Hide UI
    public void HideUi()
    {
        ui.SetActive(false);
    }

    // Show UI
    public void ShowUi()
    {
        ui.SetActive(true);
    }

    // Hide Score
    public void HideScore()
    {
        scoreUi.SetActive(false);
    }

    // Show Score
    public void ShowScore()
    {
        scoreUi.SetActive(true);
    }

    private void CreateHearts()
    {
        // Clear existing hearts
        hearts.Clear();

        var existingHearts = GameObject.FindGameObjectsWithTag("Heart");
        foreach (var existingHeart in existingHearts) Destroy(existingHeart);

        heartx = 50;

        // Create hearts
        for (var i = 0; i < playerLives; i++)
        {
            // Add a heart image prefab on to the list
            hearts.Add(heartPrefab);

            // Assign a heart list item to a new object
            var heart = Instantiate(hearts[i]);

            // Put the new image object under the heartParant aka UI Canvas
            heart.rectTransform.SetParent(heartParent.transform, true);

            // Set the position of the new object to the top left
            heart.rectTransform.anchoredPosition = new Vector2(heartx, -50);

            // Change the heartx value so the next heart is shifted to the right
            heartx += 40;
        }
    }

    public void AddToScore(int pointsToAdd)
    {
        score += pointsToAdd;
        scoreText.text = score.ToString();

        // Add life when you have enough rolls but no more than what the Max is
        if (score % 100 == 0 && playerLives < maxLives) AddLife();
    }

    public void AddToShungiteScore()
    {
        shungiteScore++;
        shungiteText.text = shungiteScore.ToString();
    }

    // Show map score
    public void ShowMapScore(string map)
    {
        // Toilet roll score
        score = scoreTracker.Scores[map];
        scoreText.text = score.ToString();

        // Shungite score
        shungiteScore = scoreTracker.ShungiteScores[map];
        shungiteText.text = shungiteScore.ToString();

        ShowScore();
    }

    // Update level scores
    public void UpdateHiScore(string map)
    {
        // Return if map has no score
        if (!scoreTracker.Scores.ContainsKey(map)) return;

        // Check toilet roll hi-score before updating
        if (scoreTracker.Scores[map] < score) scoreTracker.Scores[map] = score;

        // Check shungite hi-score before updating
        if (scoreTracker.ShungiteScores[map] < shungiteScore) scoreTracker.ShungiteScores[map] = shungiteScore;
    }

    public void ResetScore()
    {
        // Reset toilet rolls
        score = 0;
        scoreText.text = score.ToString();

        // Reset shungites
        shungiteScore = 0;
        shungiteText.text = shungiteScore.ToString();
    }

    public void ResetHealth()
    {
        playerLives = 3;
        maxLives = playerLives;
    }

    public void ProcessPlayerDeath()
    {
        if (playerLives > 1)
        {
            TakeLife();
        }
        else
        {
            FindObjectOfType<Steam>().Dead();
            ResetGameSession();
        }
    }

    private void TakeLife()
    {
        playerLives--;

        // Remove last item in heart list
        hearts.RemoveAt(hearts.Count - 1);

        // Remove one of the hearts
        DestroyWithTag("Heart");

        // Change the heartx value so the next heart is shifted to the right
        heartx -= 40;
    }

    public void AddLife()
    {
        // Increase the Max Hearts the Player can have
        maxLives++;

        // While loop to keep adding hearts until all filled
        while (hearts.Count < maxLives)
        {
            // Add heart item to list
            hearts.Add(heartPrefab);

            // Assign a heart list item to a new object
            var heart = Instantiate(hearts[hearts.Count - 1]);

            // Put the new image object under the heartParant aka UI Canvas
            heart.rectTransform.SetParent(heartParent.transform, true);

            // Set the position of the new object to the top left
            heart.rectTransform.anchoredPosition = new Vector2(heartx, -50);

            // Change the heartx value so the next heart is shifted to the right
            heartx += 40;
        }

        playerLives = maxLives;
    }

    private void DestroyWithTag(string destroyTag)
    {
        // Create a new array
        GameObject[] destroyObject;

        // Add all objects with specified tag to array
        destroyObject = GameObject.FindGameObjectsWithTag(destroyTag);

        // Remove last heart in array
        Destroy(destroyObject[hearts.Count]);
    }

    private void ResetGameSession()
    {
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }
}
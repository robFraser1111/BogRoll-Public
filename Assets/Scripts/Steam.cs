using System;
using System.Collections;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Steam : MonoBehaviour
{
    [SerializeField] private ScoreTracker scoreTracker;

    public float timeLimit = 20f;
    private float currentTime;
    private bool isRunning;

    private void Awake()
    {
        // Persist the GameSession object across scenes
        var steamworks = FindObjectsOfType<Steam>().Length;
        if (steamworks > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);

        try
        {
            SteamClient.Init(2406060);
        }
        catch (Exception e)
        {
            // Something went wrong - it's one of these:
            //
            //     Steam is closed?
            //     Can't find steam_api dll?
            //     Don't have permission to play app?
            //
            Debug.Log(e);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Map") CheckMapAchievements();
        if (scene.name == "Main Menu") StartCoroutine(CheckMenuIdle());
        if (scene.name == "sa") StartTimer();
    }

    private void Start()
    {
        Debug.Log("SteamId: " + SteamClient.SteamId + ". Steam name: " + SteamClient.Name);

        currentTime = timeLimit;
        isRunning = false;

        if (CheckState(new Achievement("OPENED_GAME"))) AchievementUnlocked(new Achievement("OPENED_GAME_2"));

        AchievementUnlocked(new Achievement("OPENED_GAME"));
    }

    private void Update()
    {
        SteamClient.RunCallbacks();

        if (isRunning)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0f) TimerEnded();
        }
    }


    private void OnApplicationQuit()
    {
        SteamClient.Shutdown();
    }


    private void ClearAchievements()
    {
        foreach (var ach in SteamUserStats.Achievements) ach.Clear();
    }

    private void AchievementUnlocked(Achievement ach)
    {
        ach.Trigger();
    }

    private bool CheckState(Achievement ach)
    {
        return ach.State;
    }

    private void CheckMapAchievements()
    {
        if (!scoreTracker.HasZeroScore()) AchievementUnlocked(new Achievement("FINISH_ALL_LEVELS"));
        if (scoreTracker.Has100Score()) AchievementUnlocked(new Achievement("100_A_LEVEL"));
        if (scoreTracker.All100Score()) AchievementUnlocked(new Achievement("100_ALL_LEVELS"));
        if (scoreTracker.UnlockTas()) AchievementUnlocked(new Achievement("TAS"));
    }

    public void Psycho()
    {
        AchievementUnlocked(new Achievement("PSYCHO"));
    }

    private IEnumerator CheckMenuIdle()
    {
        yield return new WaitForSeconds(300);
        if (SceneManager.GetActiveScene().name == "Main Menu") AchievementUnlocked(new Achievement("IDLE"));
    }

    public void Dead()
    {
        AchievementUnlocked(new Achievement("GHOST"));
    }

    private void SpeedRun()
    {
        AchievementUnlocked(new Achievement("SPEED"));
    }

    private void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
        if (currentTime >= 0f) SpeedRun();
    }

    private void TimerEnded()
    {
        isRunning = false;
    }
}

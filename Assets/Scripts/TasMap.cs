using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TasMap : MonoBehaviour
{
    [SerializeField] private GameObject tas;

    [SerializeField] private GameObject water;

    [SerializeField] private ScoreTracker scoreTracker;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Map") CheckScore();
    }

    private void CheckScore()
    {
        if (scoreTracker.UnlockTas()) HandleShowTas(true);
    }

    private void HandleShowTas(bool showTas)
    {
        if (showTas)
        {
            tas.SetActive(true);
            water.SetActive(false);
        }
        else
        {
            tas.SetActive(false);
            water.SetActive(true);
        }
    }
}
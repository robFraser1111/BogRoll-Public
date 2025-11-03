using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class ScoreTracker : ScriptableObject
{
    // Toilet rolls hi-scores
    public Dictionary<string, int> Scores { get; set; } = new Dictionary<string, int>
    {
        { "vic", 0 },
        { "nsw", 0 },
        { "wa", 0 },
        { "nt", 0 },
        { "sa", 0 },
        { "qld", 0 },
        { "act", 0 },
        { "tas", 0 }
    };

    // Shungites hi-scores
    public Dictionary<string, int> ShungiteScores { get; set; } = new Dictionary<string, int>
    {
        { "vic", 0 },
        { "nsw", 0 },
        { "wa", 0 },
        { "nt", 0 },
        { "sa", 0 },
        { "qld", 0 },
        { "act", 0 },
        { "tas", 0 }
    };

    public void InitialiseRolls()
    {
        var rolls = SaveSystem.LoadRolls();
        Scores["vic"] = rolls.ElementAt(0);
        Scores["nsw"] = rolls.ElementAt(1);
        Scores["wa"] = rolls.ElementAt(2);
        Scores["nt"] = rolls.ElementAt(3);
        Scores["sa"] = rolls.ElementAt(4);
        Scores["qld"] = rolls.ElementAt(5);
        Scores["act"] = rolls.ElementAt(6);
        Scores["tas"] = rolls.ElementAt(7);
    }

    public void InitialiseShungites()
    {
        var shungites = SaveSystem.LoadShungites();
        ShungiteScores["vic"] = shungites.ElementAt(0);
        ShungiteScores["nsw"] = shungites.ElementAt(1);
        ShungiteScores["wa"] = shungites.ElementAt(2);
        ShungiteScores["nt"] = shungites.ElementAt(3);
        ShungiteScores["sa"] = shungites.ElementAt(4);
        ShungiteScores["qld"] = shungites.ElementAt(5);
        ShungiteScores["act"] = shungites.ElementAt(6);
        ShungiteScores["tas"] = shungites.ElementAt(7);
    }

    public void ResetScores()
    {
        foreach (var score in Scores.ToList()) Scores[score.Key] = 0;
        foreach (var score in ShungiteScores.ToList()) ShungiteScores[score.Key] = 0;
    }

    public bool HasZeroScore()
    {
        foreach (var score in Scores.ToList())
            if (score.Value == 0)
                return true;

        return false;
    }

    public bool Has100Score()
    {
        foreach (var score in Scores.ToList())
            if (score.Value == 100)
                return true;

        return false;
    }

    public bool All100Score()
    {
        foreach (var score in Scores.ToList())
            if (score.Value < 100)
                return false;

        return true;
    }

    public bool UnlockTas()
    {
        return Scores.Sum(x => x.Value) >= 700 && ShungiteScores.Sum(x => x.Value) >= 21;
    }
}
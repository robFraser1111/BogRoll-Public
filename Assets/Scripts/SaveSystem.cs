using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void SaveRolls(Dictionary<string, int> scoreTracker)
    {
        var path = Application.persistentDataPath + "/rolls.sav";

        var formatter = new BinaryFormatter();
        var stream = new FileStream(path, FileMode.OpenOrCreate,
            FileAccess.ReadWrite,
            FileShare.None);

        var data = scoreTracker.Values.ToArray();

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void SaveShungites(Dictionary<string, int> scoreTracker)
    {
        var path = Application.persistentDataPath + "/shungites.sav";

        var formatter = new BinaryFormatter();
        var stream = new FileStream(path, FileMode.OpenOrCreate,
            FileAccess.ReadWrite,
            FileShare.None);

        var data = scoreTracker.Values.ToArray();

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static IEnumerable<int> LoadRolls()
    {
        // Load the game
        var path = Application.persistentDataPath + "/rolls.sav";
        if (File.Exists(path))
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(path, FileMode.Open);

            var data = formatter.Deserialize(stream) as int[];
            stream.Close();

            return data;
        }

        return null;
    }

    public static IEnumerable<int> LoadShungites()
    {
        // Load the game
        var path = Application.persistentDataPath + "/shungites.sav";
        if (File.Exists(path))
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(path, FileMode.Open);

            var data = formatter.Deserialize(stream) as int[];
            stream.Close();

            return data;
        }

        return null;
    }

    public static void DeleteSaveFiles()
    {
        if (File.Exists(Application.persistentDataPath + "/rolls.sav"))
            File.Delete(Application.persistentDataPath + "/rolls.sav");

        if (File.Exists(Application.persistentDataPath + "/shungites.sav"))
            File.Delete(Application.persistentDataPath + "/shungites.sav");
    }
}
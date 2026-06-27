using UnityEngine;

public static class SaveSystem
{
    public static void Save()
    {
        PlayerPrefs.SetInt("Coins", PlayerProfile.Coins);

        PlayerPrefs.SetInt("SpeedLevel", PlayerProfile.SpeedLevel);
        PlayerPrefs.SetInt("LapMoneyLevel", PlayerProfile.LapMoneyLevel);
        PlayerPrefs.SetInt("DriftMoneyLevel", PlayerProfile.DriftMoneyLevel);
        PlayerPrefs.SetInt("PassiveIncomeLevel", PlayerProfile.PassiveIncomeLevel);
        PlayerPrefs.SetInt("NitroLevel", PlayerProfile.NitroLevel);
        PlayerPrefs.SetInt("AerodynamicsLevel", PlayerProfile.AerodynamicsLevel);

        PlayerPrefs.Save();
    }

    public static void Load()
    {
        PlayerProfile.Coins =
            PlayerPrefs.GetInt("Coins", 0);

        PlayerProfile.SpeedLevel =
            PlayerPrefs.GetInt("SpeedLevel", 0);

        PlayerProfile.LapMoneyLevel =
            PlayerPrefs.GetInt("LapMoneyLevel", 0);

        PlayerProfile.DriftMoneyLevel =
            PlayerPrefs.GetInt("DriftMoneyLevel", 0);

        PlayerProfile.PassiveIncomeLevel =
            PlayerPrefs.GetInt("PassiveIncomeLevel", 0);

        PlayerProfile.NitroLevel =
            PlayerPrefs.GetInt("NitroLevel", 0);

        PlayerProfile.AerodynamicsLevel =
            PlayerPrefs.GetInt("AerodynamicsLevel", 0);
    }

    public static void DeleteRaceResults()
    {
        foreach (TrackDataMenu track in Resources.LoadAll<TrackDataMenu>("Tracks"))
        {
            PlayerPrefs.DeleteKey($"BestRaceTime_{track.sceneName}");
        }

        PlayerPrefs.Save();
    }
}
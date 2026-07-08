using UnityEngine;

public static class SaveSystem
{
    public static void Save()
    {
        PlayerPrefs.SetInt(
            "TutorialStep",
            PlayerProfile.TutorialStep);

        PlayerPrefs.SetInt(
            "TutorialCompleted",
            PlayerProfile.TutorialCompleted ? 1 : 0);

        PlayerPrefs.SetInt("Coins", PlayerProfile.Coins);

        PlayerPrefs.SetInt(
            "CurrentCarLevel",
            PlayerProfile.CurrentCarLevel);

        for (int i = 0; i < PlayerProfile.OwnedCars.Length; i++)
        {
            PlayerPrefs.SetInt(
                $"OwnedCar_{i}",
                PlayerProfile.OwnedCars[i] ? 1 : 0);
        }

        for (int i = 0; i < PlayerProfile.Cars.Length; i++)
        {
            CarUpgradeData car = PlayerProfile.Cars[i];

            PlayerPrefs.SetInt($"EngineLevel_{i}",
                car.EngineLevel);

            PlayerPrefs.SetInt($"TurboLevel_{i}",
                car.TurboLevel);

            PlayerPrefs.SetInt($"TiresLevel_{i}",
                car.TiresLevel);

            PlayerPrefs.SetInt($"AerodynamicsLevel_{i}",
                car.AerodynamicsLevel);

            PlayerPrefs.SetInt($"NitroLevel_{i}",
                car.NitroLevel);
        }

        for (int car = 0; car < PlayerProfile.Colors.Length; car++)
        {
            for (int color = 0; color < PlayerProfile.Colors[car].OwnedColors.Length; color++)
            {
                PlayerPrefs.SetInt(
                    $"OwnedColor_{car}_{color}",
                    PlayerProfile.Colors[car].OwnedColors[color] ? 1 : 0);
            }

            PlayerPrefs.SetInt(
                $"CurrentColor_{car}",
                PlayerProfile.Colors[car].CurrentColor);
        }

        PlayerPrefs.SetInt(
            "LapMoneyLevel",
            PlayerUpgrades.LapMoneyLevel);

        PlayerPrefs.SetInt(
            "DriftMoneyLevel",
            PlayerUpgrades.DriftMoneyLevel);

        PlayerPrefs.SetInt(
            "PassiveIncomeLevel",
            PlayerUpgrades.PassiveIncomeLevel);

        PlayerPrefs.Save();
    }

    public static void Load()
    {
        PlayerProfile.TutorialStep =
            PlayerPrefs.GetInt("TutorialStep", 0);

        PlayerProfile.TutorialCompleted =
            PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;

        PlayerProfile.Coins =
            PlayerPrefs.GetInt("Coins", 0);

        PlayerProfile.CurrentCarLevel =
            PlayerPrefs.GetInt("CurrentCarLevel", 0);

        for (int i = 0; i < PlayerProfile.OwnedCars.Length; i++)
        {
            PlayerProfile.OwnedCars[i] =
                PlayerPrefs.GetInt(
                    $"OwnedCar_{i}",
                    i == 0 ? 1 : 0) == 1;
        }

        for (int i = 0; i < PlayerProfile.Cars.Length; i++)
        {
            CarUpgradeData car = PlayerProfile.Cars[i];

            car.EngineLevel =
                PlayerPrefs.GetInt($"EngineLevel_{i}", 0);

            car.TurboLevel =
                PlayerPrefs.GetInt($"TurboLevel_{i}", 0);

            car.TiresLevel =
                PlayerPrefs.GetInt($"TiresLevel_{i}", 0);

            car.AerodynamicsLevel =
                PlayerPrefs.GetInt($"AerodynamicsLevel_{i}", 0);

            car.NitroLevel =
                PlayerPrefs.GetInt($"NitroLevel_{i}", 0);
        }

        for (int car = 0; car < PlayerProfile.Colors.Length; car++)
        {
            for (int color = 0; color < PlayerProfile.Colors[car].OwnedColors.Length; color++)
            {
                PlayerProfile.Colors[car].OwnedColors[color] =
                    PlayerPrefs.GetInt(
                        $"OwnedColor_{car}_{color}",
                        color == 0 ? 1 : 0) == 1;
            }

            PlayerProfile.Colors[car].CurrentColor =
                PlayerPrefs.GetInt($"CurrentColor_{car}", 0);
        }

        PlayerUpgrades.LapMoneyLevel =
            PlayerPrefs.GetInt("LapMoneyLevel", 0);

        PlayerUpgrades.DriftMoneyLevel =
            PlayerPrefs.GetInt("DriftMoneyLevel", 0);

        PlayerUpgrades.PassiveIncomeLevel =
            PlayerPrefs.GetInt("PassiveIncomeLevel", 0);

        PlayerProfile.ApplyDefaultUnlocks();
    }

    public static void DeleteRaceResults()
    {
        foreach (TrackDataMenu track in Resources.LoadAll<TrackDataMenu>("Tracks"))
        {
            PlayerPrefs.DeleteKey(
                $"BestRaceTime_{track.sceneName}");
        }

        PlayerPrefs.Save();
    }
}
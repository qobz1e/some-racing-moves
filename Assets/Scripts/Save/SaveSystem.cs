using UnityEngine;

public static class SaveSystem
{
    public static void Save()
    {
        PlayerPrefs.SetInt("Coins", PlayerProfile.Coins);

        PlayerPrefs.SetInt(
            "CurrentCarLevel",
            PlayerProfile.CurrentCarLevel);

        PlayerPrefs.SetInt(
            "CurrentCarColor",
            PlayerProfile.CurrentCarColor);

        for (int i = 0; i < PlayerProfile.OwnedCars.Length; i++)
        {
            PlayerPrefs.SetInt(
                $"OwnedCar_{i}",
                PlayerProfile.OwnedCars[i] ? 1 : 0);
        }

        for (int i = 0; i < PlayerProfile.Cars.Length; i++)
        {
            CarUpgradeData car = PlayerProfile.Cars[i];

            PlayerPrefs.SetInt($"SpeedLevel_{i}",
                car.SpeedLevel);

            PlayerPrefs.SetInt($"AerodynamicsLevel_{i}",
                car.AerodynamicsLevel);

            PlayerPrefs.SetInt($"LapMoneyLevel_{i}",
                car.LapMoneyLevel);

            PlayerPrefs.SetInt($"DriftMoneyLevel_{i}",
                car.DriftMoneyLevel);

            PlayerPrefs.SetInt($"PassiveIncomeLevel_{i}",
                car.PassiveIncomeLevel);

            PlayerPrefs.SetInt($"NitroLevel_{i}",
                car.NitroLevel);
        }

        PlayerPrefs.Save();
    }

    public static void Load()
    {
        PlayerProfile.Coins =
            PlayerPrefs.GetInt("Coins", 0);

        PlayerProfile.CurrentCarLevel =
            PlayerPrefs.GetInt("CurrentCarLevel", 0);

        PlayerProfile.CurrentCarColor =
            PlayerPrefs.GetInt("CurrentCarColor", 0);

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

            car.SpeedLevel =
                PlayerPrefs.GetInt($"SpeedLevel_{i}", 0);

            car.AerodynamicsLevel =
                PlayerPrefs.GetInt($"AerodynamicsLevel_{i}", 0);

            car.LapMoneyLevel =
                PlayerPrefs.GetInt($"LapMoneyLevel_{i}", 0);

            car.DriftMoneyLevel =
                PlayerPrefs.GetInt($"DriftMoneyLevel_{i}", 0);

            car.PassiveIncomeLevel =
                PlayerPrefs.GetInt($"PassiveIncomeLevel_{i}", 0);

            car.NitroLevel =
                PlayerPrefs.GetInt($"NitroLevel_{i}", 0);
        }
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
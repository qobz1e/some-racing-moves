using UnityEngine;

public static class SaveSystem
{
    public static void Save()
    {
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

        for (int car = 0; car < PlayerProfile.Colors.Length; car++)
        {
            for (int color = 0; color < PlayerProfile.Colors[car].OwnedColors.Length; color++)
            {
                PlayerPrefs.SetInt(
                    $"OwnedColor_{car}_{color}",
                    PlayerProfile.Colors[car].OwnedColors[color] ? 1 : 0);
            }
        }

        PlayerPrefs.Save();
    }

    public static void Load()
    {
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

        for (int car = 0; car < PlayerProfile.Colors.Length; car++)
        {
            for (int color = 0; color < PlayerProfile.Colors[car].OwnedColors.Length; color++)
            {
                PlayerProfile.Colors[car].OwnedColors[color] =
                    PlayerPrefs.GetInt(
                        $"OwnedColor_{car}_{color}",
                        color == 0 ? 1 : 0) == 1;
            }
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
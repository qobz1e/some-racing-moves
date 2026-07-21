using UnityEngine;

public static class UnlockSystem
{
    public static bool TrackUnlocked { get; private set; }
    public static bool CarUnlocked { get; private set; }

    public static int TrackIndex { get; private set; }
    public static int CarIndex { get; private set; }

    public static void Reset()
    {
        TrackUnlocked = false;
        CarUnlocked = false;

        TrackIndex = -1;
        CarIndex = -1;
    }

    public static void UnlockTrack(int index)
    {
        TrackUnlocked = true;
        TrackIndex = index;

        Debug.Log($"New track unlocked with index {index}");
    }

    public static void UnlockCar(int index)
    {
        CarUnlocked = true;
        CarIndex = index;

        Debug.Log($"New car unlocked with index {index}");
    }
}
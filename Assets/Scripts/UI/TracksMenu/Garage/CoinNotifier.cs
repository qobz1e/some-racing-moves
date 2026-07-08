using System;

public static class CoinNotifier
{
    public static Action<int, int> OnCoinsChanged;


    public static void Notify(int delta)
    {
        OnCoinsChanged?.Invoke(
            PlayerProfile.Coins,
            delta
        );
    }
}
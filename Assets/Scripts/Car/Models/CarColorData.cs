[System.Serializable]
public class CarColorData
{
    public int CurrentColor;

    public bool[] OwnedColors =
    {
        true,
        false,
        false,
        false,
        false,
        false,
        false,
        false
    };

    public void UnlockStarterColors()
    {
        for (int i = 0; i < 6; i++)
            OwnedColors[i] = true;

        OwnedColors[6] = false;
        OwnedColors[7] = false;
    }
}
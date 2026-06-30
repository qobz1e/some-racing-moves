using UnityEngine;

public class PlayerCarLoader : MonoBehaviour
{
    [SerializeField] private CarDatabase database;
    [SerializeField] private Transform visualRoot;

    void Awake()
    {
        Refresh();
    }

    public void Refresh()
    {
        foreach (Transform child in visualRoot)
            Destroy(child.gameObject);

        CarData data =
            database.cars[PlayerProfile.CurrentCarLevel];

        Instantiate(
            data.prefabs[PlayerProfile.CurrentCarColor],
            visualRoot);
    }
}
using UnityEngine;

public class PlayerCarLoader : MonoBehaviour
{
    [SerializeField] private CarController carController;

    [SerializeField] private CarDatabase database;
    [SerializeField] private Transform visualRoot;

    void Awake()
    {
        Refresh();
    }

    public void Refresh()
    {
        CarVisual oldVisual =
            visualRoot.GetComponentInChildren<CarVisual>();

        if (oldVisual != null)
            Destroy(oldVisual.gameObject);

        CarData data =
            database.cars[PlayerProfile.CurrentCarLevel];

        Instantiate(
            data.prefabs[PlayerProfile.CurrentCarColor],
            visualRoot);

        carController.SetCarData(data);
    }
}
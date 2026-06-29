using UnityEngine;

[CreateAssetMenu(menuName = "Cars/Car Data")]
public class CarData : ScriptableObject
{
    [Header("Info")]
    public string id;
    public string carName;
    public Sprite icon;
    public GameObject[] prefabs;

    [Header("Stats")]
    public CarStats stats;

    [Header("Economy")]
    public int price;
    public bool unlockedByDefault;
}
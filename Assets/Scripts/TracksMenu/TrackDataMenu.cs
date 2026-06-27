using UnityEngine;

[CreateAssetMenu(menuName = "Tracks/Track Data")]
public class TrackDataMenu : ScriptableObject
{
    public string trackName;
    public Sprite previewSprite;

    public float goldTime;
    public float silverTime;
    public float bronzeTime;

    public string sceneName;
}
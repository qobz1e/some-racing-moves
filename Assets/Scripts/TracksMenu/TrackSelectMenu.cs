using UnityEngine;

public class TrackSelectMenu : MonoBehaviour
{
    [SerializeField] private TrackDataMenu[] tracks;
    [SerializeField] private TrackButtonUI[] buttons;

    void Start()
    {
        for (int i = 0; i < tracks.Length; i++)
        {
            buttons[i].Setup(tracks[i]);
        }
    }
}
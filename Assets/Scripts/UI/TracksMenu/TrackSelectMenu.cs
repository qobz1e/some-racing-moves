using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackSelectMenu : MonoBehaviour
{
    [SerializeField] private TrackButtonUI[] buttons;

    private TrackDataMenu[] tracks;

    private float returnDelay = 0.5f;
    private bool isExiting;

    void Start()
    {
        tracks = Resources.LoadAll<TrackDataMenu>("Tracks");

        for (int i = 0; i < buttons.Length; i++)
            buttons[i].Setup(tracks[i]);
    }

    public void ReturnToMenu()
    {
        if (isExiting) return;

        isExiting = true;
        StartCoroutine(LoadMenu());
    }

    System.Collections.IEnumerator LoadMenu()
    {
        yield return new WaitForSecondsRealtime(returnDelay);

        SceneManager.LoadScene("MainMenu");
    }
}
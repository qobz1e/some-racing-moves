using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackSelectMenu : MonoBehaviour
{
    [SerializeField] private TrackButtonUI[] buttons;

    [SerializeField] private TrackDataMenu[] tracks;

    private float returnDelay = 0.5f;
    private bool isExiting;

    void Start()
    {
        for (int i = 0; i < buttons.Length && i < tracks.Length; i++)
        {
            buttons[i].Setup(tracks[i]);

            buttons[i].SetLocked(
                !PlayerProfile.UnlockedTracks[i]
            );
        }
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
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackSelectMenu : MonoBehaviour
{
    [SerializeField] private TrackDataMenu[] tracks;
    [SerializeField] private TrackButtonUI[] buttons;

    private float returnDelay = 0.5f;
    private bool isExiting;

    void Start()
    {
        for (int i = 0; i < tracks.Length; i++)
        {
            buttons[i].Setup(tracks[i]);
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
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetProgressButton : MonoBehaviour
{
    public void ResetProgress()
    {
        PlayerProfile.ResetProgress();

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetProgressPopup : MonoBehaviour
{
    [SerializeField] private GameObject window;

    public void Open()
    {
        window.SetActive(true);
    }

    public void Close()
    {
        window.SetActive(false);
    }

    public void Confirm()
    {
        PlayerProfile.ResetProgress();

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );

        Close();
    }
}
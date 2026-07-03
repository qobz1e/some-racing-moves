using UnityEngine;

public class TutorialAction : MonoBehaviour
{
    [SerializeField] private FtueAction action;

    public void InvokeAction()
    {
        TutorialManager.Instance.NotifyAction(action);
    }
}
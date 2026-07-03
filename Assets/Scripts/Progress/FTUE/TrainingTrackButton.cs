using UnityEngine;

public class TrainingTrackButton : MonoBehaviour
{
    [SerializeField] private TrackButtonUI button;
    [SerializeField] private TrackDataMenu trainingTrack;

    void Start()
    {
        bool show =
            !PlayerProfile.TutorialCompleted &&
            PlayerProfile.TutorialStep ==
            (int)TutorialStep.StartTrainingRace;

        gameObject.SetActive(show);

        if (show)
            button.Setup(trainingTrack);
    }
}
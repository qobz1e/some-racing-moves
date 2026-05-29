using UnityEngine;

public class CarTrackBridge : MonoBehaviour
{
    [Tooltip("The GameObject that has TrackData on it.")]
    public GameObject trackGO;

    private void Awake()
    {
        if (trackGO == null)
        {
            Debug.LogError("[CarTrackBridge] trackGO is not assigned!");
            return;
        }

        TrackData td = trackGO.GetComponent<TrackData>();
        if (td == null)
        {
            Debug.LogError("[CarTrackBridge] trackGO has no TrackData component!");
            return;
        }

        // Add CarController to this GO and wire up TrackData
        var ctrl = GetComponent<CarController>();

        if (ctrl == null)
            ctrl = gameObject.AddComponent<CarController>();

        ctrl.SetTrack(td);
    }
}

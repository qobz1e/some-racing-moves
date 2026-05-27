using UnityEngine;

/// <summary>
/// Bridge component that lives on the Car GameObject.
/// It copies the TrackData reference to CarController at runtime,
/// so the car and the track can be separate GameObjects.
///
/// Attach this alongside CarController on the Car GO.
/// Assign trackGO in the Inspector (or via SceneBootstrap).
/// </summary>
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

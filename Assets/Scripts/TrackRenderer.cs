using UnityEngine;

/// <summary>
/// Renders the track visually using a LineRenderer.
/// Also draws checkered finish line at waypoint[0].
/// Attach to the same GameObject as TrackData.
/// </summary>
[RequireComponent(typeof(TrackData))]
public class TrackRenderer : MonoBehaviour
{
    [Header("Line Renderer settings")]
    public float lineWidth = 1.2f;
    public Material lineMaterial;          // assign "Sprites/Default" in Inspector
    public Color trackColor = new Color(0.2f, 0.2f, 0.2f, 1f);

    [Header("Finish line")]
    public Color finishColor = Color.white;

    private TrackData    _track;
    private LineRenderer _lr;

    private void Awake()
    {
        _track = GetComponent<TrackData>();
        BuildLineRenderer();
    }

    private void BuildLineRenderer()
    {
        _lr = gameObject.AddComponent<LineRenderer>();

        // material
        if (lineMaterial == null)
            lineMaterial = new Material(Shader.Find("Sprites/Default"));

        _lr.material        = lineMaterial;
        _lr.startColor      = trackColor;
        _lr.endColor        = trackColor;
        _lr.startWidth      = lineWidth;
        _lr.endWidth        = lineWidth;
        _lr.loop            = true;
        _lr.useWorldSpace   = true;
        _lr.sortingOrder    = -1;

        int n = _track.Count;
        _lr.positionCount = n;
        for (int i = 0; i < n; i++)
            _lr.SetPosition(i, _track.Get(i));
    }

    // ── Draw a simple finish line in OnDrawGizmos (editor only) ──────────────
    private void OnDrawGizmos()
    {
        TrackData td = GetComponent<TrackData>();
        if (td == null || td.Count < 2) return;

        Vector3 p0  = td.Get(0);
        Vector3 dir = td.GetDirection(0);
        Vector3 perp = new Vector3(-dir.y, dir.x, 0f);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(p0 - perp * 0.8f, p0 + perp * 0.8f);
    }
}

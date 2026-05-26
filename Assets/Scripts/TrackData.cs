using UnityEngine;

/// <summary>
/// Defines a closed loop track as a series of waypoints.
/// The car will follow them in order, looping back to index 0.
/// Attach this to an empty GameObject called "Track".
/// </summary>
public class TrackData : MonoBehaviour
{
    [Header("Track Shape (world positions)")]
    public Vector3[] waypoints = new Vector3[]
    {
        // ───── TOP ─────
        new Vector3(-4.5f,  3.5f, 0f),
        new Vector3(-3.0f,  3.5f, 0f),
        new Vector3(-1.5f,  3.5f, 0f),
        new Vector3( 0.0f,  3.5f, 0f),
        new Vector3( 1.5f,  3.5f, 0f),
        new Vector3( 3.0f,  3.5f, 0f),
        new Vector3( 4.5f,  3.5f, 0f),

        // ───── TOP RIGHT CURVE ─────
        new Vector3( 5.2f,  3.4f, 0f),
        new Vector3( 5.8f,  3.1f, 0f),
        new Vector3( 6.2f,  2.7f, 0f),
        new Vector3( 6.4f,  2.2f, 0f),
        new Vector3( 6.5f,  1.5f, 0f),

        // ───── RIGHT ─────
        new Vector3( 6.5f,  0.5f, 0f),
        new Vector3( 6.5f, -0.5f, 0f),
        new Vector3( 6.5f, -1.4f, 0f),
        new Vector3( 6.45f, -2.0f, 0f),
    
        // ───── BOTTOM RIGHT CURVE ─────
        new Vector3( 6.35f, -2.5f, 0f),
        new Vector3( 6.1f,  -2.9f, 0f),
        new Vector3( 5.7f,  -3.2f, 0f),
        new Vector3( 5.2f,  -3.4f, 0f),

        // ───── BOTTOM ─────
        new Vector3( 4.5f, -3.5f, 0f),
        new Vector3( 3.0f, -3.5f, 0f),
        new Vector3( 1.5f, -3.5f, 0f),
        new Vector3( 0.0f, -3.5f, 0f),
        new Vector3(-1.5f, -3.5f, 0f),
        new Vector3(-3.0f, -3.5f, 0f),
        new Vector3(-4.5f, -3.5f, 0f),

        // ───── BOTTOM LEFT CURVE ─────
        new Vector3(-5.2f, -3.4f, 0f),
        new Vector3(-5.8f, -3.1f, 0f),
        new Vector3(-6.2f, -2.7f, 0f),
        new Vector3(-6.4f, -2.2f, 0f),

        // ───── LEFT ─────
        new Vector3( -6.5f, -0.5f, 0f),
        new Vector3( -6.5f, 0.5f, 0f),
        new Vector3( -6.5f, 1.4f, 0f),
        new Vector3( -6.45f, 2.0f, 0f),

        // ───── TOP LEFT CURVE ─────
        new Vector3( -6.35f, 2.5f, 0f),
        new Vector3( -6.1f,  2.9f, 0f),
        new Vector3( -5.7f,  3.2f, 0f),
        new Vector3( -5.2f,  3.4f, 0f),
    };

    [Header("Visual")]
    public Color trackColor   = new Color(0.25f, 0.25f, 0.25f);
    public Color lineColor    = Color.yellow;
    public float trackWidth   = 1.2f;

    // ── Public helpers ────────────────────────────────────────────────────────

    /// Total number of waypoints
    public int Count => waypoints.Length;

    /// Waypoint at index (wraps around)
    public Vector3 Get(int index) => waypoints[index % waypoints.Length];

    /// Direction from waypoint[i] to waypoint[i+1]
    public Vector3 GetDirection(int index)
    {
        Vector3 from = Get(index);
        Vector3 to   = Get(index + 1);
        return (to - from).normalized;
    }

    // ── Gizmos ────────────────────────────────────────────────────────────────
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = lineColor;
        for (int i = 0; i < waypoints.Length; i++)
        {
            Vector3 a = waypoints[i];
            Vector3 b = waypoints[(i + 1) % waypoints.Length];
            Gizmos.DrawLine(a, b);
            Gizmos.DrawSphere(a, 0.15f);
        }
    }
}

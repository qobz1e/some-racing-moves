using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TrackData : MonoBehaviour
{
    [Header("Control Points")]
    public Vector3[] controlPoints =
    {
        new Vector3(-21, 9),

        new Vector3(-1, 9),

        new Vector3(-2,  -3),

        new Vector3(12,  3),

        new Vector3(15,  -6),

        new Vector3(21,  -3),

        new Vector3(21, -12),

        new Vector3(4, -15),

        new Vector3(-12, -15),

        new Vector3(-15, -9),

        new Vector3(-24, -9),

        new Vector3(-24, 0)
    };

    [Header("Spline")]
    [Range(4, 40)]
    public int pointsPerSegment = 12;

    public bool closedLoop = true;

    [Header("Generated")]
    public Vector3[] waypoints;

    public int Count =>
        waypoints != null ? waypoints.Length : 0;

    private void Awake()
    {
        GenerateTrack();
    }

    private void OnValidate()
    {
        GenerateTrack();
    }

    public void GenerateTrack()
    {
        List<Vector3> pts = new List<Vector3>();

        for (int i = 0; i < controlPoints.Length; i++)
        {
            Vector3 p0 = GetControlPoint(i - 1);
            Vector3 p1 = GetControlPoint(i);
            Vector3 p2 = GetControlPoint(i + 1);
            Vector3 p3 = GetControlPoint(i + 2);

            for (int j = 0; j < pointsPerSegment; j++)
            {
                float t = j / (float)pointsPerSegment;

                Vector3 pos =
                    CatmullRom(p0, p1, p2, p3, t);

                pts.Add(pos);
            }
        }

        waypoints = pts.ToArray();
    }

    private Vector3 GetControlPoint(int index)
    {
        if (closedLoop)
        {
            index =
                (index + controlPoints.Length) %
                controlPoints.Length;

            return controlPoints[index];
        }

        index = Mathf.Clamp(
            index,
            0,
            controlPoints.Length - 1
        );

        return controlPoints[index];
    }

    private Vector3 CatmullRom(
        Vector3 p0,
        Vector3 p1,
        Vector3 p2,
        Vector3 p3,
        float t)
    {
        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t
        );
    }

    public Vector3 Get(int index)
    {
        if (waypoints == null || waypoints.Length == 0)
            return Vector3.zero;

        return waypoints[index % waypoints.Length];
    }

    public Vector3 GetDirection(int index)
    {
        if (waypoints == null || waypoints.Length < 2)
            return Vector3.right;

        Vector3 from = Get(index);
        Vector3 to = Get(index + 1);

        return (to - from).normalized;
    }

    private void OnDrawGizmos()
    {
        if (controlPoints == null || controlPoints.Length < 2)
            return;

        Gizmos.color = Color.red;

        foreach (var p in controlPoints)
        {
            Gizmos.DrawSphere(p, 0.35f);
        }

        GenerateTrack();

        if (waypoints == null || waypoints.Length < 2)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < waypoints.Length; i++)
        {
            Vector3 a = waypoints[i];
            Vector3 b =
                waypoints[(i + 1) % waypoints.Length];

            Gizmos.DrawLine(a, b);
        }
    }
}
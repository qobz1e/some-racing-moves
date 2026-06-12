using UnityEngine;

[RequireComponent(typeof(TrackData))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TrackRenderer : MonoBehaviour
{
    [Header("Track Width")]
    public float trackWidth = 2.5f;
    public float borderWidth = 0.5f;

    [Header("Materials")]
    public Material roadMaterial;
    public Material borderMaterial;

    private TrackData track;
    private Mesh mesh;
    private PolygonCollider2D roadCollider;

    void Awake()
    {
        track = GetComponent<TrackData>();

        GetComponent<MeshRenderer>().sortingLayerName = "Track";
        GetComponent<MeshRenderer>().sortingOrder = -1;

        transform.position = Vector3.zero;

        mesh = new Mesh();
        mesh.name = "Track Mesh";

        GetComponent<MeshFilter>().mesh = mesh;

        if (roadMaterial != null)
            GetComponent<MeshRenderer>().material = roadMaterial;

        roadCollider = gameObject.AddComponent<PolygonCollider2D>();
        roadCollider.isTrigger = true;

        BuildMesh();
        BuildBorder();
        BuildCollider();
    }

    void BuildMesh()
    {
        int n = track.Count;
        if (n < 2) return;

        Vector3[] verts = new Vector3[n * 2];
        int[] tris = new int[n * 6];
        Vector2[] uv = new Vector2[n * 2];

        // vertices
        for (int i = 0; i < n; i++)
        {
            Vector3 curr = track.Get(i);
            Vector3 prev = track.Get((i - 1 + n) % n);
            Vector3 next = track.Get((i + 1) % n);

            Vector3 dir = (next - prev).normalized;
            Vector3 right = new Vector3(-dir.y, dir.x, 0f);

            verts[i * 2] = curr + right * (trackWidth * 0.5f);
            verts[i * 2 + 1] = curr - right * (trackWidth * 0.5f);

            float v = i * 0.5f;

            uv[i * 2] = new Vector2(0, v);
            uv[i * 2 + 1] = new Vector2(1, v);
        }

        // triangles (loop)
        int ti = 0;

        for (int i = 0; i < n; i++)
        {
            int next = (i + 1) % n;

            int i0 = i * 2;
            int i1 = i * 2 + 1;
            int i2 = next * 2;
            int i3 = next * 2 + 1;

            tris[ti++] = i0;
            tris[ti++] = i2;
            tris[ti++] = i1;

            tris[ti++] = i1;
            tris[ti++] = i2;
            tris[ti++] = i3;
        }

        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uv;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void BuildBorder()
    {
        if (borderMaterial == null)
            return;

        GameObject border = new GameObject("TrackBorder");
        border.transform.SetParent(transform, false);

        border.transform.localPosition = new Vector3(0, 0, 0.1f);

        MeshFilter mf = border.AddComponent<MeshFilter>();
        MeshRenderer mr = border.AddComponent<MeshRenderer>();

        mr.sortingLayerName = "Track";
        mr.tag = "Road";
        mr.sortingOrder = -3;

        mr.material = borderMaterial;

        Mesh borderMesh = new Mesh();

        int n = track.Count;

        Vector3[] verts = new Vector3[n * 2];
        int[] tris = new int[n * 6];
        Vector2[] uv = new Vector2[n * 2];

        float width = trackWidth + borderWidth;

        float uvScale = 0.03f;

        for (int i = 0; i < n; i++)
        {
            Vector3 curr = track.Get(i);
            Vector3 prev = track.Get((i - 1 + n) % n);
            Vector3 next = track.Get((i + 1) % n);

            Vector3 dir = (next - prev).normalized;
            Vector3 right = new Vector3(-dir.y, dir.x, 0f);

            verts[i * 2] = curr + right * (width * 0.5f);
            verts[i * 2 + 1] = curr - right * (width * 0.5f);

            float v = i * uvScale;

            uv[i * 2] = new Vector2(0, v);
            uv[i * 2 + 1] = new Vector2(1, v);
        }

        int ti = 0;

        for (int i = 0; i < n; i++)
        {
            int next = (i + 1) % n;

            int i0 = i * 2;
            int i1 = i * 2 + 1;
            int i2 = next * 2;
            int i3 = next * 2 + 1;

            tris[ti++] = i0;
            tris[ti++] = i2;
            tris[ti++] = i1;

            tris[ti++] = i1;
            tris[ti++] = i2;
            tris[ti++] = i3;
        }

        borderMesh.vertices = verts;
        borderMesh.triangles = tris;
        borderMesh.uv = uv;

        borderMesh.RecalculateNormals();
        borderMesh.RecalculateBounds();

        mf.mesh = borderMesh;
    }

    void BuildCollider()
    {
        int n = track.Count;
        if (n < 3) return;

        Vector2[] points = new Vector2[n * 2];

        for (int i = 0; i < n; i++)
        {
            Vector3 curr = track.Get(i);
            Vector3 prev = track.Get((i - 1 + n) % n);
            Vector3 next = track.Get((i + 1) % n);

            Vector3 dir = (next - prev).normalized;
            Vector3 right = new Vector3(-dir.y, dir.x, 0f);

            Vector3 leftPoint = curr + right * (trackWidth * 0.5f);
            Vector3 rightPoint = curr - right * (trackWidth * 0.5f);

            points[i] = new Vector2(leftPoint.x, leftPoint.y);
            points[n * 2 - 1 - i] = new Vector2(rightPoint.x, rightPoint.y);
        }

        roadCollider.points = points;
    }
}
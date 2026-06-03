using UnityEngine;

[RequireComponent(typeof(TrackData))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TrackRenderer : MonoBehaviour
{
    [Header("Track Width")]
    public float trackWidth = 2.5f;

    [Header("Material")]
    public Material roadMaterial;

    private TrackData track;
    private Mesh mesh;
    private MeshCollider meshCollider;

    void Awake()
    {
        track = GetComponent<TrackData>();

        GetComponent<MeshRenderer>().sortingOrder = -10;

        mesh = new Mesh();
        mesh.name = "Track Mesh";

        GetComponent<MeshFilter>().mesh = mesh;

        if (roadMaterial != null)
            GetComponent<MeshRenderer>().material = roadMaterial;

        meshCollider = GetComponent<MeshCollider>();

        BuildMesh();
    }

    void BuildMesh()
    {
        int n = track.Count;
        if (n < 2) return;

        Vector3[] verts = new Vector3[n * 2];
        int[] tris = new int[n * 6];

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
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }
}
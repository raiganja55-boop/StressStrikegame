using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[ExecuteAlways]
public class ParabolaDeformer : MonoBehaviour
{
    [Range(0f, 5f)]
    public float curveIntensity = 2.0f;

    [Range(1, 100)]
    public int subdivisions = 50;

    private Mesh deformedMesh;
    private Vector3[] baseVertices;
    private int lastSubdivisions;
    private float lastIntensity;

    void OnEnable()
    {
        BuildMesh();
    }

    void Update()
    {
        if (deformedMesh == null || baseVertices == null)
        {
            BuildMesh();
            return;
        }

        if (subdivisions != lastSubdivisions)
        {
            BuildMesh();
            return;
        }

        if (Mathf.Approximately(lastIntensity, curveIntensity))
            return;

        ApplyCurve();
    }

    void BuildMesh()
    {
        lastSubdivisions = subdivisions;
        int resolution = subdivisions + 1;
        Vector3[] verts = new Vector3[resolution * resolution];
        int[] tris = new int[subdivisions * subdivisions * 6];
        Vector2[] uv = new Vector2[verts.Length];

        float half = 5f;
        float step = (half * 2f) / subdivisions;

        int ti = 0;
        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = z * resolution + x;
                verts[i] = new Vector3(-half + x * step, 0, -half + z * step);
                uv[i] = new Vector2((float)x / subdivisions, (float)z / subdivisions);

                if (x < subdivisions && z < subdivisions)
                {
                    int topLeft = z * resolution + x;
                    int topRight = topLeft + 1;
                    int bottomLeft = (z + 1) * resolution + x;
                    int bottomRight = bottomLeft + 1;

                    tris[ti++] = topLeft;
                    tris[ti++] = bottomLeft;
                    tris[ti++] = topRight;
                    tris[ti++] = topRight;
                    tris[ti++] = bottomLeft;
                    tris[ti++] = bottomRight;
                }
            }
        }

        baseVertices = verts;
        deformedMesh = new Mesh();
        deformedMesh.name = "ParabolaPlane";
        GetComponent<MeshFilter>().mesh = deformedMesh;
        deformedMesh.vertices = verts;
        deformedMesh.triangles = tris;
        deformedMesh.uv = uv;
        deformedMesh.RecalculateNormals();

        lastIntensity = curveIntensity;
        ApplyCurve();
    }

    void ApplyCurve()
    {
        if (deformedMesh == null || baseVertices == null)
            return;

        lastIntensity = curveIntensity;
        Vector3[] verts = new Vector3[baseVertices.Length];
        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector3 v = baseVertices[i];
            v.y = curveIntensity * (v.x * v.x + v.z * v.z);
            verts[i] = v;
        }
        deformedMesh.vertices = verts;
        deformedMesh.RecalculateNormals();
    }

    void OnDestroy()
    {
        if (deformedMesh != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(deformedMesh);
#else
            Destroy(deformedMesh);
#endif
        }
    }
}

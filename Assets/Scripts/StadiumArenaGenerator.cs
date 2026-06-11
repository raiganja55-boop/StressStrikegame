using UnityEngine;

[ExecuteAlways]
public class StadiumArenaGenerator : MonoBehaviour
{
    [Header("Arena Dimensions")]
    public float radius = 12f;
    public float wallHeight = 8f;
    [Range(8, 64)]
    public int segments = 32;

    [Header("Wall Appearance")]
    public Color wallColor = new Color(0.15f, 0.15f, 0.2f);
    public Color edgeColor = new Color(0.8f, 0.1f, 0.1f);
    public bool showEdges = true;

    [Header("Floor")]
    public bool generateFloor = true;
    public float floorRadius = 14f;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    void OnEnable()
    {
        BuildArena();
    }

    void OnValidate()
    {
        if (isActiveAndEnabled)
            BuildArena();
    }

    [ContextMenu("Build Arena")]
    public void BuildArena()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        int edgeSegments = showEdges ? segments : 0;
        int wallVerts = (segments + 1) * 2;
        int floorVerts = generateFloor ? (segments + 1) : 0;
        int totalVerts = wallVerts + floorVerts + edgeSegments * 2;

        Vector3[] verts = new Vector3[totalVerts];
        Vector2[] uv = new Vector2[totalVerts];
        int[] tris;

        int wallTris = segments * 6;
        int floorTris = generateFloor ? segments * 3 : 0;
        int edgeTris = showEdges ? segments * 6 : 0;
        tris = new int[wallTris + floorTris + edgeTris];

        int vi = 0;
        int ti = 0;

        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            float sin = Mathf.Sin(angle);
            float cos = Mathf.Cos(angle);

            verts[vi] = new Vector3(cos * radius, 0, sin * radius);
            uv[vi] = new Vector2((float)i / segments, 0);
            vi++;

            verts[vi] = new Vector3(cos * radius, wallHeight, sin * radius);
            uv[vi] = new Vector2((float)i / segments, 1);
            vi++;

            if (i < segments)
            {
                int bl = i * 2;
                int tl = bl + 1;
                int br = bl + 2;
                int tr = br + 1;

                tris[ti++] = bl;
                tris[ti++] = br;
                tris[ti++] = tl;
                tris[ti++] = tl;
                tris[ti++] = br;
                tris[ti++] = tr;
            }
        }

        if (generateFloor)
        {
            int floorStart = vi;
            verts[vi] = Vector3.zero;
            uv[vi] = new Vector2(0.5f, 0.5f);
            vi++;

            for (int i = 0; i < segments; i++)
            {
                float angle = (float)i / segments * Mathf.PI * 2f;
                verts[vi] = new Vector3(Mathf.Sin(angle) * floorRadius, 0, Mathf.Cos(angle) * floorRadius);
                uv[vi] = new Vector2(Mathf.Sin(angle) * 0.5f + 0.5f, Mathf.Cos(angle) * 0.5f + 0.5f);
                vi++;
            }

            for (int i = 0; i < segments; i++)
            {
                int next = (i + 1) % segments;
                tris[ti++] = floorStart;
                tris[ti++] = floorStart + 1 + next;
                tris[ti++] = floorStart + 1 + i;
            }
        }

        if (showEdges)
        {
            int edgeStart = vi;
            float edgeRadius = radius + 0.05f;
            float edgeHeight = wallHeight + 0.05f;

            for (int i = 0; i < segments; i++)
            {
                float angle = (float)i / segments * Mathf.PI * 2f;
                float nextAngle = (float)(i + 1) / segments * Mathf.PI * 2f;

                float sin = Mathf.Sin(angle);
                float cos = Mathf.Cos(angle);
                float nextSin = Mathf.Sin(nextAngle);
                float nextCos = Mathf.Cos(nextAngle);

                int base0 = vi;
                verts[vi++] = new Vector3(cos * edgeRadius, 0, sin * edgeRadius);
                verts[vi++] = new Vector3(cos * edgeRadius, edgeHeight, sin * edgeRadius);
                verts[vi++] = new Vector3(nextCos * edgeRadius, 0, nextSin * edgeRadius);
                verts[vi++] = new Vector3(nextCos * edgeRadius, edgeHeight, nextSin * edgeRadius);

                tris[ti++] = base0;
                tris[ti++] = base0 + 1;
                tris[ti++] = base0 + 2;
                tris[ti++] = base0 + 2;
                tris[ti++] = base0 + 1;
                tris[ti++] = base0 + 3;
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "StadiumArena";
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        if (meshFilter.sharedMesh != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(meshFilter.sharedMesh);
            else
                Destroy(meshFilter.sharedMesh);
#else
            Destroy(meshFilter.sharedMesh);
#endif
        }
        meshFilter.sharedMesh = mesh;

        if (meshRenderer.sharedMaterial == null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = wallColor;
            mat.SetFloat("_Metallic", 0.3f);
            mat.SetFloat("_Glossiness", 0.2f);
            meshRenderer.sharedMaterial = mat;
        }
    }

    void OnDestroy()
    {
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(meshFilter.sharedMesh);
            else
                Destroy(meshFilter.sharedMesh);
#else
            Destroy(meshFilter.sharedMesh);
#endif
        }
    }
}

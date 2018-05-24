using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// リーチライン描画
/// m_Spritesのスプライトが全て１枚のアトラスに入っていないと正常な色になりません（プレハブ側のマテリアルもアトラスのものである必要がある）
/// </summary>
[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class BattleReachLineViewControl : MonoBehaviour
{
    public Sprite[] m_Sprites = new Sprite[(int)MasterDataDefineLabel.ElementType.MAX];

    private const int LINE_COUNT_MAX = 5 * 5;
    private const float LINE_WIDTH_SCALE = 0.001f;

    private MeshRenderer m_MeshRenderer = null;

    private Mesh m_Mesh;

    private Vector3[] m_Vertexs;
    private Vector2[] m_UVs;
    private int[] m_Indices;

    private Vector3 m_CameraPosition = Vector3.zero;
    private Vector3[] m_StartPositions;
    private Vector3[] m_EndPositions;
    private MasterDataDefineLabel.ElementType[] m_Elements;
    private int m_LineCount = 0;

    private class UvInfo
    {
        public Vector2 top_left;
        public Vector2 top_right;
        public Vector2 bottom_left;
        public Vector2 bottom_right;

        public UvInfo(Sprite sprite)
        {
            if (sprite != null)
            {
                calcUv((int)sprite.textureRect.xMin, (int)sprite.textureRect.yMin, (int)sprite.textureRect.width, (int)sprite.textureRect.height, sprite.texture.width, sprite.texture.height);
            }
        }

        private void calcUv(int left, int top, int width, int height, int texture_width, int texture_height)
        {
            float wrk_left = left / (float)texture_width;
            float wrk_top = top / (float)texture_height;
            float wrk_right = (left + width) / (float)texture_width;
            float wrk_bottom = (top + height) / (float)texture_height;

            top_left.x = wrk_left;
            top_left.y = wrk_top;

            top_right.x = wrk_right;
            top_right.y = wrk_top;

            bottom_left.x = wrk_left;
            bottom_left.y = wrk_bottom;

            bottom_right.x = wrk_right;
            bottom_right.y = wrk_bottom;
        }
    }

    private UvInfo[] m_UvInfos = null;

    // Use this for initialization
    void Start()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();

        m_Mesh = new Mesh();
        m_Mesh.name = "ReachLine";

        MeshFilter mesh_filter = GetComponent<MeshFilter>();
        mesh_filter.sharedMesh = m_Mesh;

        m_Vertexs = new Vector3[LINE_COUNT_MAX * 4];
        m_UVs = new Vector2[LINE_COUNT_MAX * 4];
        m_Indices = new int[LINE_COUNT_MAX * 3 * 2];

        m_StartPositions = new Vector3[LINE_COUNT_MAX];
        m_EndPositions = new Vector3[LINE_COUNT_MAX];
        m_Elements = new MasterDataDefineLabel.ElementType[LINE_COUNT_MAX];

        m_UvInfos = new UvInfo[(int)MasterDataDefineLabel.ElementType.MAX];

        Material material = null;
        for (int idx = 0; idx < m_Sprites.Length; idx++)
        {
            m_UvInfos[idx] = new UvInfo(m_Sprites[idx]);

            if (material == null)
            {
                material = new Material(m_MeshRenderer.sharedMaterial);
                material.mainTexture = m_Sprites[idx].texture;
            }
        }

        if (material != null)
        {
            m_MeshRenderer.material = material;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // GameObjectのスケーリングを打ち消すための係数を計算
        float scale = 1.0f;
        if (transform.parent != null)
        {
            scale = 1.0f / transform.parent.transform.lossyScale.x;
        }

        for (int line_count = 0; line_count < m_LineCount; line_count++)
        {
            // カメラのすぐ前の位置へ補正
            Vector3 start_pos = m_StartPositions[line_count] - m_CameraPosition;
            start_pos.Normalize();
            start_pos += (m_CameraPosition - transform.position);
            start_pos *= scale;

            // カメラのすぐ前の位置へ補正
            Vector3 end_pos = m_EndPositions[line_count] - m_CameraPosition;
            end_pos.Normalize();
            end_pos += (m_CameraPosition - transform.position);
            end_pos *= scale;

            Vector3 vec_forwad = end_pos - start_pos;
            Vector3 vec_width = Quaternion.Euler(0.0f, 0.0f, 90.0f) * vec_forwad;
            vec_width.Normalize();
            vec_width *= LINE_WIDTH_SCALE * scale;

            m_Vertexs[line_count * 4 + 0] = start_pos - vec_width;
            m_Vertexs[line_count * 4 + 1] = start_pos + vec_width;
            m_Vertexs[line_count * 4 + 2] = end_pos - vec_width;
            m_Vertexs[line_count * 4 + 3] = end_pos + vec_width;

            UvInfo uv_info = m_UvInfos[(int)m_Elements[line_count]];
            m_UVs[line_count * 4 + 0] = uv_info.top_left;
            m_UVs[line_count * 4 + 1] = uv_info.top_right;
            m_UVs[line_count * 4 + 2] = uv_info.bottom_left;
            m_UVs[line_count * 4 + 3] = uv_info.bottom_right;

            m_Indices[line_count * 3 * 2 + 0] = line_count * 4 + 0;
            m_Indices[line_count * 3 * 2 + 1] = line_count * 4 + 1;
            m_Indices[line_count * 3 * 2 + 2] = line_count * 4 + 2;

            m_Indices[line_count * 3 * 2 + 3] = line_count * 4 + 3;
            m_Indices[line_count * 3 * 2 + 4] = line_count * 4 + 2;
            m_Indices[line_count * 3 * 2 + 5] = line_count * 4 + 1;
        }

        if (m_LineCount > 0)
        {
            for (int idx = m_LineCount * 3 * 2; idx < m_Indices.Length; idx++)
            {
                m_Indices[idx] = 0;
            }

            m_Mesh.vertices = m_Vertexs;
            m_Mesh.uv = m_UVs;
            m_Mesh.triangles = m_Indices;

            m_Mesh.RecalculateNormals();
            m_Mesh.RecalculateBounds();

            m_MeshRenderer.enabled = true;
        }
        else
        {
            m_MeshRenderer.enabled = false;
        }
    }

    public void setCameraPosition(Vector3 camera_position)
    {
        m_CameraPosition = camera_position;
    }

    public void clearLine()
    {
        m_LineCount = 0;
    }

    public void addLine(Vector3 start_position, Vector3 end_position, MasterDataDefineLabel.ElementType element_type)
    {
        if (m_LineCount < m_StartPositions.Length)
        {
            m_StartPositions[m_LineCount] = start_position;
            m_EndPositions[m_LineCount] = end_position;
            m_Elements[m_LineCount] = element_type;
            m_LineCount++;
        }
    }
}

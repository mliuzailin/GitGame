using UnityEngine;
using System.Collections;

public class AssetAutoSetCharaTexture : MonoBehaviour
{
    private uint m_AutoSetCharaID = 0; //!< 自動設定情報：キャラID
    private bool m_bReady = false;
    public bool Ready { get { return m_bReady; } }

    private AssetBundler m_AssetBundler = null;

    public void SetCharaID(uint unCharaID, bool _reset = false)
    {
        m_AutoSetCharaID = unCharaID;
        if (_reset)
            ResetTexture();

        m_AssetBundler = AssetBundler.Create().
            SetAsUnitTexture(
                unCharaID,
                (o) =>
                {
                    UpdateWithTexture(GetTexture(o));
                },
                (str) =>
                {
                    Texture2D texture = new Texture2D(16, 16, TextureFormat.ARGB32, false);
                    Color[] cols = new Color[256];
                    for (int i = 0; i < 256; i++) cols[i] = Color.green;
                    texture.SetPixels(0, 0, 16, 16, cols);
                    texture.Apply();
                    UpdateWithTexture(texture);
                }).
            Load();
    }

    protected virtual Texture2D GetTexture(AssetBundlerResponse o)
    {
        return o.GetTexture2D(TextureWrapMode.Clamp);
    }

    private void UpdateWithTexture(Texture2D cAssetBundleTexture)
    {
        m_AssetBundler = null;
        if (cAssetBundleTexture != null)
        {
            SetTexture(cAssetBundleTexture);
            m_bReady = true;
        }
        else
        {
            ResetTexture();
            Debug.LogError("AssetBundle Texture  None! - unit_" + m_AutoSetCharaID.ToString());
        }
    }

    protected virtual void SetTexture(Texture2D cAssetBundleTexture)
    {
        if (gameObject.GetComponent<Renderer>().material != null)
        {
            gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", cAssetBundleTexture);


            Texture2D cAssignedTexture = gameObject.GetComponent<Renderer>().material.GetTexture("_MainTex") as Texture2D;
            if (cAssignedTexture != null)
            {
                cAssignedTexture.wrapMode = TextureWrapMode.Clamp;
            }
        }
    }

    protected virtual void ResetTexture()
    {
        gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", null);
        m_bReady = false;
    }

    private void OnDestroy()
    {
        if (m_AssetBundler != null)
        {
            m_AssetBundler.Destroy();
        }
    }
}

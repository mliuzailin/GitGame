using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class UnitIconAtlas
{
    protected static readonly int iconWidth = 128;
    protected static readonly int iconBinaryWidth = iconWidth / 4 * 64 / 8;
    protected static readonly int iconHeight = 128;
    protected static readonly int texWidth = 2048;
    protected static readonly int texBinaryWidth = texWidth / 4 * 64 / 8;
    protected static readonly int texHeight = 2048;
    protected static readonly int SpriteCountSide = 15;
    protected static readonly int SpriteCountLength = 15;
    protected static readonly int SpriteCountMax = SpriteCountSide * SpriteCountLength;

    private Sprite m_DefaultIcon = null;
    public Sprite DefaultIcon
    {
        get { return null; }
        set { m_DefaultIcon = value; }
    }

    public class IconApplyRequest
    {
        public long requestId = 0;
        public SpriteCacheInfo info = null;
        public AssetBundle assetBundle = null;
        public System.Action<Sprite> callback = null;
    }

    protected Texture2D mainTexture = null;
    public Texture Texture { get { return mainTexture; } }
    protected Sprite[] sprites = null;
    public Sprite[] Sprites { get { return sprites; } }
    protected int spriteIndex = 0;
    protected List<IconApplyRequest> applyRequest = new List<IconApplyRequest>();

    public UnitIconAtlas()
    {
    }

    public void Initialize(AssetBundle assetBundle, string name)
    {
        loadAtlus(assetBundle, name);
    }

    protected virtual void loadAtlus(AssetBundle assetBundle, string name)
    {
        sprites = assetBundle.LoadAssetWithSubAssets<Sprite>(name);
        if (sprites == null)
        {
            return;
        }

#if UNITY_STANDALONE_WIN //&& !UNITY_EDITOR
        Sprite temp = sprites[0];
        mainTexture  = new Texture2D(temp.texture.width,　temp.texture.height, temp.texture.format, false, true);
        for(int i = 0 ; i < sprites.Length ; i++)
        {
            temp = sprites[i];
            sprites[i]　= Sprite.Create(mainTexture, temp.rect, temp.pivot, temp.pixelsPerUnit);
        }
#else
        mainTexture = sprites[0].texture;
#endif
    }

    public Sprite SetSprite(AssetBundle assetbundle, long requestId, SpriteCacheInfo info, Action<Sprite> callback)
    {
        Sprite ret_sprite = addRequest(assetbundle, requestId, info, callback);
        if (ret_sprite == null)
        {
            return null;
        }

        return ret_sprite;
    }


    public int GetFreeIndex()
    {
        if (spriteIndex >= SpriteCountMax)
        {
            return -1;
        }

        int ret = spriteIndex;
        spriteIndex++;

        return ret;
    }

    protected virtual Sprite addRequest(AssetBundle assetBundle, long requestId, SpriteCacheInfo info, Action<Sprite> callback)
    {
        IconApplyRequest request = new IconApplyRequest();
        request.requestId = requestId;
        request.info = info;
        request.assetBundle = assetBundle;
        request.callback = callback;

        applyRequest.Add(request);

        return null;
    }

    public void RemoveRequest(long requestId)
    {
        var tmp = applyRequest.Find((t) => t.requestId == requestId);
        if (tmp != null)
        {
            applyRequest.Remove(tmp);
        }
    }

    public void AddRequestCallBack(SpriteCacheInfo info, Action<Sprite> callback)
    {
        IconApplyRequest request = applyRequest.Find(t => t.info.name.Equals(info.name));
        if (request != null)
        {
            request.callback += callback;
        }
    }

    protected virtual Sprite applyAtlas(int index, byte[] iconBinary, byte[] atlasBinary)
    {
        return null;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    /// <returns></returns>
    public virtual int UpdateAtlas()
    {
        int updateCount = 0;
        if (applyRequest.Count == 0)
        {
            return updateCount;
        }

        byte[] atlasBinary = mainTexture.GetRawTextureData();

        int count = applyRequest.Count;

        for (int i = 0; i < count; i++)
        {
            byte[] iconBinary;
            Sprite sprite = applyRequest[i].assetBundle.LoadAsset<Sprite>(applyRequest[i].info.name);

            if (sprite == null)
            {
                iconBinary = m_DefaultIcon.texture.GetRawTextureData();
            }
            else
            {
                iconBinary = sprite.texture.GetRawTextureData();
                //Debug.Log("sprites: " + sprite.texture.format +  " / mainTexture: " + mainTexture.format);
            }
            applyAtlas(applyRequest[i].info.spriteIndex, iconBinary, atlasBinary);
        }

        mainTexture.LoadRawTextureData(atlasBinary);
        mainTexture.Apply();

        for (int i = 0; i < count; i++)
        {
            //スプライト名をユニットアイコン名にする
            sprites[applyRequest[i].info.spriteIndex].name = applyRequest[i].info.name;

            // アイコンの取得中に表示先のオブジェクトがDestroyされるとMissingReferenceExceptionエラーになるので例外処理追加
            try
            {
                var sprite = sprites[applyRequest[i].info.spriteIndex];
                applyRequest[i].callback(sprite);
            }
            catch (MissingReferenceException e)
            {
#if BUILD_TYPE_DEBUG
                Debug.LogError(e);
#endif
            }
            //UnitIconImageProvider.Instance.SetCharaIconReady(applyRequest[i].info.name);
            applyRequest[i].info.ready = true;
        }

        updateCount = applyRequest.Count;
        applyRequest.RemoveRange(0, count);

        return updateCount;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    protected virtual byte[] getIconBinaryFromResource(string name)
    {
        Sprite sprite = Resources.Load<Sprite>("UnitIconData/" + name);
        if (sprite == null)
        {
            return null;
        }

        Texture2D spriteTex = sprite.texture;
        byte[] iconBinary = spriteTex.GetRawTextureData();
        return iconBinary;
    }
}

public class UnitIconAtlasWindows : UnitIconAtlas
{
    protected override Sprite applyAtlas(int index, byte[] iconBinary, byte[] atlasBinary)
    {
        int line = (int)iconHeight / 4;
        int atlasStart = ((int)sprites[index].textureRect.xMin / 4 * 64 / 8) + (texBinaryWidth * (int)sprites[index].textureRect.yMin / 4);
        int iconStart = 0;
        for (int i = 0; i < line; i++)
        {
            Buffer.BlockCopy(iconBinary, iconStart, atlasBinary, atlasStart, iconBinaryWidth);
            atlasStart += texBinaryWidth;
            iconStart += iconBinaryWidth;
        }

        return sprites[index];
    }
}

public class UnitIconAtlasETC : UnitIconAtlas
{
    protected override Sprite applyAtlas(int index, byte[] iconBinary, byte[] atlasBinary)
    {
        int line = (int)iconHeight / 4;
        int atlasStart = ((int)sprites[index].textureRect.xMin / 4 * 64 / 8) + (texBinaryWidth * (int)sprites[index].textureRect.yMin / 4);
        int iconStart = 0;
        for (int i = 0; i < line; i++)
        {
            Buffer.BlockCopy(iconBinary, iconStart, atlasBinary, atlasStart, iconBinaryWidth);
            atlasStart += texBinaryWidth;
            iconStart += iconBinaryWidth;
        }

        return sprites[index];
    }

}

public class UnitIconAtlasPVRTC : UnitIconAtlas
{
    protected override Sprite applyAtlas(int index, byte[] iconBinary, byte[] atlasBinary)
    {
        int copyCount = (iconWidth / 4) * (iconHeight / 4);
        int copySize = 4 * 4 / 2;

        int posX = (int)sprites[index].textureRect.xMin;
        int posY = (int)sprites[index].textureRect.yMin;

        for (int count = 0; count < copyCount; count++)
        {
            Vector2 offset = getPosFromIndex(count);
            int atlasIndex = getIndexFromPos(posX + (int)offset.x, posY + (int)offset.y);
            Buffer.BlockCopy(iconBinary, (count * copySize), atlasBinary, (atlasIndex * copySize), copySize);
        }

        return sprites[index];
    }

    private Vector2 getPosFromIndex(int index)
    {
        Vector2[] point = { new Vector2(0, 0), new Vector2(0, 4), new Vector2(4, 0), new Vector2(4, 4) };

        int dataA = iconWidth / 4 / 2;
        int dataB = ((iconWidth / 4) * (iconHeight / 4)) / 4;

        Vector2 pos = Vector2.zero;

        while (dataA != 1)
        {
            int amari = (index / dataB) % 4;
            pos += (point[amari] * (float)dataA);
            dataA /= 2;
            dataB /= 4;
        }
        pos += (point[(index / dataB) % 4] * (float)dataA);

        return pos;
    }

    private int getIndexFromPos(int x, int y)
    {
        int ret = 0;
        int dataA = texWidth / 2;
        int dataB = ((texWidth / 4) * (texHeight / 4)) / 4;

        while (dataB != 1)
        {
            ret += ((x / dataA) * 2 + (y / dataA)) * dataB;
            x -= ((x / dataA) * dataA);
            y -= ((y / dataA) * dataA);

            dataA /= 2;
            dataB /= 4;
        }
        ret += ((x / dataA) * 2 + (y / dataA));

        return ret;
    }
}

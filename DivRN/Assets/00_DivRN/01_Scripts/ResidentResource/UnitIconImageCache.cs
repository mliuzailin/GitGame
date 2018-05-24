using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UnitIconImageCache
{
    private Dictionary<string, UIAtlas> m_atlases = new Dictionary<string, UIAtlas>();
    private Dictionary<string, Sprite> m_sprites = new Dictionary<string, Sprite>();
    private Dictionary<string, SpriteCacheInfo> m_spriteInfos = new Dictionary<string, SpriteCacheInfo>();

    static public string GetCharaIconSpriteName(uint unCharaID)
    {
        return unCharaID > 0
            ? string.Format("chara_icon_{0}", DivRNUtil.UnitIdToStringMin(unCharaID))
            : "chara_icon_empty";
    }

    public void Register(string key, UIAtlas atlas)
    {
        m_atlases[key] = atlas;
    }

    public void Register(string key, Sprite sprite)
    {
        m_sprites[key] = sprite;
    }

    public void Register(string key, SpriteCacheInfo spriteInfo)
    {
        spriteInfo.count = GetSpriteInfoUseCount();
        m_spriteInfos[key] = spriteInfo;
    }

    public UIAtlas GetAtlas(string key)
    {
        if (m_atlases.ContainsKey(key))
        {
            return m_atlases[key];
        }

        return null;
    }

    public Sprite GetSprite(string key)
    {
        if (m_sprites.ContainsKey(key))
        {
            return m_sprites[key];
        }

        return null;
    }

    public SpriteCacheInfo GetSpriteInfo(string key)
    {
        if (m_spriteInfos.ContainsKey(key))
        {
            m_spriteInfos[key].count = GetSpriteInfoUseCount();
            return m_spriteInfos[key];
        }
        return null;
    }

    public void RemoveAtlas(string key)
    {
        if (m_atlases.ContainsKey(key) == false)
        {
            return;
        }

        m_atlases.Remove(key);
    }

    public void RemoveSprite(string key)
    {
        if (m_sprites.ContainsKey(key) == false)
        {
            return;
        }

        m_sprites.Remove(key);
    }

    public void RemoveSpriteInfo(string key)
    {
        if (m_spriteInfos.ContainsKey(key) == false)
        {
            return;
        }
        m_spriteInfos.Remove(key);
    }

    public SpriteCacheInfo GetSpriteInfoOldItem()
    {
        var tmpArray = m_spriteInfos.
                        Where((x) => x.Value.HiPriority == false).
                        OrderBy((x) => x.Value.count).ToArray();

        if (tmpArray == null ||
            tmpArray.Length == 0)
        {
            return null;
        }

        RemoveSpriteInfo(tmpArray[0].Value.name);

        return tmpArray[0].Value;
    }

    public int GetSpriteInfoCount()
    {
        return m_spriteInfos.Count;
    }

    private long spriteInfoUseCounter = 0;

    private long GetSpriteInfoUseCount()
    {
        spriteInfoUseCounter++;

        if (spriteInfoUseCounter < 0)
        {
            spriteInfoUseCounter = 1;
        }

        return spriteInfoUseCounter;
    }

    public void Reset(uint fix_id)
    {
        if (fix_id == 0)
        {
            return;
        }

        SpriteCacheInfo ci = GetSpriteInfo(GetCharaIconSpriteName(fix_id));
        if (ci != null)
        {
            ci.ResetPriority();
        }
    }

    public void ResetAll()
    {
        if (m_spriteInfos.IsNullOrEmpty() == false)
        {
            SpriteCacheInfo[] tempArray = m_spriteInfos.Values.ToArray();
            for (int i = 0; i < tempArray.Length; i++)
            {
                tempArray[i].ResetPriority();
            }
        }
    }

    public void Clear()
    {
        m_atlases.Clear();
        m_sprites.Clear();
        m_spriteInfos.Clear();
    }
}

public class SpriteCacheInfo
{
    public uint chara_fix_id = 0;
    public string name = "";
    public long count = 0;
    public int atlusIndex = 0;
    public int spriteIndex = 0;
    public long requestId = 0;
    public bool ready = false;

    private bool hipriority = false;
    public bool HiPriority
    {
        get
        {
            return hipriority;
        }
        set
        {
            hipriority = value;
        }
    }

    public SpriteCacheInfo() : this(0, 0)
    {
    }

    public SpriteCacheInfo(int AtlusIndex, int SpriteIndex)
    {
        name = "";
        count = 0;
        ready = false;
        HiPriority = false;

        // インスタンスの初期化用のコードを書く
        atlusIndex = AtlusIndex;
        spriteIndex = SpriteIndex;
    }

    public void ResetPriority()
    {
        //リファレンスカウントに変えた場合には全てリセットにする
        hipriority = false;
    }
}

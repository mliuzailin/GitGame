/**
 *  @file   AssetLoadStoryViewResource.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/04/10
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class StoryBGMData
{
    public AudioClip audio_clip = null;
    public List<string> bgm_asset_name_list = new List<string>();
}

public class AssetLoadStoryViewResource
{
    public enum ResourceType
    {
        CHARA,
        MAP,
        BGM,
    }

    public uint m_fix_id = 0;
    /// <summary>失敗したかどうか</summary>
    public bool IsFail { get; private set; }
    /// <summary>読み込み中かどうか</summary>
    public bool IsLoading { get; private set; }
    public ResourceType Type { get; private set; }
    public MasterDataStoryChara CharaMasterData { get; set; }
    public StoryBGMData BGMData = null;
    public AssetBundler m_AssetBundler = null;

    Texture2D m_Texture = null;
    Texture2D m_Texture_mask = null;
    Sprite m_Sprite = null;

    string m_AssetBundleName;
    string m_AssetName;

    public AssetLoadStoryViewResource()
    {
        IsFail = false;
        IsLoading = false;
    }

    /// <summary>
    /// キャラクタのロード
    /// </summary>
    /// <param name="charaMasterData"></param>
    public void SetStoryCharaId(uint fix_id)
    {
        if (fix_id == 0)
        {
            IsLoading = true;
            return;
        }

        CharaMasterData = MasterFinder<MasterDataStoryChara>.Instance.Find((int)fix_id);

        if (CharaMasterData == null || CharaMasterData.chara_id == 0)
        {
            IsLoading = true;
            return;
        }

        m_fix_id = CharaMasterData.fix_id;
        Type = ResourceType.CHARA;

        switch (CharaMasterData.chara_type)
        {
            case MasterDataDefineLabel.CharaType.CHARA:
                LoadUnit(CharaMasterData.chara_id);
                break;
            case MasterDataDefineLabel.CharaType.HERO:
                LoadHero(CharaMasterData.chara_id);
                break;
            case MasterDataDefineLabel.CharaType.NPC:
                LoadNPC(CharaMasterData.chara_id);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 背景画像のロード
    /// </summary>
    /// <param name="fix_id"></param>
    public void SetMapID(uint fix_id)
    {
        if (fix_id == 0)
        {
            IsLoading = true;
            return;
        }

        m_fix_id = fix_id;
        Type = ResourceType.MAP;

        m_AssetBundleName = string.Format("background_{0:D4}", fix_id);
        m_AssetName = string.Format("background_{0:D4}", fix_id);
        m_AssetBundler = AssetBundler.Create().Set(m_AssetBundleName, m_AssetName, typeof(Sprite),
                    (o) =>
                    {
                        m_Sprite = o.GetAsset<Sprite>();
                        IsLoading = true;
                    },
                    (str) =>
                    {
                        IsLoading = true;
                        IsFail = true;
                    })
                    .Load();
    }

    /// <summary>
    /// BGMのロード
    /// </summary>
    /// <param name="fix_id"></param>
    public void SetBgmID(uint fix_id)
    {
        BGMData = null;

        if (fix_id == 0)
        {
            IsLoading = true;
            return;
        }

        m_fix_id = fix_id;
        Type = ResourceType.BGM;

        string bgmAssetName = BGMManager.getStoryBgmAssetBundleName((int)fix_id);
        m_AssetBundler = AssetBundler.Create().Set(bgmAssetName,
                    (o) =>
                    {
                        BGMData = new StoryBGMData();
                        BGMData.audio_clip = o.GetAsset<AudioClip>();

                        string[] bgm_list = o.AssetBundle.GetAllAssetNames();
                        for (int idx_bgm = 0; idx_bgm < bgm_list.Length; idx_bgm++)
                        {
                            string name = Path.GetFileNameWithoutExtension(bgm_list[idx_bgm]);
                            BGMData.bgm_asset_name_list.Add(name);
                        }

                        IsLoading = true;
                    },
                    (str) =>
                    {
                        IsLoading = true;
                        IsFail = true;
                    })
                    .Load();
    }

    /// <summary>
    /// ユニット画像のロード開始
    /// </summary>
    /// <param name="fix_id"></param>
    private void LoadUnit(uint fix_id)
    {
        MasterDataParamChara charaMaster = MasterDataUtil.GetCharaParamFromID(fix_id);
        if (charaMaster == null)
        {
            IsLoading = true;
            return;
        }

        m_AssetBundler = AssetBundler.Create().SetAsUnitTexture(fix_id,
                    (o) =>
                    {
                        m_Texture = o.GetTexture2D(TextureWrapMode.Clamp);
                        m_Texture_mask = null;
                        IsLoading = true;
                    },
                    (str) =>
                    {
                        IsLoading = true;
                        IsFail = true;
                    })
                    .Load();
    }

    /// <summary>
    /// 主人公画像のロード開始
    /// </summary>
    /// <param name="fix_id"></param>
    private void LoadHero(uint fix_id)
    {
        int heroId = (int)fix_id;

        m_AssetBundleName = string.Format("hero_{0:D4}", heroId);
        m_AssetName = string.Format("tex_hero_stand_l_{0:D4}", heroId);

        m_AssetBundler = AssetBundler.Create().Set(m_AssetBundleName, m_AssetName, typeof(Texture2D),
                    (o) =>
                    {
                        m_Texture = o.GetTexture2D(m_AssetName, TextureWrapMode.Clamp);
                        m_Texture_mask = o.GetTexture2D(m_AssetName + "_mask", TextureWrapMode.Clamp);
                        IsLoading = true;
                    },
                    (str) =>
                    {
                        IsLoading = true;
                        IsFail = true;
                    })
                    .Load();
    }

    /// <summary>
    /// NPC画像のロード開始
    /// </summary>
    /// <param name="fix_id"></param>
    private void LoadNPC(uint fix_id)
    {
        int npcID = (int)fix_id;

        m_AssetBundleName = string.Format("npc_{0:D4}", npcID);
        m_AssetName = string.Format("NPC_{0:D4}", npcID);


        m_AssetBundler = AssetBundler.Create().Set(m_AssetBundleName, m_AssetName,
                (o) =>
                {
                    m_Texture = o.GetTexture2D(TextureWrapMode.Clamp);
                    m_Texture_mask = o.GetTexture2D(m_Texture.name + "_mask", TextureWrapMode.Clamp);
                    IsLoading = true;
                },
                (str) =>
                {
                    IsLoading = true;
                    IsFail = true;
                })
                .Load();
    }

    public Texture2D GetTexture()
    {
        return m_Texture;
    }
    public Texture2D GetTextureMask()
    {
        return m_Texture_mask;
    }

    public Sprite GetSprite()
    {
        if (m_Sprite != null)
        {
            return m_Sprite;
        }

        if (m_Texture != null)
        {
            return GetSprite(new Rect(0, 0, m_Texture.width, m_Texture.height), Vector2.zero);
        }

        return null;
    }

    public Sprite GetSprite(Rect rect, Vector2 pivot)
    {
        if (m_Texture == null) { return null; }
        Sprite sprite = Sprite.Create(m_Texture, rect, pivot);
        return sprite;
    }
}

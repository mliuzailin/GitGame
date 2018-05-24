using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ユニットのテクスチャをメモリ上にキャッシュするクラス
/// スキルカットインなどスピードが要求される場所ではメモリ上にキャッシュしていないと間に合わない
/// </summary>
public class BattleUnitTextureCache : SingletonComponent<BattleUnitTextureCache>
{
    private const int CACHE_MAX = 5 + 5 + 8;    // プレイヤー５人＋リンク５人＋敵８体

    public enum Status
    {
        NO_CACHE,   // キャッシュされていない
        LOADING,    // テクスチャロード中
        READY,      // テクスチャ準備完了
        ERROR,      // 使用できないテクスチャ
    }

    private class Info
    {
        public Status m_Status;
        public int m_TextureID;
        public Texture2D m_TextureMain;
        public Texture2D m_TextureMask;
        public Sprite m_Sprite;

        /// <summary>
        /// キャラのテクスチャをキャッシュ
        /// </summary>
        /// <param name="chara_id">キャラFixID  マイナスの値を指定した場合は主人公FixIDの画像をキャッシュ</param>
        public void load(int chara_id)
        {
            if (chara_id == 0)
            {
                return;
            }

            m_TextureID = chara_id;
            m_TextureMain = null;
            m_TextureMask = null;
            m_Sprite = null;
            m_Status = Status.LOADING;

            if (m_TextureID > 0)
            {
                // 通常のユニットの画像
                AssetBundler.Create().
                    SetAsUnitTexture((uint)m_TextureID,
                        (o) =>
                        {
                            m_TextureMain = o.GetTexture2D(TextureWrapMode.Clamp);
                            if (m_TextureMain != null)
                            {
                                m_Sprite = Sprite.Create(m_TextureMain, new Rect(0.0f, 0.0f, m_TextureMain.width, m_TextureMain.height), Vector2.one * 0.5f, 128, 0, SpriteMeshType.Tight);
                                m_Status = Status.READY;
                            }
                            else
                            {
                                m_Status = Status.ERROR;
                            }
                        },
                        (s) =>
                        {
                            m_Status = Status.ERROR;
                        }).Load();
            }
            else
            {
                // 主人公の画像
                string asset_bundle_name = string.Format("hero_{0:D4}", -chara_id);
                AssetBundler.Create().
                    Set(asset_bundle_name, typeof(Texture2D),
                        (o) =>
                        {
                            string assetname = string.Format("tex_hero_perform_l_{0:D4}", -chara_id);
                            m_TextureMain = o.GetTexture2D(assetname, TextureWrapMode.Clamp);
                            m_TextureMask = o.GetTexture2D(assetname + "_mask", TextureWrapMode.Clamp);
                            if (m_TextureMain != null)
                            {
                                m_Sprite = Sprite.Create(m_TextureMain, new Rect(0.0f, 0.0f, m_TextureMain.width, m_TextureMain.height), Vector2.one * 0.5f, 128, 0, SpriteMeshType.Tight);
                                m_Status = Status.READY;
                            }
                            else
                            {
                                m_Status = Status.ERROR;
                            }
                        },
                        (s) =>
                        {
                            m_Status = Status.ERROR;
                        }).Load();
            }
        }
    }

    private Info[] m_Infos = new Info[CACHE_MAX];

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnDestroy()
    {
        clearCache();
        base.OnDestroy();
    }

    /// <summary>
    /// キャッシュをクリア
    /// </summary>
    public void clearCache()
    {
        for (int idx = 0; idx < m_Infos.Length; idx++)
        {
            m_Infos[idx] = null;
        }
    }

    /// <summary>
    /// テクスチャを読み込む
    /// </summary>
    /// <param name="chara_id">キャラFixID  マイナスの値を指定した場合は主人公FixIDの画像を取得</param>
    public void loadTexture(int chara_id, bool is_mask)
    {
        getTexture(chara_id, is_mask);
    }

    /// <summary>
    /// テクスチャをアンロード
    /// </summary>
    /// <param name="chara_id">キャラFixID  マイナスの値を指定した場合は主人公FixIDの画像を取得</param>
    public void unloadTexture(int chara_id)
    {
        int index = search(chara_id);
        if (index >= 0)
        {
            delInfo(index);
        }
    }

    /// <summary>
    /// 状態を取得
    /// </summary>
    /// <param name="chara_id"></param>
    /// <returns></returns>
    public Status getStatus(int chara_id)
    {
        int index = search(chara_id);
        if (index >= 0)
        {
            Info info = m_Infos[index];
            return info.m_Status;
        }

        return Status.NO_CACHE;
    }

    /// <summary>
    /// テクスチャを取得
    /// </summary>
    /// <param name="chara_id">キャラFixID  マイナスの値を指定した場合は主人公FixIDの画像を取得</param>
    /// <returns></returns>
    public Texture2D getTexture(int chara_id, bool is_mask)
    {
        Info info = getInfo(chara_id);
        if (info != null)
        {
            if (is_mask)
            {
                return info.m_TextureMask;
            }
            return info.m_TextureMain;
        }

        return null;
    }

    public Sprite getSprite(int chara_id)
    {
        Info info = getInfo(chara_id);
        if (info != null)
        {
            return info.m_Sprite;
        }

        return null;
    }

    public bool isLoading()
    {
        for (int idx = 0; idx < m_Infos.Length; idx++)
        {
            Info info = m_Infos[idx];
            if (info != null && info.m_Status == Status.LOADING)
            {
                return true;
            }
        }

        return false;
    }

    private void delInfo(int index)
    {
        for (int idx = index; idx < m_Infos.Length - 1; idx++)
        {
            m_Infos[idx] = m_Infos[idx + 1];
        }
    }

    private int search(int chara_id)
    {
        if (chara_id != 0)
        {
            for (int idx = 0; idx < m_Infos.Length; idx++)
            {
                Info wrk_info = m_Infos[idx];
                if (wrk_info != null && wrk_info.m_TextureID == chara_id)
                {
                    return idx;
                }
            }
        }

        return -1;
    }

    private Info getInfo(int chara_id)
    {
        if (chara_id != 0)
        {
            Info info = null;
            int index = search(chara_id);
            if (index >= 0)
            {
                info = m_Infos[index];
                delInfo(index);
            }
            else
            {
                info = new Info();
                info.load(chara_id);
            }

            // 最後に参照されたものを先頭に置く（もっとも使われていないものはキャッシュから追い出される）
            for (int idx = m_Infos.Length - 2; idx >= 0; idx--)
            {
                m_Infos[idx + 1] = m_Infos[idx];
            }

            m_Infos[0] = info;

            return info;
        }

        return null;
    }



    public static void cacheBattleUnitTexture()
    {
        if (BattleParam.m_PlayerParty != null)
        {
            CharaOnce[] player_party_members = BattleParam.m_PlayerParty.getPartyMembers(CharaParty.CharaCondition.EXIST);
            for (int idx = 0; idx < player_party_members.Length; idx++)
            {
                CharaOnce player_party_member = player_party_members[idx];
                BattleUnitTextureCache.Instance.loadTexture((int)player_party_member.m_CharaMasterDataParam.fix_id, false);
                if (player_party_member.m_LinkParam != null
                    && player_party_member.m_LinkParam.m_cCharaMasterDataParam != null
                )
                {
                    BattleUnitTextureCache.Instance.loadTexture((int)player_party_member.m_LinkParam.m_cCharaMasterDataParam.fix_id, false);
                }
            }
        }

        if (BattleParam.m_EnemyParam != null)
        {
            for (int idx = 0; idx < BattleParam.m_EnemyParam.Length; idx++)
            {
                BattleEnemy battle_enemy = BattleParam.m_EnemyParam[idx];
                if (battle_enemy != null)
                {
                    MasterDataParamChara enemy_chara = battle_enemy.getMasterDataParamChara();
                    if (enemy_chara != null)
                    {
                        BattleUnitTextureCache.Instance.loadTexture((int)enemy_chara.fix_id, false);
                    }
                }
            }
        }

        // 主人公の画像をキャッシュ
        int current_hero_id = (int)MasterDataUtil.GetCurrentHeroID();
        BattleUnitTextureCache.Instance.loadTexture(-current_hero_id, false);
    }
}

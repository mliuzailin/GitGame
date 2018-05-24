/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	SoundManager.cs
	@brief	サウンド管理クラス
	@author Developer
	@date 	2012/10/08
*/
/*==========================================================================*/
/*==========================================================================*/
//#define DEF_SOUND_TASAKI

/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


/*==========================================================================*/
/*		namespace Begin 													*/
/*==========================================================================*/
/*==========================================================================*/
/*		define																*/
/*==========================================================================*/

/*==========================================================================*/
/*		macro																*/
/*==========================================================================*/
/*==========================================================================*/
/*		class																*/
/*==========================================================================*/
//----------------------------------------------------------------------------
/*!
	@brief	サウンド管理クラス
*/
//----------------------------------------------------------------------------
public class SoundManager : SingletonComponent<SoundManager>
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    // TODO:多重再生防止の簡易対応
    //		時期的にすべてを作り替えるわけにはいかないため、一部問題のあったサウンドのみ
    //		チャンネルを専用で用意し、隔離して再生する
    [SerializeField] private AudioSource m_AudioSrcLeaderSkill = null;                                                  //!< リーダースキルによるパワーアップ効果音
    [SerializeField] private AudioSource m_AudioSrcEnemyDead = null;                                                    //!< 敵が死亡した効果音
    [SerializeField] private AudioSource m_AudioSrcSkillExec = null;                                                    //!< スキル成立効果音

    [SerializeField] private AudioSource m_AudioSrcDefault = null;                                                  //!< SE共通チャンネル(1ch)
    [SerializeField] private AudioSource m_AudioSrcVoice = null;                                                    //!< Voice共通チャンネル(1ch)


    private bool m_SEEnable = false;                                                //!< SE共通有効/無効フラグ
    private bool m_VoiceEnable = true;                                              //!< Voice共通有効/無効フラグ

    private TemplateList<uint> m_WorkSelectVoice = new TemplateList<uint>();                            //!< ボイスSEのランダム再生の選択に使用
    private TemplateList<MasterDataAudioData> m_AudioDataList = new TemplateList<MasterDataAudioData>();            //!< オーディオ再生情報マスターテーブル
    private IDictionary<uint, AudioClip> m_AudioClipTable = new Dictionary<uint, AudioClip>();          //!< オーディオクリップテーブル
    private IDictionary<SEID, AudioClip> m_AudioClipSystemTable = new Dictionary<SEID, AudioClip>();            //!< 埋め込みサウンドテーブル
    private IDictionary<SEID, AudioClip[]> m_AudioClipReplaceTable = new Dictionary<SEID, AudioClip[]>();           //!< 置き換えサウンドテーブル

    //TODO Developer
    private bool m_AssetResourceLoaded = true;
    private float m_LastPlayTime;

    private SELoader m_seLoader = new SELoader();
    private Dictionary<SEID, string> m_sePathMap = null;

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //----------------------------------------------------------------------------
    //	@brief		読み込み完了確認
    //----------------------------------------------------------------------------
    public bool isLoeded { get { return m_AssetResourceLoaded; } private set { } }


    //----------------------------------------------------------------------------
    //	@brief		再生終了確認
    //	@retval		[true=再生中/false=再生していない]
    //----------------------------------------------------------------------------
    public bool isPlaying
    {
        get
        {
            //	現在時間が再生終了予定時間を過ぎていたら再生が終了していると判断する
            if (m_LastPlayTime > Time.time)
            {
                return true;
            }

            return false;
        }
    }


    //----------------------------------------------------------------------------
    /*!
		@brief
	*/
    //----------------------------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();

        if (m_sePathMap == null)
            m_sePathMap = SESettings.pathMap;
        m_AudioClipSystemTable.Add(SEID.SE_INGAME_ACTIVITY_TRAP, null); // SE：インゲームSE：パネル発動音：虎ばさみ (deprecated)
        m_AudioClipSystemTable.Add(SEID.SE_INGAME_ACTIVITY_BOMB, null); // SE：インゲームSE：パネル発動音：地雷 (deprecated)

        m_AssetResourceLoaded = true;
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
	*/
    //----------------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();

        m_SEEnable = false;
    }

    public void PrepareSE(SEID seid, System.Action callback = null)
    {
        if (m_sePathMap == null)
            m_sePathMap = SESettings.pathMap;

        if (!m_sePathMap.ContainsKey(seid))
        {
            if (callback != null)
                callback();
            return;
        }

        m_seLoader.Load(m_sePathMap[seid],
            (AudioClip clip) =>
            {
                if (clip != null)
                    m_AudioClipSystemTable.Add(seid, clip);

                if (callback != null)
                    callback();
            });
    }

    /// <summary>
    /// 差し替えＳＥを設定
    /// </summary>
    /// <param name="seid"></param>
    /// <param name="clips"></param>
    public void setReplaceSE(SEID seid, AudioClip[] clips)
    {
        m_AudioClipReplaceTable.Add(seid, clips);
    }

    /// <summary>
    /// 差し替えＳＥを破棄
    /// </summary>
    public void clearReplaceSE()
    {
        m_AudioClipReplaceTable.Clear();
    }


    public void RemoveSE(SEID seid)
    {
        m_AudioClipSystemTable.Remove(seid);
    }

    public void RemoveAllSEs()
    {
        m_AudioClipSystemTable.Clear();
        clearReplaceSE();
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：インスタンス制御関連：インスタンス破棄時に呼出し
	*/
    //----------------------------------------------------------------------------
    protected override void OnDestroy()
    {
        base.OnDestroy();

        try
        {
            AudioClip clip;

            //	共通SE破棄
            if (m_AudioClipSystemTable != null)
            {
                for (SEID i = SEID.SE_NONE; i < SEID.SE_MAX; i++)
                {
                    bool ret = m_AudioClipSystemTable.TryGetValue(i, out clip);
                    if (ret)
                    {
                        Resources.UnloadAsset(clip);
                        m_AudioClipSystemTable[i] = null;
                    }
                }
                m_AudioClipSystemTable = null;
            }


            //	読み込んだSEの破棄
            if (m_AudioClipTable != null)
            {
                for (int i = 0; i < m_AudioDataList.m_BufferSize; i++)
                {
                    if (m_AudioDataList[i] == null)
                    {
                        continue;
                    }

                    uint fix_id = m_AudioDataList[i].fix_id;
                    bool ret = m_AudioClipTable.TryGetValue(fix_id, out clip);
                    if (ret)
                    {
                        Resources.UnloadAsset(clip);
                        m_AudioClipTable[fix_id] = null;
                    }
                }
                m_AudioClipTable = null;
            }


            //	マスター破棄
            if (m_AudioDataList != null)
            {
                for (int i = 0; i < m_AudioDataList.m_BufferSize; i++)
                {
                    m_AudioDataList[i] = null;
                }
                m_AudioDataList.Release();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }


    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    protected void Update()
    {
        if (m_seLoader != null)
            m_seLoader.Tick();

        //	オプション設定の反映
        if (LocalSaveManager.HasInstance == true)
        {
            LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
            if (cOption != null)
            {
                bool bSEOptionActive = (cOption.m_OptionSE == (int)LocalSaveDefine.OptionSE.ON);

                if (m_SEEnable != bSEOptionActive)
                {
                    m_SEEnable = bSEOptionActive;

                    AudioSource _audio = gameObject.GetComponent<AudioSource>();
                    if (_audio != null)
                    {
                        if (!m_SEEnable)
                        {
                            _audio.volume = 0.0f;
                        }
                        else
                        {
                            _audio.volume = 1.0f;
                        }
                    }
                }

                bool bVoiceOptionActive = (cOption.m_OptionVoice == (int)LocalSaveDefine.OptionVoice.ON);

                if (m_VoiceEnable != bVoiceOptionActive)
                {
                    m_VoiceEnable = bVoiceOptionActive;
                    if (!m_VoiceEnable)
                    {
                        //	ミュートON
                        m_AudioSrcVoice.mute = true;
                    }
                    else
                    {
                        //	ミュートOFF
                        m_AudioSrcVoice.mute = false;
                    }
                }
            }
        }

    }


    //----------------------------------------------------------------------------
    /*!
		@brief		埋め込みSE再生
		@param[in]	nSoundLabel		任意のSEラベル
	*/
    //----------------------------------------------------------------------------
    public void PlaySE(SEID nSoundLabel)
    {
        if (!m_SEEnable)
        {
            Debug.LogError("DISABLE_SE");
            return;
        }

        if (nSoundLabel == SEID.SE_NONE)
        {
            return;
        }

        try
        {
            AudioClip clip = null;
            bool waitPlayEnd = false;

            // 置き換えSEが存在する場合は置き換えが優先
            {
                AudioClip[] clips;
                bool is_exist_replace = m_AudioClipReplaceTable.TryGetValue(nSoundLabel, out clips);
                if (is_exist_replace)
                {
                    if (clips.IsNullOrEmpty() == false)
                    {
                        int n = (int)RandManager.GetRand(0, (uint)clips.Length);
                        clip = clips[n];
                    }
                }
            }

            //	ラベルに該当するSEの取得
            if (clip == null)
            {
                bool ret = m_AudioClipSystemTable.TryGetValue(nSoundLabel, out clip);
                if (!ret)
                {
                    Debug.LogError("NOT_FOUND_SE:" + nSoundLabel);
                    return;
                }
            }

            //--------------------------------
            // 指定ラベルのSE再生指示
            //--------------------------------
            switch (nSoundLabel)
            {
                case SEID.SE_INGAME_LEADERSKILL:
                    // リーダースキルパワーアップ音
                    if (m_AudioSrcLeaderSkill != null)
                    {
                        if (m_AudioSrcLeaderSkill.clip == null)
                        {
                            m_AudioSrcLeaderSkill.clip = clip;
                        }

                        if (m_AudioSrcLeaderSkill.isPlaying == false)
                        {
                            m_AudioSrcLeaderSkill.Play();
                        }
                    }
                    return;

                case SEID.SE_BATTLE_ENEMYDEATH:
                    // 敵死亡音
                    if (m_AudioSrcEnemyDead != null)
                    {
                        if (m_AudioSrcEnemyDead.clip == null)
                        {
                            m_AudioSrcEnemyDead.clip = clip;
                        }

                        if (m_AudioSrcEnemyDead.isPlaying == false)
                        {
                            m_AudioSrcEnemyDead.Play();
                        }
                    }
                    return;

                case SEID.SE_BATLE_SKILL_EXEC:
                    // スキル成立音
                    if (m_AudioSrcSkillExec != null)
                    {
                        if (m_AudioSrcSkillExec.clip == null)
                        {
                            m_AudioSrcSkillExec.clip = clip;
                        }

                        if (m_AudioSrcSkillExec.isPlaying == false)
                        {
                            m_AudioSrcSkillExec.Play();
                        }
                    }
                    return;

                default:
                    // その他の音
                    break;
            }

            m_AudioSrcDefault.PlayOneShot(clip);
            //	再生終了予定時間を記録する(終了待ちが必要な物のみ)
            if (waitPlayEnd == true)
            {
                m_LastPlayTime = Time.time + clip.length;
            }
        }
        catch
        {
#if BUILD_TYPE_DEBUG
            Debug.LogError("PlaySE Sound Null! - " + nSoundLabel);
#endif
            return;
        }

    }


    //----------------------------------------------------------------------------
    //	@brief		ボイス再生
    //	@param[in]	fix_id			オーディオ再生情報マスターFixID
    //	@param[out]	playTime		クリップの長さ
    //----------------------------------------------------------------------------
    public void PlayVoice(uint fix_id, out float playTime)
    {
        playTime = 0.0f;


        try
        {

            if (m_AudioClipTable == null)
            {
                return;
            }

            MasterDataAudioData audioData = GetAudioMaster(fix_id);
            if (audioData == null)
            {
                return;
            }

            if (audioData.res_name != string.Empty)
            {
                AudioClip clip;
                bool ret;

                //	fix_idに該当するオーディオクリップの取得
                ret = m_AudioClipTable.TryGetValue(fix_id, out clip);
                if (ret)
                {
                    //	通常再生
                    m_AudioSrcVoice.PlayOneShot(m_AudioClipTable[fix_id], audioData.vol_lv / 100.0f);
                    if (audioData.ducking_disable != MasterDataDefineLabel.BoolType.ENABLE)
                    {
                        //	再生時間を返す
                        playTime = m_AudioClipTable[fix_id].length;
                    }
                }
            }
            else
            {
                //	ランダム再生
                m_WorkSelectVoice.Clear();

                uint id = 0;

                if (audioData.rand_id_00 != 0) m_WorkSelectVoice.Add(audioData.rand_id_00);
                if (audioData.rand_id_01 != 0) m_WorkSelectVoice.Add(audioData.rand_id_01);
                if (audioData.rand_id_02 != 0) m_WorkSelectVoice.Add(audioData.rand_id_02);
                if (audioData.rand_id_03 != 0) m_WorkSelectVoice.Add(audioData.rand_id_03);
                if (audioData.rand_id_04 != 0) m_WorkSelectVoice.Add(audioData.rand_id_04);
                if (audioData.rand_id_05 != 0) m_WorkSelectVoice.Add(audioData.rand_id_05);

                int nSelect = (int)RandManager.GetRand(0, (uint)m_WorkSelectVoice.m_BufferSize);
                id = m_WorkSelectVoice[nSelect];


                //	再生する音が選択できたらもう一度呼出
                PlayVoice(id, out playTime);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }


    //----------------------------------------------------------------------------
    //	@brief		グループIDに該当するマスターデータからマスターを検索
    //	@param[in]	fix_id		マスターデータのFixID
    //----------------------------------------------------------------------------
    private MasterDataAudioData GetAudioMaster(uint fix_id)
    {
        for (int i = 0; i < m_AudioDataList.m_BufferSize; i++)
        {
            if (m_AudioDataList[i].fix_id != fix_id)
            {
                continue;
            }

            return m_AudioDataList[i];
        }

        return null;
    }

    //----------------------------------------------------------------------------
    //	@brief		オーディオ読み込み関連の変数初期化
    //----------------------------------------------------------------------------
    private void ResetLoadAudioResource()
    {
        //	グループIDから抽出したマスターをクリア
        if (m_AudioDataList != null)
        {
            m_AudioDataList.Release();
        }

        //	オーディオクリップ管理テーブルをクリア
        m_AudioClipTable.Clear();
    }



} // class SoundManager


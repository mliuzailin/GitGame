using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


//----------------------------------------------------------------------------
//	class
//----------------------------------------------------------------------------
//----------------------------------------------------------------------------
/*!
	@brief		BGM再生データ
*/
//----------------------------------------------------------------------------
public class BGMPlayData
{
    public static double PLAYDATA_DELAY_NONE = 0.0d;

    private bool m_dataLoop = false;                    //!< ループフラグ
    private bool m_dataUsing = false;                   //!< 使用中フラグ
    private string m_dataBGMPath = "";                      //!< リソース名
    private float m_dataPlayVolume = 0.0f;                  //!< 再生ボリューム
    private double m_dataDelayTime = 0.0d;                  //!< 再生までのディレイ
    private float m_dataFadeTime = 0.0f;                    //!< フェードイン所要時間
    private float m_dataFadeSpeed = 0.0f;                   //!< フェードイン変化量
    private AudioClip m_dataAudioClip = null;                   //!< オーディオクリップ

    #region ==== アクセサ群 ====
    //	@brief		ループフラグ取得
    public bool dataLoop { get { return m_dataLoop; } }

    //	@brief		使用中フラグ取得
    public bool dataUsing { get { return m_dataUsing; } }

    //	@brief		再生データパス取得
    public string dataBGMPath { get { return m_dataBGMPath; } }

    //	@brief		再生開始までのディレイ取得
    public double dataDelayTime { get { return m_dataDelayTime; } }

    //	@brief		再生ボリューム取得
    public float dataPlayVolume { get { return m_dataPlayVolume; } }

    //	@brief		フェードイン時間取得
    public float dataFadeTime { get { return m_dataFadeTime; } }

    //	@brief		フェードイン速度取得
    public float dataFadeSpeed { get { return m_dataFadeSpeed; } }

    //	@brief		オーディオソース取得
    public AudioClip dataAudioClip { get { return m_dataAudioClip; } }
    #endregion


    //------------------------------------------------------------------------
    //	@brief		コンストラクタ
    //------------------------------------------------------------------------
    public BGMPlayData()
    {
        m_dataLoop = false;
        m_dataUsing = false;
        m_dataBGMPath = "";
        m_dataPlayVolume = 0.0f;
        m_dataDelayTime = 0.0d;
        m_dataFadeTime = 0.0f;
        m_dataFadeSpeed = 0.0f;
        m_dataAudioClip = null;
    }


    //------------------------------------------------------------------------
    //	@brief		セットアップ
    //	@param[in]	BGMPlayData		(data)			再生設定
    //------------------------------------------------------------------------
    public void Setup(BGMPlayData data)
    {
        if (data == null)
        {
            return;
        }

        m_dataLoop = data.m_dataLoop;
        m_dataUsing = true;
        m_dataBGMPath = data.m_dataBGMPath;
        m_dataPlayVolume = data.m_dataPlayVolume;
        m_dataDelayTime = data.m_dataDelayTime;
        m_dataFadeTime = data.m_dataFadeTime;
        m_dataFadeSpeed = data.m_dataFadeSpeed;
        m_dataAudioClip = data.m_dataAudioClip;
    }


    //------------------------------------------------------------------------
    //	@brief		セットアップ
    //	@param[in]	string			(path)			BGMリソース名
    //	@param[in]	float			(volume)		再生ボリューム
    //	@param[in]	float			(delayTime)		再生までの待機時間(sec)
    //	@param[in]	float			(fadeTime)		フェードイン時間
    //	@param[in]	bool			(loop)			ループ再生設定
    //------------------------------------------------------------------------
    public void Setup(string path, float volume, double delayTime, float fadeTime, bool loop)
    {
        m_dataLoop = loop;
        m_dataUsing = true;
        m_dataBGMPath = path;
        m_dataPlayVolume = volume;
        m_dataDelayTime = delayTime;
        m_dataFadeTime = fadeTime;

        if (fadeTime > 0.0f)
        {
            m_dataFadeSpeed = (volume / fadeTime);
        }

        m_dataAudioClip = null;

    }


    //------------------------------------------------------------------------
    //	@brief		セットアップ
    //	@param[in]	AudioClip		(clip)			再生オーディオクリップ
    //	@param[in]	float			(volume)		再生ボリューム
    //	@param[in]	float			(delayTime)		再生までの待機時間(sec)
    //	@param[in]	float			(fadeTime)		フェードイン時間
    //	@param[in]	bool			(loop)			ループ再生設定
    //------------------------------------------------------------------------
    public void Setup(AudioClip clip, float volume, double delayTime, float fadeTime, bool loop)
    {

        m_dataLoop = loop;
        m_dataUsing = true;
        m_dataBGMPath = clip.name;
        m_dataPlayVolume = volume;
        m_dataDelayTime = delayTime;
        m_dataFadeTime = fadeTime;

        if (fadeTime > 0.0f)
        {
            m_dataFadeSpeed = (volume / fadeTime);
        }

        m_dataAudioClip = clip;

    }


    //------------------------------------------------------------------------
    //	@brief		データクリア
    //------------------------------------------------------------------------
    public void Clear()
    {
        m_dataLoop = false;
        m_dataUsing = false;
        m_dataBGMPath = "";
        m_dataDelayTime = 0.0d;
        m_dataFadeTime = 0.0f;
        m_dataFadeSpeed = 0.0f;
        m_dataAudioClip = null;
    }

} // class BGMPlayData



//----------------------------------------------------------------------------
//	class
//----------------------------------------------------------------------------
//----------------------------------------------------------------------------
/*!
	@brief		BGM再生状態
*/
//----------------------------------------------------------------------------
public class BGMPlayState
{
    public const int BGM_STATE_NONE = 0;                    //!< 再生状態：無し
    public const int BGM_STATE_READY = 1;                   //!< 再生状態：準備中
    public const int BGM_STATE_PLAYING = 2;                 //!< 再生状態：再生中
    public const int BGM_STATE_FADEOUT = 3;                 //!< 再生状態：フェードアウト
    public const int BGM_STATE_FADEIN = 4;                  //!< 再生状態：フェードイン

    public float m_stopFadeTime = 0.0f;                 //!< フェードアウト所要時間
    public float m_stopFadeSpeed = 0.0f;                    //!< フェードアウト変化量
    public float m_playVolume = 0.0f;                   //!< 再生ボリューム
    public float m_playTime = 0.0f;                 //!< 再生時間
    public int m_playState = BGM_STATE_NONE;		//!< 再生ステータス

} // class BGMPlayState



//----------------------------------------------------------------------------
//	class
//----------------------------------------------------------------------------
//----------------------------------------------------------------------------
//	@class		BGMAudioSource
//	@brief		BGMチャンネル管理クラス
//----------------------------------------------------------------------------
public class BGMAudioSource
{
    private BGMPlayData m_PlayData = new BGMPlayData();     //!< 再生データ
    private BGMPlayState m_PlayState = new BGMPlayState();      //!< 再生状態

    private AudioSource m_AudioSource = null;                       //!< BGM再生チャンネル(1ch)
    private AudioClip m_AudioClip = null;                       //!< BGMクリップ

    private float m_DuckingScale = 1.0f;                        //!< ダッキングスケール


    //	@brief		状態取得
    public int playState { get { return m_PlayState.m_playState; } private set { } }

    //	@brief		ミュート状態取得
    public bool isMute { get { return m_AudioSource.mute; } private set { } }

    //	@brief		ダッキングスケール
    public float duckingScale { set { m_DuckingScale = value; } }

    /// <summary>再生データ</summary>
    public BGMPlayData playData { get { return m_PlayData; } }
    /// <summary>BGMクリップ</summary>
    public AudioClip audioClip { get { return m_AudioClip; } }

    //------------------------------------------------------------------------
    //	@brief		セットアップ
    //------------------------------------------------------------------------
    public bool Setup(AudioSource src)
    {
        if (src == null)
        {
            return false;
        }

        m_PlayState.m_playState = BGMPlayState.BGM_STATE_NONE;
        m_PlayState.m_playVolume = 0.0f;

        m_AudioSource = src;
        m_AudioClip = null;

        return true;
    }


    //------------------------------------------------------------------------
    //	@brief		再生
    //------------------------------------------------------------------------
    public void Play(BGMPlayData data)
    {
        //--------------------------------------------------------------------
        //	エラーチェック
        //--------------------------------------------------------------------
        if (data == null)
        {
            return;
        }
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL BGMManager#Play:" + data.dataBGMPath);
#endif
        if (data.dataBGMPath.IsNullOrEmpty())
        {
            Debug.Log("bgmPath is null or empty");
            return;
        }

        // AssetBundle読み込み終了時にactionを呼ばれるとdataの中身が保障されないので一旦ローカルに保存
        BGMPlayData _data = new BGMPlayData();
        _data.Setup(data);

        Action<AudioClip> action = (clip) =>
        {
            //--------------------------------------------------------------------
            //	再生設定
            //--------------------------------------------------------------------
            m_PlayData.Setup(_data);


            //--------------------------------------------------------------------
            //	オーディオクリップセット
            //--------------------------------------------------------------------
            m_AudioClip = clip;


            //--------------------------------------------------------------------
            //	ステータス設定
            //--------------------------------------------------------------------
            m_PlayState.m_playTime = 0.0f;
            m_PlayState.m_playVolume = 0.0f;
            m_PlayState.m_playState = BGMPlayState.BGM_STATE_READY;
        };

        if (data.dataAudioClip != null)
        {
            action(data.dataAudioClip);
            return;
        }

        if (data.dataBGMPath == BGMManager.RESOURCE_BGM_01_1)
        {
            string res_path = GlobalDefine.DATAPATH_SOUND_BGM + data.dataBGMPath;
            AudioClip clip = Resources.Load(res_path) as AudioClip;
            action(clip);
            return;
        }

        string path = data.dataBGMPath;

        if (path.StartsWith(BGMManager.ASSETBUNELE_BATTLE_PREFIX))
        {
            string asset_bundle_name = "pack_" + path.Substring(0, (BGMManager.ASSETBUNELE_BATTLE_PREFIX + "0000").Length);
            AssetBundler.Create().SetAsAudioClip(asset_bundle_name, path,
                (clip) =>
                {
                    if (clip == null)
                    {
                        Debug.LogError("NOT_FOUND_AUDIOCLIP:" + path);
                        return;
                    }

                    action(clip);

                }
                ).Load();
        }
        else
        {
            AssetBundler.Create().SetAsAudioClip(BGMManager.ASSETBUNELE_MENU, path,
                (clip) =>
                {
                    if (clip == null)
                    {
                        Debug.LogError("NOT_FOUND_AUDIOCLIP:" + path);
                        return;
                    }

                    action(clip);

                }).Load();
        }


    }


    //------------------------------------------------------------------------
    //	@brief		停止
    //------------------------------------------------------------------------
    public void Stop(float fadeTime)
    {

        //--------------------------------------------------------------------
        //	フェードアウト設定
        //--------------------------------------------------------------------
        if (fadeTime > 0.0f)
        {
            m_PlayState.m_stopFadeTime = fadeTime;
            m_PlayState.m_stopFadeSpeed = m_PlayState.m_playVolume / fadeTime;
        }


        //--------------------------------------------------------------------
        //	ステータス更新
        //--------------------------------------------------------------------
        m_PlayState.m_playState = BGMPlayState.BGM_STATE_FADEOUT;

    }


    //------------------------------------------------------------------------
    //	@brief		ミュート
    //	@param[in]	flag		ミュートon/off
    //------------------------------------------------------------------------
    public void Mute(bool flag)
    {
        if (m_AudioSource == null)
        {
            return;
        }

        m_AudioSource.mute = flag;
    }


    //------------------------------------------------------------------------
    //	@brief		定期更新
    //------------------------------------------------------------------------
    public void Update()
    {

        //--------------------------------------------------------------------
        //	エラーチェック
        //--------------------------------------------------------------------
        if (m_AudioClip == null
        || m_PlayState == null
        || m_PlayData == null)
        {
            return;
        }


        //--------------------------------------------------------------------
        // 再生時間更新
        //--------------------------------------------------------------------
        m_PlayState.m_playTime += Time.deltaTime;


        //--------------------------------------------------------------------
        // 各ステータスでの状態処理 
        //--------------------------------------------------------------------
        switch (m_PlayState.m_playState)
        {
            case BGMPlayState.BGM_STATE_READY:
                //----------------------------------------------------------------
                //	再生待ち時間チェック
                //----------------------------------------------------------------
                {
                    m_AudioSource.clip = m_AudioClip;
                    m_AudioSource.loop = m_PlayData.dataLoop;

                    m_AudioSource.PlayScheduled(m_PlayData.dataDelayTime);

                    m_PlayState.m_playState = BGMPlayState.BGM_STATE_FADEIN;

                }
                break;


            case BGMPlayState.BGM_STATE_PLAYING:
                //----------------------------------------------------------------
                //	再生中
                //----------------------------------------------------------------
                break;


            case BGMPlayState.BGM_STATE_FADEIN:
                //----------------------------------------------------------------
                //	フェードイン
                //----------------------------------------------------------------
                {
                    float fadeSpeed = 0.0f;
                    if (m_PlayData.dataFadeTime <= 0.0f)
                    {
                        fadeSpeed = m_PlayData.dataPlayVolume;
                    }
                    else
                    {
                        fadeSpeed = m_PlayData.dataFadeSpeed * Time.deltaTime;
                    }

                    m_PlayState.m_playVolume += fadeSpeed;
                    if (m_PlayState.m_playVolume >= m_PlayData.dataPlayVolume)
                    {
                        m_PlayState.m_playVolume = m_PlayData.dataPlayVolume;
                        m_PlayState.m_playState = BGMPlayState.BGM_STATE_PLAYING;
                    }
                }
                break;


            case BGMPlayState.BGM_STATE_FADEOUT:
                //----------------------------------------------------------------
                //	フェードアウト
                //----------------------------------------------------------------
                {
                    float fadeSpeed;
                    // フェードアウト時間が０だった場合はボリュームが０になるようにする
                    if (m_PlayState.m_stopFadeTime <= 0.0f)
                    {
                        fadeSpeed = m_PlayState.m_playVolume;
                    }
                    else
                    {
                        fadeSpeed = m_PlayState.m_stopFadeSpeed * Time.deltaTime;
                    }

                    m_PlayState.m_playVolume -= fadeSpeed;
                    if (m_PlayState.m_playVolume <= 0.0f)
                    {
                        // ステータス停止
                        m_PlayState.m_playState = BGMPlayState.BGM_STATE_NONE;
                        m_PlayState.m_playVolume = 0.0f;

                        // 停止
                        m_AudioSource.Stop();
                        m_PlayData.Clear();

                        m_AudioSource.clip = null;
                        m_AudioClip = null;
                    }
                }
                break;


            case BGMPlayState.BGM_STATE_NONE:
            default:
                //----------------------------------------------------------------
                //	無し
                //----------------------------------------------------------------
                break;
        }


        //--------------------------------------------------------------------
        // ボリューム設定
        //--------------------------------------------------------------------
        if (m_AudioSource.volume != (m_PlayState.m_playVolume * m_DuckingScale))
        {
            m_AudioSource.volume = (m_PlayState.m_playVolume * m_DuckingScale);
        }
    }

    //------------------------------------------------------------------------
    //	@brief		指定のオーディオリソースが再生中か
    //	@retval		bool		[再生中/異なるオーディオリソース]
    //------------------------------------------------------------------------
    public bool ChkPlayingSound(BGMPlayData data)
    {
        if (m_PlayData.dataBGMPath != data.dataBGMPath)
        {
            return false;
        }

        return true;
    }

} // class BGMAudioSource



//============================================================================
//	class
//============================================================================
//----------------------------------------------------------------------------
/*!
	@class		BGMManager
	@brief		BGMの管理
*/
//----------------------------------------------------------------------------
public class BGMManager : SingletonComponent<BGMManager>
{
    //------------------------------------------------------------------------
    // BGMのID定義
    //------------------------------------------------------------------------
    public enum EBGM_ID : int
    {
        eBGM_INIT = 0,
        eBGM_a_1,
        eBGM_1_1,
        eBGM_2_1,
        eBGM_Story,
        eBGM_MAX,
    };

    public const string ASSETBUNELE_MENU = "pack_bgm_menu";                                 //!< メニュー用BGMアセットバンドル
    public const string ASSETBUNELE_BATTLE = "pack_bgm_battle_";                                //!< バトル用BGMアセットバンドル
    public const string ASSETBUNELE_BATTLE_PREFIX = "bgm_battle_";                                  //!< バトル用BGM判定
    public const string RESOURCE_BGM_0a_1 = "bgm_mm_bgm0a";                                 //!< BGM「無限」
    public const string RESOURCE_BGM_01_1 = "bgm_title_002";                                    //!< BGM「タイトル」
    public const string RESOURCE_BGM_02_1 = "bgm_mm_002";                                       //!< BGM「メインメニュー」
    public const string RESOURCE_BGM_TEST_INTRO = "pqd_bgm05_special_intro";                        //!< イントロ
    public const string RESOURCE_BGM_TEST_BODY = "pqd_bgm05_special_body";                         //!< 本体
    public const string RESOURCE_BGM_STORY = "";                                               //!< BGM「ストーリーパート」

    private const int DEF_BGM_AUDIOSRC_NUM = 2;                                             //!< BGMチャンネル数定義

    [SerializeField] AudioSource[] m_AudioSource = new AudioSource[DEF_BGM_AUDIOSRC_NUM];           //!< インスペクターでオーディオソースの参照を設定するための変数

    private string[] m_ResourceBGMName = new string[(int)EBGM_ID.eBGM_MAX];             //!< 埋め込みBGM名アクセス用配列

    private BGMPlayData[] m_BGMPlayData = new BGMPlayData[4];                               //!< BGM再生リクエストクラス
    private BGMAudioSource[] m_BGMAudioSource = new BGMAudioSource[DEF_BGM_AUDIOSRC_NUM];       //!< BGMチャンネル管理クラス(2ch)
                                                                                                //private int						m_CacheMuteSetting		= LocalSaveDefine.OPTION_BGM_OFF;					//!< 前回のオプション設定 // @Change Developer 2015/11/04 warning除去。
    private bool mDuckingEnable;
    private float mDuckingVolumeScale;
    private double mDuckingTime;
    private bool mIsMute = false;                                            //!< ミュートにしているかどうか

    const double DEF_INGAME_BGM_DELAY_DEFAULT = 0.3d;   //!< 再生までのディレイ（秒）：リクエストタイミングからどれくらいあとに再生されるか
    const double DEF_INGAME_BGM_CROSSFADE_TIME = 1.0d;  //!< イントロとメインループが重なる時間（秒）：現在のフォーマットでは１秒


    //------------------------------------------------------------------------
    //	@brief		フェードアウト中かどうか
    //------------------------------------------------------------------------
    public bool isFadeOut
    {
        private set { }

        get
        {
            //	全てのオーディオソースを検索し、フェードアウト中の物がないか調べる
            BGMAudioSource src = null;
            for (int i = 0; i < m_BGMAudioSource.Length; i++)
            {
                src = m_BGMAudioSource[i];
                if (src == null)
                {
                    continue;
                }

                // フェードアウト中かどうかステータスをチェック
                if (src.playState != BGMPlayState.BGM_STATE_FADEOUT)
                {
                    continue;
                }

                return true;
            }

            return false;
        }
    }


    //------------------------------------------------------------------------
    //	@brief		ダッキングリクエスト
    //	@param[in]	time			実行時間
    //------------------------------------------------------------------------
    public void Ducking(double time)
    {
        mDuckingTime = AudioSettings.dspTime + time;
    }

    /// <summary>
    /// 再生中のデータを取得
    /// </summary>
    /// <returns></returns>
    public BGMPlayData[] GetPlayingBGM()
    {
        List<BGMPlayData> playDatas = new List<BGMPlayData>();

        BGMAudioSource audio = null;
        for (int i = 0; i < m_BGMAudioSource.Length; ++i)
        {
            audio = m_BGMAudioSource[i];
            if (audio.playState != BGMPlayState.BGM_STATE_PLAYING
            && audio.playState != BGMPlayState.BGM_STATE_FADEIN)
            {
                continue;
            }

            if (audio.playData != null)
            {
                BGMPlayData playData = new BGMPlayData();
                playData.Setup(audio.playData);
                playDatas.Add(playData);
            }
        }

        return playDatas.ToArray();
    }

    //------------------------------------------------------------------------
    /*!
		@brief		BGM再生
		@param[in]	SoundManager.EBGM_ID		(eID)		再生するBGMのID
		@param[in]	bool						(fadein)	フェードイン
	*/
    //------------------------------------------------------------------------
    public void PlayBGM(BGMManager.EBGM_ID eID, bool fadein)
    {
        // 再生
        PlayBGM(m_ResourceBGMName[(int)eID], fadein);
    }


    //------------------------------------------------------------------------
    /*!
		@brief		BGM再生
		@param[in]	string		(strBGM)		BGMファイル名
		@param[in]	bool		(fadein)		フェードイン
	*/
    //------------------------------------------------------------------------
    public void PlayBGM(string strBGM, bool fadein)
    {
        if (strBGM.Length == 0) return;

        float fadeTime = 0.0f;
        if (fadein == true)
        {
            fadeTime = 1.0f;
        }

        PlayBGM(strBGM, 1.0f, BGMPlayData.PLAYDATA_DELAY_NONE, fadeTime, true);
    }


    //------------------------------------------------------------------------
    /*!
		@brief		BGM再生
		@param[in]	strBGM		BGMクリップ名
		@param[in]	volume		再生ボリューム
		@param[in]	delayTime	再生開始までのディレイ
		@param[in]	loop		ループ再生するか
	*/
    //------------------------------------------------------------------------
    public void PlayBGM(string strBGM, float volume, float fadeTime, bool loop)
    {
        PlayBGM(strBGM, volume, BGMPlayData.PLAYDATA_DELAY_NONE, fadeTime, loop);
    }


    //------------------------------------------------------------------------
    /*!
		@brief		BGM再生
		@param[in]	strBGM		BGMクリップ名
		@param[in]	volume		再生ボリューム
		@param[in]	delayTime	再生開始までのディレイ
		@param[in]	fadeTime	フェードIN/OUTにかかる時間
		@param[in]	loop		ループ再生するか
	*/
    //------------------------------------------------------------------------
    public void PlayBGM(string strBGM, float volume, double delayTime, float fadeTime, bool loop)
    {
        //	再生リクエスト保存領域検索
        BGMPlayData blankData = GetBlankPlayData();
        if (blankData == null)
        {
            return;
        }

        // BGM

        //	再生リクエストセットアップ
        blankData.Setup(strBGM, volume, delayTime, fadeTime, loop);


        //	既に再生しているものは受け付けない
        if (IsPlayingSound(blankData) == true)
        {
            blankData.Clear();
        }
    }


    //------------------------------------------------------------------------
    /*!
		@brief		BGM再生
	*/
    //------------------------------------------------------------------------
    public void PlayBGM(AudioClip clip, float volume, double delayTime, float fadeTime, bool loop)
    {
        //	再生リクエスト保存領域検索
        BGMPlayData blankData = GetBlankPlayData();
        if (blankData == null)
        {
            return;
        }


        //	再生リクエストセットアップ
        blankData.Setup(clip, volume, delayTime, fadeTime, loop);


        //	既に再生しているものは受け付けない
        if (IsPlayingSound(blankData) == true)
        {

            blankData.Clear();

        }
    }


    //------------------------------------------------------------------------
    /*!
		@brief		BGM停止
		@param[in]	bool		(fade)		フェード
		@param[in]	float		(sec)		時間(秒)
	*/
    //------------------------------------------------------------------------
    public void StopBGM(bool fade, float sec)
    {

        //	再生中の全てのオーディオソースに対し、停止命令を出す
        BGMAudioSource audio = null;
        for (int i = 0; i < m_BGMAudioSource.Length; i++)
        {
            audio = m_BGMAudioSource[i];
            // 再生中の物を停止していく
            if (audio.playState != BGMPlayState.BGM_STATE_PLAYING
            && audio.playState != BGMPlayState.BGM_STATE_FADEIN)
            {
                continue;
            }

            //	停止
            audio.Stop(sec);

        }
    }

    /// <summary>
    /// BGMのミュート設定
    /// </summary>
    public void MuteBGM(bool is_mute)
    {
        mIsMute = is_mute;
    }

    /// <summary>
    /// バトルＢＧＭのアセットバンドル名を取得
    /// </summary>
    /// <param name="story_bgm_id"></param>
    /// <returns></returns>
    public static string getStoryBgmAssetBundleName(int story_bgm_id)
    {
        string ret_val = string.Format("pack_bgm_story_{0:D4}"
            , story_bgm_id
        );

        return ret_val;
    }

    /// <summary>
    /// バトルＢＧＭのアセット名を取得
    /// </summary>
    /// <param name="story_bgm_id"></param>
    /// <returns></returns>
    public static string getStoryBgmName(int story_bgm_id)
    {
        string ret_val = string.Format("bgm_story_{0:D4}"
            , story_bgm_id
        );

        return ret_val;
    }

    /// <summary>
    /// バトルＢＧＭのアセットバンドル名を取得
    /// </summary>
    /// <param name="battle_bgm_id"></param>
    /// <returns></returns>
    public static string getBattleBgmAssetBundleName(int battle_bgm_id)
    {
        string ret_val = string.Format("pack_bgm_battle_{0:D4}"
            , battle_bgm_id / 100
        );

        return ret_val;
    }

    /// <summary>
    /// バトルＢＧＭのアセット名を取得
    /// </summary>
    /// <param name="battle_bgm_id"></param>
    /// <returns></returns>
    public static string getBattleBgmName(int battle_bgm_id)
    {
        string ret_val = string.Format("bgm_battle_{0:D4}_{1:D2}"
            , battle_bgm_id / 100
            , battle_bgm_id % 100
        );

        return ret_val;
    }

    /// <summary>
    /// バトルＢＧＭ再生処理
    /// </summary>
    /// <param name="bgm_asset_name">アセット名(「bgm_battle_0012_78」等)</param>
    /// <param name="is_fade"></param>
    /// バトルＢＧＭのデータ命名規則
    /// アセットバンドル名は「pack_bgm_battle_0000」。0000のところにはイベント単位で番号を振っていく。0000はデフォルトＢＧＭが登録済み
    /// アセット名は「bgm_battle_0000_00」。0000のところはイベント番号。00の部分はＢＧＭ番号。
    /// 敵グループマスターデータ上で再生したいＢＧＭを指定するには、「イベント番号 × 100 + ＢＧＭ番号」を設定する（例：「bgm_battle_0012_78」なら「1278」を設定）
    /// アセット名+"i" のＢＧＭデータをアセットバンドルに含めておけばこのデータはイントロデータになる
    /// 　（例：「bgm_battle_0012_78」と「bgm_battle_0012_78i」があれば、「bgm_battle_0012_78i」に続けて「bgm_battle_0012_78」がループ再生される）
    /// アセット名+"j" のＢＧＭデータをアセットバンドルに含めておけばこのデータはイントロデータになる
    /// 　こちらもイントロデータだがイントロ部とループ部をクロスフェード（１秒）させて再生する（イントロデータはループ部の先頭１秒を含んだデータにする必要がある(v400では主にこの形式でイントロデータが作成されている)）
    public static void PlayBattleBGM(string bgm_asset_name, bool is_fade, List<string> bgm_asset_name_list = null)
    {
        string asset_bundle_name = "pack_" + bgm_asset_name.Substring(0, (BGMManager.ASSETBUNELE_BATTLE_PREFIX + "0000").Length);
        PlayBGM(asset_bundle_name, bgm_asset_name, is_fade, bgm_asset_name_list);
    }

    public static void PlayBGM(string asset_bundle_name, string bgm_asset_name, bool is_fade, List<string> bgm_asset_name_list = null)
    {
        if (bgm_asset_name_list != null)
        {
            bool is_exist_intro = false;
            bool is_exist_body = false;
            bool is_cross_fade_intro = false;

            for (int idx = 0; idx < bgm_asset_name_list.Count; idx++)
            {
                string work_bgm_asset_name = bgm_asset_name_list[idx];

                if (is_exist_intro == false)
                {
                    if (work_bgm_asset_name == bgm_asset_name + "i")
                    {
                        is_exist_intro = true;
                        is_cross_fade_intro = false;
                    }
                    else if (work_bgm_asset_name == bgm_asset_name + "j")
                    {
                        is_exist_intro = true;
                        is_cross_fade_intro = true;
                    }
                }

                if (is_exist_body == false
                    && work_bgm_asset_name == bgm_asset_name
                )
                {
                    is_exist_body = true;
                }
            }

            if (is_exist_body || is_exist_intro)
            {
                AudioClip audio_clip_intro = null;
                AudioClip audio_clip_body = null;

                AssetBundlerMultiplier assetbundler_multiplier = AssetBundlerMultiplier.Create();
                if (is_exist_intro)
                {
                    string intro_asset_name;
                    if (is_cross_fade_intro)
                    {
                        intro_asset_name = bgm_asset_name + "j";
                    }
                    else
                    {
                        intro_asset_name = bgm_asset_name + "i";
                    }

                    assetbundler_multiplier.Add(
                        AssetBundler.Create()
                            .SetAsAudioClip(asset_bundle_name, intro_asset_name,
                                (o) =>
                                {
                                    audio_clip_intro = o;
                                }
                        )
                    );
                }

                if (is_exist_body)
                {
                    assetbundler_multiplier.Add(
                        AssetBundler.Create()
                            .SetAsAudioClip(asset_bundle_name, bgm_asset_name,
                                (o) =>
                                {
                                    audio_clip_body = o;
                                }
                        )
                    );
                }

                assetbundler_multiplier.Load(
                    () =>
                    {
                        // BGM再生
                        _PlayBattleBgmSub(audio_clip_intro, audio_clip_body, is_fade, is_cross_fade_intro);
                    }
                );
            }
        }
        else
        {
            // データを読み込み
            AssetBundler.Create().SetAsAudioClipPack(asset_bundle_name
                , (clips) =>
                {
                    AudioClip audio_clip_intro = null;
                    AudioClip audio_clip_body = null;
                    bool is_cross_fade_intro = false;

                    for (int idx = 0; idx < clips.Count; idx++)
                    {
                        // イントロ部分のデータ
                        if (audio_clip_intro == null)
                        {
                            if (clips[idx].name == bgm_asset_name + "j")
                            {
                                // クロスフェード用１秒ありのイントロデータ（v400は主にこちらの方式を使っている模様）
                                audio_clip_intro = clips[idx];
                                is_cross_fade_intro = true;
                            }
                            else if (clips[idx].name == bgm_asset_name + "i")
                            {
                                // イントロデータ（通常のイントロデータ）
                                audio_clip_intro = clips[idx];
                            }
                        }

                        // ループ部分のデータ
                        if (audio_clip_body == null)
                        {
                            if (clips[idx].name == bgm_asset_name)
                            {
                                audio_clip_body = clips[idx];
                            }
                        }
                    }

                    // BGM再生
                    _PlayBattleBgmSub(audio_clip_intro, audio_clip_body, is_fade, is_cross_fade_intro);
                }
                , (str) =>
                {
                }
            ).Load();
        }
    }

    public static void PlayStoryBGM(StoryBGMData bgmData)
    {
        BGMManager.Instance.PlayBGM(bgmData.audio_clip, 1.0f, AudioSettings.dspTime + DEF_INGAME_BGM_DELAY_DEFAULT, 1.0f, true);
    }

    /// <summary>
    /// イントロありバトルＢＧＭ再生処理
    /// </summary>
    /// <param name="audio_clip_intro">イントロ部分のＢＧＭデータ</param>
    /// <param name="audio_clip_body">ループ部分のＢＧＭデータ</param>
    /// <param name="is_fade">フェードインするかどうか</param>
    /// <param name="is_cross_fade_intro">イントロ部分とループ部分のつなぎでクロスフェードさせるかどうか</param>
    /// クロスフェードする場合はイントロ部分は１秒長い（ループ部分の先頭１秒を含んだ）データである必要がある。
    /// BGMMnager のフェード処理は PlayScheduled() を考慮していないっぽいのでクロスフェードは意図した通りに動作しないかも（問題がありそうだがとりあえずv400と同じ動作のままで実装）
    /// v400のＢＧＭデータはクロスフェードありなしが混在しているっぽい。v400の最近のデータはクロスフェードありのデータになっている模様。
    private static void _PlayBattleBgmSub(AudioClip audio_clip_intro, AudioClip audio_clip_body, bool is_fade, bool is_cross_fade_intro)
    {
        float time_fade = 0.0f;
        if (is_fade)
        {
            time_fade = (float)DEF_INGAME_BGM_CROSSFADE_TIME;
        }

        double start_time_intro = AudioSettings.dspTime + DEF_INGAME_BGM_DELAY_DEFAULT; // イントロ部開始時刻
        double start_time_body = start_time_intro;  // ループ部開始時刻(start_time_intro 基準で計算すること（AudioSettings.dspTimeを直接取得するとＣＰＵ処理時間の分だけ時間が変わっている可能性もある）)

        // イントロ
        if (audio_clip_intro != null)
        {
            BGMManager.Instance.PlayBGM(audio_clip_intro, 1.0f, start_time_intro, time_fade, false);

            if (is_cross_fade_intro)
            {
                start_time_body = start_time_intro + audio_clip_intro.length - DEF_INGAME_BGM_CROSSFADE_TIME;
                time_fade = (float)DEF_INGAME_BGM_CROSSFADE_TIME;
            }
            else
            {
                start_time_body = start_time_intro + audio_clip_intro.length;
                time_fade = 0.0f;
            }
        }

        // メインループ
        if (audio_clip_body != null)
        {
            BGMManager.Instance.PlayBGM(audio_clip_body, 1.0f, start_time_body, time_fade, true);
        }
    }


    //------------------------------------------------------------------------
    /*!
		@brief		開始処理
	*/
    //------------------------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();


        //--------------------------------------------------------------------
        //	データ領域作成
        //--------------------------------------------------------------------
        for (int i = 0; i < m_BGMPlayData.Length; i++)
        {
            m_BGMPlayData[i] = new BGMPlayData();
        }


        //--------------------------------------------------------------------
        //	再生管理クラス生成
        //--------------------------------------------------------------------
        for (int i = 0; i < m_BGMAudioSource.Length; i++)
        {
            m_BGMAudioSource[i] = new BGMAudioSource();
            m_BGMAudioSource[i].Setup(m_AudioSource[i]);
        }
    }


    //------------------------------------------------------------------------
    /*!
		@brief		定期更新前処理
	*/
    //------------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();

        m_ResourceBGMName[(int)EBGM_ID.eBGM_a_1] = RESOURCE_BGM_0a_1;
        m_ResourceBGMName[(int)EBGM_ID.eBGM_1_1] = RESOURCE_BGM_01_1;
        m_ResourceBGMName[(int)EBGM_ID.eBGM_2_1] = RESOURCE_BGM_02_1;
        m_ResourceBGMName[(int)EBGM_ID.eBGM_Story] = RESOURCE_BGM_STORY;

        mDuckingVolumeScale = 1.0f;
    }


    //------------------------------------------------------------------------
    /*!
		@brief		定期更新処理
	*/
    //------------------------------------------------------------------------
    void Update()
    {

        if (!LocalSaveManager.HasInstance)
        {
            return;
        }

        //--------------------------------------------------------------------
        //	Mute設定
        //--------------------------------------------------------------------
        if (mIsMute == false)
        {
            LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
            if (cOption != null)
            {
                bool bOptionActiveBGM = (cOption.m_OptionBGM == (int)LocalSaveDefine.OptionBGM.OFF);
                Mute(bOptionActiveBGM);
            }
        }
        else
        {
            Mute(true);
        }

        //--------------------------------------------------------------------
        // 再生リクエスト処理 
        //--------------------------------------------------------------------
        BGMPlayData data = null;
        for (int i = 0; i < m_BGMPlayData.Length; i++)
        {
            data = m_BGMPlayData[i];
            if (data == null)
            {
                continue;
            }


            if (data.dataUsing == false)
            {
                continue;
            }


            // 再生クラスの空きを検索
            BGMAudioSource audio = null;
            for (int j = 0; j < m_BGMAudioSource.Length; j++)
            {
                audio = m_BGMAudioSource[j];
                if (audio == null)
                {
                    continue;
                }


                if (audio.playState != BGMPlayState.BGM_STATE_NONE)
                {
                    continue;
                }

                // 再生
                audio.Play(data);
                data.Clear();
                break;
            }

        }


        //--------------------------------------------------------------------
        //	ダッキング処理
        //--------------------------------------------------------------------
        if (mDuckingTime != 0.0f
        && mDuckingTime > AudioSettings.dspTime)
        {
            if (mDuckingEnable == false)
            {
                StartCoroutine(TransVolume(1.0f, 0.5f, 0.05f, true));
            }
        }
        else
        {
            if (mDuckingEnable == true)
            {
                StartCoroutine(TransVolume(0.5f, 1.0f, 0.6f, false));
            }
        }



        //--------------------------------------------------------------------
        //	再生管理クラス更新
        //--------------------------------------------------------------------
        for (int i = 0; i < m_BGMAudioSource.Length; i++)
        {
            BGMAudioSource audio = m_BGMAudioSource[i];
            if (audio == null)
            {
                continue;
            }


            if (audio.playState != BGMPlayState.BGM_STATE_READY
            && audio.playState != BGMPlayState.BGM_STATE_PLAYING
            && audio.playState != BGMPlayState.BGM_STATE_FADEIN
            && audio.playState != BGMPlayState.BGM_STATE_FADEOUT)
            {
                continue;
            }

            // 再生情報更新
            audio.Update();

            //	ダッキング処理
            audio.duckingScale = mDuckingVolumeScale;
        }

    }

    /// <summary>
    /// ミュート設定
    /// </summary>
    /// <param name="is_mute"></param>
    private void Mute(bool is_mute)
    {
        BGMAudioSource audio = null;
        for (int i = 0; i < m_BGMAudioSource.Length; i++)
        {
            audio = m_BGMAudioSource[i];
            if (audio == null)
            {
                continue;
            }

            if (audio.isMute != is_mute)
            {
                // ミュート設定
                audio.Mute(is_mute);
            }
        }
    }

    //------------------------------------------------------------------------
    //	@brief		ダッキング中のボリューム遷移処理 
    //	@param[in]	lerp_src		元ボリューム
    //	@param[in]	lerp_dst		先ボリューム
    //	@param[in]	lerp_time		変化にかかる時間
    //	@param[in]	end_val			遷移完了時のフラグ設定値
    //------------------------------------------------------------------------
    private IEnumerator TransVolume(float lerp_src, float lerp_dst, float lerp_time, bool end_val)
    {
        float time = lerp_time;
        float interval = 0.0f;

        while (time > interval)
        {
            interval += Time.deltaTime;
            mDuckingVolumeScale = Mathf.Lerp(lerp_src, lerp_dst, interval / time);

            yield return 0;
        }

        mDuckingEnable = end_val;
    }


    //------------------------------------------------------------------------
    /*!
		@brief		空き再生データ取得
		@return		PlayData		[未使用領域]
	*/
    //------------------------------------------------------------------------
    private BGMPlayData GetBlankPlayData()
    {

        BGMPlayData blankData = null;

        // 未使用再生データを検索
        BGMPlayData data = null;
        for (int i = 0; i < m_BGMPlayData.Length; i++)
        {
            data = m_BGMPlayData[i];
            if (data == null)
            {
                continue;
            }

            // 使用中かどうかチェック
            if (data.dataUsing == true)
            {
                continue;
            }

            blankData = data;
            break;
        }

        // 未使用データ領域を返す
        return blankData;

    }


    //------------------------------------------------------------------------
    /*!
		@brief		再生済みチェック
	*/
    //------------------------------------------------------------------------
    private bool IsPlayingSound(BGMPlayData data)
    {

        // 全てのオーディオソースを検索して再生中でないかを確認
        BGMAudioSource src = null;
        for (int i = 0; i < m_BGMAudioSource.Length; i++)
        {
            src = m_BGMAudioSource[i];
            if (src == null)
            {
                continue;
            }

            if (src.playState == BGMPlayState.BGM_STATE_NONE)
            {
                continue;
            }

            // 指定のサウンドを既に再生していないかチェック
            if (src.ChkPlayingSound(data) == false)
            {
                continue;
            }

            // 同じサウンドでもフェードアウト中なら再生していないことにする
            if (src.playState == BGMPlayState.BGM_STATE_FADEOUT)
            {
                return false;
            }

            return true;
        }

        return false;
    }


} // class BGMManager

using UnityEngine;
using System.Collections;

//----------------------------------------------------------------------------
/*!
	@class		SoundUtil
	@brief		サウンドユーティーリティ
*/
//----------------------------------------------------------------------------
static public class SoundUtil
{
    //------------------------------------------------------------------------
    /*!
		@brief		SE発行
		@param[in]	nSoundLabel		任意のSEラベル
	*/
    //------------------------------------------------------------------------
    static public void PlaySE(SEID nSoundLabel)
    {
        if (SoundManager.HasInstance)
        {
            SoundManager.Instance.PlaySE(nSoundLabel);
        }
    }


    //------------------------------------------------------------------------
    //	@brief		ボイス再生
    //	@param[in]	fix_id		再生情報FixID
    //------------------------------------------------------------------------
    static public void PlayVoice(uint fix_id)
    {
        if (SoundManager.HasInstance)
        {
            float playTime = 0.0f;

            SoundManager.Instance.PlayVoice(fix_id, out playTime);
            if (BGMManager.HasInstance)
            {
                bool voice_on = true;
                LocalSaveOption cOption = LocalSaveManager.Instance.LoadFuncOption();
                if (cOption != null)
                {
                    voice_on = (cOption.m_OptionVoice == (int)LocalSaveDefine.OptionVoice.ON);
                }
                if (voice_on == true)
                {
                    BGMManager.Instance.Ducking(playTime);
                }
            }
        }
    }


    //------------------------------------------------------------------------
    /*!
		@brief		BGM再生			<static>
		@param[in]	eID				任意のBGMID
		@param[in]	fadeIN			フェードイン有無
	*/
    //------------------------------------------------------------------------
    static public void PlayBGM(BGMManager.EBGM_ID eID, bool fadeIN)
    {
        if (BGMManager.HasInstance)
        {
            BGMManager.Instance.PlayBGM(eID, fadeIN);
        }
    }

    static public void PlayBGM(string strBGM, bool fadeIN)
    {
        if (BGMManager.HasInstance)
        {
            BGMManager.Instance.PlayBGM(strBGM, fadeIN);
        }
    }

    static public void PlayBGM(BGMPlayData playData)
    {
        if (playData != null || BGMManager.HasInstance)
        {
            if (playData.dataAudioClip == null)
            {
                BGMManager.Instance.PlayBGM(playData.dataBGMPath,
                                            playData.dataPlayVolume,
                                            playData.dataDelayTime,
                                            playData.dataFadeTime,
                                            playData.dataLoop);
            }
            else
            {
                BGMManager.Instance.PlayBGM(playData.dataAudioClip,
                                            playData.dataPlayVolume,
                                            playData.dataDelayTime,
                                            playData.dataFadeTime,
                                            playData.dataLoop);
            }
        }
    }


    //------------------------------------------------------------------------
    /*!
		@brief		BGM停止			<static>
		@param[in]	fade			フェードアウト有無
	*/
    //------------------------------------------------------------------------
    static public void StopBGM(bool fade)
    {
        if (BGMManager.HasInstance)
        {
            // 今は適当
            if (fade == true)
            {
                BGMManager.Instance.StopBGM(true, 1.5f);
            }
            else
            {
                BGMManager.Instance.StopBGM(true, 0.3f);
            }
        }
    }

} // class SoundUtil

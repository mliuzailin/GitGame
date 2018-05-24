/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	CtrUILabelSwitch.cs
	@brief	UI操作：ラベル切り替え
	@author Developer
	@date 	2014/07/02
*/
/*==========================================================================*/
/*==========================================================================*/
/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using UnityEngine;
using System.Collections;

/*==========================================================================*/
/*		script easy to find 												*/
/*==========================================================================*/
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
	@brief	UI操作：ラベル切り替え
*/
//----------------------------------------------------------------------------
public class CtrUILabelSwitch : MonoBehaviour
{
    //	const		int DEF_CYCLE_SWITCH		= 12;		//!< メッセージ自動切り替えサイクル（秒）
    const int DEF_CYCLE_SWITCH = 18;        //!< メッセージ自動切り替えサイクル（秒）
    const int DEF_CYCLE_SWITCH_OUT = 1;     //!< メッセージ自動切り替えサイクル（秒）

    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    public string[] m_LabelTextList = null;
    public Color[] m_LabelColorList = null;
    public float m_LabelFadeSpeed = 0.2f;
    public int m_LabelSwitchCycle = 5;
    public bool m_EnableResetFlg = false;


    private TemplateList<string> m_TextList = null;
    private TemplateList<Color> m_ColorList = null;     // 色も変えられるように

    private int m_MessageParamSwitchCt = 0;     //!< 表示切替処理：更新カウント
    private float m_ResetDeltaTotal = 0.0f;     //!< 表示切換処理：リセット時のDeltaTime総計

    private float m_MessageParamAlpha = 1.0f;       //!< 表示切替処理：アルファ値
    private bool m_MessageParamFixSet = false;  //!< 表示切替処理：強制的に文言差し替え

    private bool m_MessageParamFadeOne = false; //!< 表示フェード：文字列が1つでもフェード
    private Color m_LabelColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    private int m_LabelSwitchCycleMult = 1;
    private int m_LabelSwitchCycleMultOut = 1;


    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：※オブジェクトアクティブ時に呼び出し
	*/
    //----------------------------------------------------------------------------
    void OnEnable()
    {
        if (m_EnableResetFlg == true)
        {
            UnityUtil.SetUILabelValue(gameObject, "");
            CtrUILabelSwitch cLabelObject = gameObject.GetComponent<CtrUILabelSwitch>();
            if (cLabelObject != null)
            {
                cLabelObject.ResetSwitchAnimation();
            }
        }
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：初期化処理	※初回のUpdateを呼び出す直前に呼出し
	*/
    //----------------------------------------------------------------------------
    void Start()
    {
        //--------------------------------
        // インスペクタ側で設定があるならそれを反映
        //--------------------------------
        if (m_LabelTextList != null
        && m_LabelTextList.Length > 0
        )
        {
            ClrText();
            for (int i = 0; i < m_LabelTextList.Length; i++)
            {
                AddText(GameTextUtil.GetText(m_LabelTextList[i]));
            }
        }
        // 色も
        if (m_LabelColorList != null
        && m_LabelColorList.Length > 0
        )
        {
            ClrText();
            for (int i = 0; i < m_LabelColorList.Length; i++)
            {
                AddColor(m_LabelColorList[i]);
            }
        }

        //--------------------------------
        // 初回更新時に強制的にテキストを適用するためにフラグを立てておく
        //--------------------------------
        m_MessageParamFixSet = true;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	Unity固有処理：更新処理	※定期処理
	*/
    //----------------------------------------------------------------------------
    void Update()
    {
        //--------------------------------
        // 指定がないならカラ文字設定
        //--------------------------------
        if (m_TextList == null
        || m_TextList.m_BufferSize <= 0
        )
        {
            UnityUtil.SetUILabelValue(gameObject, " ");
            return;
        }

        //--------------------------------
        // レベルとプラス値が同時に表示発生している場合、
        // 自動遷移で切り替わりつつ表示する
        //--------------------------------
        {
            Color cColor;
            // 表示用のアルファ値：更新されてしまうので値をここで保持する
            float fOldMessageParamAlpha = m_MessageParamAlpha;
            //--------------------------------
            // １つだけしか指定が無い場合
            //--------------------------------
            if (m_TextList.m_BufferSize == 1
            && m_TextList[0] != null
            && m_MessageParamFadeOne == false
            )
            {
                UnityUtil.SetUILabelValue(gameObject, m_TextList[0]);

                // 色情報があれば更新する：あくまでテキストに追従する形で色変え
                if (m_ColorList != null
                && m_ColorList.m_BufferSize > 0)
                {
                    // 指定された色に強制的に変更
                    SetColor(m_ColorList[0]);
                }
                cColor = new Color(m_LabelColor.r, m_LabelColor.g, m_LabelColor.b, fOldMessageParamAlpha);
                UnityUtil.SetUILabelColor(gameObject, cColor);
                return;
            }

            int nCycleSwitchTotal = DEF_CYCLE_SWITCH * m_LabelSwitchCycleMult;
            int nCycleSwitchTotalOut = DEF_CYCLE_SWITCH_OUT * m_LabelSwitchCycleMultOut;

            //--------------------------------
            // 文言差し替えのタイミングを判断
            //--------------------------------
            bool bSwitch = false;
            int nMainMenuDeltaSecond = (int)((TimeManager.Instance.m_DeltaTotal - m_ResetDeltaTotal) * m_LabelSwitchCycle);
            if (m_MessageParamSwitchCt != nMainMenuDeltaSecond)
            {
                if (m_MessageParamSwitchCt < 0
                || m_MessageParamSwitchCt % nCycleSwitchTotal == (nCycleSwitchTotal - 1)
                )
                {
                    bSwitch = true;
                }
                m_MessageParamSwitchCt = nMainMenuDeltaSecond;
            }
            else
            if (m_MessageParamSwitchCt % nCycleSwitchTotal >= nCycleSwitchTotal - nCycleSwitchTotalOut)
            {
                m_MessageParamAlpha -= m_LabelFadeSpeed * TimeUtil.GetDeltaTimeRate();
                if (m_MessageParamAlpha <= 0.0f)
                {
                    m_MessageParamAlpha = 0.0f;
                }
            }
            else
            {
                m_MessageParamAlpha += m_LabelFadeSpeed * TimeUtil.GetDeltaTimeRate();
                if (m_MessageParamAlpha >= 1.0f)
                {
                    m_MessageParamAlpha = 1.0f;
                }
            }


            //--------------------------------
            // 文言差し替えのタイミングであれば差し替え。
            // 初回のみ初期化も兼ねて強制的に置き換える
            //--------------------------------
            if (bSwitch == true
            || m_MessageParamFixSet == true
            )
            {
                m_MessageParamFixSet = false;

                int nSwitchCtNum = (m_MessageParamSwitchCt / nCycleSwitchTotal) % m_TextList.m_BufferSize;
                UnityUtil.SetUILabelValue(gameObject, m_TextList[nSwitchCtNum]);
                // 色情報があれば更新する：あくまでテキストに追従する形で色変え
                if (m_ColorList != null
                && m_ColorList.m_BufferSize > nSwitchCtNum)
                {
                    // 指定された色に強制的に変更
                    SetColor(m_ColorList[nSwitchCtNum]);
                }
            }
            cColor = new Color(m_LabelColor.r, m_LabelColor.g, m_LabelColor.b, fOldMessageParamAlpha);
            UnityUtil.SetUILabelColor(gameObject, cColor);
        }

    }

    //----------------------------------------------------------------------------
    /*!
		@brief	テキスト登録件数取得
	*/
    //----------------------------------------------------------------------------
    public int GetTextCt()
    {
        if (m_TextList == null)
        {
            return 0;
        }
        return m_TextList.m_BufferSize;
    }
    //----------------------------------------------------------------------------
    /*!
		@brief	テキスト追加登録
	*/
    //----------------------------------------------------------------------------
    public void AddText(string strText)
    {
        if (m_TextList == null)
        {
            m_TextList = new TemplateList<string>();
        }

        m_TextList.Add(strText);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	テキスト追加削除
	*/
    //----------------------------------------------------------------------------
    public void ClrText()
    {
        m_TextList = null;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	強制的に文言更新
	*/
    //----------------------------------------------------------------------------
    public void MessageFixUpdate()
    {
        m_MessageParamFixSet = true;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	色変更
	*/
    //----------------------------------------------------------------------------
    public void SetColor(Color cColor)
    {
        m_LabelColor = cColor;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	色追加登録
	*/
    //----------------------------------------------------------------------------
    public void AddColor(Color cColor)
    {
        if (m_ColorList == null)
        {
            m_ColorList = new TemplateList<Color>();
            SetColor(cColor);
        }

        m_ColorList.Add(cColor);
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	色追加削除
	*/
    //----------------------------------------------------------------------------
    public void ClrColor()
    {
        m_ColorList = null;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	文字列が1つでもフェードするかの設定
	*/
    //----------------------------------------------------------------------------
    public void IsFadeOne(bool bFlag)
    {
        m_MessageParamFadeOne = bFlag;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	切り替わりのサイクル倍率
		@note	他のレイアウトとのサイクルが半端にずれないように、intでの指定に限定
	*/
    //----------------------------------------------------------------------------
    public void SetCycleMult(int nCycleMult, int nCycleOutMult)
    {
        m_LabelSwitchCycleMult = nCycleMult;
        m_LabelSwitchCycleMultOut = nCycleOutMult;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	よく使うレイアウトのデフォルト設定
	*/
    //----------------------------------------------------------------------------
    public void SetUILayoutDefaultParam()
    {
#if true
        m_LabelSwitchCycle = 10;
#else
		m_LabelSwitchCycle			= 10;
		m_LabelFadeSpeed			= 0.2f;
		m_LabelSwitchCycleMult		= 1;
		m_LabelSwitchCycleMultOut	= 4;
#endif
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	白いフォントの上に同じ文字を重ねて光らせる場合の設定
	*/
    //----------------------------------------------------------------------------
    public void SetUILayoutOverwriteParam()
    {
        m_LabelSwitchCycle = 10;
        m_LabelFadeSpeed = 0.2f;
        m_LabelSwitchCycleMult = 1;
        m_LabelSwitchCycleMultOut = 4;
    }

    //----------------------------------------------------------------------------
    /*!
		@brief	アニメーション初期化
	*/
    //----------------------------------------------------------------------------
    public void ResetSwitchAnimation()
    {
        m_MessageParamSwitchCt = 0;     //!< 表示切替処理：更新カウント

        m_MessageParamAlpha = 1.0f;     //!< 表示切替処理：アルファ値
        m_MessageParamFixSet = false;   //!< 表示切替処理：強制的に文言差し替え

        m_ResetDeltaTotal = TimeManager.Instance.m_DeltaTotal;  //!< 表示切換処理：リセット時のDeltaTime総計

        // 文字も初期値を出す
        if (m_TextList != null
        && m_TextList.m_BufferSize > 0)
        {
            UnityUtil.SetUILabelValue(gameObject, m_TextList[0]);
        }

        // 色変更を行っている場合は色を初期値に戻す
        if (m_ColorList != null
        && m_ColorList.m_BufferSize > 0)
        {
            SetColor(m_ColorList[0]);
            UnityUtil.SetUILabelColor(gameObject, m_ColorList[0]);
        }
    }
}


/*==========================================================================*/
/*		namespace End 														*/
/*==========================================================================*/

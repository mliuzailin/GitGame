/**
 *  @file   DebugPartyOffset.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/05/04
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using M4u;

public class DebugPartyOffset : M4uContextMonoBehaviour
{
    const float BUTTON_MOVE_RATE = 0.001f;

    public Action ClickNextAction = delegate { };
    public Action ClickPrevAction = delegate { };

    public Action ClickSkillCutinAction = delegate { };
    public Action ClickStoryViewAction = delegate { };

    public Action ClickClearAction = delegate { };

    public Action<uint> ChangedIDAction = delegate { };
    public Action<uint> ChangedNumberAction = delegate { };
    public Action<float> ChangedOffsetYAction = delegate { };
    public Action<float> ChangedOffsetXAction = delegate { };
    public Action<float> ChangedTilingAction = delegate { };

    [SerializeField]
    public InputField m_IdInputField;
    [SerializeField]
    public InputField m_NoInputField;

    [SerializeField]
    public InputField m_OffsetYInputField;
    [SerializeField]
    public InputField m_OffsetXInputField;
    [SerializeField]
    public InputField m_TilingInputField;

    [SerializeField]
    public GameObject ThirdUnitPanel;
    public Material ThirdUnitMaterial;

    public float m_adjustX = 0;
    public float m_adjustY = 0;
    public float m_adjustT = 0;

    public float m_X = 0;
    public float m_Y = 0;
    public float m_T = 0;

    M4uProperty<string> nameText = new M4uProperty<string>();
    public string NameText { get { return nameText.Value; } set { nameText.Value = value; } }

    M4uProperty<Color> offsetYTextColor = new M4uProperty<Color>(Color.black);
    public Color OffsetYTextColor { get { return offsetYTextColor.Value; } set { offsetYTextColor.Value = value; } }

    M4uProperty<Color> offsetXTextColor = new M4uProperty<Color>(Color.black);
    public Color OffsetXTextColor { get { return offsetXTextColor.Value; } set { offsetXTextColor.Value = value; } }

    M4uProperty<Color> tilingTextColor = new M4uProperty<Color>(Color.black);
    public Color TilingTextColor { get { return tilingTextColor.Value; } set { tilingTextColor.Value = value; } }

    M4uProperty<bool> isActiveSkillCutinButton = new M4uProperty<bool>();
    public bool IsActiveSkillCutinButton { get { return isActiveSkillCutinButton.Value; } set { isActiveSkillCutinButton.Value = value; } }

    M4uProperty<bool> isActiveStoryViewButton = new M4uProperty<bool>();
    public bool IsActiveStoryViewButton { get { return isActiveStoryViewButton.Value; } set { isActiveStoryViewButton.Value = value; } }

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        ThirdUnitMaterial = ThirdUnitPanel.GetComponent<Renderer>().materials[0];
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 変更前のUVの値を設定する
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="t"></param>
    public void SetupAdjustUV(float x, float y, float t)
    {
        m_adjustX = x;
        m_adjustY = y;
        m_adjustT = t;
    }

    /// <summary>
    /// UVの値を設定する
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="t"></param>
    public void SetupUV(float x, float y, float t)
    {
        m_X = x;
        m_Y = y;
        m_T = t;

        m_OffsetYInputField.text = m_Y.ToString("F3");
        m_OffsetXInputField.text = m_X.ToString("F3");
        m_TilingInputField.text = m_T.ToString("F3");
    }

    /// <summary>
    /// 変更前の値と違う場合はInputFieldの色を変える
    /// </summary>
    public void CheckAdjustUV()
    {
        OffsetXTextColor = (m_X == m_adjustX) ? Color.black : Color.red;
        OffsetYTextColor = (m_Y == m_adjustY) ? Color.black : Color.red;
        TilingTextColor = (m_T == m_adjustT) ? Color.black : Color.red;
    }

    public void OnClickNext()
    {
        if (ClickNextAction != null)
        {
            ClickNextAction();
        }
    }

    public void OnClickPrev()
    {
        if (ClickPrevAction != null)
        {
            ClickPrevAction();
        }
    }

    public void OnClickSkillCutin()
    {
        if (ClickSkillCutinAction != null)
        {
            ClickSkillCutinAction();
        }
    }

    public void OnClickStoryView()
    {
        if (ClickStoryViewAction != null)
        {
            ClickStoryViewAction();
        }
    }

    #region UV設定ボタン
    public void OnClickOffsetYUp()
    {
        m_Y += BUTTON_MOVE_RATE;
        m_OffsetYInputField.text = m_Y.ToString("F3");
        CheckAdjustUV();

        if (ChangedOffsetYAction != null)
        {
            ChangedOffsetYAction(m_Y);
        }
    }

    public void OnClickOffsetYDown()
    {
        m_Y -= BUTTON_MOVE_RATE;
        m_OffsetYInputField.text = m_Y.ToString("F3");
        CheckAdjustUV();

        if (ChangedOffsetYAction != null)
        {
            ChangedOffsetYAction(m_Y);
        }
    }

    public void OnClickOffsetXUp()
    {
        m_X += BUTTON_MOVE_RATE;
        m_OffsetXInputField.text = m_X.ToString("F3");
        CheckAdjustUV();

        if (ChangedOffsetXAction != null)
        {
            ChangedOffsetXAction(m_X);
        }
    }

    public void OnClickOffsetXDown()
    {
        m_X -= BUTTON_MOVE_RATE;
        m_OffsetXInputField.text = m_X.ToString("F3");
        CheckAdjustUV();

        if (ChangedOffsetXAction != null)
        {
            ChangedOffsetXAction(m_X);
        }
    }

    public void OnClickTilingUp()
    {
        m_T += BUTTON_MOVE_RATE;
        m_TilingInputField.text = m_T.ToString("F3");
        CheckAdjustUV();

        if (ChangedTilingAction != null)
        {
            ChangedTilingAction(m_T);
        }
    }

    public void OnClickTilingDown()
    {
        m_T -= BUTTON_MOVE_RATE;
        m_TilingInputField.text = m_T.ToString("F3");
        CheckAdjustUV();

        if (ChangedTilingAction != null)
        {
            ChangedTilingAction(m_T);
        }
    }
    #endregion

    public void OnClickClear()
    {
        m_X = m_adjustX;
        m_Y = m_adjustY;
        m_T = m_adjustT;
        m_OffsetYInputField.text = m_Y.ToString("F3");
        m_OffsetXInputField.text = m_X.ToString("F3");
        m_TilingInputField.text = m_T.ToString("F3");

        CheckAdjustUV();

        if (ClickClearAction != null)
        {
            ClickClearAction();
        }
    }

    #region テキスト入力
    public void OnEndEditID(string value)
    {
        uint u;
        uint.TryParse(value, out u);
        if (ChangedIDAction != null)
        {
            ChangedIDAction(u);
        }
    }

    public void OnEndEditNo(string value)
    {
        uint u;
        uint.TryParse(value, out u);
        if (ChangedNumberAction != null)
        {
            ChangedNumberAction(u);
        }
    }

    public void OnEndEditOffsetY(string value)
    {
        float.TryParse(value, out m_Y);
        m_OffsetYInputField.text = m_Y.ToString("F3");
        CheckAdjustUV();

        if (ChangedOffsetYAction != null)
        {
            ChangedOffsetYAction(m_Y);
        }
    }

    public void OnEndEditOffsetX(string value)
    {
        float.TryParse(value, out m_X);
        m_OffsetXInputField.text = m_X.ToString("F3");
        CheckAdjustUV();

        if (ChangedOffsetXAction != null)
        {
            ChangedOffsetXAction(m_X);
        }
    }

    public void OnEndEditTiling(string value)
    {
        float.TryParse(value, out m_T);
        m_TilingInputField.text = m_T.ToString("F3");
        CheckAdjustUV();

        if (ChangedTilingAction != null)
        {
            ChangedTilingAction(m_T);
        }
    }
    #endregion
}

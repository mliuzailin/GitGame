using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;
using DG.Tweening;

public class UnitResultBuildup : UnitResultBase
{
    [SerializeField]
    private GameObject m_levelUpEffectRoot;
    [SerializeField]
    private GameObject m_skillUpEffectRoot;

    private static readonly float IconDisappearInterval = 0.1f;

    private IconLevelUpEffect m_levelupEffect = null;
    private IconSkillUpEffect m_skillupEffect = null;
    private UnitResultBuildupModel m_model = null;

    public enum Status
    {
        None = -1,
        Material,
        LevelUp,
        SkillUp,
        LimitOver,
        Finish,
    }

    M4uProperty<bool> isViewLevelUp = new M4uProperty<bool>(false);
    public bool IsViewLevelUp
    {
        get { return isViewLevelUp.Value; }
        set
        {
            if (!IsViewLevelUp
                && value
                && m_levelupEffect == null)
            {
                m_levelupEffect = IconLevelUpEffect.Attach(m_levelUpEffectRoot).Show();
            }
            else if (IsViewLevelUp
                && !value
                && m_levelupEffect != null)
            {
                m_levelupEffect.Detach();
                m_levelupEffect = null;
            }


            isViewLevelUp.Value = value;
        }
    }

    M4uProperty<bool> isViewSkillUp = new M4uProperty<bool>(false);
    public bool IsViewSkillUp
    {
        get { return isViewSkillUp.Value; }
        set
        {
            if (!IsViewSkillUp
                && value
                && m_skillupEffect == null)
            {
                m_skillupEffect = IconSkillUpEffect.Attach(m_skillUpEffectRoot).Show();
            }
            else if (IsViewSkillUp
                && !value
                && m_skillupEffect != null)
            {
                m_skillupEffect.Detach();
                m_skillupEffect = null;
            }

            isViewSkillUp.Value = value;
        }
    }

    M4uProperty<bool> isViewLimitOver = new M4uProperty<bool>(false);
    public bool IsViewLimitOver { get { return isViewLimitOver.Value; } set { isViewLimitOver.Value = value; } }

    private Status m_Status = Status.None;
    private float m_TimeCounter = 0.0f;
    private Tweener m_Tweener = null;
    private UnitStatusPanel m_ParamPanel = null;

    private void Update()
    {
        m_TimeCounter += Time.deltaTime;

        switch (m_Status)
        {
            case Status.Material: updateMaterial(); break;
            case Status.LevelUp: updateLevelUp(); break;
            case Status.SkillUp: updateSkillUp(); break;
            case Status.LimitOver: updateLimitOver(); break;
            case Status.Finish:
                break;
        }
    }

    public void setup(
        UnitStatusPanel _paramPanel,
        UnitResultBuildupModel model,
        System.Action _finishCall)
    {
        m_ParamPanel = _paramPanel;
        m_model = model;
        m_FinishCallAction += _finishCall;
        m_Status = Status.None;
        m_Ready = true;
        nextStatus();
    }


    private void nextStatus()
    {
        m_Status = (Status)((int)m_Status + 1);
        bool bSkip = true;
        while (bSkip)
        {
            switch (m_Status)
            {
                case Status.Material:
                    bSkip = initMaterial();
                    break;
                case Status.LevelUp:
                    bSkip = initLevelUp();
                    break;
                case Status.SkillUp:
                    bSkip = initSkillUp();
                    break;
                case Status.LimitOver:
                    bSkip = initLimitOver();
                    break;
                case Status.Finish:
                    m_FinishCallAction();
                    StartCoroutine(MainMenuWebViewShowChk.PopupWebViewStart(MainMenuWebViewShowChk.PopupWebViewType.Mission));
                    bSkip = false;
                    break;
            }
            if (bSkip)
            {
                m_Status = (Status)((int)m_Status + 1);
            }
        }
    }

    private bool initMaterial()
    {
        m_TimeCounter = 0.0f;
        return false;
    }

    private void updateMaterial()
    {
        if (m_model.LeftMaterialCount() > 0)
        {
            if (m_TimeCounter > IconDisappearInterval)
            {
                m_TimeCounter = 0.0f;
                m_model.DisappearMaterial();
            }
        }
        else
        {
            nextStatus();
        }
    }

    private bool initLevelUp()
    {
        m_Tweener = DOTween.To(() => m_ParamPanel.ExpRate, rate => m_ParamPanel.ExpRate = rate, m_ParamPanel.BuildupExpRate, 0.5f);
        return false;
    }

    private void updateLevelUp()
    {
        if (IsViewLevelUp)
        {
            return;
        }

        if (m_Tweener != null && !m_Tweener.IsPlaying())
        {
            if (MainMenuParam.m_BlendBuildUpUnitPrev.level != MainMenuParam.m_BlendBuildUpUnitAfter.level)
            {
                IsViewLevelUp = true;
                StartCoroutine(DelayExec(nextStatus, 0.5f));
            }
            else
            {
                nextStatus();
            }
        }
    }

    private bool initLimitOver()
    {
        if (MainMenuParam.m_BlendBuildUpUnitPrev.limitover_lv != MainMenuParam.m_BlendBuildUpUnitAfter.limitover_lv)
        {
            SoundUtil.PlaySE(SEID.VOICE_INGAME_MM_LIMITOVER);
            IsViewLimitOver = true;
            StartCoroutine(DelayExec(nextStatus, 0.5f));
            return false;
        }
        return true;
    }

    private void updateLimitOver()
    {
    }

    private bool initSkillUp()
    {
        if (MainMenuParam.m_BlendBuildUpUnitPrev.limitbreak_lv != MainMenuParam.m_BlendBuildUpUnitAfter.limitbreak_lv)
        {
            IsViewSkillUp = true;
            StartCoroutine(DelayExec(nextStatus, 0.5f));
            return false;
        }
        return true;
    }

    private void updateSkillUp()
    {

    }

}

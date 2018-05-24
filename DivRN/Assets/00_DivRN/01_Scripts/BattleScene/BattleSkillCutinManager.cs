using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSkillCutinManager : SingletonComponent<BattleSkillCutinManager>
{
    public NewBattleSkillCutin m_NewBattleSkillCutin = null;
    private SkillRequestParam m_SkillRequestParam = null;

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
        base.OnDestroy();
    }

    public bool isRunning()
    {
        return m_NewBattleSkillCutin.isUpdating();
    }

    public void ClrSkillCutin()
    {
        m_NewBattleSkillCutin.clearSkill();
        m_SkillRequestParam = null;
    }
    public void CutinStart2(bool is_step_mode = false, bool is_no_chara_mode = false, bool is_no_cutin_mode = false)
    {
        m_NewBattleSkillCutin.startCutin(is_step_mode, is_no_chara_mode, is_no_cutin_mode);
    }

    public void SetSkillCutin(SkillRequestParam skillRequest)
    {
        for (int idx = 0; idx < skillRequest.getRequestCount(); idx++)
        {
            BattleSkillActivity skill_activity = skillRequest.getSkillRequestByIndex(idx);
            SetSkillCutin(skill_activity);
        }

        m_SkillRequestParam = skillRequest;
    }

    public SkillRequestParam getSkillRequestParam()
    {
        return m_SkillRequestParam;
    }

    private void SetSkillCutin(BattleSkillActivity cBattleSkillActivity)
    {
        if (cBattleSkillActivity != null)
        {
            string skill_name = cBattleSkillActivity.getMainText();
            ESKILLTYPE skill_type = cBattleSkillActivity.m_SkillType;
            MasterDataDefineLabel.ElementType skill_element = cBattleSkillActivity.m_Element;

            int id = m_NewBattleSkillCutin.addSkill(cBattleSkillActivity.m_SkillParamOwnerNum, skill_name, skill_type, skill_element, cBattleSkillActivity.m_SkillIndex);

            cBattleSkillActivity.m_CutinID = id;
        }
    }

    public void SkillCutinLeader(GlobalDefine.PartyCharaIndex charaIdx, CharaParty PlayerParty)
    {
        BattleSkillActivity activity = getLeaderSkillActivity(charaIdx, PlayerParty);

        SetSkillCutin(activity);
    }

    public void SkillCutinPassive(GlobalDefine.PartyCharaIndex charaIdx, uint skillID)
    {
        BattleSkillActivity activity = getSkillActivity(charaIdx, skillID, ESKILLTYPE.ePASSIVE);

        SetSkillCutin(activity);
    }

    public void SkillCutinRequest(GlobalDefine.PartyCharaIndex charaIdx, uint skillID, ESKILLTYPE skillType)
    {
        BattleSkillActivity activity = getSkillActivity(charaIdx, skillID, skillType);

        SetSkillCutin(activity);
    }

    /// <summary>
    /// 現在再生中のカットインのカットインIDを取得
    /// </summary>
    /// <returns></returns>
    public int GetPlayingCutinID()
    {
        return m_NewBattleSkillCutin.getCurrentSkillIndex();
    }

    /// <summary>
    /// 現在再生中のカットインのアニメが終了したかどうか
    /// </summary>
    /// <returns></returns>
    public bool GetPlayingCutinAnimEnd(bool is_exec_next_cutin)
    {
        return m_NewBattleSkillCutin.isWaitNextStep(is_exec_next_cutin);
    }

    /// <summary>
    /// 現在再生中のカットインのスキル発動者を取得
    /// </summary>
    /// <returns></returns>
    public GlobalDefine.PartyCharaIndex GetPlayingCutinOwner()
    {
        return m_NewBattleSkillCutin.getCurrentSkillCaster();
    }

    public void setSpeedUp(bool is_speed_up)
    {
        m_NewBattleSkillCutin.setSpeedUp(is_speed_up);
    }

    private static BattleSkillActivity getSkillActivity(GlobalDefine.PartyCharaIndex charaIdx, uint skillID, ESKILLTYPE skillType)
    {
        BattleSkillActivity activity = new BattleSkillActivity();

        //--------------------------------
        // スキル情報を設定
        //--------------------------------
        activity.m_SkillParamOwnerNum = charaIdx;
        activity.m_SkillParamFieldID = 0;
        activity.m_SkillParamSkillID = skillID;
        activity.m_SkillType = skillType;

        return activity;
    }

    private static BattleSkillActivity getLeaderSkillActivity(GlobalDefine.PartyCharaIndex charaIdx, CharaParty PlayerParty)
    {
        BattleSkillActivity activity = new BattleSkillActivity();
        if (activity == null)
        {
            return null;
        }

        if (charaIdx < 0 || charaIdx >= GlobalDefine.PartyCharaIndex.MAX)
        {
            return null;
        }

        if (PlayerParty == null)
        {
            return null;
        }
        // パーティリーダーキャラ情報を取得
        CharaOnce chara = PlayerParty.getPartyMember(charaIdx, CharaParty.CharaCondition.EXIST);
        if (chara == null)
        {
            return null;
        }

        if (chara == null)
        {
            return null;
        }

        if (!chara.m_bHasCharaMasterDataParam)
        {
            return null;
        }

        MasterDataSkillLeader skill = BattleParam.m_MasterDataCache.useSkillLeader(chara.m_CharaMasterDataParam.skill_leader);
        if (skill == null)
        {
            return null;
        }

        //--------------------------------------------------------------------
        // スキル発動情報設定
        //--------------------------------------------------------------------
        activity.m_SkillParamOwnerNum = charaIdx;
        activity.m_SkillParamFieldID = 0;
        activity.m_SkillParamSkillID = chara.m_CharaMasterDataParam.skill_leader;
        activity.m_SkillType = ESKILLTYPE.eLEADER;

        return activity;
    }

    public void setResurrectInfo(int resurrect_value)
    {
        m_NewBattleSkillCutin.setResurrectInfo(resurrect_value);
    }
}

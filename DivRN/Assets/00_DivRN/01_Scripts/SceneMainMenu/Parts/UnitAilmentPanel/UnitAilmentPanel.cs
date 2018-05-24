using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;
using ServerDataDefine;

public class UnitAilmentPanel : MenuPartsBase
{
    M4uProperty<List<UnitAilmentContext>> ailment_info_list = new M4uProperty<List<UnitAilmentContext>>();
    public List<UnitAilmentContext> Ailment_info_list { get { return ailment_info_list.Value; } set { ailment_info_list.Value = value; } }

    List<GameObject> ailment_obj_list = new List<GameObject>();
    public List<GameObject> Ailment_obj_list { get { return ailment_obj_list; } set { ailment_obj_list = value; } }

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;

        Ailment_info_list = new List<UnitAilmentContext>();
    }

    private void AddAilmentInfo(string detail, int turn)
    {
        UnitAilmentContext _newAilment = new UnitAilmentContext();
        _newAilment.setupAilmentInfo(detail, turn);
        Ailment_info_list.Add(_newAilment);
    }

    public void setupCharaAilmentInfo(StatusAilmentChara ailmentChara)
    {
        AddAilmentInfo(GameTextUtil.GetText("battle_infotext1"), -1);
        int count = 1;
        for (int num = 0; num < (int)MasterDataDefineLabel.AilmentType.MAX; ++num)
        {
            if (ailmentChara.IsHavingAilment((MasterDataDefineLabel.AilmentType)num) == false)
            {
                continue;
            }

            int turn = 0;
            string detail = "";
            // 残りターン数を取得
            for (int n = 0; n < ailmentChara.GetAilmentCount(); ++n)
            {
                StatusAilment status_ailment = ailmentChara.GetAilment(n);
                if (status_ailment == null)
                {
                    continue;
                }

                MasterDataDefineLabel.AilmentType type = status_ailment.nType;
                if (type != (MasterDataDefineLabel.AilmentType)num)
                {
                    continue;
                }

                turn = status_ailment.nLife;
                MasterDataStatusAilmentParam master = MasterDataUtil.GetMasterDataStatusAilmentParam(status_ailment.nMasterDataStatusAilmentID);
                detail = master.detail;
                if (detail == "")
                {
                    detail = master.name;
                }
                break;
            }
            AddAilmentInfo(detail, turn);
            ++count;
        }
        if (count == 1)
        {
            AddAilmentInfo("-", -1);
        }
    }

    private void AddAbilityInfo(string detail, int turn)
    {
        string wrk_string = detail;
        wrk_string = wrk_string.Replace("\r\n", "\n");
        wrk_string = wrk_string.Replace("\r", "\n");

        // 行数を計算
        int line_count = 1;
        int start_index = 0;
        while (true)
        {
            int kaigyo_index = wrk_string.IndexOf("\n", start_index);
            if (kaigyo_index < 0)
            {
                break;
            }

            line_count++;
            start_index = kaigyo_index + 1;
        }

        {
            UnitAilmentContext _newAilment = new UnitAilmentContext();
            _newAilment.setupAbilityInfo(detail, turn);
            Ailment_info_list.Add(_newAilment);
        }

        // 表示行数分の空間を開ける
        for (int idx = 1; idx < line_count; idx++)
        {
            UnitAilmentContext _newAilment = new UnitAilmentContext();
            _newAilment.setupAbilityInfo("", -1);
            Ailment_info_list.Add(_newAilment);
        }
    }

    public void setupEnemyAbilityInfo(BattleEnemy enemy)
    {
        //--------------------------------
        // エネミーマスターを取得
        //--------------------------------
        MasterDataParamEnemy enemyMaster = enemy.getMasterDataParamEnemy();
        if (enemyMaster == null)
        {
            return;
        }

        AddAbilityInfo(GameTextUtil.GetText("battle_infotext2"), -1);

        // 特性を配列化
        uint[] enemyAbility = enemy.getEnemyAbilitys();
        int[] enemy_ability_turns = enemy.getEnemyAbilityTurns();

        //--------------------------------
        // 所持特性を全チェック
        //--------------------------------
        bool is_write = false;
        for (int num = 0; num < enemyAbility.Length; ++num)
        {
            if (enemyAbility[num] == 0)
            {
                continue;
            }

            // 特性マスターを取得
            MasterDataEnemyAbility abilityMaster = MasterDataUtil.GetEnemyAbilityParamFromID((int)enemyAbility[num]);
            if (abilityMaster == null)
            {
                continue;
            }

            // 表示文を取得
            int enemy_ability_turn = enemy_ability_turns[num];
            if (abilityMaster.name != "")
            {
                AddAbilityInfo(abilityMaster.name, enemy_ability_turn);
                enemy_ability_turn = 0;
            }

            if (abilityMaster.detail != "")
            {
                AddAbilityInfo(abilityMaster.detail, enemy_ability_turn);
            }

            is_write = true;
        }

        if (is_write)
        {
            // 何か敵特性があるときは、状態異常表示の前に空行がある
            AddAbilityInfo("", -1);
        }
        else
        {
            AddAbilityInfo("-", -1);
        }
    }

    public void AllClear()
    {
        Ailment_info_list.Clear();
    }
}

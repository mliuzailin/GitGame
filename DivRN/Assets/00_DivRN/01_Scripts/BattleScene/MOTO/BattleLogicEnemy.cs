using UnityEngine;
using System;

/// <summary>
/// バトル中の敵処理をまとめたもの
/// </summary>
public class BattleLogicEnemy
{
    private BattleLogic m_Owner;
    public BattleLogicEnemy(BattleLogic owner_class)
    {
        m_Owner = owner_class;
    }

    public void update(float delta_time)
    {
        if (m_EnemyTurnWait > delta_time)
        {
            m_EnemyTurnWait -= delta_time;
        }
        else
        {
            m_EnemyTurnWait = 0.0f;
        }
    }

    private const float ENEMY_TURN_WAIT_TIME = (0.5f);                                                      //!< 敵情報：ターン更新待機時間

    public float m_EnemyTurnDelta = 0.0f;                                                           //!< 敵情報：敵ターン経過時間
    public int m_CurrentProcessIndex = 0;   // 現在処理中の敵番号
    private GlobalDefine.PartyCharaIndex[] m_CurrentProcessTargets = null;   // 敵情報：ターゲット情報（複数回攻撃対応）
    private int m_CurrentProcessTargetIndex = 0; // 敵情報：何番目のターゲットを攻撃しているか

    private float m_EnemyTurnWait = 0.0f;


    // @add Developer 2015/12/11 v320 敵連続行動/敵テーブル切り替え時行動対応
    private int[] m_anEnemyNextActionID = null;         //!< 敵情報：連続行動ID
    public bool[] m_abEnemyActionSwitch = null;     //!< 敵情報：行動テーブル切り替え時行動フラグ


    private bool m_SkillTitle = false;      //!< 例外時のタイトル管理フラグ
    private bool m_bPlayVoice = false;

    // 敵行動時間待ち（敵の行動が速すぎるのを待つ）
    public bool isWaitingActionTimer()
    {
        if (m_EnemyTurnWait > 0.0f)
        {
            return true;
        }
        return false;
    }

    // 敵行動時間待ちをクリア
    public void clearActionTimer()
    {
        m_EnemyTurnWait = ENEMY_TURN_WAIT_TIME;
    }


    public void init(uint[] enemy_id_list, int[] drop_unit_id_list)
    {
        // 変数を初期化
        m_EnemyTurnDelta = 0.0f;
        m_CurrentProcessIndex = 0;
        m_CurrentProcessTargets = null;
        m_CurrentProcessTargetIndex = 0;
        m_EnemyTurnWait = 0.0f;
        m_anEnemyNextActionID = null;
        m_abEnemyActionSwitch = null;
        m_SkillTitle = false;
        m_bPlayVoice = false;

        bool is_restore_enemy = restoreEnemy();
        if (is_restore_enemy)
        {
            return;
        }

#if BUILD_TYPE_DEBUG
        //Developer 旧バトルでは１バトル中では最初にドロップした１ユニットしか取得できないようなコーディングになっていたのを修正し複数個ドロップへ対応しました。
        //もし旧バトルで複数個ドロップするようなデータが来た場合、旧バトルでの動作が変わってしまうのでそれを検出。
        if (BattleParam.IsKobetsuHP == false && drop_unit_id_list != null)
        {
            int drop_count = 0;
            for (int idx = 0; idx < drop_unit_id_list.Length; idx++)
            {
                if (drop_unit_id_list[idx] != 0)
                {
                    drop_count++;
                }
            }

            if (drop_count >= 2)
            {
                Debug.LogError("旧バトルでは複数個ユニットドロップは不可");
            }
        }
#endif

        //--------------------------------
        // バトルリクエストから出現する敵ID一覧を構築
        //--------------------------------
        int enemy_count = 0;
        for (int idx = 0; idx < enemy_id_list.Length; idx++)
        {
            if (enemy_id_list[idx] != 0)
            {
                enemy_count++;
            }
        }

        int nCreateEnemyTotal = 0;

        CharaOnce cChara = new CharaOnce();


        //--------------------------------
        //	敵配列を初期化
        //--------------------------------
        BattleParam.m_EnemyParam = new BattleEnemy[enemy_count];
        m_anEnemyNextActionID = new int[enemy_count * EnemyAbility.ENEMY_ABILITY_MAX];
        m_abEnemyActionSwitch = new bool[enemy_count * EnemyAbility.ENEMY_ABILITY_MAX];

        for (int i = 0; i < BattleParam.m_EnemyParam.Length; i++)
        {
            BattleParam.m_EnemyParam[i] = null;
        }

        //--------------------------------
        // 選定したエネミーを生成
        //--------------------------------
        for (int i = 0; i < enemy_id_list.Length; i++)
        {

            //--------------------------------
            //	0番はダミーデータ
            //--------------------------------
            if (enemy_id_list[i] == 0)
            {
                continue;
            }


            //--------------------------------
            //	ドロップ情報のチェック
            //--------------------------------
            int nCreateEnemyDrop = 0;
            if (drop_unit_id_list.Length == enemy_id_list.Length)
            {
                nCreateEnemyDrop = drop_unit_id_list[i];
            }


            //--------------------------------
            // 指定の敵キャラに対応するマスターデータ取得
            //--------------------------------
            MasterDataParamEnemy cMasterDataEnemy = BattleParam.m_MasterDataCache.useEnemyParam(enemy_id_list[i]);
            if (cMasterDataEnemy == null)
            {
                Debug.LogError("Enemy Is MasterData None - [ " + enemy_id_list[i] + " ] ");
                continue;
            }


            //--------------------------------
            //	マスターデータから敵パラメータを生成
            //--------------------------------
            cChara.CharaSetupFromParamEnemy(cMasterDataEnemy);


            //--------------------------------
            //	単体パラメータを元に敵戦闘情報を設定
            //--------------------------------
            BattleEnemy battle_enemy = new BattleEnemy();
            BattleParam.m_EnemyParam[nCreateEnemyTotal] = battle_enemy;
            battle_enemy.setMasterData(cMasterDataEnemy, cChara.m_CharaMasterDataParam);

            battle_enemy.m_EnemyAttack = cChara.m_CharaPow;
            battle_enemy.m_EnemyDefense = cChara.m_CharaDef;
            battle_enemy.m_EnemyHP = cChara.m_CharaHP;
            battle_enemy.m_EnemyHPMax = cChara.m_CharaHP;
            battle_enemy.m_EnemyDrop = nCreateEnemyDrop;
            battle_enemy.m_StatusAilmentChara = new StatusAilmentChara(StatusAilmentChara.OwnerType.ENEMY);
            battle_enemy.setAcquireDropUnit(false);


            //--------------------------------
            //	行動テーブルコントロールを登録
            //--------------------------------
#if UNITY_EDITOR && DEF_INGAME_LOG_ENEMY_ACTION
        Debug.LogError( "RegisterEnemyActionTableControl <<< " + nCreateEnemyTotal );
#endif // #if UNITY_EDITOR && DEF_INGAME_LOG_ENEMY_ACTION

            EnemyActionTableControl tableControl = battle_enemy.getEnemyActionTableControl();
            if (tableControl != null)
            {
                tableControl.Clear();
                tableControl.Setup();
                tableControl.SelectActionTable(m_Owner.m_BattleTotalTurn);

                //----------------------------------------
                // テーブル切り替え時行動チェック
                // @add Developer 2016/01/04 v320
                //----------------------------------------
                if (tableControl.m_ActionSwitchParamID != 0)
                {
                    // テーブル切り替え時行動フラグON
                    m_abEnemyActionSwitch[nCreateEnemyTotal] = true;
                }
            }


            //--------------------------------
            //	初期ターンの計算
            //--------------------------------
            const int MAGIC_NUMBER = 57621; // 適当な固定数値
            int nTurn = (int)BattleParam.GetRandFix(MAGIC_NUMBER + BattleParam.BattleRound * 100 + i, (uint)(cMasterDataEnemy.status_turn - 1), (uint)(cMasterDataEnemy.status_turn + 1));
            if (nTurn >= InGameDefine.TURN_ENEMY_MAX)
            {
                nTurn = InGameDefine.TURN_ENEMY_MAX;
            }
            if (nTurn < InGameDefine.TURN_ENEMY_MIN)
            {
                nTurn = InGameDefine.TURN_ENEMY_MIN;
            }

            battle_enemy.m_EnemyTurn = nTurn;
            battle_enemy.m_EnemyTurnMax = nTurn;

            nCreateEnemyTotal++;
        }
    }

    /// <summary>
    /// 中断データから敵情報を復帰
    /// </summary>
    /// <returns></returns>
    private bool restoreEnemy()
    {
        RestoreBattle restore_battle = BattleParam.getRestoreData();
        if (restore_battle == null)
        {
            return false;
        }

        //--------------------------------
        //	敵パラメータ復帰
        //--------------------------------
        BattleParam.m_EnemyParam = restore_battle.m_BattleEnemy;
        if (BattleParam.m_EnemyParam == null)
        {
            return false;
        }

        int enemy_count = BattleParam.m_EnemyParam.Length;

        m_anEnemyNextActionID = new int[enemy_count * EnemyAbility.ENEMY_ABILITY_MAX];
        m_abEnemyActionSwitch = new bool[enemy_count * EnemyAbility.ENEMY_ABILITY_MAX];

        for (int nCreateEnemyTotal = 0; nCreateEnemyTotal < enemy_count; nCreateEnemyTotal++)
        {
            BattleEnemy battle_enemy = BattleParam.m_EnemyParam[nCreateEnemyTotal];

            if (battle_enemy != null)
            {
                // マスターデータを設定しなおし
                MasterDataParamEnemy master_data_param_enemy = BattleParam.m_MasterDataCache.useEnemyParam((uint)battle_enemy.m_EnemyID);
                if (master_data_param_enemy != null)
                {
                    MasterDataParamChara master_data_param_chara = BattleParam.m_MasterDataCache.useCharaParam(master_data_param_enemy.chara_id);
                    if (master_data_param_chara != null)
                    {
                        battle_enemy.setMasterData(master_data_param_enemy, master_data_param_chara);
                    }
                }

                // 表示状態へ戻す（ユニットドロップ処理を動作させるため）
                battle_enemy.setShow(true);

                //--------------------------------
                //	状態異常復帰
                //--------------------------------
                battle_enemy.m_StatusAilmentChara.restoreFromSaveData();

                EnemyActionTableControl tableControl = battle_enemy.getEnemyActionTableControl();

                if (tableControl != null)
                {
                    tableControl.Clear();
                    tableControl.Setup();
                    tableControl.SelectActionTable(m_Owner.m_BattleTotalTurn);

                    //----------------------------------------
                    // テーブル切り替え時行動チェック
                    // @add Developer 2016/01/04 v320
                    //----------------------------------------
                    if (tableControl.m_ActionSwitchParamID != 0)
                    {
                        // テーブル切り替え時行動フラグON
                        m_abEnemyActionSwitch[nCreateEnemyTotal] = true;
                    }
                }

                //--------------------------------
                //	敵行動テーブル選択処理を終えた上で、敵行動テーブル進行度を復帰
                //--------------------------------
                if (tableControl != null)
                {
                    //--------------------------------
                    //	敵行動テーブルに乱数シードを復帰
                    //--------------------------------
                    if (restore_battle.m_BattleEnemyRandSeed != null)
                    {
                        uint unSeed = restore_battle.m_BattleEnemyRandSeed[nCreateEnemyTotal];
                        if (unSeed != 0)
                        {
                            tableControl.SetupRandomSeed(unSeed);
                        }
                        restore_battle.m_BattleEnemyRandSeed[nCreateEnemyTotal] = 0;
                    }

                    //--------------------------------
                    //	敵行動テーブル選択
                    //	ここまでに
                    //	行動選択に関係する敵の状態と、
                    //	行動選択に関係するゲームの状態を復帰させる
                    //--------------------------------
                    tableControl.SelectActionTable(m_Owner.m_BattleTotalTurn);


                    //--------------------------------
                    //	敵行動テーブル進行度を復帰
                    //--------------------------------
                    int actionTableStep = restore_battle.m_BattleEnemyActStep[nCreateEnemyTotal];
                    if (actionTableStep != -1)
                    {

                        // 敵行動テーブル進行度を復帰
                        tableControl.m_ActionStep = actionTableStep;

                        // 未記録にしておく
                        restore_battle.m_BattleEnemyActStep[nCreateEnemyTotal] = -1;

                        //--------------------------------
                        //	敵テーブル切り替え時行動フラグを復帰
                        //	@add Developer 2015/12/28 v320
                        //--------------------------------
                        m_abEnemyActionSwitch[nCreateEnemyTotal] = restore_battle.m_EnemyActionSwitch[nCreateEnemyTotal];

                        // 切り替え時行動フラグがOFFの場合
                        if (m_abEnemyActionSwitch[nCreateEnemyTotal] == false)
                        {
                            // 行動IDをクリアする
                            tableControl.m_ActionSwitchParamID = 0;
                        }

                        // 未記録にしておく
                        restore_battle.m_EnemyActionSwitch[nCreateEnemyTotal] = false;
                    }
                }
            }
        }


        // 敵復帰し終わったので復帰データを消す
        restore_battle.m_BattleEnemy = null;

        return true;
    }




    /// <summary>
    /// アクションテーブルの進行度合いを先頭へ戻す
    /// </summary>
    public void resetActionTableProgress()
    {
        for (int num = 0; num < m_anEnemyNextActionID.Length; ++num)
        {
            m_anEnemyNextActionID[num] = 0;
            m_abEnemyActionSwitch[num] = false;
        }
    }


    //------------------------------------------------------------------------
    /*!
        @brief		敵へダメージ発行
        @param[in]	int				(enemyIdx)		対象敵インデックス
        @param[in]	int				(damage)		ダメージ量
        @param[in]	EDAMAGE_TYPE	(type)			属性相性チェック結果
        @param[in]	bool			(disp)			ダメージ表示あり/なし
        @param[in]	GlobalDefine.PartyCharaIndex	(caster_index)	ダメージの発動者
    */
    //------------------------------------------------------------------------
    public void AddDamageEnemyHP(int enemyIdx, int damage, EDAMAGE_TYPE type, GlobalDefine.PartyCharaIndex caster_index, bool disp = true, int dmg_min = BattleLogic.BATTLE_DAMAGE_MIN)
    {
        if (BattleParam.m_EnemyParam == null)
        {
            return;
        }

        if (enemyIdx >= BattleParam.m_EnemyParam.Length || enemyIdx < 0)
        {
            return;
        }

        BattleEnemy enemyParam = BattleParam.m_EnemyParam[enemyIdx];
        if (enemyParam == null)
        {
            return;
        }

        // 既に非表示化されていれば処理しない
        if (!enemyParam.isShow())
        {
            return;
        }

        bool nodead = false;
        // チュートリアル
        if (BattleParam.IsTutorial())
        {
            nodead = BattleSceneManager.Instance.isTutorialNoDeadEnemy();
        }

        // 最低ダメージ保障
        if (damage <= dmg_min)
        {
            // damage = InGameDefine.BATTLE_DAMAGE_MIN;
            damage = dmg_min;
        }

        // 瞬間火力記録（実績を集計）
        BattleParam.m_AchievementTotalingInBattle.damageEnemy(damage);

#if BUILD_TYPE_DEBUG
        // デバッグ用超ダメージ
        if (BattleParam.IsKobetsuHP
            && BattleParam.m_DebugEnemyDamage1000
        )
        {
            damage = (int)(damage * 1000);
        }
#endif

        int before_hp = BattleParam.m_EnemyParam[enemyIdx].m_EnemyHP;   // 増減前のHP

        // 体力計算
        enemyParam.DelHP(damage, dmg_min, nodead);
        BattleParam.m_PlayerParty.m_Hate_1TurnDamage.addValue(caster_index, damage);

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　　敵" + enemyIdx.ToString() + "へ" + damage.ToString() + "ダメージ！"
            + " (" + before_hp.ToString() + "→" + enemyParam.m_EnemyHP.ToString() + ")"
            + ((caster_index != GlobalDefine.PartyCharaIndex.MAX) ? "  ダメージ元:" + caster_index.ToString() : "")
        );

        // ダメージ描画
        if (disp == true)
        {
            m_Owner.m_BattleDispEnemy.damageEnemy(enemyIdx, damage, type);
        }
    }


    //------------------------------------------------------------------------
    /*!
        @brief		敵回復
    */
    //------------------------------------------------------------------------
    public void AddHealEnemyHP(int enemyIdx, int healVal)
    {

        //--------------------------------
        //	エラーチェック
        //--------------------------------
        if (BattleParam.m_EnemyParam == null)
        {
            return;
        }

        if (enemyIdx >= BattleParam.m_EnemyParam.Length || enemyIdx < 0)
        {
            return;
        }

        BattleEnemy enemyParam = BattleParam.m_EnemyParam[enemyIdx];
        if (enemyParam == null)
        {
            return;
        }


        //--------------------------------
        //	敵が死んでいたら処理しない
        //--------------------------------
        if (enemyParam.isDead() == true)
        {
            return;
        }


        //--------------------------------
        // 既に非表示化されていれば処理しない
        //--------------------------------
        if (!enemyParam.isShow())
        {
            return;
        }

        int before_hp = BattleParam.m_EnemyParam[enemyIdx].m_EnemyHP;   // 増減前のHP

        //--------------------------------
        // 体力計算
        //--------------------------------
        enemyParam.AddHP(healVal);

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　　敵" + enemyIdx.ToString() + "のＨＰ" + healVal.ToString() + "回復"
            + " (" + before_hp.ToString() + "→" + enemyParam.m_EnemyHP.ToString() + ")"
        );

        //--------------------------------
        // ダメージ描画
        //--------------------------------
        m_Owner.m_BattleDispEnemy.damageEnemy(enemyIdx, healVal, EDAMAGE_TYPE.eDAMAGE_TYPE_HEAL);

    }


    //------------------------------------------------------------------------
    /*!
        @brief		敵の死亡処理
    */
    //------------------------------------------------------------------------
    public void DeadEnemy()
    {

        //--------------------------------
        //	エラーチェック
        //--------------------------------
        if (BattleParam.m_EnemyParam == null)
        {
            return;
        }

        //--------------------------------
        //	全ての敵をチェック
        //--------------------------------
        for (int i = 0; i < BattleParam.m_EnemyParam.Length; i++)
        {

            //--------------------------------
            //	敵パラメータ取得
            //--------------------------------
            BattleEnemy enemyParam = BattleParam.m_EnemyParam[i];
            if (enemyParam == null)
            {
                continue;
            }


            //--------------------------------
            // 既に非表示化されていれば処理しない
            //--------------------------------
            if (!enemyParam.isShow())
            {
                continue;
            }

            //--------------------------------
            // 死亡時処理
            //--------------------------------
            if (enemyParam.isDead() == false)
            {
                continue;
            }

            //--------------------------------
            // お金
            //--------------------------------
            if (InGameQuestData.Instance != null)
            {
                // 敵からの取得金
                int nAddMoney = enemyParam.getMasterDataParamEnemy().acquire_money;

                // 期間限定クエストボーナス
                float bonusRate = 1.0f;
                if (SceneGoesParam.Instance != null
                && SceneGoesParam.Instance.m_SceneGoesParamToQuest2 != null)
                {
                    bonusRate = InGameUtilBattle.GetDBRevisionValue(SceneGoesParam.Instance.m_SceneGoesParamToQuest2.m_QuestAreaAmendCoin);
                }
                nAddMoney = (int)InGameUtilBattle.AvoidErrorMultiple(nAddMoney, bonusRate);

                if (enemyParam.isAcquireDropUnit() == false)
                {
                    // お金取得
                    InGameUtil.AddMoney(BattleParam.m_QuestFloor, nAddMoney, false);
                }
            }


            //--------------------------------
            // ドロップ
            //--------------------------------
            if (enemyParam.m_EnemyDrop != 0)
            {

                int drop_param_uniqueID = enemyParam.m_EnemyDrop;

                //--------------------------------
                //	クエスト構築情報から該当ドロップ情報を検索
                //--------------------------------
                ServerDataDefine.PacketStructQuest2BuildDrop drop_param = InGameUtilBattle.GetQuestBuildDrop(BattleParam.m_BattleRequest.m_QuestBuild, drop_param_uniqueID);

                //--------------------------------
                //	ドロップ情報があれば処理
                //--------------------------------
                if (drop_param != null)
                {
                    ServerDataDefine.PacketStructQuest2BuildDrop.KindType kind_type = drop_param.getKindType();
                    int item_id = drop_param.item_id;
                    int item_num = drop_param.num;

                    int unit_level = 0;
                    int floor = BattleParam.m_QuestFloor;

                    int plus_pow = drop_param.plus_pow;
                    int plus_hp = drop_param.plus_hp;

                    //--------------------------------
                    // 一度ドロップした後は中断復帰等で何度も追加されないようにするためのチェック
                    //--------------------------------
                    if (enemyParam.isAcquireDropUnit() == false)
                    {
                        //--------------------------------
                        // 取得ユニットにユニットを追加
                        //--------------------------------
                        if (InGameQuestData.Instance != null
                            && item_id != 0
                        )
                        {
                            switch (kind_type)
                            {
                                case ServerDataDefine.PacketStructQuest2BuildDrop.KindType.UNIT:
                                    InGameQuestData.Instance.AddAcquireUnitParam((uint)item_id, unit_level,
                                                                                      floor, plus_pow, plus_hp);
                                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "敵" + i.ToString() + "ドロップ：ユニット："
                                        + " fix_id:" + item_id.ToString()
                                        + " lv:" + unit_level.ToString()
                                        + " atk+" + plus_pow.ToString()
                                        + " hp+" + plus_hp.ToString()
                                    );
                                    break;

                                case ServerDataDefine.PacketStructQuest2BuildDrop.KindType.TICKET:
                                    if (item_num > 0)
                                    {
                                        InGameQuestData.Instance.AddAcquireTicket(floor, item_num);
                                        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "敵" + i.ToString() + "ドロップ：チケット：" + item_num.ToString());
                                    }
                                    break;

                                case ServerDataDefine.PacketStructQuest2BuildDrop.KindType.MONEY:
                                    if (item_num > 0)
                                    {
                                        InGameQuestData.Instance.AddAcquireMoneyParam(item_num, floor);
                                        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "敵" + i.ToString() + "ドロップ：マネー：" + item_num.ToString());
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                    }

                    //--------------------------------
                    // ドロップしたものを表示
                    //--------------------------------
                    if (item_id != 0)
                    {
                        switch (kind_type)
                        {
                            case ServerDataDefine.PacketStructQuest2BuildDrop.KindType.UNIT:
                                {
                                    // 駒を表示
                                    MasterDataParamChara dropCharaParam = BattleParam.m_MasterDataCache.useCharaParam((uint)item_id);

                                    if (dropCharaParam != null)
                                    {
                                        m_Owner.m_BattleDispEnemy.showDropObject(i,
                                            dropCharaParam.rare,
                                            (plus_pow != 0 || plus_hp != 0),
                                            false,
                                            false
                                        );
                                    }
                                }
                                break;

                            case ServerDataDefine.PacketStructQuest2BuildDrop.KindType.TICKET:
                            case ServerDataDefine.PacketStructQuest2BuildDrop.KindType.MONEY:
                                {
                                    // 駒を表示
                                    m_Owner.m_BattleDispEnemy.showDropObject(i,
                                        MasterDataDefineLabel.RarityType.STAR_1,
                                        (plus_pow != 0 || plus_hp != 0),
                                        (kind_type == ServerDataDefine.PacketStructQuest2BuildDrop.KindType.TICKET),
                                        (kind_type == ServerDataDefine.PacketStructQuest2BuildDrop.KindType.MONEY)
                                    );
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }

                //--------------------------------
                // ユニットドロップフラグ
                //--------------------------------
                enemyParam.setAcquireDropUnit(true);
            }

            // チュートリアル時の偽ドロップ表示
            if (BattleParam.IsTutorial())
            {
                // ２戦目の一番左の敵
                if (BattleParam.BattleRound == 1
                    && i == 0
                )
                {
                    m_Owner.m_BattleDispEnemy.showDropObject(i,
                            enemyParam.getMasterDataParamChara().rare,
                            false,
                            false,
                            false
                        );
                }
            }


            // 実績を集計
            BattleParam.m_AchievementTotalingInBattle.killEnemy((int)enemyParam.getMasterDataParamChara().fix_id);

            //--------------------------------
            // コンポーネント更新OFF。
            //--------------------------------
            enemyParam.setShow(false);

            //--------------------------------
            // 連続行動：行動IDをクリア
            // テーブル切り替え時：行動フラグをクリア
            //--------------------------------
            m_anEnemyNextActionID[i] = 0;
            m_abEnemyActionSwitch[i] = false;

            //--------------------------------
            // 状態異常情報のクリア
            //--------------------------------
            enemyParam.m_StatusAilmentChara.ClearChara();

            //--------------------------------
            // 死亡SE再生
            //--------------------------------
            SoundUtil.PlaySE(SEID.SE_BATTLE_ENEMYDEATH);


            //--------------------------------
            // 死亡エフェクト再生
            //--------------------------------
            m_Owner.m_BattleDispEnemy.showDeadEffect(i, BattleParam.m_BattleRequest != null && BattleParam.m_BattleRequest.isBoss, BattleLogic.EFFECT_HANDLE_ENEMY_DEATH);
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		敵が全滅してるならクリア。全滅してないなら入力画面へ遷移
        @return		bool		[死んでる/生きてる]
    */
    //----------------------------------------------------------------------------
    public bool IsEnemyDestroyAll()
    {

        //--------------------------------
        //	エラーチェック
        //--------------------------------
        if (BattleParam.m_EnemyParam == null)
        {
            return false;
        }

        bool bActiveEnemy = true;


        //--------------------------------
        //	全ての敵をチェック
        //--------------------------------
        for (int i = 0; i < BattleParam.m_EnemyParam.Length; i++)
        {

            if (BattleParam.m_EnemyParam[i] == null)
            {
                continue;
            }

            //--------------------------------
            //	死亡チェック
            //--------------------------------
            if (BattleParam.m_EnemyParam[i].m_EnemyHP <= 0)
            {
                continue;
            }


            //--------------------------------
            //	生存ユニットが一体でも居た場合チェックを終了
            //--------------------------------
            bActiveEnemy = false;
            break;
        }

        return bActiveEnemy;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		敵ユニット攻撃更新：事前更新
    */
    //----------------------------------------------------------------------------
    public void EnemyAttackAllPreProc()
    {
        m_CurrentProcessIndex = 0;
        m_CurrentProcessTargets = null;
        m_bPlayVoice = false;
        m_EnemyTurnDelta = 0.0f;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		敵ユニット攻撃更新：メイン更新
        @param[in]	bool		(bCounter)		カウンターあり/なし
        @param[in]	int			(phase)			更新フェイズ
        @retval		bool		[更新終了/継続]
        @note		カウンターは特定タイミングのみで呼び出すこと
                    現在は、バックアタック時と敵の攻撃を受けた時
    */
    //----------------------------------------------------------------------------
    public bool EnemyAttackAllMainProc(bool bCounter, int phase)
    {

        //----------------------------------------
        // 敵の攻撃は少しずつずらして発行
        //----------------------------------------
        m_EnemyTurnDelta -= Time.deltaTime;
        if (m_EnemyTurnDelta > 0.0f)
        {
            return false;
        }
        else
        {
            m_EnemyTurnDelta = 0.0f;
        }


        //----------------------------------------
        // 敵更新継続フラグ
        //----------------------------------------
        bool bEnemyUpdate = false;
        bool forceNext = false;

        //----------------------------------------
        // 全敵チェック(１更新１キャラづつ処理していく)
        //----------------------------------------
        while (m_CurrentProcessIndex < BattleParam.m_EnemyParam.Length)
        {
            BattleEnemy battle_enemy_param = BattleParam.m_EnemyParam[m_CurrentProcessIndex];

            //----------------------------------------
            // エラーチェック
            // 死亡した敵は処理しない
            //----------------------------------------
            if (battle_enemy_param == null
            || battle_enemy_param.m_EnemyHP <= 0)
            {
                m_CurrentProcessIndex++;
                m_CurrentProcessTargets = null;
                m_bPlayVoice = false;
                continue;
            }

            MasterDataEnemyActionParam enemyActionParam = null;

            forceNext = false;

            //----------------------------------------
            //	敵テーブルコントロールの取得
            //----------------------------------------
            EnemyActionTableControl tableControl = battle_enemy_param.getEnemyActionTableControl();
            if (m_anEnemyNextActionID[m_CurrentProcessIndex] == 0)
            {
                if (tableControl != null)
                {

                    //----------------------------------------
                    //	アクションパラメータを取得
                    //----------------------------------------
                    switch (phase)
                    {
                        case 0:
                            //----------------------------------------
                            //	単発アクションが設定されていないか確認する
                            //----------------------------------------
                            enemyActionParam = tableControl.GetActionFirstAttack();
                            forceNext = true;
                            break;


                        default:
                            //----------------------------------------
                            //	通常処理
                            //----------------------------------------
                            enemyActionParam = tableControl.GetCurrentActionParam();
                            break;
                    }
                }
            }
            //----------------------------------------
            // 連続行動取得
            // @add Developer 2015/12/11 v320 敵連続行動対応
            //----------------------------------------
            else
            {
                //----------------------------------------
                //	動きが速すぎるので、適当に待つ
                //----------------------------------------
                if (isWaitingActionTimer())
                {
                    return false;
                }

                // 次の行動情報に更新
                enemyActionParam = BattleParam.m_MasterDataCache.useEnemyActionParam((uint)m_anEnemyNextActionID[m_CurrentProcessIndex]);

                // カウントダウンチェックはしない
                forceNext = true;
            }


            if (forceNext == false)
            {

                //----------------------------------------
                //	カウントダウンが完遂したら攻撃する
                //----------------------------------------
                if (battle_enemy_param.m_EnemyTurn > 0)
                {
                    // まだ行動ターンになっていないので次へ
                    m_CurrentProcessIndex++;
                    m_CurrentProcessTargets = null;
                    m_bPlayVoice = false;
                    continue;

                }

            }


            //----------------------------------------
            //	更新中フラグをたてる
            //----------------------------------------
            bEnemyUpdate = true;


            //----------------------------------------
            //	カットイン終了待ち-何かしらカットインが発生した場合
            //----------------------------------------
            if (BattleSkillCutinManager.Instance.isRunning() == true)
            {
                return false;
            }

            //----------------------------------------
            //	フェーズタイトルの切り替え-タイトル変更時（リーダースキルか、パッシブスキルになっている）
            //----------------------------------------
            if (m_SkillTitle == true
            && m_Owner.m_DispCaption.getRequestCaption() != BattleCaptionControl.CaptionType.ENEMY_PHASE)
            {
                m_SkillTitle = false;
                m_Owner.m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.ENEMY_PHASE);
            }

            if (enemyActionParam != null
            && m_bPlayVoice == false)
            {
                m_bPlayVoice = true;

                //----------------------------------------
                //	ボイス再生
                //----------------------------------------
                MasterDataAudioData cAudioMaster = MasterDataUtil.GetMasterDataAudioDataFromID(enemyActionParam.audio_data_id);
                if (cAudioMaster != null)
                {
                    SoundUtil.PlayVoice(cAudioMaster.fix_id);
                }

                //----------------------------------------
                //	メッセージ表示
                //----------------------------------------
                m_Owner.m_BattleDispEnemy.talk(m_CurrentProcessIndex, enemyActionParam);
            }

            //----------------------------------------
            //	メッセージ表示待機
            //----------------------------------------
            if (m_Owner.m_BattleDispEnemy.isTalking())
            {
                return false;
            }

            // 攻撃対象を決定（複数ターゲットの内の一体）
            GlobalDefine.PartyCharaIndex target_player = updateEnemyActionTargetPlayer(enemyActionParam);

            if (target_player != GlobalDefine.PartyCharaIndex.ERROR)
            {
                BattleParam.m_EnemyToPlayerTarget = target_player;

                //----------------------------------------
                //	ダメージを受ける前のHPを取得
                //----------------------------------------
                BattleSceneUtil.MultiInt playerHP = new BattleSceneUtil.MultiInt(BattleParam.m_PlayerParty.m_HPCurrent);


                //----------------------------------------
                //	ダメージ処理
                //----------------------------------------
                BattleSceneUtil.MultiInt damage_target;
                BattleSceneUtil.MultiInt damage_value = EnemyAttackAllDamage(m_CurrentProcessIndex, enemyActionParam, target_player, out damage_target);
#if BUILD_TYPE_DEBUG
                BattleDebugMenu.DispDamagePlayer(battle_enemy_param, damage_value, damage_target);
#endif

                //----------------------------------------
                //	@change	Developer 2016/02/03 v330 敵攻撃対象指定
                //	@note	プレイヤーに、ダメージが発生する場合のみ処理する
                //----------------------------------------
                MasterDataDefineLabel.TargetType nAttackTarget = MasterDataDefineLabel.TargetType.NONE;
                if (enemyActionParam != null)
                {
                    nAttackTarget = enemyActionParam.attack_target;
                }
                if (damage_value.getValue(GlobalDefine.PartyCharaIndex.MAX) > 0
                && nAttackTarget != MasterDataDefineLabel.TargetType.SELF
                && nAttackTarget != MasterDataDefineLabel.TargetType.FRIEND
                && nAttackTarget != MasterDataDefineLabel.TargetType.SELF_OTHER_FRIEND)
                {
                    //----------------------------------------
                    //	被ダメージ時のプレイヤースキルカットインチェック
                    //----------------------------------------
                    EnemyAttackCutinDamagePlayer(playerHP);

                    //----------------------------------------
                    //	ふんばりチェック
                    //----------------------------------------
                    EnemyAttackAllFunbari(playerHP, target_player);


                    //----------------------------------------
                    //	カウンターチェック
                    //	場所によっては都合が悪い場合があるのでカウンターON/OFFができるようにした。
                    //----------------------------------------
                    if (bCounter == true)
                    {

                        m_Owner.m_BattleLogicSkill.EnemyAttackAllCounter(m_CurrentProcessIndex, damage_value, playerHP);

                    }
                }


                //----------------------------------------
                // 演出関連処理
                //----------------------------------------
                m_Owner.m_BattleDispEnemy.attack(m_CurrentProcessIndex, enemyActionParam, damage_value, damage_target);

                //----------------------------------------
                //	待機時間の更新
                //----------------------------------------
                clearActionTimer();
                m_EnemyTurnDelta = 0.5f;    //攻撃アニメーションの時間
                return false;
            }

            //----------------------------------------
            // 連続行動チェック(プレイヤーの状態に関係なく、出し切る)
            // @add Developer 2015/12/11 v320 敵連続行動対応
            //----------------------------------------
            if (enemyActionParam != null
            && enemyActionParam.add_fix_id > 0)
            {
                // 次の行動を保存
                m_anEnemyNextActionID[m_CurrentProcessIndex] = enemyActionParam.add_fix_id;

                //----------------------------------------
                //	待機時間の更新
                //----------------------------------------
                clearActionTimer();
                m_EnemyTurnDelta = (float)RandManager.GetRand(20, 30) * 0.01f;
                m_bPlayVoice = false;

                return false;
            }
            else
            {
                m_anEnemyNextActionID[m_CurrentProcessIndex] = 0;
            }

            //----------------------------------------
            //	攻撃済みフラグをセット
            //----------------------------------------
            battle_enemy_param.setAttackFlag(true);

            //----------------------------------------
            //	攻撃を行ったため、敵のターン遅延の状態異常を回復させる
            //----------------------------------------
            battle_enemy_param.m_StatusAilmentChara.DelStatusAilment(MasterDataDefineLabel.AilmentType.FEAR);

            //----------------------------------------
            // テーブル切り替え時行動は、テーブルを進めない
            // @add Developer 2015/12/15 v320 テーブル切り替え時行動対応
            //----------------------------------------
            if (phase != 0
            && m_abEnemyActionSwitch[m_CurrentProcessIndex] == true)
            {
                // テーブル切り替え時行動フラグOFF
                m_abEnemyActionSwitch[m_CurrentProcessIndex] = false;

                // テーブル切り替え時行動は一度しか発動しないので、初期化する
                if (tableControl != null)
                {
                    tableControl.m_ActionSwitchParamID = 0;
                }

                // テーブルを進めないため
                phase = 0;
            }

            //----------------------------------------
            // 初回殴り時はテーブルを進めない
            //----------------------------------------
            bool tableStep = true;
            if (phase == 0)
            {
                tableStep = false;
            }


            //----------------------------------------
            //	アクションテーブルを１コマ進める
            //----------------------------------------
#if UNITY_EDITOR && DEF_INGAME_LOG_ENEMY_ACTION
            Debug.LogError( "table step add" );
#endif // #if UNITY_EDITOR && DEF_INGAME_LOG_ENEMY_ACTION
            if (tableControl != null
            && tableStep == true)
            {
                tableControl.StepActionTable();
            }


            //----------------------------------------
            //	次の処理対象へ
            //----------------------------------------
            m_CurrentProcessIndex++;
            m_CurrentProcessTargets = null;
            m_bPlayVoice = false;


            //----------------------------------------
            //	待機時間の更新
            //----------------------------------------
            clearActionTimer();
            m_EnemyTurnDelta = (float)RandManager.GetRand(20, 30) * 0.01f;

            break;
        }


        //----------------------------------------
        // 敵更新中ならシーケンス維持
        //----------------------------------------
        if (bEnemyUpdate == true)
        {
            return false;
        }


        //----------------------------------------
        // 敵の死亡処理
        //----------------------------------------
        //DeadEnemy();


        //----------------------------------------
        // すべて終わった後で敵ターンのリセット
        //----------------------------------------
        for (int i = 0; i < BattleParam.m_EnemyParam.Length; ++i)
        {
            if (BattleParam.m_EnemyParam[i] == null)
            {
                continue;
            }


            if (BattleParam.m_EnemyParam[i].m_EnemyTurn > 0)
            {
                continue;
            }


            BattleParam.m_EnemyParam[i].m_EnemyTurn = BattleParam.m_EnemyParam[i].m_EnemyTurnMax;
        }


        return true;
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		敵ユニット攻撃更新：終了処理
    */
    //----------------------------------------------------------------------------
    public bool EnemyAttackAllPostProc()
    {

        m_Owner.m_BattleDispEnemy.stopTalk();

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		敵ユニット攻撃更新：ダメージ処理
        @return		int		[ダメージ量]
    */
    //----------------------------------------------------------------------------
    private BattleSceneUtil.MultiInt EnemyAttackAllDamage(int enemyIndex, MasterDataEnemyActionParam enemyActionParam, GlobalDefine.PartyCharaIndex target_player, out BattleSceneUtil.MultiInt damage_target)
    {
        BattleSceneUtil.MultiFloat fDamageValue = new BattleSceneUtil.MultiFloat(0.0f);
        damage_target = new BattleSceneUtil.MultiInt(0);
        damage_target.setValue(target_player, 1);

        //----------------------------------------
        //	エラーチェック
        //----------------------------------------
        if (enemyIndex < 0
        || enemyIndex >= BattleParam.m_EnemyParam.Length)
        {
            return fDamageValue.toMultiInt();
        }

        if (BattleParam.m_EnemyParam == null
        || BattleParam.m_EnemyParam[enemyIndex] == null)
        {
            return fDamageValue.toMultiInt();
        }

        // 攻撃対象
        MasterDataDefineLabel.TargetType nAttackTarget = MasterDataDefineLabel.TargetType.NONE;

        // 敵種族
        MasterDataDefineLabel.KindType nEnemyKind = BattleParam.m_EnemyParam[enemyIndex].getKind();
        MasterDataDefineLabel.KindType nEnemyKind_sub = BattleParam.m_EnemyParam[enemyIndex].getKindSub();

        // 敵属性
        MasterDataDefineLabel.ElementType nEnemyElement = BattleParam.m_EnemyParam[enemyIndex].getMasterDataParamChara().element;
        MasterDataDefineLabel.ElementType nEnemyElemOrg = nEnemyElement;                                                    // @add Developer 2016/06/30 v343:リーダースキル改修対応(敵属性：途中で変えてはいけない)

        // 状態異常管理ID
        StatusAilmentChara nStatusAilment = BattleParam.m_EnemyParam[enemyIndex].m_StatusAilmentChara;

        // ダメージ描画
        MasterDataDefineLabel.BoolType damageDraw = MasterDataDefineLabel.BoolType.ENABLE;

        // 行動パターン
        MasterDataDefineLabel.EnemySkillCategory actionCategory = MasterDataDefineLabel.EnemySkillCategory.NONE;

        // 状態異常補正アリ/ナシ
        bool statusAilment_bonus = false;


        // 手札変化が起こった(手札変換)
        bool enemyHandRemake = false;

        // 即死フラグ
        bool bDeath = false;

        // 手札変化属性
        MasterDataDefineLabel.ElementType[] enemyHandRemakeElem = new MasterDataDefineLabel.ElementType[] { MasterDataDefineLabel.ElementType.NONE,
                                                MasterDataDefineLabel.ElementType.NONE,
                                                MasterDataDefineLabel.ElementType.NONE,
                                                MasterDataDefineLabel.ElementType.NONE,
                                                MasterDataDefineLabel.ElementType.NONE,
                                                };


        //----------------------------------------
        //	敵アクションパラメータがあれば各種フラグを更新
        //----------------------------------------
        if (enemyActionParam != null)
        {

            // 攻撃対象
            nAttackTarget = enemyActionParam.attack_target;

            // アクションカテゴリ
            actionCategory = enemyActionParam.skill_type;

            // ダメージ描画
            damageDraw = enemyActionParam.damage_draw;

        }
        else
        {

            //----------------------------------------
            //	デフォルト行動
            //----------------------------------------
            // アクションカテゴリ
            actionCategory = MasterDataDefineLabel.EnemySkillCategory.NONE;

            // ダメージ描画
            damageDraw = MasterDataDefineLabel.BoolType.ENABLE;
        }


        //----------------------------------------
        //	アクションカテゴリ
        //----------------------------------------
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "敵" + enemyIndex.ToString() + "の行動"
            + ((enemyActionParam != null) ? " FixId:" + enemyActionParam.fix_id.ToString() : "")
            + " 行動種類:" + ((actionCategory != MasterDataDefineLabel.EnemySkillCategory.NONE) ? actionCategory.ToString() : "デフォルト攻撃")
            + " ターゲット:Player_" + ((target_player != GlobalDefine.PartyCharaIndex.MAX) ? target_player.ToString() : "ALL")
        );
        switch (actionCategory)
        {
            case MasterDataDefineLabel.EnemySkillCategory.WAIT:
                {
                    //----------------------------------------
                    //	待機
                    //----------------------------------------
                }
                break;


            case MasterDataDefineLabel.EnemySkillCategory.ATK:
                {
                    //----------------------------------------
                    // 大砲::倍率のみ
                    //----------------------------------------
                    int skillparam_rate = enemyActionParam.Get_ATK_NORMAL_POW_RATE();
                    int skillparam_rate_r = enemyActionParam.Get_ATK_NORMAL_POW_RATE_R();

                    float fSkillparam_Rate = InGameUtilBattle.GetDBRevisionValue(skillparam_rate);
                    float fSkillparam_Rate_r = InGameUtilBattle.GetDBRevisionValue(skillparam_rate_r);

                    fSkillparam_Rate = fSkillparam_Rate + RandManager.GetRand(0, (uint)(fSkillparam_Rate_r + 1));

                    fDamageValue.setValue(target_player, BattleParam.m_EnemyParam[enemyIndex].m_EnemyAttack);
                    fDamageValue.mulValue(target_player, fSkillparam_Rate);


                    // 状態異常ダメージ補正:ON
                    statusAilment_bonus = true;
                }
                break;


            case MasterDataDefineLabel.EnemySkillCategory.ATK_ELEM:
                {
                    //----------------------------------------
                    //	大砲：倍率＋属性指定
                    //----------------------------------------
                    MasterDataDefineLabel.ElementType skillparam_elem = enemyActionParam.Get_ATK_ELEM_TARGET_ELEM();
                    int skillparam_rate = enemyActionParam.Get_ATK_ELEM_POW_RATE();

                    float fSkillparam_rate = InGameUtilBattle.GetDBRevisionValue(skillparam_rate);

                    nEnemyElement = skillparam_elem;

                    fDamageValue.setValue(target_player, BattleParam.m_EnemyParam[enemyIndex].m_EnemyAttack);
                    fDamageValue.mulValue(target_player, fSkillparam_rate);


                    // 状態異常ダメージ補正:ON
                    statusAilment_bonus = true;
                }
                break;


            case MasterDataDefineLabel.EnemySkillCategory.HANDREMAKE:
                {
                    //----------------------------------------
                    //	手札変換
                    //----------------------------------------
                    //----------------------------------------
                    //	ダメージ( パラメータが足りない )
                    //----------------------------------------
                    fDamageValue.setValue(target_player, BattleParam.m_EnemyParam[enemyIndex].m_EnemyAttack);

                    enemyHandRemake = true;

                    // 状態異常ダメージ補正:ON
                    statusAilment_bonus = true;
                }
                break;

            case MasterDataDefineLabel.EnemySkillCategory.ATK_HP_RATE:
                //----------------------------------------
                //	プレイヤーHPの割合ダメージ
                //----------------------------------------
                {
                    bool chk_hpmax = enemyActionParam.Get_ATKHP_RATE_CHK_HPMAX();
                    int atk_rate = enemyActionParam.Get_ATKHP_RATE_RATE();
                    BattleSceneUtil.MultiInt hp = new BattleSceneUtil.MultiInt();

                    if (BattleParam.m_PlayerParty != null)
                    {

                        if (chk_hpmax)
                        {
                            // 最大値から割合ダメージを求める
                            hp.setValue(target_player, BattleParam.m_PlayerParty.m_HPMax);
                        }
                        else
                        {
                            // 現在値から割合ダメージを求める
                            hp.setValue(target_player, BattleParam.m_PlayerParty.m_HPCurrent);
                        }

                        float fRate = InGameUtilBattle.GetDBRevisionValue(atk_rate);
                        fDamageValue.setValue(target_player, hp.toMultiFloat());
                        fDamageValue.mulValue(target_player, fRate);
                    }

                    // 状態異常ダメージ補正:OFF
                    statusAilment_bonus = false;
                }
                break;

            case MasterDataDefineLabel.EnemySkillCategory.HEAL:
                //----------------------------------------
                //	回復スキル
                //----------------------------------------
                {
                    MasterDataDefineLabel.TargetType param_target = enemyActionParam.Get_HEAL_TARGET();
                    bool param_rate_mode = enemyActionParam.Get_HEAL_RATE_MODE();
                    int param_value = enemyActionParam.Get_HEAL_VALUE();
                    float param_value_rate = InGameUtilBattle.GetDBRevisionValue(enemyActionParam.Get_HEAL_VALUE_RATE());

                    float heal_value_self = 0.0f;
                    BattleSceneUtil.MultiFloat heal_value_player = new BattleSceneUtil.MultiFloat(0.0f);


                    //	効果量の計算
                    if (param_rate_mode == false)
                    {
                        heal_value_self = param_value;
                        heal_value_player.setValue(target_player, param_value);
                    }
                    else
                    {
                        heal_value_self = InGameUtilBattle.AvoidErrorMultiple(BattleParam.m_EnemyParam[enemyIndex].m_EnemyHPMax, param_value_rate);
                        heal_value_player.setValue(target_player, BattleParam.m_PlayerParty.m_HPMax.toMultiFloat());
                        heal_value_player.mulValue(target_player, param_value_rate);
                    }

                    //----------------------------------------
                    // 状態異常確認
                    // @add Developer 2016/05/30 v350 回復不可[全]対応
                    //----------------------------------------
                    // プレイヤー状態異常：回復不可[全]の場合
                    if (BattleParam.m_PlayerParty.m_Ailments.getAilment(GlobalDefine.PartyCharaIndex.MAX).GetNonRecoveryAll() == true)
                    {
                        // 回復量を0へ
                        heal_value_player.setValue(target_player, 0.0f);
                    }

                    //	対象を回復
                    switch (param_target)
                    {
                        case MasterDataDefineLabel.TargetType.SELF:
                            //----------------------------------------
                            //	自身の回復
                            //----------------------------------------
                            {
                                //----------------------------------------
                                // 状態異常確認
                                // @add Developer 2016/05/31 v350 回復不可[全]対応
                                //----------------------------------------
                                // エネミー状態異常：回復不可[全]の場合
                                if (BattleParam.m_EnemyParam[enemyIndex].m_StatusAilmentChara.GetNonRecoveryAll() == true)
                                {
                                    // 回復量を0へ
                                    heal_value_self = 0.0f;
                                }

                                AddHealEnemyHP(enemyIndex, (int)heal_value_self);
                            }
                            break;

                        case MasterDataDefineLabel.TargetType.FRIEND:
                            //----------------------------------------
                            //	自身の味方の回復
                            //----------------------------------------
                            {
                                //----------------------------------------
                                // 状態異常確認
                                // @change Developer 2016/05/31 v350 回復不可[全]対応
                                //----------------------------------------
                                float heal_value;
                                for (int idx = 0; idx < BattleParam.m_EnemyParam.Length; ++idx)
                                {
                                    // nullチェック
                                    if (BattleParam.m_EnemyParam[idx] == null)
                                    {
                                        continue;
                                    }

                                    // 回復量を取得
                                    heal_value = heal_value_self;

                                    // エネミー状態異常：回復不可[全]の場合
                                    if (BattleParam.m_EnemyParam[idx].m_StatusAilmentChara.GetNonRecoveryAll() == true)
                                    {
                                        // 回復量を0へ
                                        heal_value = 0.0f;
                                    }

                                    AddHealEnemyHP(idx, (int)heal_value);
                                }
                            }
                            break;

                        case MasterDataDefineLabel.TargetType.OTHER:
                        case MasterDataDefineLabel.TargetType.ENEMY:
                        case MasterDataDefineLabel.TargetType.ENE_N_1:
                        case MasterDataDefineLabel.TargetType.ENE_1N_1:
                        case MasterDataDefineLabel.TargetType.ENE_R_N:
                        case MasterDataDefineLabel.TargetType.ENE_1_N:
                            //----------------------------------------
                            //	プレイヤー側体力回復
                            //----------------------------------------
                            {
                                if (BattleParam.m_PlayerParty != null)
                                {
                                    BattleParam.m_PlayerParty.RecoveryHP(heal_value_player.toMultiInt(), true, true);
                                }
                            }
                            break;

                        case MasterDataDefineLabel.TargetType.ALL:
                            //----------------------------------------
                            //	全員回復
                            //----------------------------------------
                            {
                                //	自身の味方の回復
                                //----------------------------------------
                                // 状態異常確認
                                // @change Developer 2016/05/31 v350 回復不可[全]対応
                                //----------------------------------------
                                float heal_value;
                                for (int idx = 0; idx < BattleParam.m_EnemyParam.Length; ++idx)
                                {
                                    // nullチェック
                                    if (BattleParam.m_EnemyParam[idx] == null)
                                    {
                                        continue;
                                    }

                                    // 回復量を取得
                                    heal_value = heal_value_self;

                                    // エネミー状態異常：回復不可[全]の場合
                                    if (BattleParam.m_EnemyParam[idx].m_StatusAilmentChara.GetNonRecoveryAll() == true)
                                    {
                                        // 回復量を0へ
                                        heal_value = 0.0f;
                                    }

                                    AddHealEnemyHP(idx, (int)heal_value);
                                }

                                //	プレイヤー側体力回復
                                if (BattleParam.m_PlayerParty != null)
                                {
                                    BattleParam.m_PlayerParty.RecoveryHP(heal_value_player.toMultiInt(), true, true);
                                }
                            }
                            break;

                        case MasterDataDefineLabel.TargetType.NONE:
                        default:
                            break;
                    }
                }
                break;


            case MasterDataDefineLabel.EnemySkillCategory.ALIMENT_CLEAR_PLAYER:
                //----------------------------------------
                //	プレイヤー状態異常を無効化
                //----------------------------------------
                {
                    BattleParam.m_PlayerParty.m_Ailments.DelAllStatusAilment(target_player);
                }
                break;


            case MasterDataDefineLabel.EnemySkillCategory.ALIMENT_CLEAR:
                //----------------------------------------
                // 敵側PTの状態異常をクリア
                //----------------------------------------
                {
                    //	全ての敵をチェック
                    BattleEnemy enemyParam = null;
                    for (int j = 0; j < BattleParam.m_EnemyParam.Length; j++)
                    {

                        //	敵パラメータ取得
                        enemyParam = BattleParam.m_EnemyParam[j];
                        if (enemyParam == null)
                        {
                            continue;
                        }

                        // 既に非表示化されていれば処理しない
                        if (!enemyParam.isShow())
                        {
                            continue;
                        }

                        //	敵が死んでいたら処理しない
                        if (enemyParam.isDead() == true)
                        {
                            continue;
                        }

                        // 敵の状態異常をクリア
                        enemyParam.m_StatusAilmentChara.DelAllStatusAilment();
                    }
                }
                break;

            case MasterDataDefineLabel.EnemySkillCategory.BATTLEFIELD_PANEL:
                //----------------------------------------
                // パネル配置
                //----------------------------------------
                {
                    InGameUtilBattle.SkillBattlefieldPanel(actionCategory, enemyActionParam.GetBattleFieldPanelChangeParam(), ESKILLTYPE.eENEMY);
                }
                break;

            case MasterDataDefineLabel.EnemySkillCategory.DEATH:
                //----------------------------------------
                // 即死攻撃
                //----------------------------------------
                {
                    int rand = (int)RandManager.GetRand(0, 100);
                    if (rand < enemyActionParam.Get_DEATH_RATE())
                    {
                        bDeath = true;
                    }
                }
                break;

            case MasterDataDefineLabel.EnemySkillCategory.GIVE_ENEMY_ABILITY:
                //----------------------------------------
                // 敵特性付与
                //----------------------------------------
                {
                    uint enemy_ability_fix_id = enemyActionParam.Get_ENEMY_ABILITY_FIX_ID();
                    int turn = enemyActionParam.Get_ENEMY_ABILITY_TURN();
                    BattleParam.m_EnemyParam[enemyIndex].giveAdditionalEnemyAbility(enemy_ability_fix_id, turn);
                }
                break;

            case MasterDataDefineLabel.EnemySkillCategory.NONE:
            default:
                //----------------------------------------
                //	通常ダメージ発行
                //----------------------------------------
                {
                    // ダメージ
                    fDamageValue.setValue(target_player, BattleParam.m_EnemyParam[enemyIndex].m_EnemyAttack);

                    // ダメージ値あり
                    damageDraw = MasterDataDefineLabel.BoolType.ENABLE;

                    // 状態異常ダメージ補正:ON
                    statusAilment_bonus = true;
                }
                break;
        }



        //----------------------------------------
        // 状態異常によるダメージ増減
        //----------------------------------------
        float fAilmentRate = 1.0f;
        // 攻撃力倍率
        fAilmentRate = InGameUtilBattle.AvoidErrorMultiple(fAilmentRate, nStatusAilment.GetOffenceRate());
        // 属性攻撃力倍率
        fAilmentRate = InGameUtilBattle.AvoidErrorMultiple(fAilmentRate, nStatusAilment.GetOffenceElementRate(nEnemyElement));
        // 種族攻撃力倍率
        fAilmentRate = InGameUtilBattle.AvoidErrorMultiple(fAilmentRate, nStatusAilment.GetOffenceKindRate(nEnemyKind));

        //----------------------------------------
        // 敵特性によるダメージ増減
        //----------------------------------------
        float fAbilityRate = 1.0f;
        fAbilityRate = EnemyAbility.GetAbilityDamageRateEnemy(BattleParam.m_EnemyParam[enemyIndex]);


        //----------------------------------------
        //	状態異常付与
        //----------------------------------------
        bool is_status_ailment_player = false;  // プレイヤー側に状態異常を与えるかどうか
        BattleEnemy[] ailment_enemys = null;    // 敵自身に与える状態異常
        if (enemyActionParam != null)
        {

            //----------------------------------------
            //	攻撃対象
            //----------------------------------------
            switch (enemyActionParam.status_ailment_target)
            {
                case MasterDataDefineLabel.TargetType.SELF:
                    //----------------------------------------
                    //	攻撃対象が自身の場合
                    //----------------------------------------
                    ailment_enemys = new BattleEnemy[1];
                    ailment_enemys[0] = BattleParam.m_EnemyParam[enemyIndex];
                    break;

                case MasterDataDefineLabel.TargetType.OTHER:
                case MasterDataDefineLabel.TargetType.ENEMY:
                case MasterDataDefineLabel.TargetType.ENE_N_1:
                case MasterDataDefineLabel.TargetType.ENE_1N_1:
                case MasterDataDefineLabel.TargetType.ENE_R_N:
                case MasterDataDefineLabel.TargetType.ENE_1_N:
                    //----------------------------------------
                    //	攻撃対象がプレイヤーの場合
                    //----------------------------------------
                    is_status_ailment_player = true;
                    break;

                case MasterDataDefineLabel.TargetType.FRIEND:
                    //----------------------------------------
                    //	敵側から見たの味方
                    //----------------------------------------
                    ailment_enemys = new BattleEnemy[BattleParam.m_EnemyParam.Length];
                    for (int i = 0; i < BattleParam.m_EnemyParam.Length; i++)
                    {
                        if (BattleParam.m_EnemyParam[i] == null)
                        {
                            ailment_enemys[i] = null;
                            continue;
                        }
                        ailment_enemys[i] = BattleParam.m_EnemyParam[i];
                    }
                    break;

                case MasterDataDefineLabel.TargetType.ALL:
                    //----------------------------------------
                    //	全員
                    //----------------------------------------
                    ailment_enemys = new BattleEnemy[BattleParam.m_EnemyParam.Length];
                    for (int i = 0; i < BattleParam.m_EnemyParam.Length; i++)
                    {
                        if (BattleParam.m_EnemyParam[i] == null)
                        {
                            ailment_enemys[i] = null;
                            continue;
                        }
                        ailment_enemys[i] = BattleParam.m_EnemyParam[i];
                    }

                    is_status_ailment_player = true;
                    break;
            }

            int[] ailmentArray = { enemyActionParam.status_ailment1,
                                   enemyActionParam.status_ailment2,
                                   enemyActionParam.status_ailment3,
                                   enemyActionParam.status_ailment4 };

            if (is_status_ailment_player)
            {
                //　@change Developer 2016/03/14 v330 プレイヤーを対象とした場合、パーティの最大HPをいれる
                for (int i = 0; i < ailmentArray.Length; i++)
                {

                    //----------------------------------------
                    //	設定された対象に状態異常を付与
                    //----------------------------------------
                    BattleParam.m_PlayerParty.m_Ailments.AddStatusAilmentToPlayerParty(target_player,
                                                                        ailmentArray[i],
                                                                        BattleParam.m_EnemyParam[enemyIndex].m_EnemyAttack,
                                                                        BattleParam.m_PlayerParty.m_HPMax);
                }
            }

            if (ailment_enemys != null)
            {

                int nHPMax = BattleParam.m_EnemyParam[enemyIndex].m_EnemyHPMax;
                for (int n = 0; n < ailment_enemys.Length; n++)
                {

                    if (ailment_enemys[n] == null)
                    {
                        continue;
                    }

                    for (int i = 0; i < ailmentArray.Length; i++)
                    {

                        //----------------------------------------
                        //	設定された対象に状態異常を付与
                        //----------------------------------------
                        ailment_enemys[n].m_StatusAilmentChara.AddStatusAilment(
                                                                        ailmentArray[i],
                                                                        BattleParam.m_EnemyParam[enemyIndex].m_EnemyAttack,
                                                                        nHPMax,
                                                                        ailment_enemys[n].getMasterDataParamChara());
                    }
                }
            }
        }

        // 全体攻撃の時の死亡ユニットへはダメージを与えない処理
        if (target_player == GlobalDefine.PartyCharaIndex.MAX)
        {
            BattleSceneUtil.MultiInt alive_players = new BattleSceneUtil.MultiInt(BattleParam.m_PlayerParty.m_HPCurrent);
            alive_players.minValue(GlobalDefine.PartyCharaIndex.MAX, 1);
            fDamageValue.mulValue(GlobalDefine.PartyCharaIndex.MAX, alive_players.toMultiFloat());
            damage_target.mulValue(GlobalDefine.PartyCharaIndex.MAX, alive_players);
        }

#if BUILD_TYPE_DEBUG
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　敵基本攻撃力:" + fDamageValue.getDebugString(target_player, 1));
#endif

        //----------------------------------------
        //	状態異常ダメージ補正
        //----------------------------------------
        if (statusAilment_bonus == true)
        {
            fDamageValue.mulValue(target_player, fAilmentRate);
        }

        //----------------------------------------
        //	敵特性ダメージ補正
        //----------------------------------------
        fDamageValue.mulValue(target_player, fAbilityRate);


        if (statusAilment_bonus == true)
        {
#if BUILD_TYPE_DEBUG
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　敵行動基本攻撃力:" + fDamageValue.getDebugString(target_player, 1)
                + " = 敵基本攻撃力"
                + " x" + ((int)(nStatusAilment.GetOffenceRate() * 100.0f)).ToString() + "%(攻撃力バフ効果)"
                + " x" + ((int)(nStatusAilment.GetOffenceElementRate(nEnemyElement) * 100.0f)).ToString() + "%(属性攻撃力バフ効果)"
                + " x" + ((int)(nStatusAilment.GetOffenceKindRate(nEnemyKind) * 100.0f)).ToString() + "%(種族攻撃力バフ効果)"
                + " x" + ((int)(fAbilityRate * 100.0f)).ToString() + "%(敵特性補正)"
            );
#endif
        }
        else
        {
#if BUILD_TYPE_DEBUG
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　敵行動基本攻撃力:" + fDamageValue.getDebugString(target_player, 1)
                + " = 敵基本攻撃力"
                + " x100%(攻撃力バフ効果無効)"
                + " x100%(属性攻撃力バフ効果無効)"
                + " x100%(種族攻撃力バフ効果無効)"
                + " x" + ((int)(fAbilityRate * 100.0f)).ToString() + "%(敵特性補正)"
            );
#endif
        }

        bool bDrawDamage = (damageDraw == MasterDataDefineLabel.BoolType.ENABLE) ? true : false;

        //----------------------------------------
        // ダメージ処理
        // @change Developer 2016/02/03 v330 敵攻撃対象指定
        //----------------------------------------
        if (nAttackTarget != MasterDataDefineLabel.TargetType.SELF)
        {
            //----------------------------------------
            // エネミー全体ダメージ処理
            //----------------------------------------
            if (nAttackTarget != MasterDataDefineLabel.TargetType.NONE
            && nAttackTarget != MasterDataDefineLabel.TargetType.OTHER
            && nAttackTarget != MasterDataDefineLabel.TargetType.ENEMY
            && nAttackTarget != MasterDataDefineLabel.TargetType.ENE_N_1
            && nAttackTarget != MasterDataDefineLabel.TargetType.ENE_1N_1
            && nAttackTarget != MasterDataDefineLabel.TargetType.ENE_R_N
            && nAttackTarget != MasterDataDefineLabel.TargetType.ENE_1_N
            )
            {
                for (int num = 0; num < BattleParam.m_EnemyParam.Length; ++num)
                {
                    //----------------------------------------
                    // 生存チェック
                    //----------------------------------------
                    if (BattleParam.m_EnemyParam[num] == null
                    || BattleParam.m_EnemyParam[num].isShow() == false
                    || BattleParam.m_EnemyParam[num].isDead() == true)
                    {
                        continue;
                    }

                    //----------------------------------------
                    // 発動者は効果適用外
                    //----------------------------------------
                    if (num == enemyIndex)
                    {
                        if (nAttackTarget == MasterDataDefineLabel.TargetType.SELF_OTHER_FRIEND
                        || nAttackTarget == MasterDataDefineLabel.TargetType.SELF_OTHER_ALL)
                        {
                            continue;
                        }
                    }


                    BattleEnemy cEnemyParam = BattleParam.m_EnemyParam[num];
                    EDAMAGE_TYPE eDamageType;
                    int nEnemyDamage;
                    if (bDeath == false)
                    {
                        //----------------------------------------
                        // 弱点チェック
                        //----------------------------------------
                        eDamageType = InGameUtilBattle.GetSkillElementAffinity(nEnemyElement, cEnemyParam.getMasterDataParamChara().element);
                        if (eDamageType != EDAMAGE_TYPE.eDAMAGE_TYPE_NORMAL)
                        {
                            // 属性倍率
                            float fElementRate = InGameUtilBattle.GetSkillElementRate(nEnemyElement, cEnemyParam.getMasterDataParamChara().element);
                            nEnemyDamage = (int)InGameUtilBattle.AvoidErrorMultiple(fDamageValue.getValue(GlobalDefine.PartyCharaIndex.LEADER), fElementRate);

                            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "敵" + num.ToString()
                                + "への攻撃力:" + nEnemyDamage.ToString()
                                + " = " + fDamageValue.getValue(GlobalDefine.PartyCharaIndex.LEADER).ToString() + "(敵行動基本攻撃力)"
                                + " x" + InGameUtilBattle.getDebugPercentString(fElementRate) + "%(属性倍率)"
                            );
                        }
                        else
                        {
                            nEnemyDamage = (int)fDamageValue.getValue(GlobalDefine.PartyCharaIndex.LEADER);
                            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "敵" + num.ToString()
                                + "への攻撃力:" + nEnemyDamage.ToString()
                                + " = " + fDamageValue.getValue(GlobalDefine.PartyCharaIndex.LEADER).ToString() + "(敵行動基本攻撃力)"
                            );
                        }

                        //----------------------------------------
                        // ダメージ軽減
                        //----------------------------------------
                        nEnemyDamage = InGameUtilBattle.DamageReduce(nEnemyDamage, nEnemyElement, cEnemyParam.m_StatusAilmentChara,
                                                                cEnemyParam.m_EnemyDefense, false, false, false);
                    }
                    else
                    {
                        eDamageType = EDAMAGE_TYPE.eDAMAGE_TYPE_NORMAL;
                        nEnemyDamage = InGameUtilBattle.VALUE_DMG_MAX_ENEMY;

                        // 強制的にHPを0に設定
                        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "敵" + num.ToString() + "への即死効果");
                        cEnemyParam.SetHP(0);
                    }

                    //----------------------------------------
                    // HPから減算
                    //----------------------------------------
                    AddDamageEnemyHP(num, nEnemyDamage, eDamageType, GlobalDefine.PartyCharaIndex.MAX, bDrawDamage);
                }
            }

            //----------------------------------------
            // プレイヤーダメージ処理
            //----------------------------------------
            if (nAttackTarget != MasterDataDefineLabel.TargetType.FRIEND
            && nAttackTarget != MasterDataDefineLabel.TargetType.SELF_OTHER_FRIEND)
            {
                BattleSceneUtil.MultiInt wrk_damage = new BattleSceneUtil.MultiInt();
                if (bDeath == false)
                {
                    if (BattleParam.IsKobetsuHP)
                    {
                        if (target_player == GlobalDefine.PartyCharaIndex.MAX)
                        {
                            BattleSceneUtil.MultiFloat elem_rate = new BattleSceneUtil.MultiFloat(0.0f);
                            for (int idx = 0; idx < (int)GlobalDefine.PartyCharaIndex.MAX; idx++)
                            {
                                CharaOnce cChara = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.EXIST);
                                if (cChara != null)
                                {
                                    elem_rate.setValue((GlobalDefine.PartyCharaIndex)idx, InGameUtilBattle.GetSkillElementRate(nEnemyElement, cChara.m_CharaMasterDataParam.element));
                                }
                            }
                            fDamageValue.mulValue(target_player, elem_rate);

#if BUILD_TYPE_DEBUG
                            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "敵攻撃力:" + fDamageValue.getDebugString(target_player, 1)
                                + " = (敵行動基本攻撃力)"
                                + " x" + elem_rate.getDebugString(target_player, 100) + "%(属性倍率)"
                            );
#endif
                        }
                        else
                        {
                            CharaOnce cChara = BattleParam.m_PlayerParty.getPartyMember(target_player, CharaParty.CharaCondition.EXIST);
                            float elem_rate = InGameUtilBattle.GetSkillElementRate(nEnemyElement, cChara.m_CharaMasterDataParam.element);
                            fDamageValue.mulValue(target_player, elem_rate);

#if BUILD_TYPE_DEBUG
                            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "敵攻撃力:" + fDamageValue.getDebugString(target_player, 1)
                                + " = (敵行動基本攻撃力)"
                                + " x" + InGameUtilBattle.getDebugPercentString(elem_rate) + "%(属性倍率)"
                            );
#endif
                        }
                    }
                    else
                    {
                        //----------------------------------------
                        //	パーティメンバーによる分散補正
                        //----------------------------------------
                        CharaOnce cChara0 = BattleParam.m_PlayerParty.getPartyMember(GlobalDefine.PartyCharaIndex.LEADER, CharaParty.CharaCondition.EXIST);
                        CharaOnce cChara1 = BattleParam.m_PlayerParty.getPartyMember(GlobalDefine.PartyCharaIndex.MOB_1, CharaParty.CharaCondition.EXIST);
                        CharaOnce cChara2 = BattleParam.m_PlayerParty.getPartyMember(GlobalDefine.PartyCharaIndex.MOB_2, CharaParty.CharaCondition.EXIST);
                        CharaOnce cChara3 = BattleParam.m_PlayerParty.getPartyMember(GlobalDefine.PartyCharaIndex.MOB_3, CharaParty.CharaCondition.EXIST);
                        CharaOnce cChara4 = BattleParam.m_PlayerParty.getPartyMember(GlobalDefine.PartyCharaIndex.FRIEND, CharaParty.CharaCondition.EXIST);
                        float fDamage0 = 0;
                        float fDamage1 = 0;
                        float fDamage2 = 0;
                        float fDamage3 = 0;
                        float fDamage4 = 0;

                        // 敵から受けるダメージをパーティで分散
                        int nCharaTotal = 0;

                        //----------------------------------------
                        // 敵の属性を見て、属性相性ダメージ補正
                        //----------------------------------------
                        if (cChara0 != null && cChara0.m_bHasCharaMasterDataParam) { fDamage0 = InGameUtilBattle.AvoidErrorMultiple(InGameUtilBattle.GetSkillElementRate(nEnemyElement, cChara0.m_CharaMasterDataParam.element), fDamageValue.getValue(GlobalDefine.PartyCharaIndex.LEADER)); nCharaTotal++; }
                        if (cChara1 != null && cChara1.m_bHasCharaMasterDataParam) { fDamage1 = InGameUtilBattle.AvoidErrorMultiple(InGameUtilBattle.GetSkillElementRate(nEnemyElement, cChara1.m_CharaMasterDataParam.element), fDamageValue.getValue(GlobalDefine.PartyCharaIndex.MOB_1)); nCharaTotal++; }
                        if (cChara2 != null && cChara2.m_bHasCharaMasterDataParam) { fDamage2 = InGameUtilBattle.AvoidErrorMultiple(InGameUtilBattle.GetSkillElementRate(nEnemyElement, cChara2.m_CharaMasterDataParam.element), fDamageValue.getValue(GlobalDefine.PartyCharaIndex.MOB_2)); nCharaTotal++; }
                        if (cChara3 != null && cChara3.m_bHasCharaMasterDataParam) { fDamage3 = InGameUtilBattle.AvoidErrorMultiple(InGameUtilBattle.GetSkillElementRate(nEnemyElement, cChara3.m_CharaMasterDataParam.element), fDamageValue.getValue(GlobalDefine.PartyCharaIndex.MOB_3)); nCharaTotal++; }
                        if (cChara4 != null && cChara4.m_bHasCharaMasterDataParam) { fDamage4 = InGameUtilBattle.AvoidErrorMultiple(InGameUtilBattle.GetSkillElementRate(nEnemyElement, cChara4.m_CharaMasterDataParam.element), fDamageValue.getValue(GlobalDefine.PartyCharaIndex.FRIEND)); nCharaTotal++; }
                        fDamage0 /= (float)(nCharaTotal);
                        fDamage1 /= (float)(nCharaTotal);
                        fDamage2 /= (float)(nCharaTotal);
                        fDamage3 /= (float)(nCharaTotal);
                        fDamage4 /= (float)(nCharaTotal);

                        fDamageValue.setValue(target_player, fDamage0 + fDamage1 + fDamage2 + fDamage3 + fDamage4);

                    }

                    // 整数変換
                    fDamageValue = fDamageValue.toMultiInt().toMultiFloat();


                    float rate;
                    //----------------------------------------
                    //	リーダースキルによるダメージ軽減
                    //	※先制不意打ちの際には発動しない→仕様が新しくなり、不意打ちの際にもスキルが発動するようになった
                    //----------------------------------------
                    rate = InGameUtilBattle.GetLeaderSkillDamageDecrease(GlobalDefine.PartyCharaIndex.LEADER, nEnemyElement, nEnemyElemOrg, target_player);
                    fDamageValue.mulValue(target_player, rate);
#if BUILD_TYPE_DEBUG
                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "敵攻撃力:" + fDamageValue.getDebugString(target_player, 1)
                        + " = (敵攻撃力)"
                        + " x" + InGameUtilBattle.getDebugPercentString(rate) + "%(リーダースキルダメージ軽減率)"
                    );
#endif
                    rate = InGameUtilBattle.GetLeaderSkillDamageDecrease(GlobalDefine.PartyCharaIndex.FRIEND, nEnemyElement, nEnemyElemOrg, target_player);
                    fDamageValue.mulValue(target_player, rate);
#if BUILD_TYPE_DEBUG
                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "敵攻撃力:" + fDamageValue.getDebugString(target_player, 1)
                        + " = (敵攻撃力)"
                        + " x" + InGameUtilBattle.getDebugPercentString(rate) + "%(フレンドリーダースキルダメージ軽減率)"
                    );
#endif

                    //----------------------------------------
                    //	パッシブスキルによるダメージ軽減
                    //	※ダメージカット率の累積値をダメージに適用する
                    //	※PT1 3%カット PT2 3%カット = 6%カット
                    //	※ダメージ＝ダメージ*0.94
                    //	※手札バリアにて手札変化処理アリ
                    //----------------------------------------
                    BattleSceneUtil.MultiFloat reduce_rate = InGameUtilBattle.PassiveChkDamageReduce(nEnemyElement, nEnemyKind, nEnemyKind_sub, fDamageValue);
                    fDamageValue.mulValue(target_player, reduce_rate);
#if BUILD_TYPE_DEBUG
                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "敵攻撃力:" + fDamageValue.getDebugString(target_player, 1)
                        + " = (敵攻撃力)"
                        + " x" + reduce_rate.getDebugString(target_player, 100) + "%(パッシブスキルダメージ軽減率)"
                    );
#endif

                    //----------------------------------------
                    //	状態異常によるダメージ軽減
                    //	※先制不意打ちの際には発動しない→仕様が新しくなり、不意打ちの際にもスキルが発動するようになった
                    //----------------------------------------
                    fDamageValue = InGameUtilBattle.DamageReduce(fDamageValue.toMultiInt(), nEnemyElement, BattleParam.m_PlayerParty.m_Ailments, false, false).toMultiFloat();

                    //----------------------------------------
                    //	ダメージ補正
                    //----------------------------------------
                    fDamageValue.maxValue(GlobalDefine.PartyCharaIndex.MAX, 0.0f);

#if BUILD_TYPE_DEBUG
                    // 個別ＨＰ時の敵攻撃力補正（検証用）
                    if (BattleParam.IsKobetsuHP)
                    {
                        // 整数変換
                        BattleSceneUtil.MultiInt tmp_damage = fDamageValue.toMultiInt();

                        BattleSceneUtil.MultiInt damage_mask = fDamageValue.toMultiInt();
                        damage_mask.minValue(GlobalDefine.PartyCharaIndex.MAX, 1);

                        tmp_damage.mulValueF(GlobalDefine.PartyCharaIndex.MAX, BattleLogic.m_KobetsuHP_EnemyAtkRate);
                        // 補正の結果ゼロになってしまったのを１に直す
                        tmp_damage.maxValue(GlobalDefine.PartyCharaIndex.MAX, damage_mask);

                        fDamageValue.setValue(GlobalDefine.PartyCharaIndex.MAX, tmp_damage.toMultiFloat());
                    }
#endif

                    wrk_damage = fDamageValue.toMultiInt();
                }
                else
                {
                    wrk_damage.setValue(target_player, InGameUtilBattle.VALUE_DMG_MAX_PLYAER);
                }

                BattleParam.m_PlayerParty.DamageHP(wrk_damage, damage_target, false, bDrawDamage, nEnemyElement);
            }
        }
        //----------------------------------------
        // 攻撃エネミー自身に、ダメージ処理
        //----------------------------------------
        else
        {
            EDAMAGE_TYPE eDamageType;
            int nEnemyDamage;

            if (bDeath == false)
            {
                //----------------------------------------
                // 弱点チェック
                //----------------------------------------
                eDamageType = InGameUtilBattle.GetSkillElementAffinity(nEnemyElement, nEnemyElemOrg);
                if (eDamageType != EDAMAGE_TYPE.eDAMAGE_TYPE_NORMAL)
                {
                    // 属性倍率
                    float fElementRate = InGameUtilBattle.GetSkillElementRate(nEnemyElement, nEnemyElemOrg);
                    fDamageValue.mulValue(GlobalDefine.PartyCharaIndex.LEADER, fElementRate);

                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "敵" + enemyIndex.ToString()
                        + "への攻撃力:" + fDamageValue.getValue(GlobalDefine.PartyCharaIndex.LEADER).ToString()
                        + " = "/* + fDamageValue.getValue(GlobalDefine.PartyCharaIndex.LEADER).ToString()*/ + "(敵行動基本攻撃力)"
                        + " x" + InGameUtilBattle.getDebugPercentString(fElementRate) + "%(属性倍率)"
                    );
                }
                else
                {
                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "敵" + enemyIndex.ToString()
                        + "への攻撃力:" + fDamageValue.getValue(GlobalDefine.PartyCharaIndex.LEADER).ToString()
                        + " = " + fDamageValue.getValue(GlobalDefine.PartyCharaIndex.LEADER).ToString() + "(敵行動基本攻撃力)"
                    );
                }

                //----------------------------------------
                // 弱点チェック
                //----------------------------------------
                eDamageType = InGameUtilBattle.GetSkillElementAffinity(nEnemyElement, nEnemyElemOrg);

                //----------------------------------------
                // ダメージ軽減
                //----------------------------------------
                nEnemyDamage = InGameUtilBattle.DamageReduce((int)fDamageValue.getValue(GlobalDefine.PartyCharaIndex.LEADER), nEnemyElement, BattleParam.m_EnemyParam[enemyIndex].m_StatusAilmentChara,
                                                        BattleParam.m_EnemyParam[enemyIndex].m_EnemyDefense, false, false, false);
            }
            else
            {
                eDamageType = EDAMAGE_TYPE.eDAMAGE_TYPE_NORMAL;
                nEnemyDamage = InGameUtilBattle.VALUE_DMG_MAX_ENEMY;

                // 強制的にHPを0に設定
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "敵" + enemyIndex.ToString() + "への即死効果");
                BattleParam.m_EnemyParam[enemyIndex].SetHP(0);
            }

            //----------------------------------------
            // HPから減算
            //----------------------------------------
            AddDamageEnemyHP(enemyIndex, nEnemyDamage, eDamageType, GlobalDefine.PartyCharaIndex.MAX, bDrawDamage);
        }

        //----------------------------------------
        //	手札変化
        //----------------------------------------
        if (enemyHandRemake == true)
        {

            MasterDataDefineLabel.ElementType[] elementArray = {    MasterDataDefineLabel.ElementType.NONE,
                        enemyActionParam.Get_HANDCARD_REMAKE_NAUGHT(),
                        enemyActionParam.Get_HANDCARD_REMAKE_FIRE(),
                        enemyActionParam.Get_HANDCARD_REMAKE_WATER(),
                        enemyActionParam.Get_HANDCARD_REMAKE_LIGHT(),
                        enemyActionParam.Get_HANDCARD_REMAKE_DARK(),
                        enemyActionParam.Get_HANDCARD_REMAKE_WIND(),
                        enemyActionParam.Get_HANDCARD_REMAKE_HEAL(),
            };


            //----------------------------------------
            //	手札をすべてチェック
            //----------------------------------------
            for (int i = 0; i < m_Owner.m_BattleCardManager.m_HandArea.getCardMaxCount(); i++)
            {

                BattleScene.BattleCard battle_card = m_Owner.m_BattleCardManager.m_HandArea.getCard(i);
                if (battle_card == null)
                {
                    continue;
                }


                MasterDataDefineLabel.ElementType card_element = battle_card.getElementType();


                bool param_rand = enemyActionParam.Get_HANDCARD_REMAKE_RANDOM();
                if (param_rand)
                {

                    //----------------------------------------
                    //	ランダムフラグON
                    //----------------------------------------
                    if (elementArray[(int)card_element] != MasterDataDefineLabel.ElementType.NONE)
                    {

                        enemyHandRemakeElem[i] = elementArray[(int)card_element];

                    }
                    else
                    {

                        enemyHandRemakeElem[i] = (MasterDataDefineLabel.ElementType)m_Owner.m_EnemySkillHandCardRemake.GetRand((int)MasterDataDefineLabel.ElementType.NAUGHT,
                                                                                        (int)MasterDataDefineLabel.ElementType.MAX);
                    }


                }
                else
                {

                    //----------------------------------------
                    //	ランダムフラグOFF
                    //----------------------------------------
                    if (elementArray[(int)card_element] != MasterDataDefineLabel.ElementType.NONE)
                    {

                        enemyHandRemakeElem[i] = elementArray[(int)card_element];
                    }

                }

            }

            // 変換処理
            for (int i = 0; i < enemyHandRemakeElem.Length; i++)
            {

                //	手札属性と変化先属性が違う場合に処理
                if (enemyHandRemakeElem[i] != MasterDataDefineLabel.ElementType.NONE)
                {

                    BattleScene.BattleCard battle_card = m_Owner.m_BattleCardManager.m_HandArea.getCard(i);
                    battle_card.setElementType(enemyHandRemakeElem[i], BattleScene.BattleCard.ChangeCause.SKILL);

                    //----------------------------------------
                    //	手札変化エフェクト
                    //----------------------------------------
                    m_Owner.m_BattleCardManager.addEffectInfo(BattleScene._BattleCardManager.EffectInfo.EffectPosition.HAND_CARD_AREA, i, BattleScene._BattleCardManager.EffectInfo.EffectType.CARD_DESTROY);
                }

            }

            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "手札変化効果(発動者:敵" + enemyIndex.ToString() + ")");
            DebugBattleLog.outputCard(m_Owner.m_BattleCardManager);
        }

        return fDamageValue.toMultiInt();
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		敵ユニット攻撃更新：ふんばり判定
        @param[in]	int		(playerHP)		ダメージを受ける前のHP
    */
    //----------------------------------------------------------------------------
    private void EnemyAttackAllFunbari(BattleSceneUtil.MultiInt playerHP, GlobalDefine.PartyCharaIndex target_player)
    {

        bool is_disp_cutin = false; // カットインなどの演出を行ったか.

        // リーダー
        {
            BattleSceneUtil.MultiInt funbari_value = InGameUtilBattle.GetLeaderSkillFunbari(GlobalDefine.PartyCharaIndex.LEADER,
                                                   BattleParam.m_PlayerParty.m_HPCurrent,
                                                   BattleParam.m_PlayerParty.m_HPMax, playerHP);
            if (funbari_value.getValue(GlobalDefine.PartyCharaIndex.MAX) > 0)
            {
                bool is_funbatta = false;
                if (target_player >= GlobalDefine.PartyCharaIndex.LEADER && target_player <= GlobalDefine.PartyCharaIndex.FRIEND)
                {
                    if (funbari_value.getValue(target_player) > 0
                        && BattleParam.m_PlayerParty.m_HPCurrent.getValue(target_player) <= 0
                    )
                    {
                        BattleParam.m_PlayerParty.m_HPCurrent.setValue(target_player, 1);
                        is_funbatta = true;
                    }
                }
                else
                {
                    for (int idx = 0; idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); idx++)
                    {
                        if (funbari_value.getValue((GlobalDefine.PartyCharaIndex)idx) > 0
                            && BattleParam.m_PlayerParty.m_HPCurrent.getValue((GlobalDefine.PartyCharaIndex)idx) <= 0
                        )
                        {
                            BattleParam.m_PlayerParty.m_HPCurrent.setValue((GlobalDefine.PartyCharaIndex)idx, 1);
                            is_funbatta = true;
                        }
                    }
                }

                if (is_funbatta && is_disp_cutin == false)
                {
                    is_disp_cutin = true;

                    m_Owner.m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.LEADER_SKILL);
                    m_SkillTitle = true;

                    BattleSkillCutinManager.Instance.ClrSkillCutin();
                    BattleSkillCutinManager.Instance.SkillCutinLeader(GlobalDefine.PartyCharaIndex.LEADER, BattleParam.m_PlayerParty);
                    BattleSkillCutinManager.Instance.CutinStart2(false, true);  //踏ん張り（リーダースキル）
                }
            }
        }

        // フレンド
        {
            BattleSceneUtil.MultiInt funbari_value = InGameUtilBattle.GetLeaderSkillFunbari(GlobalDefine.PartyCharaIndex.FRIEND,
                                                          BattleParam.m_PlayerParty.m_HPCurrent,
                                                          BattleParam.m_PlayerParty.m_HPMax, playerHP);
            if (funbari_value.getValue(GlobalDefine.PartyCharaIndex.MAX) > 0)
            {
                bool is_funbatta = false;
                if (target_player >= GlobalDefine.PartyCharaIndex.LEADER && target_player <= GlobalDefine.PartyCharaIndex.FRIEND)
                {
                    if (funbari_value.getValue(target_player) > 0
                        && BattleParam.m_PlayerParty.m_HPCurrent.getValue(target_player) <= 0
                    )
                    {
                        BattleParam.m_PlayerParty.m_HPCurrent.setValue(target_player, 1);
                        is_funbatta = true;
                    }
                }
                else
                {
                    for (int idx = 0; idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); idx++)
                    {
                        if (funbari_value.getValue((GlobalDefine.PartyCharaIndex)idx) > 0
                            && BattleParam.m_PlayerParty.m_HPCurrent.getValue((GlobalDefine.PartyCharaIndex)idx) <= 0
                        )
                        {
                            BattleParam.m_PlayerParty.m_HPCurrent.setValue((GlobalDefine.PartyCharaIndex)idx, 1);
                            is_funbatta = true;
                        }
                    }
                }

                if (is_funbatta && is_disp_cutin == false)
                {
                    is_disp_cutin = true;

                    m_Owner.m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.LEADER_SKILL);
                    m_SkillTitle = true;

                    BattleSkillCutinManager.Instance.ClrSkillCutin();
                    BattleSkillCutinManager.Instance.SkillCutinLeader(GlobalDefine.PartyCharaIndex.FRIEND, BattleParam.m_PlayerParty);
                    BattleSkillCutinManager.Instance.CutinStart2(false, true);  //踏ん張り（フレンドリーダースキル）
                }
            }
        }

        // パッシブ
        {
            ESKILLTYPE skillType = ESKILLTYPE.eLINKPASSIVE;
            GlobalDefine.PartyCharaIndex charaID = GlobalDefine.PartyCharaIndex.ERROR;
            uint passiveID = 0;
            BattleSceneUtil.MultiInt funbari_value = InGameUtilBattle.PassiveChkFunbari(BattleParam.m_PlayerParty.m_HPCurrent,
                                                    BattleParam.m_PlayerParty.m_HPMax, playerHP,
                                                    ref charaID, ref passiveID, ref skillType);
            if (funbari_value.getValue(GlobalDefine.PartyCharaIndex.MAX) > 0)
            {
                bool is_funbatta = false;
                if (target_player >= GlobalDefine.PartyCharaIndex.LEADER && target_player <= GlobalDefine.PartyCharaIndex.FRIEND)
                {
                    if (funbari_value.getValue(target_player) > 0
                        && BattleParam.m_PlayerParty.m_HPCurrent.getValue(target_player) <= 0
                    )
                    {
                        BattleParam.m_PlayerParty.m_HPCurrent.setValue(target_player, 1);
                        is_funbatta = true;
                    }
                }
                else
                {
                    for (int idx = 0; idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); idx++)
                    {
                        if (funbari_value.getValue((GlobalDefine.PartyCharaIndex)idx) > 0
                            && BattleParam.m_PlayerParty.m_HPCurrent.getValue((GlobalDefine.PartyCharaIndex)idx) <= 0
                        )
                        {
                            BattleParam.m_PlayerParty.m_HPCurrent.setValue((GlobalDefine.PartyCharaIndex)idx, 1);
                            is_funbatta = true;
                        }
                    }
                }

                if (is_funbatta && is_disp_cutin == false)
                {
                    is_disp_cutin = true;

                    // スキルタイプによる分岐
                    BattleCaptionControl.CaptionType caption = BattleCaptionControl.CaptionType.PASSIVE_SKILL;
                    switch (skillType)
                    {
                        case ESKILLTYPE.ePASSIVE: caption = BattleCaptionControl.CaptionType.PASSIVE_SKILL; break;
                        case ESKILLTYPE.eLINKPASSIVE: caption = BattleCaptionControl.CaptionType.LINK_PASSIVE; break;
                    }
                    m_Owner.m_DispCaption.requestCaption(caption);
                    m_SkillTitle = true;

                    BattleSkillCutinManager.Instance.ClrSkillCutin();
                    BattleSkillCutinManager.Instance.SkillCutinRequest(charaID, passiveID, skillType);
                    BattleSkillCutinManager.Instance.CutinStart2(false, true);  //踏ん張り（パッシブスキル）
                }
            }
        }
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		敵ユニット攻撃更新：被ダメージ時のプレイヤースキルカットイン
        @param[in]	int		(playerHP)		ダメージを受ける前のHP
        @change		Developer 2015/09/15 ver300	リンクパッシブ対応
    */
    //----------------------------------------------------------------------------
    private void EnemyAttackCutinDamagePlayer(BattleSceneUtil.MultiInt playerHP)
    {
        //Developer 2017/04/22 この効果を使用するスキルは存在しない（検証用のスキルは存在）
        MasterDataSkillPassive passiveParam = null;
        ESKILLTYPE skillType = ESKILLTYPE.ePASSIVE;
        GlobalDefine.PartyCharaIndex owner = GlobalDefine.PartyCharaIndex.MAX;
        BattleCaptionControl.CaptionType caption = BattleCaptionControl.CaptionType.NONE;
        bool passiveFlag = false;


        if (passiveFlag == false)
        {
            // パッシブ：体力最大時のダメージ軽減倍率
            InGameUtilBattle.PassiveChkDamageReduceFullHP(playerHP, ref owner, ref skillType);

            // 発動者が取得できている場合
            if (owner != GlobalDefine.PartyCharaIndex.MAX)
            {
                // メンバーが設定されているかチェック
                CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember(owner, CharaParty.CharaCondition.SKILL_PASSIVE);
                if (chara_once == null)
                {
                    return;
                }

                //------------------------------
                // スキルタイプによる分岐
                //------------------------------
                switch (skillType)
                {
                    //------------------------------
                    // パッシブスキル
                    //------------------------------
                    #region ==== パッシブスキル処理 ====
                    case ESKILLTYPE.ePASSIVE:
                        {
                            // パッシブスキル取得
                            passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(chara_once.m_CharaMasterDataParam.skill_passive);
                            if (passiveParam == null)
                            {
                                return;
                            }

                            caption = BattleCaptionControl.CaptionType.PASSIVE_SKILL;
                            break;
                        }
                    #endregion

                    //------------------------------
                    // リンクパッシブ
                    //------------------------------
                    #region ==== リンクパッシブ処理 ====
                    case ESKILLTYPE.eLINKPASSIVE:
                        {
                            // メンバーが設定されているかチェック
                            if (chara_once.m_LinkParam == null)
                            {
                                return;
                            }

                            // リンクキャラ情報取得
                            MasterDataParamChara charaParam = null;
                            charaParam = BattleParam.m_MasterDataCache.useCharaParam(chara_once.m_LinkParam.m_CharaID);
                            if (charaParam == null)
                            {
                                return;
                            }

                            // リンクパッシブ取得
                            passiveParam = BattleParam.m_MasterDataCache.useSkillPassive(charaParam.link_skill_passive);
                            if (passiveParam == null)
                            {
                                return;
                            }

                            caption = BattleCaptionControl.CaptionType.LINK_PASSIVE;
                            break;
                        }
                        #endregion
                }

                passiveFlag = true;
            }
        }

        if (passiveFlag == true)
        {
            //----------------------------------------
            // ※今後、他のカットインスキルが追加される場合、カットイン情報を配列化する必要がある
            //----------------------------------------
            //----------------------------------------
            // カットイン演出
            //----------------------------------------
            BattleSkillCutinManager.Instance.ClrSkillCutin();
            BattleSkillCutinManager.Instance.SkillCutinRequest(owner, passiveParam.fix_id, skillType);
            BattleSkillCutinManager.Instance.CutinStart2(false, true);  //ダメージ軽減
            m_Owner.m_DispCaption.requestCaption(caption);
            m_SkillTitle = true;
        }
    }


    //----------------------------------------------------------------------------
    /*!
        @brief		敵行動テーブル選択処理
    */
    //----------------------------------------------------------------------------
    public void EnemyActionTableSelectAll()
    {

        //----------------------------------------
        //	すべての敵の行動テーブルを更新
        //----------------------------------------
        for (int i = 0; i < BattleParam.m_EnemyParam.Length; i++)
        {
            BattleEnemy battle_enemy = BattleParam.m_EnemyParam[i];

            if (battle_enemy == null)
            {
                continue;
            }


            if (battle_enemy.isDead() == true)
            {
                continue;
            }


            //----------------------------------------
            //	敵テーブルコントロールの取得
            //----------------------------------------
            EnemyActionTableControl tableControl = battle_enemy.getEnemyActionTableControl();
            if (tableControl == null)
            {
                continue;
            }


            //----------------------------------------
            // 初回行動タイミングでのテーブル切り替え
            //----------------------------------------
            tableControl.SelectActionTable(m_Owner.m_BattleTotalTurn);

            //----------------------------------------
            // テーブル切り替え時行動チェック
            // @add Developer 2015/12/15 v320
            //----------------------------------------
            if (tableControl.m_ActionSwitchParamID != 0)
            {
                // テーブル切り替え時行動フラグON
                m_abEnemyActionSwitch[i] = true;
            }
        }

    }


    //----------------------------------------------------------------------------
    /*!
        @brief	敵行動テーブル切り替わり時行動確認
        @add	Developer 2015/12/15 v320
    */
    //----------------------------------------------------------------------------
    public void CheckEnemyActionSwitchTable()
    {
        //----------------------------------------
        // すべての敵の行動テーブルを更新
        //----------------------------------------
        for (int num = 0; num < BattleParam.m_EnemyParam.Length; ++num)
        {
            BattleEnemy battle_enemy = BattleParam.m_EnemyParam[num];

            if (battle_enemy == null
            || battle_enemy.isDead() == true)
            {
                continue;
            }

            //----------------------------------------
            // 敵テーブルコントロールの取得
            //----------------------------------------
            EnemyActionTableControl cTableControl = battle_enemy.getEnemyActionTableControl();
            if (cTableControl == null)
            {
                continue;
            }

            //----------------------------------------
            // テーブル切り替え時行動チェック
            //----------------------------------------
            if (cTableControl.CheckActionSwitch(m_Owner.m_BattleTotalTurn) == true)
            {
                // テーブル切り替え時行動フラグON
                m_abEnemyActionSwitch[num] = true;
            }
        }

    }

    //----------------------------------------------------------------------------
    /*!
        @brief		敵行動テーブル選択処理：初回殴りの敵が存在するか
        @retval		bool		[いる/いない]
    */
    //----------------------------------------------------------------------------
    public bool EnemyActionTableCheckFirstAttack()
    {

        MasterDataEnemyActionParam actionParam = null;


        //----------------------------------------
        //	すべての敵の行動テーブルを更新
        //----------------------------------------
        for (int i = 0; i < BattleParam.m_EnemyParam.Length; i++)
        {
            BattleEnemy battle_enemy = BattleParam.m_EnemyParam[i];

            //----------------------------------------
            //	エラーチェック
            //----------------------------------------
            if (battle_enemy == null)
            {
                continue;
            }


            //----------------------------------------
            //	敵テーブルコントロールの取得
            //----------------------------------------
            EnemyActionTableControl tableControl = battle_enemy.getEnemyActionTableControl();
            if (tableControl == null)
            {
                continue;
            }


            //----------------------------------------
            // 初回行動タイミングでのテーブル切り替え
            //----------------------------------------
            actionParam = tableControl.GetActionFirstAttack();
            if (actionParam == null)
            {
                continue;
            }

            break;
        }


        return (actionParam != null) ? true : false;

    }

    //----------------------------------------------------------------------------
    /*!
        @brief		敵ユニット攻撃更新：リアクション
        @param[in]	bool		(bCounter)		カウンターあり/なし
        @retval		bool		[更新終了/継続]
        @note		カウンターは特定タイミングのみで呼び出すこと
                    現在は、バックアタック時と敵の攻撃を受けた時
    */
    //----------------------------------------------------------------------------
    public bool EnemyAttackReaction(bool bCounter)
    {
        //----------------------------------------
        // 敵の攻撃は少しずつずらして発行
        //----------------------------------------
        m_EnemyTurnDelta -= Time.deltaTime;
        if (m_EnemyTurnDelta > 0.0f)
        {
            return false;
        }
        else
        {
            m_EnemyTurnDelta = 0.0f;
        }

        //----------------------------------------
        // 敵更新継続フラグ
        //----------------------------------------
        bool bEnemyUpdate = false;


        //----------------------------------------
        // リアクション全チェック(1更新、1行動づつ処理していく)
        //----------------------------------------
        while (m_CurrentProcessIndex < EnemyAbility.getReactionCount())
        {
            //----------------------------------------
            // 動きが速すぎるので、適当に待つ
            //----------------------------------------
            if (isWaitingActionTimer())
            {
                return false;
            }

            EnemyAbility.ReactionInfo reaction_info = EnemyAbility.getReaction(m_CurrentProcessIndex);
            BattleEnemy battle_enemy_param = BattleParam.m_EnemyParam[reaction_info.m_EnemyPartyIndex];
            //----------------------------------------
            // エラーチェック
            // 死亡した敵は処理しない
            //----------------------------------------
            if (battle_enemy_param == null
            || battle_enemy_param.m_EnemyHP <= 0)
            {
                m_CurrentProcessIndex++;
                m_CurrentProcessTargets = null;
                m_bPlayVoice = false;
                continue;
            }


            //----------------------------------------
            // リアクション行動取得
            // @change Developer 2015/12/17 v320
            //----------------------------------------
            MasterDataEnemyActionParam enemyActionParam = null;
            if (m_anEnemyNextActionID[m_CurrentProcessIndex] == 0)
            {
                enemyActionParam = reaction_info.m_EnemyReactParam;
            }
            //----------------------------------------
            // 連続行動取得
            // @add Developer 2015/12/11 v320 敵連続行動対応
            //----------------------------------------
            else
            {
                //----------------------------------------
                //	動きが速すぎるので、適当に待つ
                //----------------------------------------
                if (isWaitingActionTimer())
                {
                    return false;
                }

                // 次の行動情報に更新
                enemyActionParam = BattleParam.m_MasterDataCache.useEnemyActionParam((uint)m_anEnemyNextActionID[m_CurrentProcessIndex]);
            }

            //----------------------------------------
            // 敵行動チェック
            //----------------------------------------
            if (enemyActionParam == null)
            {
                continue;
            }


            //----------------------------------------
            // 更新中フラグをたてる
            //----------------------------------------
            bEnemyUpdate = true;


            //----------------------------------------
            // カットイン終了待ち-何かしらカットインが発生した場合
            //----------------------------------------
            if (BattleSkillCutinManager.Instance.isRunning() == true)
            {
                return false;
            }

            //----------------------------------------
            // フェーズタイトルの切り替え-タイトル変更時(リーダースキルか、パッシブスキルになっている)
            //----------------------------------------
            if (m_SkillTitle == true
            && m_Owner.m_DispCaption.getRequestCaption() != BattleCaptionControl.CaptionType.ENEMY_PHASE)
            {
                m_SkillTitle = false;
                m_Owner.m_DispCaption.requestCaption(BattleCaptionControl.CaptionType.ENEMY_PHASE);
            }

            if (m_bPlayVoice == false)
            {
                m_bPlayVoice = true;

                //----------------------------------------
                // ボイス再生
                //----------------------------------------
                MasterDataAudioData cAudioMaster = MasterDataUtil.GetMasterDataAudioDataFromID(enemyActionParam.audio_data_id);
                if (cAudioMaster != null)
                {
                    SoundUtil.PlayVoice(cAudioMaster.fix_id);
                }

                //----------------------------------------
                // メッセージ表示
                //----------------------------------------
                m_Owner.m_BattleDispEnemy.talk(reaction_info.m_EnemyPartyIndex, enemyActionParam);
            }

            //----------------------------------------
            // メッセージ表示待機
            //----------------------------------------
            if (m_Owner.m_BattleDispEnemy.isTalking())
            {
                return false;
            }

            // 攻撃対象を決定（複数ターゲットの内の一体）
            GlobalDefine.PartyCharaIndex target_player = updateEnemyActionTargetPlayer(enemyActionParam);

            if (target_player != GlobalDefine.PartyCharaIndex.ERROR)
            {
                BattleParam.m_EnemyToPlayerTarget = target_player;

                //----------------------------------------
                // ダメージを受ける前のHPを取得
                //----------------------------------------
                BattleSceneUtil.MultiInt playerHP = new BattleSceneUtil.MultiInt(BattleParam.m_PlayerParty.m_HPCurrent);


                //----------------------------------------
                // ダメージ処理
                //----------------------------------------
                BattleSceneUtil.MultiInt damage_target;
                BattleSceneUtil.MultiInt damage_value = EnemyAttackAllDamage(reaction_info.m_EnemyPartyIndex, enemyActionParam, target_player, out damage_target);
#if BUILD_TYPE_DEBUG
                BattleDebugMenu.DispDamagePlayer(battle_enemy_param, damage_value, damage_target);
#endif

                //----------------------------------------
                // @change	Developer 2016/02/03 v330 敵攻撃対象指定
                // @note	プレイヤーに、ダメージが発生する場合のみ処理する
                //----------------------------------------
                MasterDataDefineLabel.TargetType nAttackTarget = MasterDataDefineLabel.TargetType.NONE;
                if (enemyActionParam != null)
                {
                    nAttackTarget = enemyActionParam.attack_target;
                }
                if (damage_value.getValue(target_player) > 0
                && nAttackTarget != MasterDataDefineLabel.TargetType.SELF
                && nAttackTarget != MasterDataDefineLabel.TargetType.FRIEND
                && nAttackTarget != MasterDataDefineLabel.TargetType.SELF_OTHER_FRIEND)
                {
                    //----------------------------------------
                    // 被ダメージ時のプレイヤースキルカットインチェック
                    //----------------------------------------
                    EnemyAttackCutinDamagePlayer(playerHP);

                    //----------------------------------------
                    // ふんばりチェック
                    //----------------------------------------
                    EnemyAttackAllFunbari(playerHP, target_player);

                    //----------------------------------------
                    // カウンターチェック
                    // 場所によっては都合が悪い場合があるのでカウンターON/OFFができるようにした。
                    //----------------------------------------
                    if (bCounter == true)
                    {
                        m_Owner.m_BattleLogicSkill.EnemyAttackAllCounter(reaction_info.m_EnemyPartyIndex, damage_value, playerHP);
                    }
                }

                //----------------------------------------
                // 演出関連処理
                //----------------------------------------
                m_Owner.m_BattleDispEnemy.attack(reaction_info.m_EnemyPartyIndex, enemyActionParam, damage_value, damage_target);

                //----------------------------------------
                //	待機時間の更新
                //----------------------------------------
                clearActionTimer();
                m_EnemyTurnDelta = 0.5f;    //攻撃アニメーションの時間
                return false;
            }

            //----------------------------------------
            // 連続行動チェック(プレイヤーの状態に関係なく、出し切る)
            // @add Developer 2015/12/11 v320 敵連続行動対応
            //----------------------------------------
            if (enemyActionParam.add_fix_id > 0)
            {
                // 次の行動を保存
                m_anEnemyNextActionID[m_CurrentProcessIndex] = enemyActionParam.add_fix_id;

                //----------------------------------------
                //	待機時間の更新
                //----------------------------------------
                clearActionTimer();
                m_EnemyTurnDelta = (float)RandManager.GetRand(20, 30) * 0.01f;
                m_bPlayVoice = false;

                return false;
            }
            else
            {
                m_anEnemyNextActionID[m_CurrentProcessIndex] = 0;
            }

            //----------------------------------------
            // 攻撃済みフラグをセット
            //----------------------------------------
            battle_enemy_param.setAttackFlag(true);

            //----------------------------------------
            // 次の処理対象へ
            //----------------------------------------
            m_CurrentProcessIndex++;
            m_CurrentProcessTargets = null;
            m_bPlayVoice = false;

            //----------------------------------------
            // 待機時間の更新
            //----------------------------------------
            clearActionTimer();
            m_EnemyTurnDelta = (float)RandManager.GetRand(20, 30) * 0.01f;

            break;
        }


        //----------------------------------------
        // 敵更新中ならシーケンス維持
        //----------------------------------------
        if (bEnemyUpdate == true)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 敵アクションの攻撃対象を更新し取得（一つのアクションに複数ターゲットが設定されている場合、複数回に分けて処理）
    /// </summary>
    /// <param name="enemy_action_param"></param>
    /// <returns></returns>
    private GlobalDefine.PartyCharaIndex updateEnemyActionTargetPlayer(MasterDataEnemyActionParam enemy_action_param)
    {
        GlobalDefine.PartyCharaIndex target_player = GlobalDefine.PartyCharaIndex.ERROR;

        if (m_CurrentProcessTargets != null)
        {
            if (m_CurrentProcessTargetIndex >= m_CurrentProcessTargets.Length)
            {
                // 攻撃対象全てに攻撃したのでアクション終了
                m_CurrentProcessTargets = null;
                m_CurrentProcessTargetIndex = 0;
                return target_player;
            }
        }

        if (m_CurrentProcessTargets == null)
        {
            // 攻撃対象が設定されていないのでアクションの開始と判定→攻撃対象を決定
            m_CurrentProcessTargets = makeTargets(enemy_action_param);
            m_CurrentProcessTargetIndex = 0;
        }

        // 複数ターゲットの内の一体を取り出す
        if (m_CurrentProcessTargets != null
            && m_CurrentProcessTargetIndex < m_CurrentProcessTargets.Length
        )
        {
            target_player = m_CurrentProcessTargets[m_CurrentProcessTargetIndex];
            m_CurrentProcessTargetIndex++;
            if (target_player == GlobalDefine.PartyCharaIndex.GENERAL)
            {
                target_player = BattleParam.m_PlayerParty.selectTargetPlayer(!BattleLogic.m_KobetsuHPEnemyTargetHate);
            }
        }

        return target_player;
    }

    /// <summary>
    /// 攻撃対象を生成（複数ターゲット）
    /// </summary>
    /// <param name="enemy_action_param"></param>
    /// <returns></returns>
    private static GlobalDefine.PartyCharaIndex[] makeTargets(MasterDataEnemyActionParam enemy_action_param)
    {
        // 攻撃対象を生成
        GlobalDefine.PartyCharaIndex[] ret_val = null;

        if (BattleParam.IsKobetsuHP
            && enemy_action_param != null)
        {
            MasterDataDefineLabel.TargetType target_type = enemy_action_param.attack_target;
            int target_num = enemy_action_param.attack_target_num;
            if (target_num < 1)
            {
                target_num = 1;
            }

            switch (target_type)
            {
                case MasterDataDefineLabel.TargetType.OTHER:
                    // 敵単体を一回攻撃
                    ret_val = new GlobalDefine.PartyCharaIndex[1];
                    ret_val[0] = GlobalDefine.PartyCharaIndex.GENERAL;
                    break;

                case MasterDataDefineLabel.TargetType.ENEMY:
                case MasterDataDefineLabel.TargetType.ALL:
                case MasterDataDefineLabel.TargetType.SELF_OTHER_ALL:
                    // 敵全体を一回攻撃
                    ret_val = new GlobalDefine.PartyCharaIndex[1];
                    ret_val[0] = GlobalDefine.PartyCharaIndex.MAX;
                    break;

                case MasterDataDefineLabel.TargetType.ENE_N_1:  //使用者の敵Ｎ体へ１回攻撃（５を指定した場合全体攻撃）
                case MasterDataDefineLabel.TargetType.ENE_1N_1: //使用者の敵１～Ｎ体へ１回攻撃
                    {
                        if (target_type == MasterDataDefineLabel.TargetType.ENE_1N_1)
                        {
                            target_num = (int)RandManager.GetRand(0, (uint)target_num) + 1;
                        }

                        CharaOnce[] alive_party_members = BattleParam.m_PlayerParty.getPartyMembers(CharaParty.CharaCondition.ALIVE);
                        int alive_count = alive_party_members.Length;
                        GlobalDefine.PartyCharaIndex[] alive_targets = new GlobalDefine.PartyCharaIndex[alive_count];
                        for (int idx = 0; idx < alive_count; idx++)
                        {
                            alive_targets[idx] = alive_party_members[idx].m_PartyCharaIndex;
                        }

                        target_num = Mathf.Min(target_num, alive_party_members.Length);

                        ret_val = new GlobalDefine.PartyCharaIndex[target_num];
                        for (int idx = 0; idx < target_num; idx++)
                        {
                            uint rand_index = RandManager.GetRand(0, (uint)alive_count);
                            ret_val[idx] = alive_targets[rand_index];

                            alive_count--;
                            alive_targets[rand_index] = alive_targets[alive_count];
                        }
                    }
                    break;

                case MasterDataDefineLabel.TargetType.ENE_R_N:  //使用者の敵Ｎ回攻撃（攻撃対象は毎回選択）
                    {
                        ret_val = new GlobalDefine.PartyCharaIndex[target_num];
                        for (int idx = 0; idx < ret_val.Length; idx++)
                        {
                            ret_val[idx] = GlobalDefine.PartyCharaIndex.GENERAL;
                        }
                    }
                    break;

                case MasterDataDefineLabel.TargetType.ENE_1_N:  //使用者の敵１体をＮ回攻撃
                    {
                        GlobalDefine.PartyCharaIndex target = BattleParam.m_PlayerParty.selectTargetPlayer(!BattleLogic.m_KobetsuHPEnemyTargetHate);
                        ret_val = new GlobalDefine.PartyCharaIndex[target_num];
                        for (int idx = 0; idx < ret_val.Length; idx++)
                        {
                            ret_val[idx] = target;
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        //その他
        if (ret_val == null)
        {
            ret_val = new GlobalDefine.PartyCharaIndex[1];

            if (BattleLogic.m_KobetsuHPEnemyAttackAll)
            {
                //使用者の敵全体を１回攻撃
                ret_val[0] = GlobalDefine.PartyCharaIndex.MAX;
            }
            else
            {
                //使用者の敵１体を１回攻撃
                ret_val[0] = GlobalDefine.PartyCharaIndex.GENERAL;
            }
        }

        return ret_val;
    }

    // 根性を初期化
    public void setKonjo()
    {
        for (int idx = 0; idx < BattleParam.m_EnemyParam.Length; idx++)
        {
            BattleEnemy battle_enemy = BattleParam.m_EnemyParam[idx];

            if (battle_enemy != null)
            {
                battle_enemy.setKonjoHP();
            }
        }
    }

    // 根性を実行
    public void execKonjo()
    {
        for (int idx = 0; idx < BattleParam.m_EnemyParam.Length; idx++)
        {
            BattleEnemy battle_enemy = BattleParam.m_EnemyParam[idx];

            if (battle_enemy != null)
            {
                int konjo_recov_hp = battle_enemy.execKonjoHP();
                if (konjo_recov_hp > 0)
                {
                    m_Owner.m_BattleDispEnemy.damageEnemy(idx, konjo_recov_hp, EDAMAGE_TYPE.eDAMAGE_TYPE_HEAL);
                }
            }
        }
    }
}

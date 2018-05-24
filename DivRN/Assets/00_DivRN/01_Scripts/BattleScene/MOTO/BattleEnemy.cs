using System;

//============================================================================
//	class
//============================================================================
//----------------------------------------------------------------------------
/*!
    @brief	バトルシーン敵管理クラス
*/
//----------------------------------------------------------------------------
[Serializable]
public class BattleEnemy
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/
    //------------------------------------------------
    // ※※※※※※※※※※※※※※※※※※※※※※
    // ローカルセーブとして文字列セーブしてJsonで構造体化する対象クラス。
    // 変数の削減によって解析エラーが発生するようになるため、扱いには注意すること。
    // ※※※※※※※※※※※※※※※※※※※※※※
    //------------------------------------------------

    public int m_EnemyID = 0;        //!< m_MasterDataParamEnemy.fix_id

    private MasterDataParamEnemy m_MasterDataParamEnemy = null;     //!< 敵情報：敵マスターデータ
    private MasterDataParamChara m_MasterDataParamChara = null;     //!< 敵情報：敵マスターデータ

    public int m_EnemyTurn = 0;     //!< 敵情報：敵ターン
    public int m_EnemyTurnMax = 0;      //!< 敵情報：敵ターン最大
    public int m_EnemyHP = 10;      //!< 敵情報：HP
    public int m_EnemyHPMax = 10;       //!< 敵情報：HP最大
    public int m_EnemyAttack = 10;      //!< 敵情報：攻撃力
    public int m_EnemyDefense = 10;     //!< 敵情報：防御力
    public int m_EnemyDrop = 0;     //!< 敵情報：撃破時ドロップ
    public StatusAilmentChara m_StatusAilmentChara = null;  // パーティ状態異常

    public bool m_Attack = false;   //!< ターン遅延がかかってから攻撃を行ったかどうか

    public bool m_IsShow = true;    //!< 表示中かどうか
    public bool m_IsAcquireDropUnit = false;    //!< ドロップユニット取得済みかどうか

    public uint[] m_AdditionalEnemyAbilityIDs = null;     //!< 付与された追加敵特性ID
    public int[] m_AdditionalEnemyAbilityTurns = null;     //!< 付与された追加敵特性の残りターン数

#if BUILD_TYPE_DEBUG
    public bool m_DebugNoDeadFlag = false;
#endif

    private EnemyActionTableControl m_EnemyActionTableControl = null;  // 敵行動管理
    private int m_KonjoHP = 0;   // 根性判定用HP

    /*==========================================================================*/
    /*		func																*/
    /*==========================================================================*/
    //	@brief		マスター取得アクセサ(プロパティだとLitJsonで問題があるので関数化)
    public MasterDataParamEnemy getMasterDataParamEnemy()
    {
        return m_MasterDataParamEnemy;
    }
    public MasterDataParamChara getMasterDataParamChara()
    {
        return m_MasterDataParamChara;
    }

    public void setMasterData(MasterDataParamEnemy master_data_param_enemy, MasterDataParamChara master_data_param_chara)
    {
        m_EnemyID = (int)master_data_param_enemy.fix_id;
        m_MasterDataParamEnemy = master_data_param_enemy;
        m_MasterDataParamChara = master_data_param_chara;
    }

    //	@brief		メイン種族取得アクセサ(プロパティだとLitJsonで問題があるので関数化)
    public MasterDataDefineLabel.KindType getKind()
    {
        if (m_MasterDataParamChara == null)
        {
            return MasterDataDefineLabel.KindType.NONE;
        }

        return m_MasterDataParamChara.kind;
    }


    // @brief		サブ種族取得アクセサ(プロパティだとLitJsonで問題があるので関数化)
    public MasterDataDefineLabel.KindType getKindSub()
    {
        if (m_MasterDataParamChara == null)
        {
            return MasterDataDefineLabel.KindType.NONE;
        }

        return m_MasterDataParamChara.sub_kind;
    }


    //	@brief		攻撃フラグアクセサ(プロパティだとLitJsonで問題があるので関数化)
    public bool getAttackFlag()
    {
        return m_Attack;
    }

    //(プロパティだとLitJsonで問題があるので関数化)
    public void setAttackFlag(bool value)
    {
        m_Attack = value;
    }


    //	@brief		ユニット名取得(プロパティだとLitJsonで問題があるので関数化)
    public string getName()
    {
        if (m_MasterDataParamChara == null)
        {
            return "";
        }

        return m_MasterDataParamChara.name;
    }

    /// <summary>
    /// 対象が表示中かどうかを取得(プロパティだとLitJsonで問題があるので関数化)
    /// </summary>
    public bool isShow()
    {
        return m_IsShow;
    }

    //(プロパティだとLitJsonで問題があるので関数化)
    public void setShow(bool value)
    {
        m_IsShow = value;
    }

    // ドロップユニットを取得済みかどうかを取得
    public bool isAcquireDropUnit()
    {
        return m_IsAcquireDropUnit;
    }

    // ドロップユニットを取得済みかどうかを設定
    public void setAcquireDropUnit(bool is_get)
    {
        m_IsAcquireDropUnit = is_get;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		対象が死亡しているか確認(プロパティだとLitJsonで問題があるので関数化)
        @retval		bool		[死亡している/していない]
    */
    //----------------------------------------------------------------------------
    public bool isDead()
    {
        return (m_EnemyHP <= 0) ? true : false;
    }

    public bool updateTurn()
    {
        if (m_EnemyHP <= 0)
        {
            return false;
        }

        if (m_EnemyTurn < 0)
        {
            return false;
        }

        m_EnemyTurn--;

        updateAdditionalEnemyAbilityTurn();

        return true;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief	デフォルトコンストラクタ
    */
    //----------------------------------------------------------------------------
    public BattleEnemy()
    {
        m_EnemyID = 0;
        m_MasterDataParamEnemy = null;      // 敵情報：敵マスターデータ
        m_MasterDataParamChara = null;      // 敵情報：敵マスターデータ

        m_EnemyTurn = 0;        // 敵情報：敵ターン
        m_EnemyHP = 10;     // 敵情報：HP
        m_EnemyHPMax = 10;      // 敵情報：HP最大
        m_EnemyAttack = 10;     // 敵情報：攻撃力
        m_EnemyDefense = 10;        // 敵情報：防御力
        m_EnemyDrop = 0;        // 敵情報：撃破時ドロップ
        m_StatusAilmentChara = null;        // 敵情報：状態異常管理番号
        m_Attack = true;        // ターン遅延がかかってから攻撃を行ったかどうか

        m_EnemyActionTableControl = new EnemyActionTableControl();
        m_EnemyActionTableControl.m_BattleEnemy = this;
        m_AdditionalEnemyAbilityIDs = new uint[EnemyAbility.ENEMY_ABILITY_ADD_MAX];
        m_AdditionalEnemyAbilityTurns = new int[EnemyAbility.ENEMY_ABILITY_ADD_MAX];
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		ターンの加算
        @param[in]	int		(val)		変化量
    */
    //----------------------------------------------------------------------------
    public void AddTurn(int val)
    {

        if (val <= 0)
        {
            return;
        }

        m_EnemyTurn += val;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		ターンの減算
        @param[in]	int		(val)		変化量
    */
    //----------------------------------------------------------------------------
    public void DelTurn(int val)
    {

        if (val <= 0)
        {
            return;
        }

        m_EnemyTurn -= val;
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		HP加算
        @param[in]	int		(val)		加算量
    */
    //----------------------------------------------------------------------------
    public void AddHP(int val)
    {
        if (val <= 0)
        {
            return;
        }

        m_EnemyHP += val;
        if (m_EnemyHP > m_EnemyHPMax)
        {
            m_EnemyHP = m_EnemyHPMax;
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		HP減算
        @param[in]	int		(val)		加算量
        @param[in]	int		(min)		最低値
        @param[in]	bool	(nodead)	死なない
    */
    //----------------------------------------------------------------------------
    public void DelHP(int val, int min, bool nodead)
    {
#if BUILD_TYPE_DEBUG
        if (m_DebugNoDeadFlag)
        {
            nodead = true;
        }
#endif

        int old_hp = m_EnemyHP;

        // 最低ダメージの保障
        if (val <= min)
        {
            val = min;
        }

        m_EnemyHP -= val;

        // 死なないフラグの処理
        if (m_EnemyHP <= 0)
        {
            if (nodead == true)
            {
                m_EnemyHP = 1;
            }
            else
            {
                m_EnemyHP = 0;
            }
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		HP設定
        @param[in]	int		(val)		設定値
    */
    //----------------------------------------------------------------------------
    public void SetHP(int val)
    {
        if (val < 0)
        {
            val = 0;
        }

        if (val == 0)
        {
            // 即死攻撃で死亡するときは根性を無効にする
            m_KonjoHP = 0;
        }

        m_EnemyHP = val;
        if (m_EnemyHP > m_EnemyHPMax)
        {
            m_EnemyHP = m_EnemyHPMax;
        }
    }

    //----------------------------------------------------------------------------
    /*!
        @brief		ターンの更新情報のクリア
    */
    //----------------------------------------------------------------------------
    public void ClearOnAttack()
    {
        setAttackFlag(true);
    }

    public EnemyActionTableControl getEnemyActionTableControl()
    {
        return m_EnemyActionTableControl;
    }

    public uint[] getEnemyAbilitys()
    {
        uint[] ret_val = null;

        MasterDataParamEnemy master_data_param_enemy = m_MasterDataParamEnemy;
        if (master_data_param_enemy != null)
        {
            ret_val = new uint[EnemyAbility.ENEMY_ABILITY_MAX];

            ret_val[0] = master_data_param_enemy.ability1;
            ret_val[1] = master_data_param_enemy.ability2;
            ret_val[2] = master_data_param_enemy.ability3;
            ret_val[3] = master_data_param_enemy.ability4;
            ret_val[4] = master_data_param_enemy.ability5;
            ret_val[5] = master_data_param_enemy.ability6;
            ret_val[6] = master_data_param_enemy.ability7;
            ret_val[7] = master_data_param_enemy.ability8;

            for (int idx = 0; idx < m_AdditionalEnemyAbilityIDs.Length; idx++)
            {
                ret_val[idx + EnemyAbility.ENEMY_ABILITY_DEFAULT_MAX] = m_AdditionalEnemyAbilityIDs[idx];
            }
        }

        return ret_val;
    }

    public int[] getEnemyAbilityTurns()
    {
        int[] ret_val = null;

        MasterDataParamEnemy master_data_param_enemy = m_MasterDataParamEnemy;
        if (master_data_param_enemy != null)
        {
            const int FIXED_ABILITY_TURN = 0;   // 追加分でない敵特性のターン数（ターン経過してもこの値から変化しない）

            ret_val = new int[EnemyAbility.ENEMY_ABILITY_MAX];

            ret_val[0] = FIXED_ABILITY_TURN;
            ret_val[1] = FIXED_ABILITY_TURN;
            ret_val[2] = FIXED_ABILITY_TURN;
            ret_val[3] = FIXED_ABILITY_TURN;
            ret_val[4] = FIXED_ABILITY_TURN;
            ret_val[5] = FIXED_ABILITY_TURN;
            ret_val[6] = FIXED_ABILITY_TURN;
            ret_val[7] = FIXED_ABILITY_TURN;

            for (int idx = 0; idx < m_AdditionalEnemyAbilityIDs.Length; idx++)
            {
                ret_val[idx + EnemyAbility.ENEMY_ABILITY_DEFAULT_MAX] = m_AdditionalEnemyAbilityTurns[idx];
            }
        }

        return ret_val;
    }

    /// <summary>
    /// 追加敵特性を付与
    /// 同じ特性がある場合はターン数を上書き（0ターンで上書きすれば消せる）
    /// デフォルトの特性は上書きできない
    /// </summary>
    /// <param name="enemy_ability_fix_id"></param>
    /// <param name="turn"></param>
    public void giveAdditionalEnemyAbility(uint enemy_ability_fix_id, int turn)
    {
        MasterDataEnemyAbility master_data_enemy_ability = BattleParam.m_MasterDataCache.useEnemyAbility(enemy_ability_fix_id);
        if (master_data_enemy_ability != null)
        {
            _removeAdditionalEnemyAbility(enemy_ability_fix_id);

            if (turn > 0)
            {
                for (int idx = 0; idx < m_AdditionalEnemyAbilityIDs.Length; idx++)
                {
                    if (m_AdditionalEnemyAbilityIDs[idx] == 0)
                    {
                        m_AdditionalEnemyAbilityIDs[idx] = enemy_ability_fix_id;
                        m_AdditionalEnemyAbilityTurns[idx] = turn;
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 追加敵特性のターン数を更新
    /// </summary>
    private void updateAdditionalEnemyAbilityTurn()
    {
        for (int idx = 0; idx < m_AdditionalEnemyAbilityIDs.Length; idx++)
        {
            if (m_AdditionalEnemyAbilityIDs[idx] != 0)
            {
                m_AdditionalEnemyAbilityTurns[idx]--;
                if (m_AdditionalEnemyAbilityTurns[idx] <= 0)
                {
                    m_AdditionalEnemyAbilityIDs[idx] = 0;
                }
            }
        }

        _removeAdditionalEnemyAbility(0);
    }

    /// <summary>
    /// 追加敵特性を取り除く
    /// </summary>
    private void _removeAdditionalEnemyAbility(uint enemy_ability_fix_id)
    {
        if (enemy_ability_fix_id != 0)
        {
            for (int idx = 0; idx < m_AdditionalEnemyAbilityIDs.Length; idx++)
            {
                if (m_AdditionalEnemyAbilityIDs[idx] == enemy_ability_fix_id)
                {
                    m_AdditionalEnemyAbilityIDs[idx] = 0;
                }
            }
        }

        // 配列を前へ詰める
        int idx2 = 0;
        for (int idx = 0; idx < m_AdditionalEnemyAbilityIDs.Length; idx++)
        {
            if (m_AdditionalEnemyAbilityIDs[idx] != 0)
            {
                m_AdditionalEnemyAbilityIDs[idx2] = m_AdditionalEnemyAbilityIDs[idx];
                m_AdditionalEnemyAbilityTurns[idx2] = m_AdditionalEnemyAbilityTurns[idx];
                idx2++;
            }
        }
        for (int idx = idx2; idx < m_AdditionalEnemyAbilityIDs.Length; idx++)
        {
            m_AdditionalEnemyAbilityIDs[idx] = 0;
            m_AdditionalEnemyAbilityTurns[idx] = 0;
        }
    }

    public int execKonjoHP()
    {
        int ret_val = 0;
        if (m_KonjoHP > 0
            && m_EnemyHP <= 0
        )
        {
            uint[] enemy_abilitys = getEnemyAbilitys();
            if (enemy_abilitys != null)
            {
                for (int idx = 0; idx < enemy_abilitys.Length; idx++)
                {
                    MasterDataEnemyAbility master_data_enemy_ability = BattleParam.m_MasterDataCache.useEnemyAbility(enemy_abilitys[idx]);
                    if (master_data_enemy_ability != null
                        && master_data_enemy_ability.category == MasterDataDefineLabel.EnemyAbilityType.KONJO
                    )
                    {
                        float triger_hp_rate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_KONJO_TRIGER_HP_RATE());
                        if (triger_hp_rate > 0)
                        {
                            int triger_hp = (int)InGameUtilBattle.AvoidErrorMultiple(m_EnemyHPMax, triger_hp_rate);
                            if (triger_hp < 1)
                            {
                                triger_hp = 1;
                            }

                            if (m_KonjoHP >= triger_hp)
                            {
                                float keep_hp_rate = InGameUtilBattle.GetDBRevisionValue(master_data_enemy_ability.Get_KONJO_KEEP_HP_RATE());
                                int keep_hp = (int)InGameUtilBattle.AvoidErrorMultiple(m_EnemyHPMax, keep_hp_rate);
                                if (keep_hp < 1)
                                {
                                    keep_hp = 1;
                                }

                                // 発動条件を満たした中で一番残HPが多いものを採用
                                if (keep_hp > ret_val)
                                {
                                    ret_val = keep_hp;
                                }
                            }
                        }
                    }
                }
            }

            //根性発動
            if (ret_val > 0)
            {
                SetHP(ret_val);
            }
        }

        m_KonjoHP = 0;

        return ret_val;
    }

    public void setKonjoHP()
    {
        m_KonjoHP = m_EnemyHP;
    }
} // class BattleEnemy

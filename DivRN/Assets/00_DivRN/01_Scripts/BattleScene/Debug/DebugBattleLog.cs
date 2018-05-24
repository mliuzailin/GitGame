using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// デバッグログ出力クラス
/// BUILD_TYPE_DEBUG 非定義時には出力しません。また、呼び出し側のメソッドも削除されます.
/// BUILD_TYPE_DEBUG は [File]-[Build Setting]-[Player Settings]-[Other Settings]-[Scripting Define Symbols]に定義されている必要があります。
/// </summary>
public class DebugBattleLog
{
#if BUILD_TYPE_DEBUG
    private static bool m_IsEnable = false;
    private static StringBuilder m_Text = new StringBuilder("");
    private static Dictionary<string, int> m_Values = new Dictionary<string, int>();
    private static bool m_IsLineTop = true;
    private static string m_DateTime = "";
    private static int m_LineNo = 0;

    private static string m_FileName = null;
    private static bool m_IsNewFile = true;

    private static StringOperationUtil.OptimizedStringOperation m_StrOpe = StringOperationUtil.OptimizedStringOperation.i;    // 一つだけ確保して使いまわす
#endif

    public static StringOperationUtil.OptimizedStringOperation StrOpe
    {
        get
        {
#if BUILD_TYPE_DEBUG
            return m_StrOpe;
#else
            return null;
#endif
        }
    }

    [System.Diagnostics.Conditional("BUILD_TYPE_DEBUG")]
    public static void init()
    {
#if BUILD_TYPE_DEBUG
        m_Text.Length = 0;
        m_Values = new Dictionary<string, int>();
        m_IsLineTop = true;
        m_DateTime = "";
        m_LineNo = 0;
#endif
    }

    [System.Diagnostics.Conditional("BUILD_TYPE_DEBUG")]
    public static void setEnable(bool is_enable)
    {
#if BUILD_TYPE_DEBUG
        m_IsEnable = is_enable;
#endif
    }

    public static bool getEneble()
    {
#if BUILD_TYPE_DEBUG
        return m_IsEnable;
#else
        return false;
#endif
    }

    [System.Diagnostics.Conditional("BUILD_TYPE_DEBUG")]
    public static void setFileName(string file_name)
    {
#if BUILD_TYPE_DEBUG
        flush();
        m_FileName = file_name;
        m_IsNewFile = true;
#endif
    }

    public static string getFileName()
    {
#if BUILD_TYPE_DEBUG
        return m_FileName;
#else
        return "";
#endif
    }

    public static string getSavePath()
    {
#if BUILD_TYPE_DEBUG
        return Application.persistentDataPath;
#else
        return "";
#endif
    }

    public static void flush()
    {
#if BUILD_TYPE_DEBUG
        _exportToFile(m_FileName, !m_IsNewFile);
        m_IsNewFile = false;
        clearLog();
#endif
    }

    /// <summary>
    /// ログをクリア
    /// </summary>
    [System.Diagnostics.Conditional("BUILD_TYPE_DEBUG")]
    public static void clearLog()
    {
#if BUILD_TYPE_DEBUG
        m_Text.Length = 0;
        m_IsLineTop = true;
#endif
    }

    /// <summary>
    /// 改行
    /// </summary>
    [System.Diagnostics.Conditional("BUILD_TYPE_DEBUG")]
    public static void newLine()
    {
#if BUILD_TYPE_DEBUG
        if (m_IsEnable == false)
        {
            return;
        }

        _writeDateTime();

        m_Text.Append("\r\n");
        m_IsLineTop = true;
#endif
    }

    /// <summary>
    /// テキストをログへ出力
    /// </summary>
    /// <param name="text"></param>
    [System.Diagnostics.Conditional("BUILD_TYPE_DEBUG")]
    public static void writeText(string text, bool new_line = true)
    {
#if BUILD_TYPE_DEBUG
        if (m_IsEnable == false)
        {
            m_StrOpe.Length = 0;
            return;
        }

        _writeDateTime();

        m_Text.Append(text);
        m_StrOpe.Length = 0;
        if (new_line)
        {
            newLine();
        }
#endif
    }

    /// <summary>
    /// 変数を設定
    /// </summary>
    /// <param name="value_name"></param>
    /// <param name="value"></param>
    [System.Diagnostics.Conditional("BUILD_TYPE_DEBUG")]
    public static void setValue(string value_name, int value)
    {
#if BUILD_TYPE_DEBUG
        m_Values[value_name] = value;
#endif
    }

    /// <summary>
    /// 変数に加算
    /// </summary>
    /// <param name="value_name"></param>
    /// <param name="value"></param>
    [System.Diagnostics.Conditional("BUILD_TYPE_DEBUG")]
    public static void addValue(string value_name, int value)
    {
#if BUILD_TYPE_DEBUG
        if (m_Values.ContainsKey(value_name))
        {
            m_Values[value_name] += value;
        }
        else
        {
            Debug.LogError("DebugBattleLog:Error! 初期化されていない変数です[" + value_name + "]");
        }
#endif
    }

    /// <summary>
    /// 変数の値をログへ出力
    /// </summary>
    /// <param name="value_name"></param>
    [System.Diagnostics.Conditional("BUILD_TYPE_DEBUG")]
    public static void writeValue(string value_name)
    {
#if BUILD_TYPE_DEBUG
        if (m_Values.ContainsKey(value_name))
        {
            writeText(DebugBattleLog.StrOpe + m_Values[value_name].ToString());
        }
        else
        {
            Debug.LogError("DebugBattleLog:Error! 初期化されていない変数です[" + value_name + "]");
        }
#endif
    }

    /// <summary>
    /// ログを出力（現在の実装はクリップボードへコピー）
    /// </summary>
    [System.Diagnostics.Conditional("BUILD_TYPE_DEBUG")]
    public static void exportLog()
    {
#if BUILD_TYPE_DEBUG
        _exportToClipboard();
        //clearLog();
#endif
    }

#if BUILD_TYPE_DEBUG
    // クリップボードへコピー
    private static void _exportToClipboard()
    {
        if (m_IsEnable == false)
        {
            return;
        }

#if BUILD_TYPE_DEBUG
#if UNITY_EDITOR
        GUIUtility.systemCopyBuffer = m_Text.ToString();
#elif UNITY_ANDROID
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");

            activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject clipboardManager = activity.Call<AndroidJavaObject>("getSystemService","clipboard");
                //clipboardManager.Call("setText", exportData);
                AndroidJavaClass clipDataClass = new AndroidJavaClass("android.content.ClipData");
                AndroidJavaObject clipData = clipDataClass.CallStatic<AndroidJavaObject>("newPlainText","simple text", m_Text.ToString());
                clipboardManager.Call("setPrimaryClip",clipData);
            }));
        }
#endif
#endif
    }

    /// <summary>
    /// ファイルへ出力
    /// </summary>
    /// <param name="file_name">ファイル名</param>
    /// <param name="is_add">上書きか追記か</param>
    private static void _exportToFile(string file_name, bool is_add)
    {
        if (m_IsEnable == false)
        {
            return;
        }

        if (file_name != null && file_name != "")
        {
            FileInfo file_info = new FileInfo(getSavePath() + "/" + file_name);
            StreamWriter stream_writer = null;
            if (is_add)
            {
                stream_writer = file_info.AppendText();
            }
            else
            {
                stream_writer = file_info.CreateText();
            }

            if (stream_writer != null)
            {
                stream_writer.Write(m_Text.ToString());
                stream_writer.Flush();
                stream_writer.Close();
            }
        }
    }

    /// <summary>
    /// 行頭に日付時刻を付加
    /// </summary>
    private static void _writeDateTime()
    {
        if (m_IsLineTop)
        {
            m_IsLineTop = false;
            string date_time = DateTime.Now.ToString("MM/dd HH:mm:ss");
            if (date_time != m_DateTime)
            {
                m_DateTime = date_time;
                m_LineNo = 0;
            }

            m_Text.Append(string.Format("{0}.{1:D3}>", m_DateTime, m_LineNo));
            m_LineNo++;
        }
    }
#endif




    [System.Diagnostics.Conditional("BUILD_TYPE_DEBUG")]
    public static void outputPlayerParty(CharaParty chara_party)
    {
        for (int idx = 0; idx < chara_party.getPartyMemberMaxCount(); idx++)
        {
            CharaOnce chara_once = chara_party.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.EXIST);
            if (chara_once != null)
            {
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "プレイヤー情報(" + ((GlobalDefine.PartyCharaIndex)idx).ToString() + ")"
                    + " FixID:" + chara_once.m_CharaMasterDataParam.fix_id.ToString()
                    + " DrawID:" + chara_once.m_CharaMasterDataParam.draw_id.ToString()
                    + " LV:" + chara_once.m_CharaLevel.ToString()
                    + " 属性:" + chara_once.m_CharaMasterDataParam.element.ToString()
                    + " 種族1:" + chara_once.m_CharaMasterDataParam.kind.ToString()
                    + " 種族2:" + chara_once.m_CharaMasterDataParam.sub_kind.ToString()
                    + " [" + chara_once.m_CharaMasterDataParam.name.ToString() + "]"
                );
                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　"
                    + " HP:" + chara_party.m_HPCurrent.getValue((GlobalDefine.PartyCharaIndex)idx).ToString() + "/" + chara_party.m_HPMax.getValue((GlobalDefine.PartyCharaIndex)idx).ToString()
                    + " SkillTurn:" + chara_once.GetTrunToLimitBreak().ToString() + "/" + chara_once.GetMaxTurn().ToString()
                    + " Hate:" + chara_party.m_Hate.getValue((GlobalDefine.PartyCharaIndex)idx).ToString()
                );

                // 状態変化
                StatusAilmentChara ailment = chara_party.m_Ailments.getAilment((GlobalDefine.PartyCharaIndex)idx);
                outputAilment(ailment);
            }
        }
    }

    [System.Diagnostics.Conditional("BUILD_TYPE_DEBUG")]
    public static void outputEnemyParty(BattleEnemy[] enemy_party)
    {
        for (int idx_enemy = 0; idx_enemy < enemy_party.Length; idx_enemy++)
        {
            BattleEnemy battle_enemy = enemy_party[idx_enemy];
            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "敵情報(" + idx_enemy.ToString() + ")"
                + " EnemyFixID:" + battle_enemy.getMasterDataParamEnemy().fix_id.ToString()
                + " CharaFixID:" + battle_enemy.getMasterDataParamChara().fix_id.ToString()
                + " 属性:" + battle_enemy.getMasterDataParamChara().element.ToString()
                + " 種族1:" + battle_enemy.getMasterDataParamChara().kind.ToString()
                + " 種族2:" + battle_enemy.getMasterDataParamChara().sub_kind.ToString()
                + " [" + battle_enemy.getMasterDataParamChara().name + "]"
            );

            uint[] enemy_abilitys = battle_enemy.getEnemyAbilitys();
            for (int idx_ability = 0; idx_ability < enemy_abilitys.Length; idx_ability++)
            {
                uint ability_fix_id = enemy_abilitys[idx_ability];
                if (ability_fix_id != 0)
                {
                    string ability_name = "";
                    MasterDataEnemyAbility ability_master = BattleParam.m_MasterDataCache.useEnemyAbility(ability_fix_id);
                    if (ability_master != null)
                    {
                        ability_name = ability_master.name;
                    }
                    DebugBattleLog.writeText(DebugBattleLog.StrOpe + "     特性" + (idx_ability + 1).ToString()
                        + "(FixId:" + ability_fix_id.ToString() + "):"
                        + ability_name
                    );
                }
            }

            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　"
                + " HP:" + battle_enemy.m_EnemyHP.ToString() + "/" + battle_enemy.m_EnemyHPMax.ToString()
                + " SkillTurn:" + battle_enemy.m_EnemyTurn.ToString() + "/" + battle_enemy.m_EnemyTurnMax.ToString()
            );

            // 状態変化
            StatusAilmentChara ailment = battle_enemy.m_StatusAilmentChara;
            outputAilment(ailment);
        }
    }

    [System.Diagnostics.Conditional("BUILD_TYPE_DEBUG")]
    public static void outputCard(BattleScene._BattleCardManager battle_card_manager)
    {
        // 場札
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "場札");
        DebugBattleLog.writeText(DebugBattleLog.StrOpe +
            "　　0左[" + battle_card_manager.m_FieldAreas.getFieldArea(0).getCardElement(0).ToString()
            + " ," + battle_card_manager.m_FieldAreas.getFieldArea(0).getCardElement(1).ToString()
            + " ," + battle_card_manager.m_FieldAreas.getFieldArea(0).getCardElement(2).ToString()
            + " ," + battle_card_manager.m_FieldAreas.getFieldArea(0).getCardElement(3).ToString()
            + " ," + battle_card_manager.m_FieldAreas.getFieldArea(0).getCardElement(4).ToString() + "]"
            + (BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField[0] ? "(BOOST)" : "")
        );
        DebugBattleLog.writeText(DebugBattleLog.StrOpe +
            "　　1　[" + battle_card_manager.m_FieldAreas.getFieldArea(1).getCardElement(0).ToString()
            + " ," + battle_card_manager.m_FieldAreas.getFieldArea(1).getCardElement(1).ToString()
            + " ," + battle_card_manager.m_FieldAreas.getFieldArea(1).getCardElement(2).ToString()
            + " ," + battle_card_manager.m_FieldAreas.getFieldArea(1).getCardElement(3).ToString()
            + " ," + battle_card_manager.m_FieldAreas.getFieldArea(1).getCardElement(4).ToString() + "]"
            + (BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField[1] ? "(BOOST)" : "")
        );
        DebugBattleLog.writeText(DebugBattleLog.StrOpe +
            "　　2中[" + battle_card_manager.m_FieldAreas.getFieldArea(2).getCardElement(0).ToString()
            + " ," + battle_card_manager.m_FieldAreas.getFieldArea(2).getCardElement(1).ToString()
            + " ," + battle_card_manager.m_FieldAreas.getFieldArea(2).getCardElement(2).ToString()
            + " ," + battle_card_manager.m_FieldAreas.getFieldArea(2).getCardElement(3).ToString()
            + " ," + battle_card_manager.m_FieldAreas.getFieldArea(2).getCardElement(4).ToString() + "]"
            + (BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField[2] ? "(BOOST)" : "")
        );
        DebugBattleLog.writeText(DebugBattleLog.StrOpe +
            "　　3　[" + battle_card_manager.m_FieldAreas.getFieldArea(3).getCardElement(0).ToString()
            + " ," + battle_card_manager.m_FieldAreas.getFieldArea(3).getCardElement(1).ToString()
            + " ," + battle_card_manager.m_FieldAreas.getFieldArea(3).getCardElement(2).ToString()
            + " ," + battle_card_manager.m_FieldAreas.getFieldArea(3).getCardElement(3).ToString()
            + " ," + battle_card_manager.m_FieldAreas.getFieldArea(3).getCardElement(4).ToString() + "]"
            + (BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField[3] ? "(BOOST)" : "")
        );
        DebugBattleLog.writeText(DebugBattleLog.StrOpe +
            "　　4右[" + battle_card_manager.m_FieldAreas.getFieldArea(4).getCardElement(0).ToString()
            + " ," + battle_card_manager.m_FieldAreas.getFieldArea(4).getCardElement(1).ToString()
            + " ," + battle_card_manager.m_FieldAreas.getFieldArea(4).getCardElement(2).ToString()
            + " ," + battle_card_manager.m_FieldAreas.getFieldArea(4).getCardElement(3).ToString()
            + " ," + battle_card_manager.m_FieldAreas.getFieldArea(4).getCardElement(4).ToString() + "]"
            + (BattleSceneManager.Instance.PRIVATE_FIELD.m_abBoostField[4] ? "(BOOST)" : "")
        );

        // 手札
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "手札"
            + " [" + battle_card_manager.m_HandArea.getCardElement(0).ToString() + "]"
            + " [" + battle_card_manager.m_HandArea.getCardElement(1).ToString() + "]"
            + " [" + battle_card_manager.m_HandArea.getCardElement(2).ToString() + "]"
            + " [" + battle_card_manager.m_HandArea.getCardElement(3).ToString() + "]"
            + " [" + battle_card_manager.m_HandArea.getCardElement(4).ToString() + "]"
        );

        // 次手札
        DebugBattleLog.writeText(DebugBattleLog.StrOpe + "次札"
            + " [" + battle_card_manager.m_NextArea.getCardElement(0).ToString() + "]"
            + " [" + battle_card_manager.m_NextArea.getCardElement(1).ToString() + "]"
            + " [" + battle_card_manager.m_NextArea.getCardElement(2).ToString() + "]"
            + " [" + battle_card_manager.m_NextArea.getCardElement(3).ToString() + "]"
            + " [" + battle_card_manager.m_NextArea.getCardElement(4).ToString() + "]"
        );
    }

    [System.Diagnostics.Conditional("BUILD_TYPE_DEBUG")]
    public static void outputAilment(StatusAilmentChara chara_ailment)
    {
        for (int idx = 0; idx < chara_ailment.cAilment.Length; idx++)
        {
            StatusAilment ailment = chara_ailment.cAilment[idx];
            if (ailment != null
                && ailment.bUsed
                )
            {
                MasterDataStatusAilmentParam ailment_master = BattleParam.m_MasterDataCache.useAilmentParam((uint)ailment.nMasterDataStatusAilmentID);

                DebugBattleLog.writeText(DebugBattleLog.StrOpe + "　　状態変化"
                    + " FixID:" + ailment.nMasterDataStatusAilmentID.ToString()
                    + " 効果種類:" + ailment.nType.ToString()
                    + " 残ターン数:" + ailment.nLife
                    + " 発動者ATK:" + ailment.nBaseAtk
                    + " 受動者HP:" + ailment.nBaseHPMax
                    + ((ailment_master != null) ? " 効果値(割合):" + ailment_master.Get_VALUE_RATE() : "")
                    + ((ailment_master != null) ? " 効果値(固定):" + ailment_master.Get_VALUE_FIX() : "")
                    + " [" + ((ailment_master != null) ? ailment_master.name : "") + "]"
                );
            }
        }
    }

    /// <summary>
    /// 状態異常付与のデバッグログを出力
    /// </summary>
    /// <param name="chara_ailment"></param>
    /// <param name="text"></param>
    [System.Diagnostics.Conditional("BUILD_TYPE_DEBUG")]
    public static void outputAilmentChara(StatusAilmentChara chara_ailment, string text)
    {
        if (chara_ailment == null)
        {
            return;
        }

        string owner_chara_name = null;
        if (BattleParam.m_PlayerParty != null)
        {
            for (int idx = 0; idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); idx++)
            {
                StatusAilmentChara work_ailment = BattleParam.m_PlayerParty.m_Ailments.getAilment((GlobalDefine.PartyCharaIndex)idx);
                if (work_ailment == chara_ailment)
                {
                    owner_chara_name = ((GlobalDefine.PartyCharaIndex)idx).ToString();
                    break;
                }
            }
        }

        if (owner_chara_name == null
            && BattleParam.m_EnemyParam != null
        )
        {
            for (int idx = 0; idx < BattleParam.m_EnemyParam.Length; idx++)
            {
                if (BattleParam.m_EnemyParam[idx].m_StatusAilmentChara == chara_ailment)
                {
                    owner_chara_name = "敵" + idx.ToString();
                    break;
                }
            }
        }

        if (owner_chara_name == null)
        {
            owner_chara_name = "";
        }

        DebugBattleLog.writeText(DebugBattleLog.StrOpe + owner_chara_name + text);
    }
}

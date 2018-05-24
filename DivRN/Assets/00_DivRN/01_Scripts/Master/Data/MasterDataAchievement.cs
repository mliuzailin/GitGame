using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using ICSharpCode.SharpZipLib.Core;
using ServerDataDefine;
using SQLite.Attribute;

//----------------------------------------------------------------------------
/*!
	@brief	マスターデータ実体：アチーブメント（サーバー上でコンバート）
	@note	トロフィー的なアチーブメント機能用マスター
*/
//----------------------------------------------------------------------------
public class MasterDataAchievementConverted : Master
{
    //    ACHIEVEMENT_STATE_NONE = 0,         //!< 出現(　) , 達成(　) , 演出(　) , 取得(　)　→　出現前
    //    ACHIEVEMENT_STATE_S1_T0_E0_S0 = 1,      //!< 出現(○) , 達成(　) , 演出(　) , 取得(　)　→　出現後
    //    ACHIEVEMENT_STATE_S1_T1_E0_S0 = 2,      //!< 出現(○) , 達成(○) , 演出(　) , 取得(　)　→　出現後 , 達成後
    //    ACHIEVEMENT_STATE_S1_T1_E1_S0 = 3,      //!< 出現(○) , 達成(○) , 演出(○) , 取得(　)　→　出現後 , 達成後 , 演出後
    //    ACHIEVEMENT_STATE_S1_T1_E1_S1 = 4,      //!< 出現(○) , 達成(○) , 演出(○) , 取得(○)　→　出現後 , 達成後 , 演出後 , 取得後
    [Ignore]
    public bool IsState_Appear
    {
        get
        {
            return server_state > 0;
        }
    }

    [Ignore]
    public bool IsState_Achieve
    {
        get
        {
            return server_state > 1;
        }
    }

    [Ignore]
    public bool IsState_AchieveEffect
    {
        get
        {
            return server_state > 2;
        }
    }

    [Ignore]
    public bool IsState_Rewarded
    {
        get
        {
            return server_state > 3;
        }
    }

    [Ignore]
    public DateTime ExpiraDate
    {
        get
        {
            return TimeUtil.ConvertServerTimeToLocalTime(expira_time);
        }
    }


    [Ignore]
    public bool HasExpira
    {
        get
        {
            return expira_time > 0;
        }
    }

    //ex: 無限への誘い：初級を100回クリア
    [Ignore]
    public string Desc
    {
        get
        {
            return draw_msg;
        }
    }

    [Ignore]
    public string PresentName
    {
        get
        {
            return MasterDataUtil.GetPresentName(PresentMaster);
        }
    }

    public void GetPresentIcon(System.Action<Sprite> callback)
    {
        MainMenuUtil.GetPresentIcon(PresentMaster, sprite => {
            callback(sprite);
        });
    }

    /// <summary>1番目の報酬の付与数</summary>
    [Ignore]
    public int PresentCount
    {
        get
        {
            int count = MasterDataUtil.GetPresentCount(PresentMaster);
            count *= PresentMasterCount;
            return count;
        }
    }

    [Ignore]
    public MasterDataPresent PresentMaster
    {
        get
        {
            if (present_ids.IsNullOrEmpty())
            {
                return null;
            }

            return MasterDataUtil.GetPresentParamFromID(present_ids[0]);
        }
    }

    [Ignore]
    private int PresentMasterCount
    {
        get
        {
            if (present_ids.IsNullOrEmpty())
            {
                return 0;
            }

            int count = 1;

            uint presentId = present_ids[0];
            for (int i = 1; i < present_ids.Length; i++)
            {
                if (present_ids[i] == presentId)
                {
                    count++;
                }
            }
            return count;
        }
    }


    [Ignore]
    public uint TotalCount
    {
        get
        {
            // アチーブメントのタイプによって表示を変更する
            switch (achievement_type)
            {
                // クリアまでのカウントを出すタイプ
                case MasterDataDefineLabel.AchievementType.GETQUEST_COUNT: // 13:クエストクリア回数
                case MasterDataDefineLabel.AchievementType.GETUNIT_COUNT: // 14:ユニット獲得回数
                case MasterDataDefineLabel.AchievementType.SALE_UNIT_CT: // 20:ユニット売却回数
                case MasterDataDefineLabel.AchievementType.USE_ITEM:
                case MasterDataDefineLabel.AchievementType.BUY_POINT_SHOP:
                case MasterDataDefineLabel.AchievementType.HERO_SKILL_CT:
                case MasterDataDefineLabel.AchievementType.TAB_QUEST_CLEAR_CT:
                    return achievement_param_2;
                case MasterDataDefineLabel.AchievementType.BUILDUP_CT: // 18:強化合成回数
                case MasterDataDefineLabel.AchievementType.EVOLVE_CT: // 19:進化回数
                case MasterDataDefineLabel.AchievementType.LOGIN_CT:
                    //強化回数と進化回数はparam1に設定されている
                    return achievement_param_1;
                case MasterDataDefineLabel.AchievementType.KILL_ENEMY_CT:
                case MasterDataDefineLabel.AchievementType.HERO_CT:
                    return achievement_param_3;
                case MasterDataDefineLabel.AchievementType.LIMIT_OVER: // 22:ユニット（指定可）を指定回数限界突破
                    //param_1:ユニットID param_2:回数
                    if (achievement_param_1 <= 0)
                    {
                        //ユニット未指定時は回数表示
                        return achievement_param_2;
                    }
                    else
                    {
                        //ユニット指定時は固定で「あと1体入手で達成」を表示する
                        return 1;
                    }
                // そのほかは一回で達成
                //case MasterDataDefineLabel.AchievementType.ACTIVE_SKILL_CT:
                default:
                    return 1;
            }
        }
    }

    [Ignore]
    public float ProgressRate
    {
        get
        {
            if (TotalCount <= 0)
            {
                return -1;
            }

            float result = (float)ProgressCount / (float)TotalCount;
#if BUILD_TYPE_DEBUG
            //            Debug.Log("RES:" + result + " PROGESS:" + ProgressCount + " TOTAL:" + TotalCount);
#endif
            return result;
        }
    }


    [Ignore]
    public uint ProgressCount
    {
        get
        {
            // アチーブメントのタイプによって表示を変更する
            switch (achievement_type)
            {
                case MasterDataDefineLabel.AchievementType.GETNUITGROUP:
                    return 0;
                default:
                    return counter;
            }
        }
    }

    //必要数
    [Ignore]
    public uint RestCount
    {
        get
        {
            return TotalCount - ProgressCount;
        }
    }

    public ACHIEVEMENT_STATE AchievementState
    {
        get
        {
            return (ACHIEVEMENT_STATE)server_state;
        }
    }

    public int server_state
    {
        get;
        set;
    } //!< ミッションのステータス

    public uint[] present_ids
    {
        get;
        set;
    } //!< ミッションに紐づくプレゼントグループ(複数ある場合「group_id」をキーとした連想配列)

    public ulong server_time_open
    {
        get;
        set;
    } //!< 発生日時

    public ulong server_time_clear
    {
        get;
        set;
    } //!< 達成日時

    public uint expira_time
    {
        get;
        set;
    } //!< 終了日時(unixtime)

    public uint counter
    {
        get;
        set;
    } //!< クリアカウント

    public int category
    {
        get;
        set;
    } //!< achievement_masterのcategory

    public string draw_msg
    {
        get;
        set;
    } //!< 表ミッションの条件

    public uint event_id
    {
        get;
        set;
    } //!< 当ミッションが属するイベントID

    public ulong timing_start
    {
        get;
        set;
    } //!< 当ミッション公開日時

    public uint achievement_category_id
    {
        get;
        set;
    } //!< ミッションカテゴリID ※1(ノーマル), 2(イベント), 3(デイリー), 4(クエスト)

    public MasterDataDefineLabel.AchievementType achievement_type
    {
        get;
        set;
    } //!< ミッションタイプ

    public uint achievement_param_1
    {
        get;
        set;
    } //!< ミッション汎用パラメータ1

    public uint achievement_param_2
    {
        get;
        set;
    } //!< ミッション汎用パラメータ2

    public uint achievement_param_3
    {
        get;
        set;
    } //!< ミッション汎用パラメータ3

    public uint achievement_param_4
    {
        get;
        set;
    } //!< ミッション汎用パラメータ4

    public uint achievement_param_5
    {
        get;
        set;
    } //!< ミッション汎用パラメータ5

    public uint achievement_param_6
    {
        get;
        set;
    } //!< ミッション汎用パラメータ6

    public uint open_type
    {
        get;
        set;
    } //!< 	解放条件

    public uint open_param1
    {
        get;
        set;
    } //!< 解解放条件パラメータ

    public uint show_type
    {
        get;
        set;
    } //!< 表示条件タイプ

    public uint show_param1
    {
        get;
        set;
    } //!< 表示条件パラメータ

    public uint quest_id
    {
        get;
        set;
    } //!< クエストID ※関連クエストが無い場合は0
};
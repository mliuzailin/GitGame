using UnityEngine;
using System.Collections;

public class BattleLogicFSM : FSM<BattleLogicFSM>
{
    private BattleLogic.EBATTLE_STEP m_Step = BattleLogic.EBATTLE_STEP.eBATTLE_STEP_MAX;
    private BattleLogic.EBATTLE_STEP m_PrevStep = BattleLogic.EBATTLE_STEP.eBATTLE_STEP_MAX;
    private bool m_IsChangeStepTriger = false;

    /// <summary>
    /// 現在の処理ステップ
    /// </summary>
    /// <returns></returns>
    public BattleLogic.EBATTLE_STEP getCurrentStep()
    {
        return m_Step;
    }

    /// <summary>
    /// 一つ前の処理ステップ
    /// </summary>
    /// <returns></returns>
    public BattleLogic.EBATTLE_STEP getPrevStep()
    {
        return m_PrevStep;
    }

    /// <summary>
    /// 処理ステップが変化したタイミングかを調べる
    /// </summary>
    /// <param name="is_clear_flag">変化のフラグを消すかどうか</param>
    /// <returns></returns>
    public bool isChangeStepTriger(bool is_clear_flag)
    {
        bool ret_val = m_IsChangeStepTriger;
        if (is_clear_flag)
        {
            m_IsChangeStepTriger = false;
        }
        return ret_val;
    }

    /// <summary>
    /// 指定のステップに移行
    /// </summary>
    /// <param name="step"></param>
    public void step(BattleLogic.EBATTLE_STEP step)
    {
        switch (step)
        {
            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_WAIT_UPDATE:
                SendFsmEvent("BATTLE_STEP_WAIT_UPDATE");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_INIT:
                SendFsmEvent("BATTLE_STEP_INIT");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_INIT_FADEIN:
                SendFsmEvent("BATTLE_STEP_INIT_FADEIN");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_EVOL_START:
                SendFsmEvent("BATTLE_STEP_GAME_EVOL_START");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_EVOL_END:
                SendFsmEvent("BATTLE_STEP_GAME_EVOL_END");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_INIT_START:
                SendFsmEvent("BATTLE_STEP_INIT_START");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_SELECT_ACTION:
                SendFsmEvent("BATTLE_STEP_GAME_SELECT_ACTION");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_ENEMY_TURN_FIRST:
                SendFsmEvent("BATTLE_STEP_GAME_ENEMY_TURN_FIRST");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_ENEMY_TURN_FIRST_END:
                SendFsmEvent("BATTLE_STEP_GAME_ENEMY_TURN_FIRST_END");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_INITIATIVE_TITLE:
                SendFsmEvent("BATTLE_STEP_GAME_INITIATIVE_TITLE");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_INITIATIVE:
                SendFsmEvent("BATTLE_STEP_GAME_INITIATIVE");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_INITIATIVE_ATTACK:
                SendFsmEvent("BATTLE_STEP_GAME_INITIATIVE_ATTACK");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_INITIATIVE_END:
                SendFsmEvent("BATTLE_STEP_GAME_INITIATIVE_END");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_PHASE_PLAYER_START:
                SendFsmEvent("BATTLE_STEP_GAME_PHASE_PLAYER_START");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_THINK:
                SendFsmEvent("BATTLE_STEP_GAME_THINK");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_INPUT:
                SendFsmEvent("BATTLE_STEP_GAME_INPUT");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_SKILL_SORT:
                SendFsmEvent("BATTLE_STEP_GAME_SKILL_SORT");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_SKILL_SORT2:
                SendFsmEvent("BATTLE_STEP_GAME_SKILL_SORT2");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_COUNT_FADE_IN:
                SendFsmEvent("BATTLE_STEP_GAME_COUNT_FADE_IN");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_COUNT_FADE_OUT:
                SendFsmEvent("BATTLE_STEP_GAME_COUNT_FADE_OUT");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_TITLE_IN:
                SendFsmEvent("BATTLE_STEP_GAME_ACTION_TITLE_IN");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION:
                SendFsmEvent("BATTLE_STEP_GAME_ACTION");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION2:
                SendFsmEvent("BATTLE_STEP_GAME_ACTION2");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_FINISH:
                SendFsmEvent("BATTLE_STEP_GAME_ACTION_FINISH");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_TITLE_OUT:
                SendFsmEvent("BATTLE_STEP_GAME_ACTION_TITLE_OUT");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_BOOST_SKILL_TITLE:
                SendFsmEvent("BATTLE_STEP_GAME_BOOST_SKILL_TITLE");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_BOOST_SKILL:
                SendFsmEvent("BATTLE_STEP_GAME_BOOST_SKILL");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_BOOST_SKILL_FINISH:
                SendFsmEvent("BATTLE_STEP_GAME_BOOST_SKILL_FINISH");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_SKILL_TITLE:
                SendFsmEvent("BATTLE_STEP_GAME_ACTION_SKILL_TITLE");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_SKILL:
                SendFsmEvent("BATTLE_STEP_GAME_ACTION_SKILL");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_SKILL_FINISH:
                SendFsmEvent("BATTLE_STEP_GAME_ACTION_SKILL_FINISH");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_PASSIVE_SKILL_TITLE:
                SendFsmEvent("BATTLE_STEP_GAME_PASSIVE_SKILL_TITLE");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_PASSIVE_SKILL:
                SendFsmEvent("BATTLE_STEP_GAME_PASSIVE_SKILL");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_PASSIVE_SKILL_FINISH:
                SendFsmEvent("BATTLE_STEP_GAME_PASSIVE_SKILL_FINISH");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_LINK_PASSIVE_TITLE:
                SendFsmEvent("BATTLE_STEP_GAME_LINK_PASSIVE_TITLE");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_LINK_PASSIVE:
                SendFsmEvent("BATTLE_STEP_GAME_LINK_PASSIVE");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_LINK_PASSIVE_FINISH:
                SendFsmEvent("BATTLE_STEP_GAME_LINK_PASSIVE_FINISH");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_ACTION_END:
                SendFsmEvent("BATTLE_STEP_GAME_ACTION_END");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_PHASE_ENEMY_START:
                SendFsmEvent("BATTLE_STEP_GAME_PHASE_ENEMY_START");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_AILMENTUPDATE:
                SendFsmEvent("BATTLE_STEP_GAME_AILMENTUPDATE");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_ENEMY_TURNUPDATE:
                SendFsmEvent("BATTLE_STEP_GAME_ENEMY_TURNUPDATE");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_ENEMY_TURN:
                SendFsmEvent("BATTLE_STEP_GAME_ENEMY_TURN");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_PHASE_ENEMY_END:
                SendFsmEvent("BATTLE_STEP_GAME_PHASE_ENEMY_END");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_COUNTER_PLAYER:
                SendFsmEvent("BATTLE_STEP_GAME_COUNTER_PLAYER");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_RESULT:
                SendFsmEvent("BATTLE_STEP_GAME_RESULT");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_CLEAR:
                SendFsmEvent("BATTLE_STEP_GAME_CLEAR");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_DEAD_GAMEOVER:
                SendFsmEvent("BATTLE_STEP_DEAD_GAMEOVER");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_DEAD_GAMEOVER_WAIT:
                SendFsmEvent("BATTLE_STEP_DEAD_GAMEOVER_WAIT");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_DEAD_CONTINUE:
                SendFsmEvent("BATTLE_STEP_DEAD_CONTINUE");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_DEAD_RETIRE:
                SendFsmEvent("BATTLE_STEP_DEAD_RETIRE");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_LIMITBREAK:
                SendFsmEvent("BATTLE_STEP_LIMITBREAK");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_LIMITBREAK_WAIT:
                SendFsmEvent("BATTLE_STEP_LIMITBREAK_WAIT");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_LIMITBREAK_END:
                SendFsmEvent("BATTLE_STEP_LIMITBREAK_END");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_ENEMY_REACTION_START:
                SendFsmEvent("BATTLE_STEP_GAME_ENEMY_REACTION_START");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_ENEMY_REACTION:
                SendFsmEvent("BATTLE_STEP_GAME_ENEMY_REACTION");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_GAME_ENEMY_REACTION_END:
                SendFsmEvent("BATTLE_STEP_GAME_ENEMY_REACTION_END");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_ERROR:
                SendFsmEvent("BATTLE_STEP_ERROR");
                break;

            case BattleLogic.EBATTLE_STEP.eBATTLE_STEP_MAX:
                break;
        }

        if (m_Step != step)
        {
            m_IsChangeStepTriger = true;
        }

        m_PrevStep = m_Step;
        m_Step = step;

        return;
    }
}

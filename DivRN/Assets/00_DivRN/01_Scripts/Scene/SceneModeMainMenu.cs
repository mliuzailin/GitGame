using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SceneModeMainMenu : SceneMode<SceneModeMainMenu>
{
    protected override void Awake()
    {
        base.Awake();
        //----------------------------------------
        // 強制的に開催中イベントの判定を再実行するようリクエスト
        //----------------------------------------
        if (TimeEventManager.HasInstance == true)
        {
            TimeEventManager.Instance.TimeEventUpdateRequest();
        }
    }

    protected override void Start()
    {
        base.Start();

        //----------------------------------------
        // メインメニュー関連情報リセット
        //----------------------------------------
        MainMenuParam.ParamReset();

        //----------------------------------------
        // クエスト結果を取得。
        // クエストクリア直後にハングした場合に備えてローカルセーブを参照する
        //----------------------------------------
        if (SceneGoesParam.HasInstance == true)
        {
            if (SceneGoesParam.Instance.m_SceneGoesParamToMainMenu == null)
            {
                SceneGoesParam.Instance.m_SceneGoesParamToMainMenu = LocalSaveManager.Instance.LoadFuncGoesToMenuResult();
            }

            if (SceneGoesParam.Instance.m_SceneGoesParamToMainMenuRetire == null)
            {
                SceneGoesParam.Instance.m_SceneGoesParamToMainMenuRetire = LocalSaveManager.Instance.LoadFuncGoesToMenuRetire();
            }
        }
    }

    protected override void OnDestroy()
    {
        //----------------------------------------
        // メインメニュー関連情報破棄
        //----------------------------------------
        MainMenuParam.ParamReset();

        //----------------------------------------
        // BGM停止
        // ver1110でフェードアウトの尺を調整
        //----------------------------------------
        SoundUtil.StopBGM(false);

        base.OnDestroy();
    }


    public override void OnInitialized()
    {
        base.OnInitialized();
        MainMenuManagerFSM.Instance.SendFsmEvent("SCENE_INITIALIZED");
    }
}

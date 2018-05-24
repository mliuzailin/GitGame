using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTutorialManager
{
    public enum TutorialBattlePhase
    {
        NONE,
        CARD_INIT,  // カード準備
        INPUT,  // 入力開始（１ターン目）
        INPUT2, // 入力開始（２ターン目以降）
        RESULT, // リザルト（ターン終了）
        GAME_OVER,  // 味方全滅（味方を不死にしてGAMEOVERにならないようにする？）
        CREAR,  // 敵グループ撃破

        MAX
    }

    public enum TutorialOptionMenuPhase
    {
        ALL_ON = 0,	// 通常状態（全てのボタンが有効）
        INIT,	// 初期化（チュートリアルで説明する「SKILL_TURN」「SKILL_COST」をオフにしておく）
        SKILL_TURN,	// 「SKILL_TURN」のボタンだけ有効
        SKILL_COST,	// 「SKILL_COST」のボタンだけ有効
        BACK_BUTTON,	// 「戻る」のボタンだけ有効

        MAX
    }

    private int m_BattleRound = -1; //何戦目か
    private TutorialBattlePhase m_TutorialBattlePhaseCurrent = TutorialBattlePhase.NONE;    //チュートリアルフェイズ
    private int m_BattleTurn = 0;   //何ターン目か

    private TutorialBattlePhase m_TutorialBattlePhaseReq = TutorialBattlePhase.NONE;

    private string m_CurrentCommandText = null;
    private Dialog m_Dialog = null;
    private string[] m_CommandTexts = null;

    private bool m_IsNextCommand = false;


    private string[,][] m_TutorialCommands = null;

    private bool m_IsNoDeadEnemy = false;   // 敵不死フラグ
    private bool m_IsAllDeadEnemy = false;  // 敵全滅フラグ

    private bool m_IsWaitSend = false;	// 通信中フラグ
    private bool m_IsForbidButton = false;	// 各種メニューを開くボタン操作を禁止するタイミング
    private bool[] m_IsForbidLimitBreak = new bool[(int)GlobalDefine.PartyCharaIndex.MAX];  // リミットブレイク使用禁止フラグ
    private bool m_IsEnableUnitInfoWindow = false;	// ユニット情報ウィンドウの表示許可フラグ
    private bool m_IsEnableOptionButton = false;  // 「OPEN」ボタン使用禁止フラグ
    private TutorialOptionMenuPhase m_TutorialOptionMenuPhase = TutorialOptionMenuPhase.ALL_ON;

    private bool m_IsShowingTutorialDialog = false; // チュートリアルダイアログを表示中かどうか.

    public BattleTutorialManager()
    {
        m_TutorialCommands = new string[3, (int)TutorialBattlePhase.MAX][];

        // 文字列の先頭が「<!>」だったらOKダイアログ（キー番号指定）
        // 文字列の先頭が「<?>」だったらYES,NOダイアログ（キー番号指定）
        // 文字列の先頭が「<A>」だったら上向き矢印（ボタン名指定）
        // 文字列の先頭が「<B>」だったら右向き矢印（ボタン名指定）
        // 文字列の先頭が「<C>」だったら左向き矢印（ボタン名指定）
        // 文字列の先頭が「<T>」だったらチュートリアルダイアログ
        // 文字列の先頭が「<D>」だったら敵不死設定のオンオフ
        // 文字列の先頭が「<S>」だったらStep情報の送信
        // 文字列の先頭が「<L>」だったらリミットブレイクスキルの使用許可のオンオフ（初期値はオフ）
        // 文字列の先頭が「<U>」だったらユニット情報ウィンドウの表示のオンオフ
        // 文字列の先頭が「<O>」だったら右下の「オプション」ボタンの使用可能のオンオフ
        // 文字列の先頭が「<M>」オプションメニューのボタン状態を制御
        // 文字列の先頭が「<K>」アンドロイドのバックキーの有効無効を制御
        m_TutorialCommands[0, (int)TutorialBattlePhase.INPUT] = new string[] {
            "<D>1",
            "<U>0",  //ユニット情報ウィンドウの表示を禁止
            "<T>1",
            //"<!>01", //基本は各ユニットの能力である、 NORMAL SKILLを発動して攻撃します。
            //"<!>02", //NORMAL SKILLは、各ユニットが指定する エナジーパネルを場に配置すると発動します。
            //"<!>03", //エナジーパネルは１つの場につき、 最大5個まで設置できます。 ※全部の場を合わせると25枚になります
            "<!>04", //まずは、お手本をやりますので、 見ていてくださいね！
            "<S>201",
        };

        m_TutorialCommands[0, (int)TutorialBattlePhase.INPUT2] = new string[] {
            "<S>202",
            "<?>05", //もう一度、お手本を確認しますか？
        };

        m_TutorialCommands[1, (int)TutorialBattlePhase.INPUT] = new string[] {
            "<D>0",
            "<S>203",
            "<O>1", // オプションボタンを使用可能に
            "<!>06", //右下にあるオプションボタンを タップしてみましょう。
            "<S>204",
            "<B>info_button",	// 右下の「OPEN」ボタン
            "<U>1",  //ユニット情報ウィンドウの表示を許可
            "<M>INIT",	// オプションメニューのすべてのボタンを無効化、SKILL_TURN,SKILL_COSTの項目をＯＦＦに
            "<S>205",
            "<!>07", //オプションでは各種設定の ON・OFFができます。
            "<!>08", //『AS残りターン表示』をONにすると ACTIVE SKILLを残り何ターンで 使えるかが表示されます。
            "<!>24", //『AS残りターン表示』を ONにしてみましょう。
            "<M>SKILL_TURN",	// オプションメニューのSKILL_TURNのみ有効化
            "<K>0",	// アンドロイドのバックキーを無効化
            "<C>SKILL_TURN/ToggleButton",
            "<K>1",	// アンドロイドのバックキーを有効化
            "<!>25", //『NORMAL SKILL表示』をONにすると 各ユニットのNORMAL SKILL発動に必要な エナジーパネルが表示されます。
            "<!>26", //『NORMAL SKILL表示』を ONにしてみましょう。
            "<M>SKILL_COST",	// オプションメニューのSKILL_COSTボタンのみ有効化
            "<K>0",	// アンドロイドのバックキーを無効化
            "<C>SKILL_COST/ToggleButton",
            "<K>1",	// アンドロイドのバックキーを有効化
            "<!>27", //『戻る』を押して戦闘に戻りましょう。
            "<M>BACK_BUTTON",	// オプションメニューのBACKボタンのみ有効化
            "<A>InGameMenu/back",	//オプションダイアログの「戻る」ボタン
            "<K>1",	// アンドロイドのバックキーを有効化
            "<M>ALL_ON",	// オプションメニューの全てのボタンを有効化（ボタン名を指定しない）
            "<S>206",

            //"<!>09", //エナジーパネルは、 炎、水、風、光、闇、無、回復の ７種類が存在します。
            //"<!>10", //ユニットごとに指定された エナジーパネルをうまく場に配置して、 バトルを有利に進めましょう！
            "<!>11", //では、実際にやってみましょう！
//            "<!>12", //今回のバトルでは、 炎、水、風、回復の エナジーパネルのみ出現します。
            "<!>13", //まずは、同じ場に同属性のエナジーパネルを たくさん配置してみましょう！
            "<S>207",
        };

        m_TutorialCommands[1, (int)TutorialBattlePhase.INPUT2] = new string[] {
            "<S>208",
            "<!>14", //まだ、エネミーの体力が残っています。 もう一度、攻撃してみましょう。
        };

        m_TutorialCommands[1, (int)TutorialBattlePhase.CREAR] = new string[] {
            "<S>209",
            "<!>15", //属性を上手に使えば、 戦いを有利に進められます。
//            "<!>16", //まだ、エネミーの体力が残っています。 もう一度、攻撃してみましょう。
            "<S>210",
        };

        m_TutorialCommands[2, (int)TutorialBattlePhase.INPUT] = new string[] {
            "<D>1",
            "<S>211",
            "<!>17", //ボスが出現しました。 かなりの強敵です！
            "<!>18", //先ほどレクチャーしたように、 エナジーパネルを上手に配置して、 大ダメージを狙いましょう！
            "<T>2",
            "<S>212",
        };

        m_TutorialCommands[2, (int)TutorialBattlePhase.INPUT2] = new string[] {
            "<S>213",
            "<O>0", // オプションボタンを使用不可に
            "<T>3",
            "<!>19", //チャンスです！ ここでACTIVE SKILLを 使ってみましょう！
            "<!>20", //ACTIVE SKILLとは、 一定ターンを経過すると発動できる、 とても強力な必殺技です。
            "<D>0",
            "<L>1",
            "<U>0",  //ユニット情報ウィンドウの表示を禁止
            "<A>unit_base_pt_1/Button",	// パーティインターフェースのリーダー部分
            "<U>1",  //ユニット情報ウィンドウの表示を許可
            "<L>0",
            "<A>DialogBg/ButtonOK",	// スキル発動確認ダイアログの「OK」
            "<L>1",
            "<O>1", // オプションボタンを使用可能に
            "<S>214",
        };

        m_TutorialCommands[2, (int)TutorialBattlePhase.CREAR] = new string[] {
            "<S>215",
            "<!>21", //ユニットごとにACTIVE SKILLが違うので、 色々試してみてくださいね。
            "<!>23", //もう少しだけ、 戦闘について補足します。
            "<T>4",
            "<!>22", //これで戦闘チュートリアルは終了です。 お疲れ様でした！
            "<S>216",
        };
    }

    public void updateTutorial(float delta_time)
    {
        if (BattleParam.IsTutorial() == false)
        {
            return;
        }

        m_IsForbidButton = m_IsWaitSend;
        bool is_continue = true;
        while (is_continue)
        {
            is_continue = false;
            bool is_update_phase = false;

            if (m_TutorialBattlePhaseCurrent != m_TutorialBattlePhaseReq)
            {
                m_TutorialBattlePhaseCurrent = m_TutorialBattlePhaseReq;
                is_update_phase = true;
                initPhase();
            }

            // 場面別のコマンドを設定
            if (is_update_phase)
            {
                if (m_BattleRound >= 0 && m_BattleRound < m_TutorialCommands.GetLength(0))
                {
                    if (m_TutorialBattlePhaseCurrent == TutorialBattlePhase.INPUT && m_BattleTurn != 1)
                    {
                        m_CommandTexts = m_TutorialCommands[m_BattleRound, (int)TutorialBattlePhase.INPUT2];
                    }
                    else
                    {
                        m_CommandTexts = m_TutorialCommands[m_BattleRound, (int)m_TutorialBattlePhaseCurrent];
                    }
                }
            }

            // コマンドに応じて処理（ダイアログの表示など）を実行
            if (m_CurrentCommandText == null
                && m_CommandTexts != null
            )
            {
                execCommand(m_CommandTexts[0]);

                // 実行した分のコマンドを削除
                if (m_CommandTexts.Length >= 2)
                {
                    string[] dialog_texts = new string[m_CommandTexts.Length - 1];
                    for (int idx = 0; idx < dialog_texts.Length; idx++)
                    {
                        dialog_texts[idx] = m_CommandTexts[idx + 1];
                    }
                    m_CommandTexts = dialog_texts;
                }
                else
                {
                    m_CommandTexts = null;
                }
            }

            DialogButtonEventType dialog_button_event_type = DialogButtonEventType.NONE;
            // ユーザー入力によりダイアログ閉じる
            if (m_CurrentCommandText != null)
            {
                if (m_Dialog != null)
                {
                    dialog_button_event_type = m_Dialog.PushButton;
                    switch (dialog_button_event_type)
                    {
                        case DialogButtonEventType.OK:
                        case DialogButtonEventType.YES:
                            m_Dialog.Hide();
                            m_Dialog = null;
                            m_CurrentCommandText = null;
                            if (m_CommandTexts != null)
                            {
                                is_continue = true;
                            }
                            break;

                        case DialogButtonEventType.CANCEL:
                        case DialogButtonEventType.NO:
                            m_Dialog.Hide();
                            m_Dialog = null;
                            m_CurrentCommandText = null;
                            if (m_CommandTexts != null)
                            {
                                is_continue = true;
                            }
                            break;
                    }
                }
                else if (m_IsNextCommand)
                {
                    is_continue = true;	// 連続しているコマンドを一気に実行
                    m_IsNextCommand = false;
                    dialog_button_event_type = DialogButtonEventType.OK;
                    m_CurrentCommandText = null;
                }
            }

            // その他の制御
            if (m_CurrentCommandText == null
                && m_TutorialBattlePhaseCurrent == TutorialBattlePhase.INPUT
            )
            {
                switch (m_BattleRound)
                {
                    case 0:
                        if (m_TeacherProgress < teacher_data.Length)
                        {
                            Vector2 hand_pos = Vector2.zero;
                            bool is_touching = updateTeacherHandPosition(delta_time, ref hand_pos);

                            BattleSceneManager.Instance.setOverrideTouchMode(hand_pos, is_touching);
                        }

                        // お手本をもう一度見ますか？
                        if (dialog_button_event_type == DialogButtonEventType.YES)
                        {
                            // もう一度お手本
                            initTeacher();
                        }
                        if (dialog_button_event_type == DialogButtonEventType.NO)
                        {
                            // 敵を殺して次の戦闘へ
                            m_IsAllDeadEnemy = true;
                        }
                        break;

                    case 1:
                        break;

                    case 2:
                        break;
                }
            }
        }
    }

    private void execCommand(string text)
    {
        m_CurrentCommandText = text;

        string head_text = m_CurrentCommandText.Substring(0, 3);
        string body_text = m_CurrentCommandText.Substring(3);

        switch (head_text)
        {
            case "<!>": // OKダイアログ（キー番号指定）
                showDialog(int.Parse(body_text), DialogType.DialogOK);
                break;

            case "<?>": // YES/NOダイアログ（キー番号指定）
                showDialog(int.Parse(body_text), DialogType.DialogYesNo);
                break;

            case "<T>": // チュートリアルダイアログを表示
                showTutorialDialog(int.Parse(body_text));
                break;

            case "<A>": // チュートリアル矢印を表示（上向き）
                showTutorialArrow(body_text, Vector3.zero, Vector3.zero);
                break;

            case "<B>": // チュートリアル矢印を表示（右向き）
                showTutorialArrow(body_text, new Vector3(-40.0f, 160.0f, 0.0f), new Vector3(0.0f, 0.0f, -90.0f));
                break;

            case "<C>": // チュートリアル矢印を表示（左向き）
                showTutorialArrow(body_text, new Vector3(40.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 90.0f));
                break;

            case "<S>": // Step情報を送信
                sendStepInfo(int.Parse(body_text));
                break;

            case "<D>": // 敵の不死を設定
                setNoDeadEnemy(int.Parse(body_text));
                break;

            case "<L>": // リミットブレイクスキルの使用の許可・不許可
                setEnableLimitBreak(int.Parse(body_text), GlobalDefine.PartyCharaIndex.LEADER);
                break;

            case "<U>": // ユニット情報ウィンドウの表示の禁止・許可
                setEnableUnitInfo(int.Parse(body_text));
                break;

            case "<O>": // 「オプション」ボタンの使用の許可・不許可
                setEnableOptionButton(int.Parse(body_text));
                break;

            case "<M>": // オプションメニューのボタン状態を変更
                setOptionMenuButton(body_text);
                break;

            case "<K>": // アンドロイドのバックキーの有効無効を制御
                setBackKeyEnable(int.Parse(body_text));
                break;

            default:    // OKダイアログ
                showDialog("チュートリアル", text, DialogType.DialogOK);
                break;
        }
    }

    private void showDialog(string title_text, string main_text, DialogType dialog_type)
    {
        m_Dialog = Dialog.Create(dialog_type);

        m_Dialog.SetDialogText(DialogTextType.Title, title_text);
        m_Dialog.SetDialogText(DialogTextType.MainText, main_text);
        switch (dialog_type)
        {
            case DialogType.DialogOK:
                m_Dialog.SetDialogTextFromTextkey(DialogTextType.OKText, "common_button1");
                break;

            case DialogType.DialogYesNo:
                m_Dialog.SetDialogTextFromTextkey(DialogTextType.YesText, "common_button4");
                m_Dialog.SetDialogTextFromTextkey(DialogTextType.NoText, "common_button5");
                break;
        }


        m_Dialog.Show();
    }

    private void showDialog(int text_index, DialogType dialog_type)
    {
        string title_key = string.Format("tutorial_battle_result{0:00}_title", text_index);
        string main_key = string.Format("tutorial_battle_result{0:00}_main", text_index);

        string title_text = GameTextUtil.GetText(title_key);
        string main_text = GameTextUtil.GetText(main_key);

        showDialog(title_text, main_text, dialog_type);
    }

    /// <summary>
    /// チュートリアルダイアログを表示
    /// </summary>
    private void showTutorialDialog(int dialog_index)
    {
        m_IsShowingTutorialDialog = true;
        TutorialDialog.FLAG_TYPE flag_type = TutorialDialog.FLAG_TYPE.BATTLE;
        switch (dialog_index)
        {
            case 1:
                flag_type = TutorialDialog.FLAG_TYPE.BATTLE1;
                break;

            case 2:
                flag_type = TutorialDialog.FLAG_TYPE.BATTLE2;
                break;

            case 3:
                flag_type = TutorialDialog.FLAG_TYPE.BATTLE3;
                break;

            case 4:
                flag_type = TutorialDialog.FLAG_TYPE.BATTLE4;
                break;
        }
        TutorialDialog.Create()
            .SetTutorialType(flag_type)
            .Show(() =>
            {
                m_IsNextCommand = true;
                m_IsShowingTutorialDialog = false;
            }
        );
    }

    /// <summary>
    /// チュートリアルダイアログを表示
    /// </summary>
    private void showFirstTutorialDialog(TutorialDialog.FLAG_TYPE flagType, System.Action finishAction)
    {
        bool is_already_show = LocalSaveManagerRN.Instance.GetIsShowTutorialDialog(flagType);
        if (is_already_show == false)
        {
            TutorialDialog.Create()
                .SetTutorialType(flagType)
                .Show(finishAction);
        }
        else
        {
            finishAction();
        }
    }

    /// <summary>
    /// チュートリアル矢印を表示
    /// </summary>
    /// <param name="button_name"></param>
    private void showTutorialArrow(string button_name, Vector3 position, Vector3 euler_angles)
    {
        if (GameObject.Find(button_name) != null)
        {
            // 矢印を最前面に出すためにバトル表示物の描画優先度を下げる
            float near_camera_depth = BattleSceneManager.Instance.m_BattleCameraNear.depth;
            BattleSceneManager.Instance.m_BattleCameraNear.depth = BattleSceneManager.Instance.m_BattleCameraFar.depth + 1;

            TutorialArrow.Create(button_name)
                .SetLocalPosition(position)
                .SetLocalEulerAngles(euler_angles)
                .Show(() =>
                {
                    // バトル表示物の描画優先度を元に戻す
                    BattleSceneManager.Instance.m_BattleCameraNear.depth = near_camera_depth;

                    m_IsNextCommand = true;
                },
                () =>
                {
                    // バトル表示物の描画優先度を元に戻す
                    BattleSceneManager.Instance.m_BattleCameraNear.depth = near_camera_depth;

                    m_IsNextCommand = true;
                }
                );
        }
        else
        {
            // 表示できないのですぐに閉じる
            m_IsNextCommand = true;
        }
    }

    /// <summary>
    /// チュートリアルマネージャにステップ情報を送る
    /// </summary>
    /// <param name="step_no"></param>
    private void sendStepInfo(int step_no)
    {
        m_IsWaitSend = true;
        TutorialManager.SendStep(step_no,
            () =>
            {
                m_IsWaitSend = false;
                m_IsNextCommand = true;
            });
    }

    private void setNoDeadEnemy(int on_off)
    {
        m_IsNoDeadEnemy = (on_off != 0);
        m_IsNextCommand = true;
    }

    private void setEnableLimitBreak(int on_off, GlobalDefine.PartyCharaIndex caster_index)
    {
        if (caster_index >= GlobalDefine.PartyCharaIndex.LEADER && caster_index < GlobalDefine.PartyCharaIndex.MAX)
        {
            // リミブレスキルを使用可能に
            CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember(caster_index, CharaParty.CharaCondition.SKILL_TURN1);
            if (chara_once != null)
            {
                m_IsForbidLimitBreak[(int)caster_index] = !(on_off != 0);
                chara_once.AddCharaLimitBreak(99999);
            }
        }

        m_IsNextCommand = true;
    }

    private void setEnableUnitInfo(int on_off)
    {
        m_IsEnableUnitInfoWindow = (on_off != 0);
        m_IsNextCommand = true;
    }

    private void setEnableOptionButton(int on_off)
    {
        m_IsEnableOptionButton = (on_off != 0);
        m_IsNextCommand = true;
    }

    /// <summary>
    /// オプションメニューのボタン状態を制御
    /// </summary>
    /// <param name="enable_button_name">有効にしたいボタンの名前(option_button_namesから選ぶ).何も指定しないとすべてを有効にする</param>
    private void setOptionMenuButton(string enable_button_name)
    {
        m_IsNextCommand = true;

        m_TutorialOptionMenuPhase = TutorialOptionMenuPhase.ALL_ON;
        for (int idx = 0; idx < (int)TutorialOptionMenuPhase.MAX; idx++)
        {
            TutorialOptionMenuPhase phase = (TutorialOptionMenuPhase)idx;
            string phase_text = phase.ToString();

            if (enable_button_name == phase_text)
            {
                m_TutorialOptionMenuPhase = phase;
                break;
            }
        }
    }

    /// <summary>
    /// アンドロイドのバックキーの有効無効を制御
    /// </summary>
    /// <param name="on_off"></param>
    private void setBackKeyEnable(int on_off)
    {
        m_IsNextCommand = true;

        if (on_off != 0)
        {
            AndroidBackKeyManager.Instance.EnableBackKey();
        }
        else
        {
            AndroidBackKeyManager.Instance.DisableBackKey();
        }
    }

    private void initPhase()
    {
        switch (m_TutorialBattlePhaseCurrent)
        {
            case TutorialBattlePhase.CARD_INIT:
                if (BattleParam.BattleRound == 0)
                {
                    for (int idx = 0; idx < m_IsForbidLimitBreak.Length; idx++)
                    {
                        m_IsForbidLimitBreak[idx] = true;
                    }
                    m_IsEnableUnitInfoWindow = true;
                    m_IsEnableOptionButton = false;
                }
                break;

            case TutorialBattlePhase.INPUT:
                if (m_BattleRound != BattleParam.BattleRound)
                {
                    m_BattleRound = BattleParam.BattleRound;
                    m_BattleTurn = 1;
                }
                else
                {
                    m_BattleTurn++;
                }

                switch (m_BattleRound)
                {
                    case 0:
                        if (m_BattleTurn == 1)
                        {
                            // お手本を準備
                            initTeacher();
                        }
                        break;

                    case 1:
                        break;

                    case 2:
                        break;
                }
                break;
        }
    }

    /// <summary>
    /// チュートリアルのフェイズを設定(BattleLogic側から設定される)
    /// </summary>
    /// <param name="phase"></param>
    public void setTutorialPhase(TutorialBattlePhase phase)
    {
        m_TutorialBattlePhaseReq = phase;

        // チュートリアルをスキップされた場合は、初めての戦闘でチュートリアルダイアログを表示する.
        if (phase == TutorialBattlePhase.INPUT
            && BattleParam.IsTutorial() == false
        )
        {
            m_IsShowingTutorialDialog = true;
            new SerialProcess().Add((System.Action nextProcess) =>
                                {
                                    showFirstTutorialDialog(TutorialDialog.FLAG_TYPE.BATTLE1, nextProcess);
                                })
                                .Add((System.Action nextProcess) =>
                                {
                                    showFirstTutorialDialog(TutorialDialog.FLAG_TYPE.AUTO_PLAY, nextProcess);
                                })
                                .Add((System.Action nextProcess) =>
                                {
                                    m_IsNextCommand = true;
                                    m_IsShowingTutorialDialog = false;
                                })
                                .Flush();
        }

        if (BattleParam.IsTutorial())
        {
            m_IsForbidButton = true;
        }
    }

    /// <summary>
    /// ダイアログ表示中待ちか(BattleLogicがこれを見てBattleLogicを制御する)
    /// </summary>
    /// <returns></returns>
    public bool isWaitTutorial()
    {
        bool ret_val = BattleParam.IsTutorial()
            && (m_CurrentCommandText != null
                || (m_TutorialBattlePhaseCurrent != m_TutorialBattlePhaseReq)
            );

        ret_val |= m_IsShowingTutorialDialog;

        return ret_val;
    }

    /// <summary>
    /// 敵が不死になっているか(BattleLogicがこれを見てBattleLogicを制御する)
    /// </summary>
    /// <returns></returns>
    public bool isNoDeadEnemy()
    {
        bool ret_val = BattleParam.IsTutorial()
            && m_IsNoDeadEnemy;
        return ret_val;
    }

    public bool isForbidLimitBreak(GlobalDefine.PartyCharaIndex caster_index)
    {
        if (BattleParam.IsTutorial())
        {
            if (m_IsForbidButton)
            {
                return true;
            }
            if (caster_index >= GlobalDefine.PartyCharaIndex.LEADER && caster_index < GlobalDefine.PartyCharaIndex.MAX)
            {
                return m_IsForbidLimitBreak[(int)caster_index];
            }
            if (caster_index == GlobalDefine.PartyCharaIndex.HERO)
            {
                return true;
            }
        }

        return false;
    }

    public bool isEnableUnitInfoWindow()
    {
        if (BattleParam.IsTutorial())
        {
            return m_IsEnableUnitInfoWindow && (m_IsForbidButton == false);
        }

        return true;
    }

    public bool isEnableOptionButton()
    {
        if (BattleParam.IsTutorial())
        {
            return m_IsEnableOptionButton && (m_IsForbidButton == false);
        }

        return true;
    }

    public TutorialOptionMenuPhase getTutorialOptionMenuPhase()
    {
        if (BattleParam.IsTutorial())
        {
            return m_TutorialOptionMenuPhase;
        }

        return TutorialOptionMenuPhase.ALL_ON;
    }

    /// <summary>
    /// 敵の全滅要求(BattleLogicがこれを見てBattleLogicを制御する)
    /// </summary>
    /// <param name="is_reset_flag"></param>
    /// <returns></returns>
    public bool isAllDeadEnemy(bool is_reset_flag)
    {
        bool ret_val = BattleParam.IsTutorial()
            && m_IsAllDeadEnemy;

        if (is_reset_flag)
        {
            m_IsAllDeadEnemy = false;
        }

        return ret_val;
    }

    private class TeacherInfo
    {
        public float m_Y;   //0:場札位置 1:手札位置
        public float m_X;   //0:一番左 4:一番右
        public float m_Time;    // 時間
        public bool m_IsTouch;  // タッチ

        public TeacherInfo(float y, float x, float time, bool is_touch)
        {
            m_Y = y;
            m_X = x;
            m_Time = time;
            m_IsTouch = is_touch;
        }
    }

    private int m_CardIndex = 0;

    // お手本の時の札の出現順
    private static readonly int[] m_CardOrder =
    {
        0,
        3,
        0,
        4,
        3,

        5,
        1,
        1,
        4,
        3,

        2,
        6,
        7,
        3,

        2,
        2,
        5,
        6,

        2,
        2,
    };

    // お手本カードの割り当て
    public static readonly MasterDataDefineLabel.ElementType[/*hero*/][] m_CardAssign =
    {
        new MasterDataDefineLabel.ElementType[]
        {
            MasterDataDefineLabel.ElementType.FIRE,
            MasterDataDefineLabel.ElementType.FIRE,
            MasterDataDefineLabel.ElementType.NONE,
            MasterDataDefineLabel.ElementType.WIND,
            MasterDataDefineLabel.ElementType.WATER,

            MasterDataDefineLabel.ElementType.HEAL,
            MasterDataDefineLabel.ElementType.NONE,
            MasterDataDefineLabel.ElementType.NONE,
        },

        // カズシ
        new MasterDataDefineLabel.ElementType[]
        {
            MasterDataDefineLabel.ElementType.LIGHT,
            MasterDataDefineLabel.ElementType.LIGHT,
            MasterDataDefineLabel.ElementType.NONE,
            MasterDataDefineLabel.ElementType.WATER,
            MasterDataDefineLabel.ElementType.WIND,

            MasterDataDefineLabel.ElementType.HEAL,
            MasterDataDefineLabel.ElementType.WIND,
            MasterDataDefineLabel.ElementType.LIGHT,
        },

        // ココロ
        new MasterDataDefineLabel.ElementType[]
        {
            MasterDataDefineLabel.ElementType.NAUGHT,
            MasterDataDefineLabel.ElementType.NAUGHT,
            MasterDataDefineLabel.ElementType.NONE,
            MasterDataDefineLabel.ElementType.DARK,
            MasterDataDefineLabel.ElementType.WATER,

            MasterDataDefineLabel.ElementType.HEAL,
            MasterDataDefineLabel.ElementType.WATER,
            MasterDataDefineLabel.ElementType.NAUGHT,
        },

        // シンク
        new MasterDataDefineLabel.ElementType[]
        {
            MasterDataDefineLabel.ElementType.NAUGHT,
            MasterDataDefineLabel.ElementType.NAUGHT,
            MasterDataDefineLabel.ElementType.NONE,
            MasterDataDefineLabel.ElementType.LIGHT,
            MasterDataDefineLabel.ElementType.WATER,

            MasterDataDefineLabel.ElementType.HEAL,
            MasterDataDefineLabel.ElementType.WATER,
            MasterDataDefineLabel.ElementType.NAUGHT,
        },

        // リアン
        new MasterDataDefineLabel.ElementType[]
        {
            MasterDataDefineLabel.ElementType.FIRE,
            MasterDataDefineLabel.ElementType.FIRE,
            MasterDataDefineLabel.ElementType.NONE,
            MasterDataDefineLabel.ElementType.NAUGHT,
            MasterDataDefineLabel.ElementType.LIGHT,

            MasterDataDefineLabel.ElementType.HEAL,
            MasterDataDefineLabel.ElementType.LIGHT,
            MasterDataDefineLabel.ElementType.FIRE,
        },

        // ムサシ
        new MasterDataDefineLabel.ElementType[]
        {
            MasterDataDefineLabel.ElementType.WIND,
            MasterDataDefineLabel.ElementType.WIND,
            MasterDataDefineLabel.ElementType.NONE,
            MasterDataDefineLabel.ElementType.FIRE,
            MasterDataDefineLabel.ElementType.DARK,

            MasterDataDefineLabel.ElementType.HEAL,
            MasterDataDefineLabel.ElementType.DARK,
            MasterDataDefineLabel.ElementType.WIND,
        },

        // ミカ
        new MasterDataDefineLabel.ElementType[]
        {
            MasterDataDefineLabel.ElementType.DARK,
            MasterDataDefineLabel.ElementType.DARK,
            MasterDataDefineLabel.ElementType.NONE,
            MasterDataDefineLabel.ElementType.WIND,
            MasterDataDefineLabel.ElementType.FIRE,

            MasterDataDefineLabel.ElementType.HEAL,
            MasterDataDefineLabel.ElementType.FIRE,
            MasterDataDefineLabel.ElementType.DARK,
        },
    };

    // お手本の時の手の動き
    private TeacherInfo[] teacher_data = new TeacherInfo[]
    {
        new TeacherInfo(2, 7, 0.01f, false),  // 画面外（右下）待機

        // カードを置く前の並び替え
        new TeacherInfo(1, 3, 1.1f, false),
        new TeacherInfo(1, 3, 0.2f, false),
        new TeacherInfo(1, 4, 0.5f, true),
        new TeacherInfo(1, 4, 0.2f, false),

        new TeacherInfo(1, 2, 0.5f, false),
        new TeacherInfo(1, 1, 0.5f, true),
        new TeacherInfo(1.5f, 2.5f, 0.5f, false),
        new TeacherInfo(1.5f, 2.5f, 0.5f, false),

        new TeacherInfo(1, 1, 0.5f, false),
        new TeacherInfo(1, 0, 0.5f, true),

        new TeacherInfo(0, 0, 0.5f, true),
        new TeacherInfo(0, 0, 0.2f, true),

        // カウントダウン開始
        new TeacherInfo(0, 0, 0.1f, false),

        new TeacherInfo(1, 2, 0.3f, false),
        new TeacherInfo(1, 2, 0.1f, false),
        new TeacherInfo(1, 3, 0.25f, true),
        new TeacherInfo(1, 3, 0.15f, true),
        new TeacherInfo(0, 3, 0.2f, true),
        new TeacherInfo(0, 3, 0.2f, false),

        new TeacherInfo(1, 2, 0.3f, false),
        new TeacherInfo(1, 2, 0.1f, false),
        new TeacherInfo(1, 1, 0.25f, true),
        new TeacherInfo(1, 1, 0.15f, true),
        new TeacherInfo(0, 1, 0.2f, true),
        new TeacherInfo(0, 1, 0.2f, false),

        new TeacherInfo(1, 3, 0.3f, false),
        new TeacherInfo(1, 3, 0.1f, false),
        new TeacherInfo(1, 4, 0.25f, true),
        new TeacherInfo(1, 4, 0.15f, true),
        new TeacherInfo(0, 4, 0.2f, true),
        new TeacherInfo(0, 4, 0.2f, false),

        new TeacherInfo(1, 4, 0.3f, false),
        new TeacherInfo(1, 4, 0.1f, false),
        new TeacherInfo(1, 3, 0.25f, true),
        new TeacherInfo(1, 3, 0.15f, true),
        new TeacherInfo(0, 3, 0.2f, true),
        new TeacherInfo(0, 3, 0.2f, false),

        new TeacherInfo(2, 7, 1.0f, false)	// 画面外（右下）待機
    };
    private int m_TeacherProgress = 0;
    private float m_TeacherTimer = 0.0f;
    private bool m_IsTouching = false;

    private Vector3 m_HandPosStart = Vector3.zero;
    private Vector3 m_HandPosEnd = Vector3.zero;

    /// <summary>
    /// お手本を初期化
    /// </summary>
    private void initTeacher()
    {
        m_TeacherProgress = 0;
        m_TeacherTimer = 0.0f;
        m_CardIndex = 0;
        BattleSceneManager.Instance.PRIVATE_FIELD.m_BattleCardManager.reset(false);
        BattleSceneManager.Instance.PRIVATE_FIELD.FillHandCard();
        m_IsNoDeadEnemy = true;
    }

    /// <summary>
    /// お手本の手の座標を更新
    /// </summary>
    /// <param name="delta_time"></param>
    /// <param name="hand_pos"></param>
    /// <returns></returns>
    private bool updateTeacherHandPosition(float delta_time, ref Vector2 hand_pos)
    {
        TeacherInfo tehon_info = teacher_data[m_TeacherProgress];

        hand_pos = Vector2.zero;

        m_TeacherTimer += delta_time;
        if (m_TeacherTimer >= tehon_info.m_Time)
        {
            m_HandPosStart = BattleSceneManager.Instance.getCardFieldScreenPos(tehon_info.m_X, tehon_info.m_Y);
            hand_pos = m_HandPosStart;

            m_TeacherProgress++;
            m_TeacherTimer -= tehon_info.m_Time;
            if (m_TeacherProgress < teacher_data.Length)
            {
                tehon_info = teacher_data[m_TeacherProgress];
                m_HandPosEnd = BattleSceneManager.Instance.getCardFieldScreenPos(tehon_info.m_X, tehon_info.m_Y);
                m_IsTouching = tehon_info.m_IsTouch;
            }
        }
        else
        {
            if (tehon_info.m_Time > 0.0f)
            {
                hand_pos = Vector2.Lerp(m_HandPosStart, m_HandPosEnd, m_TeacherTimer / tehon_info.m_Time);
            }
            else
            {
                hand_pos = m_HandPosEnd;
            }
        }

        return m_IsTouching;
    }

    /// <summary>
    /// お手本戦闘用のカード供給
    /// </summary>
    /// <returns></returns>
    public MasterDataDefineLabel.ElementType getTutorialCard()
    {
        MasterDataDefineLabel.ElementType ret_val = MasterDataDefineLabel.ElementType.NONE;

        if (m_CardIndex < m_CardOrder.Length)
        {
            ServerDataDefine.PacketStructHero hero = UserDataAdmin.Instance.getCurrentHero();
            if (hero != null
                && hero.hero_id > 0
                && hero.hero_id < m_CardAssign.GetLength(0)
            )
            {
                ret_val = m_CardAssign[hero.hero_id][m_CardOrder[m_CardIndex]];
            }

            m_CardIndex++;
        }

        return ret_val;
    }

}

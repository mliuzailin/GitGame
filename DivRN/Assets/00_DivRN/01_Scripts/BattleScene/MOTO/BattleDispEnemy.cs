using UnityEngine;
using System.Collections;
using TMPro;

// 敵表示クラス.
public class BattleDispEnemy : MonoBehaviour
{
    private const string EFFECT_HANDLE_ENEMY_EVOLVE = "ENEMY_EVOLVE";
    private const float STATUS_BAR_MARGIN = 50.0f;  // ステータスバーの高さ＋αのマージン

    public GameObject m_BattleEnemyObjectPrefab = null;
    public Color m_ShadowColorFire = new Color32(199, 43, 34, 255);
    public Color m_ShadowColorWater = new Color32(51, 142, 199, 255);
    public Color m_ShadowColorWind = new Color32(37, 179, 70, 255);
    public Color m_ShadowColorLight = new Color32(189, 182, 55, 255);
    public Color m_ShadowColorDark = new Color32(108, 27, 175, 255);
    public Color m_ShadowColorNaught = new Color32(155, 155, 155, 255);
    public GameObject m_DropUnitPrefab = null;
    public GameObject m_BossTalkArea = null;
    public GameObject m_BossTalkAreaFrame = null;
    public GameObject m_BossTalkAreaMessage = null;


    public enum AnimationType
    {
        ATTACK, // 攻撃アニメーション
        TALK,   // 敵キャラのしゃべり
        DAMAGE, // 被ダメージアニメーション

        MAX
    }

    private BattleEnemy[] m_BattleEnemys = null;
    private BattleEnemyObjectViewControl[] m_EnemyObjectViewControls = null;
    private GameObject[] m_DropUnitObjects = null;

    private Camera m_Camera = null; // バトルの手前側のカメラ（敵の攻撃時に敵を前面に表示するためレイヤーをこのカメラの物へ変更する）
    private Canvas m_Canvas = null;

    private int m_LockOnTarget = -1;

    private float m_BossTalkTimer = 0.0f;

    private BattlePlayerPartyViewControl m_BattlePlayerPartyViewControl;
    public void setPlayerPartyViewContorl(BattlePlayerPartyViewControl player_party_view_control)
    {
        m_BattlePlayerPartyViewControl = player_party_view_control;
    }

    void Start()
    {
    }

    private void update_sub1(ref Vector2 enemy_size)
    {
        for (int idx = 0; idx < m_EnemyObjectViewControls.Length; idx++)
        {
            BattleEnemyObjectViewControl battle_enemy_object_view_control = m_EnemyObjectViewControls[idx];
            if (battle_enemy_object_view_control != null)
            {
                if (m_BattleEnemys[idx].getMasterDataParamEnemy().pos_absolute != MasterDataDefineLabel.BoolType.ENABLE)
                {
                    float enemy_width = battle_enemy_object_view_control.getCharaWidth();
                    enemy_size.x += enemy_width;
                }

                float tall = battle_enemy_object_view_control.getCharaTall() + battle_enemy_object_view_control.getEnemyDispOffset().y;
                if (tall > enemy_size.y)
                {
                    enemy_size.y = tall;
                }
            }
        }
    }

    private void update_sub2(ref Vector2 enemy_size)
    {
        // 表示位置調整

        // 敵パーティが画面内に収まらなかったら遠くに表示して収まるようにする
        float z_offset = 0.0f;
        {
            // 表示可能領域サイズを計算
            Vector2 enemy_area_size;
            {
                // 敵表示領域の原点（中央下）（スクリーン座標系）
                Vector3 area_base_2d = m_Camera.WorldToScreenPoint(gameObject.transform.localPosition + m_Camera.transform.position);

                // 画面右上位置（ワールド座標系。この座標はステータスバーなどを考慮した表示領域の右上の座標）
                Vector3 top_right_world = BattleSceneManager.Instance.m_TopRightAnchor.transform.position;
                // 画面右上位置（スクリーン座標系）
                Vector3 top_right_2d = m_Camera.WorldToScreenPoint(top_right_world);

                enemy_area_size = top_right_2d - area_base_2d;
                enemy_area_size *= 1.0f / 128.0f / m_Canvas.scaleFactor;    // 敵グループサイズに単位を合わせる.
                enemy_area_size *= 0.95f;   // 画面端ぎりぎりまで表示されないように少し狭める.
                enemy_area_size.x *= 2.0f;
            }

            // 敵グループサイズが表示可能領域をどれくらい上回っているか判定
            float scale_x = enemy_size.x / enemy_area_size.x;
            float scale_y = enemy_size.y / enemy_area_size.y;

            float scale = Mathf.Max(scale_x, scale_y);
            if (scale > 1.0f)
            {
                float enemy_base_z = gameObject.transform.localPosition.z;
                float new_z = enemy_base_z * scale;
                z_offset = new_z - enemy_base_z;
            }
        }

        float x = enemy_size.x * -0.5f;
        for (int idx = 0; idx < m_EnemyObjectViewControls.Length; idx++)
        {
            BattleEnemyObjectViewControl battle_enemy_object_view_control = m_EnemyObjectViewControls[idx];
            if (battle_enemy_object_view_control != null)
            {
                MasterDataParamEnemy enemy_master = m_BattleEnemys[idx].getMasterDataParamEnemy();

                Vector3 offset = battle_enemy_object_view_control.getEnemyDispOffset();
                float Z_OFFSET_SCALE_Y = gameObject.transform.localPosition.y / gameObject.transform.localPosition.z;   // 敵表示の大きさ補正をすると敵の表示位置が下に移動してしまうのを抑制するための値.
                offset.y += z_offset * Z_OFFSET_SCALE_Y;
                offset.z += z_offset;
                if (m_BattleEnemys[idx].getMasterDataParamEnemy().pos_absolute != MasterDataDefineLabel.BoolType.ENABLE)
                {
                    // 位置指定がされていない場合は左から並べる
                    float enemy_width = battle_enemy_object_view_control.getCharaWidth();
                    battle_enemy_object_view_control.gameObject.transform.localPosition = new Vector3(x + enemy_width * 0.5f, 0.0f, 0.0f) + offset;

                    x += enemy_width;
                }
                else
                {
                    // 位置指定がされている場合はその位置に出す
                    Vector3 pos = new Vector3(enemy_master.posx_offset, enemy_master.posy_offset, enemy_master.posz_value) / 128.0f;
                    battle_enemy_object_view_control.gameObject.transform.localPosition = pos + offset;
                }

                // カメラへ向くように補正（左右方向を補正）
                {
                    battle_enemy_object_view_control.gameObject.transform.up = gameObject.transform.up;
                    Vector3 forward = m_Camera.transform.position + gameObject.transform.up * battle_enemy_object_view_control.gameObject.transform.localPosition.y - battle_enemy_object_view_control.gameObject.transform.position;
                    battle_enemy_object_view_control.gameObject.transform.forward = -forward.normalized;
                }

                if (enemy_master != null)
                {
                    // HPゲージ表示位置
                    {
                        battle_enemy_object_view_control.setHpGaugeType(enemy_master.hp_gauge_type);
                        Vector2 hp_offset = new Vector2(enemy_master.hp_posx_offset, enemy_master.hp_posy_offset) / 128.0f;
                        battle_enemy_object_view_control.setHpGaugeOffest(hp_offset);
                    }

                    // ターゲットカーソル位置
                    {
                        Vector2 target_offset = Vector2.zero;
                        if (enemy_master.target_pos_absolute == (int)MasterDataDefineLabel.BoolType.ENABLE)
                        {
                            target_offset = new Vector2(enemy_master.target_posx_offset, enemy_master.target_posy_offset) / 128.0f;
                        }
                        battle_enemy_object_view_control.setTargetCursorOffset(target_offset);
                    }
                }
            }
        }
    }

    private void update_sub3()
    {
        // 表示内容更新
        for (int idx = 0; idx < m_EnemyObjectViewControls.Length; idx++)
        {
            BattleEnemyObjectViewControl battle_enemy_object_view_control = m_EnemyObjectViewControls[idx];
            if (battle_enemy_object_view_control != null)
            {
                battle_enemy_object_view_control.m_Hp = m_BattleEnemys[idx].m_EnemyHP;
                battle_enemy_object_view_control.m_HpMax = m_BattleEnemys[idx].m_EnemyHPMax;
                battle_enemy_object_view_control.m_AttackWaitTurn = m_BattleEnemys[idx].m_EnemyTurn;
                battle_enemy_object_view_control.m_TargetCursorObject.SetActive(idx == m_LockOnTarget);

                // 死亡済みでもテクスチャロードが終わるまではアクティブにしておく
                bool is_active = battle_enemy_object_view_control.isLoadingTexture() || m_BattleEnemys[idx].isShow();
                battle_enemy_object_view_control.gameObject.SetActive(is_active);

                // 攻撃アニメーションが終わったらレイヤーを元に戻す.
                if (battle_enemy_object_view_control.m_ImageObject.layer != battle_enemy_object_view_control.gameObject.layer)
                {
                    if (battle_enemy_object_view_control.isPlayingAnim() == false)
                    {
                        battle_enemy_object_view_control.m_ImageObject.layer = battle_enemy_object_view_control.gameObject.layer;
                    }
                }
            }
        }
    }

    void Update()
    {
        if (m_EnemyObjectViewControls != null)
        {
            Vector2 enemy_size = Vector2.zero;  // 敵グループのサイズ
            update_sub1(ref enemy_size);
            update_sub2(ref enemy_size);
            update_sub3();
        }

        if (m_BossTalkTimer > 0.0f)
        {
            m_BossTalkTimer -= Time.deltaTime;
            if (m_BossTalkTimer <= 0.0f)
            {
                m_BossTalkArea.SetActive(false);
            }
        }
    }

    // 生成直後の初期化
    public void init(Camera camera, Canvas canvas)
    {
        m_Camera = camera;
        m_Canvas = canvas;
    }

    // 表示用オブジェクトを初期化.
    public void initObject()
    {
        BattleTouchInput.Instance.setWorking(false);
        hideTargetCursor();
    }

    // インスタンス生成
    public void instanceObject(BattleEnemy[] enemy_params)
    {
        destroyEnemy();

        m_BattleEnemys = enemy_params;
        if (m_BattleEnemys != null)
        {
            m_EnemyObjectViewControls = new BattleEnemyObjectViewControl[m_BattleEnemys.Length];
            hideDropObject();
            m_DropUnitObjects = new GameObject[m_BattleEnemys.Length];
            for (int idx = 0; idx < m_EnemyObjectViewControls.Length; idx++)
            {
                if (m_BattleEnemys[idx] != null)
                {
                    GameObject battle_enemy_object = Instantiate(m_BattleEnemyObjectPrefab);
                    if (battle_enemy_object != null)
                    {
                        BattleEnemyObjectViewControl battle_enemy_object_view_control = battle_enemy_object.GetComponent<BattleEnemyObjectViewControl>();
                        if (battle_enemy_object_view_control != null)
                        {
                            battle_enemy_object_view_control.m_EnemyCharaID = (int)m_BattleEnemys[idx].getMasterDataParamChara().fix_id;

                            switch (m_BattleEnemys[idx].getMasterDataParamChara().element)
                            {
                                case MasterDataDefineLabel.ElementType.FIRE:
                                    battle_enemy_object_view_control.m_ShadowColor = m_ShadowColorFire;
                                    break;

                                case MasterDataDefineLabel.ElementType.WATER:
                                    battle_enemy_object_view_control.m_ShadowColor = m_ShadowColorWater;
                                    break;

                                case MasterDataDefineLabel.ElementType.WIND:
                                    battle_enemy_object_view_control.m_ShadowColor = m_ShadowColorWind;
                                    break;

                                case MasterDataDefineLabel.ElementType.LIGHT:
                                    battle_enemy_object_view_control.m_ShadowColor = m_ShadowColorLight;
                                    break;

                                case MasterDataDefineLabel.ElementType.DARK:
                                    battle_enemy_object_view_control.m_ShadowColor = m_ShadowColorDark;
                                    break;

                                case MasterDataDefineLabel.ElementType.NAUGHT:
                                    battle_enemy_object_view_control.m_ShadowColor = m_ShadowColorNaught;
                                    break;

                                default:
                                    battle_enemy_object_view_control.m_ShadowColor = Color.black;
                                    break;
                            }

                            m_EnemyObjectViewControls[idx] = battle_enemy_object_view_control;

                            battle_enemy_object_view_control.setAilment(m_BattleEnemys[idx].m_StatusAilmentChara);

                            battle_enemy_object.transform.parent = gameObject.transform;
                            battle_enemy_object.transform.localPosition = Vector3.zero;
                            battle_enemy_object.transform.localRotation = Quaternion.identity;
                            battle_enemy_object.transform.localScale = Vector3.one;
                        }
                        else
                        {
                            GameObject.Destroy(battle_enemy_object);
                        }
                    }
                }
            }
        }
    }

    public void destroyEnemy()
    {
        m_BattleEnemys = null;
        if (m_EnemyObjectViewControls != null)
        {
            for (int idx = 0; idx < m_EnemyObjectViewControls.Length; idx++)
            {
                BattleEnemyObjectViewControl battle_enemy_object_view_control = m_EnemyObjectViewControls[idx];
                if (battle_enemy_object_view_control != null)
                {
                    if (battle_enemy_object_view_control.gameObject != null)
                    {
                        GameObject.Destroy(battle_enemy_object_view_control.gameObject);
                    }
                }
            }
            m_EnemyObjectViewControls = null;
        }
        hideDropObject();
    }

    public void playTurnChangeEffect(int enemy_index)
    {
        m_EnemyObjectViewControls[enemy_index].m_TurnChangeEffect = true;
    }

    // ターゲットカーソルの表示を設定.
    public void showTargetCursor(int enemy_index)
    {
        m_LockOnTarget = enemy_index;
    }

    // ターゲットカーソルを消す.
    public void hideTargetCursor()
    {
        m_LockOnTarget = -1;
    }

    // ドロップアイテムを表示
    public void showDropObject(int enemy_index, MasterDataDefineLabel.RarityType rarity, bool is_show_plus, bool is_ticket, bool is_coin)
    {
        if (m_DropUnitPrefab != null)
        {
            if (m_DropUnitObjects[enemy_index] != null)
            {
                GameObject.Destroy(m_DropUnitObjects[enemy_index]);
                m_DropUnitObjects[enemy_index] = null;
            }
            GameObject drop_unit_object = GameObject.Instantiate(m_DropUnitPrefab);
            m_DropUnitObjects[enemy_index] = drop_unit_object;

            drop_unit_object.transform.SetParent(getEnemyHpGaugeTransform(enemy_index), false);
            drop_unit_object.transform.SetParent(transform, true);

            // レアリティ選択（とりあえず階層・名前決め打ちで設定）
            string mesh_name;
            if (is_ticket)
            {
                mesh_name = "Ticket";
            }
            else if (is_coin)
            {
                mesh_name = "Coin";
            }
            else
            {
                string[] mesh_names = new string[(int)MasterDataDefineLabel.RarityType.MAX]
                {
                    "LV1",
                    "LV2",
                    "LV3",
                    "LV4",
                    "LV5",
                    "LV6",
                    "LV7"
                };
                mesh_name = mesh_names[(int)rarity];
            }

            Transform mesh_trans_base = drop_unit_object.transform.GetChild(0).GetChild(0);
            mesh_trans_base = mesh_trans_base.Find(mesh_name);
            if (mesh_trans_base != null)
            {
                mesh_trans_base.gameObject.SetActive(true);
            }

            // プラス表示（とりあえず階層・名前決め打ちで設定）
            if (is_show_plus)
            {
                Transform plus_trans = drop_unit_object.transform.GetChild(0).Find("plus");
                if (plus_trans != null)
                {
                    plus_trans.gameObject.SetActive(true);
                }
            }

            // 駒エフェクトを表示
            if (EffectManager.Instance != null)
            {
                EffectManager.Instance.playEffect(SceneObjReferGameMain.Instance.m_EffectPrefab.m_2DUnitDrop, new Vector3(0.0f, 0.5f, -1.0f), Vector3.zero, drop_unit_object.transform, null, 128.0f * 2.0f, BattleLogic.EFFECT_HANDLE_UNIT_DROP);
            }

            SoundUtil.PlaySE(SEID.SE_INGAME_PANEL_MEKURI_SPECIAL);
        }
    }

    // ドロップアイテムの表示を削除.
    public void hideDropObject()
    {
        if (m_DropUnitObjects != null)
        {
            for (int idx = 0; idx < m_DropUnitObjects.Length; idx++)
            {
                if (m_DropUnitObjects[idx] != null)
                {
                    GameObject.Destroy(m_DropUnitObjects[idx]);
                    m_DropUnitObjects[idx] = null;
                }
            }
        }
    }

    /// <summary>
    /// 敵ユニットによる攻撃
    /// </summary>
    /// <param name="enemyIndex"></param>
    /// <param name="actionParam"></param>
    public void attack(int enemyIndex, MasterDataEnemyActionParam actionParam, BattleSceneUtil.MultiInt damage_value, BattleSceneUtil.MultiInt damage_target)
    {
        //----------------------------------------
        //	エラーチェック
        //----------------------------------------
        if (enemyIndex < 0
        || enemyIndex >= BattleParam.m_EnemyParam.Length)
        {
            return;
        }

        //----------------------------------------
        //	各種設定の取得
        //----------------------------------------
        MasterDataDefineLabel.EnemyMotionType attackMotion;
        MasterDataDefineLabel.EnemyATKShakeType shakeScreen;
        MasterDataDefineLabel.BoolType damageEffect;
        MasterDataDefineLabel.UIEffectType effectID;
        int seID;

        if (actionParam != null)
        {

            //----------------------------------------
            //	行動定義の設定値
            //----------------------------------------
            attackMotion = actionParam.attack_motion;
            shakeScreen = actionParam.shake_screen;
            damageEffect = actionParam.damage_effect;
            effectID = actionParam.effect_id;
            seID = actionParam.se_id;

        }
        else
        {

            //----------------------------------------
            //	デフォルト値
            //----------------------------------------
            attackMotion = MasterDataDefineLabel.EnemyMotionType.ATK_00;
            shakeScreen = MasterDataDefineLabel.EnemyATKShakeType.NORMAL;
            damageEffect = MasterDataDefineLabel.BoolType.ENABLE;
            effectID = 0;
            seID = (int)SEID.SE_BATLE_ATTACK_EN_NORMAL;

        }


        //----------------------------------------
        //	攻撃モーション再生
        //----------------------------------------
        if (attackMotion != MasterDataDefineLabel.EnemyMotionType.NONE)
        {

            playAnimation(enemyIndex, AnimationType.ATTACK);
        }


        if (damageEffect == MasterDataDefineLabel.BoolType.ENABLE
            && m_BattlePlayerPartyViewControl != null
        )
        {
            MasterDataDefineLabel.ElementType element_type = BattleParam.m_EnemyParam[enemyIndex].getMasterDataParamChara().element;

            // 属性指定攻撃の場合はその属性のエフェクト
            if (actionParam != null
                && actionParam.skill_type == MasterDataDefineLabel.EnemySkillCategory.ATK_ELEM)
            {
                element_type = actionParam.Get_ATK_ELEM_TARGET_ELEM();
            }

            if (element_type != MasterDataDefineLabel.ElementType.NONE)
            {
                GameObject effect_prefab = null;
                switch (element_type)
                {
                    case MasterDataDefineLabel.ElementType.FIRE:
                        effect_prefab = SceneObjReferGameMain.Instance.m_EffectPrefab.m_2DPlayerDamage[(int)MasterDataDefineLabel.ElementType.FIRE];
                        break;
                    case MasterDataDefineLabel.ElementType.WATER:
                        effect_prefab = SceneObjReferGameMain.Instance.m_EffectPrefab.m_2DPlayerDamage[(int)MasterDataDefineLabel.ElementType.WATER];
                        break;
                    case MasterDataDefineLabel.ElementType.DARK:
                        effect_prefab = SceneObjReferGameMain.Instance.m_EffectPrefab.m_2DPlayerDamage[(int)MasterDataDefineLabel.ElementType.DARK];
                        break;
                    case MasterDataDefineLabel.ElementType.LIGHT:
                        effect_prefab = SceneObjReferGameMain.Instance.m_EffectPrefab.m_2DPlayerDamage[(int)MasterDataDefineLabel.ElementType.LIGHT];
                        break;
                    case MasterDataDefineLabel.ElementType.NAUGHT:
                        effect_prefab = SceneObjReferGameMain.Instance.m_EffectPrefab.m_2DPlayerDamage[(int)MasterDataDefineLabel.ElementType.NAUGHT];
                        break;
                    case MasterDataDefineLabel.ElementType.WIND:
                        effect_prefab = SceneObjReferGameMain.Instance.m_EffectPrefab.m_2DPlayerDamage[(int)MasterDataDefineLabel.ElementType.WIND];
                        break;
                    case MasterDataDefineLabel.ElementType.NONE:
                    default:
                        break;
                }

                if (effect_prefab != null)
                {
                    m_BattlePlayerPartyViewControl.setDamageEffectMember(effect_prefab, damage_target);
                }
            }
        }

        //----------------------------------------
        //	攻撃エフェクト
        //----------------------------------------
        playDamageEffect(effectID, enemyIndex, true);

        //----------------------------------------
        //	SE再生
        //----------------------------------------
        if (seID != (int)SEID.SE_NONE)
        {

            SoundUtil.PlaySE((SEID)seID);

        }


    }


    /// <summary>
    /// ダメージエフェクト
    /// </summary>
    public void playDamageEffect(MasterDataDefineLabel.UIEffectType effect_id, int enemy_index, bool is_enemy_side)
    {
        //----------------------------------------
        //	攻撃エフェクト
        //----------------------------------------
        if (effect_id != MasterDataDefineLabel.UIEffectType.NONE)
        {
            InGameDefine.Effect2DAnchorType anchorID = InGameDefine.Effect2DAnchorType.CENTER;

            //----------------------------------------
            //	再生エフェクト取得
            //----------------------------------------
            GameObject damageEff = BattleSceneUtil.GetSkillEffectPrefab(effect_id, ref anchorID);
            if (damageEff == null)
            {
                return;
            }

            if (is_enemy_side)
            {
                if (enemy_index < 0)
                {
                    anchorID = InGameDefine.Effect2DAnchorType.CENTER;
                }
            }

            bool is_new_effect_data = true;

            // 新旧エフェクトデータが混在しているので判別して出し方を変える
            ParticleSystem particle_system = damageEff.GetComponent<ParticleSystem>();
            if (particle_system != null)
            {
                //判定方法は適当（旧データは playOnAwake が true になっているのでそれを使っている）
                if (particle_system.main.playOnAwake != false)
                {
                    is_new_effect_data = false;
                }
            }

            // 敵がプレイヤーへ回復エフェクトを発生させた場合の特殊処理
            if (anchorID == InGameDefine.Effect2DAnchorType.BOTTOM
                && is_enemy_side
                && BattleParam.m_PlayerParty != null
            )
            {
                int heal_level = -1;
                if (damageEff == SceneObjReferGameMain.Instance.m_EffectPrefab.m_Heal_00)
                {
                    heal_level = 0;
                }
                else if (damageEff == SceneObjReferGameMain.Instance.m_EffectPrefab.m_Heal_01)
                {
                    heal_level = 1;
                }
                else if (damageEff == SceneObjReferGameMain.Instance.m_EffectPrefab.m_Heal_02)
                {
                    heal_level = 2;
                }
                else if (damageEff == SceneObjReferGameMain.Instance.m_EffectPrefab.m_Heal_03)
                {
                    heal_level = 3;
                }

                if (heal_level >= 0)
                {
                    for (int idx = 0; idx < BattleParam.m_PlayerParty.getPartyMemberMaxCount(); idx++)
                    {
                        CharaOnce chara_once = BattleParam.m_PlayerParty.getPartyMember((GlobalDefine.PartyCharaIndex)idx, CharaParty.CharaCondition.ALIVE);
                        if (chara_once != null)
                        {
                            m_BattlePlayerPartyViewControl.setEffectHeal((GlobalDefine.PartyCharaIndex)idx, heal_level);
                        }
                    }
                    return;
                }
            }

            //----------------------------------------
            //	指定位置への表示
            //----------------------------------------
            switch (anchorID)
            {
                case InGameDefine.Effect2DAnchorType.ENEMY_ONCE:
                    if (is_new_effect_data)
                    {
                        EffectManager.Instance.playEffect(damageEff, new Vector3(0.0f, 0.0f, -1.0f), Vector3.zero, getEnemyCenterTransform(enemy_index), null, 0.8f);
                    }
                    else
                    {
                        EffectManager.Instance.playEffect(damageEff, new Vector3(0.0f, 0.0f, -1.0f), Vector3.zero, getEnemyCenterTransform(enemy_index), null, 4.0f);
                    }
                    break;

                case InGameDefine.Effect2DAnchorType.CENTER:
                    if (is_new_effect_data)
                    {
                        EffectManager.Instance.playEffect(damageEff, new Vector3(0.0f, -0.5f, -1.0f), Vector3.zero, getEnemyAreaRootTransform(), null, 3.0f);
                    }
                    else
                    {
                        EffectManager.Instance.playEffect(damageEff, new Vector3(0.0f, 0.0f, -1.0f), Vector3.zero, getEnemyAreaRootTransform(), null, 2.5f);
                    }
                    break;

                case InGameDefine.Effect2DAnchorType.CENTER2:
                    if (is_new_effect_data)
                    {
                        // 音符全体攻撃の位置・サイズ補正
                        EffectManager.Instance.playEffect(damageEff, new Vector3(0.0f, 0.5f, -1.0f), Vector3.zero, getEnemyAreaRootTransform(), null, 3.0f * 0.5f);
                    }
                    else
                    {
                        EffectManager.Instance.playEffect(damageEff, new Vector3(0.0f, 0.0f, -1.0f), Vector3.zero, getEnemyAreaRootTransform(), null, 2.5f);
                    }
                    break;

                case InGameDefine.Effect2DAnchorType.BOTTOM:
                    m_BattlePlayerPartyViewControl.setEffectParty(damageEff);
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 敵にしゃべらせる
    /// </summary>
    /// <param name="enemyIndex"></param>
    /// <param name="enemyActionParam"></param>
    public void talk(int enemyIndex, MasterDataEnemyActionParam enemyActionParam)
    {
        if (enemyActionParam == null)
        {
            return;
        }

        //----------------------------------------
        //	エラーチェック
        //----------------------------------------
        if (enemyIndex < 0
        || enemyIndex >= BattleParam.m_EnemyParam.Length)
        {
            return;
        }

        string message = enemyActionParam.skill_name;

        if (message != null && message != "")
        {
            if (m_BossTalkArea != null && m_BossTalkAreaFrame != null && m_BossTalkAreaMessage != null)
            {
                TextMeshPro text_mesh = m_BossTalkAreaMessage.GetComponent<TextMeshPro>();
                text_mesh.text = message;

                // 行数を調べる
                int line_count = 1;
                int start_index = 0;
                while (true)
                {
                    int idx = message.IndexOf('\n', start_index);
                    if (idx >= 0)
                    {
                        line_count++;
                        start_index = idx + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                // 表示行数に合わせて下地のサイズを広げる
                const float FRAME_LINE_HEIGHT = 0.66f;  // 文字一行分の高さ
                const float FRAME_BORDER_HEIGHT = 1.0f - FRAME_LINE_HEIGHT; // 下地枠の高さ
                float talk_area_frame_y_scale = FRAME_BORDER_HEIGHT + FRAME_LINE_HEIGHT * line_count;
                m_BossTalkAreaFrame.transform.localScale = new Vector3(14.0f, talk_area_frame_y_scale, 1.0f);

                // 表示時間設定
                m_BossTalkTimer = enemyActionParam.wait_time / 1000.0f; // 表示時間（ミリ秒を秒へ変換）
                if (m_BossTalkTimer > 0.0f)
                {
                    if (m_BossTalkTimer < 0.3f)
                    {
                        // 短すぎる時間は設定ミスの可能性があるので一瞬だけ見えるようにする（何も見えないとバグに気付けない）
                        m_BossTalkTimer = 0.3f;
                    }
                }
                else
                {
                    const float TALK_TIME_DEFAULT = 1.5f;   // デフォルトの表示時間（1.5秒）
                    m_BossTalkTimer = TALK_TIME_DEFAULT;
                }

                m_BossTalkArea.SetActive(true);
            }

            playAnimation(enemyIndex, AnimationType.TALK);

            DebugBattleLog.writeText(DebugBattleLog.StrOpe + "敵行動：セリフ「" + message + "」");
        }
    }

    /// <summary>
    /// 敵がしゃべっている最中かどうかを取得
    /// </summary>
    /// <returns></returns>
    public bool isTalking()
    {
        return m_BossTalkArea.IsActive();
    }

    public void stopTalk()
    {
        if (m_BossTalkTimer > 0.0f)
        {
            m_BossTalkTimer = 0.0f;
            m_BossTalkArea.SetActive(false);
        }
    }


    // ダメージ描画
    public void damageEnemy(int enemy_index, int damage, EDAMAGE_TYPE damage_type)
    {
        m_EnemyObjectViewControls[enemy_index].setDmageDisp(damage, damage_type);
        if (damage_type != EDAMAGE_TYPE.eDAMAGE_TYPE_HEAL)
        {
            playAnimation(enemy_index, AnimationType.DAMAGE);
        }
    }

    // 死亡エフェクト描画
    public void showDeadEffect(int enemy_index, bool is_boss, string effect_handle)
    {
        if (EffectManager.Instance != null)
        {
            //--------------------------------
            // ボスは専用エフェクトに切り替え
            //--------------------------------
            GameObject effDead = SceneObjReferGameMain.Instance.m_EffectPrefab.m_EnemyDeath;
            if (is_boss)
            {
                effDead = SceneObjReferGameMain.Instance.m_EffectPrefab.m_EnemyDeath_BOSS;
            }

            EffectManager.Instance.playEffect(effDead, new Vector3(0.0f, 0.3f, -1.0f), Vector3.zero, getEnemyCenterTransform(enemy_index), null, 3.0f, effect_handle);
        }
    }

    /// <summary>
    /// 敵進化描画１（進化前）
    /// </summary>
    public void showEvolve1()
    {
        EffectManager.Instance.playEffect(SceneObjReferGameMain.Instance.m_EffectPrefab.m_2DEnemyEvol,
            new Vector3(0.0f, 0.0f, -1.0f), Vector3.zero, getEnemyCenterTransform(0), null, 3.5f, EFFECT_HANDLE_ENEMY_EVOLVE);
    }

    /// <summary>
    /// 敵進化描画２（進化後）
    /// </summary>
    public void showEvolve2(MasterDataDefineLabel.EvoluveEffectType evolve_effect_type, TextCutinViewControl text_cutin)
    {
        EffectManager.Instance.playEffect(SceneObjReferGameMain.Instance.m_EffectPrefab.m_2DEnemyEvol2,
            new Vector3(0.0f, 0.0f, -1.0f), Vector3.zero, getEnemyCenterTransform(0), null, 3.5f, EFFECT_HANDLE_ENEMY_EVOLVE);

        //----------------------------------------
        // 演出方法選択
        //----------------------------------------
        switch (evolve_effect_type)
        {
            // 文字表示OFF
            case MasterDataDefineLabel.EvoluveEffectType.LABEL_NONE:
                //--------------------------------
                // SE-EVOLVE(声無し)
                //--------------------------------
                SoundUtil.PlaySE(SEID.SE_MM_D10_EVOLVE_COMP);
                break;

            // 従来通りのEvolve!
            case MasterDataDefineLabel.EvoluveEffectType.NONE:
            case MasterDataDefineLabel.EvoluveEffectType.DEFAULT:
            default:

                // 文字表示ON
                text_cutin.startAnim(TextCutinViewControl.TitleType.EVOLVE);

                //--------------------------------
                // SE-EVOLVE
                //--------------------------------
                SoundUtil.PlaySE(SEID.VOICE_INGAME_MM_EVOLVE);
                SoundUtil.PlaySE(SEID.SE_MM_D10_EVOLVE_COMP);
                break;
        }
    }

    /// <summary>
    /// 進化演出中かどうか
    /// </summary>
    /// <returns></returns>
    public bool isShowEvolve()
    {
        bool ret_val = EffectManager.Instance.isPlayingEffect(EFFECT_HANDLE_ENEMY_EVOLVE);
        return ret_val;
    }


    // アニメーション再生.
    public void playAnimation(int enemy_index, AnimationType animation_type)
    {
        switch (animation_type)
        {
            case AnimationType.ATTACK:
                m_EnemyObjectViewControls[enemy_index].playAnim("BattleEnemyAttack");
                m_EnemyObjectViewControls[enemy_index].m_ImageObject.layer = m_Camera.gameObject.layer;
                break;

            case AnimationType.TALK:
                m_EnemyObjectViewControls[enemy_index].playAnim("BattleEnemyTalk");
                break;

            case AnimationType.DAMAGE:
                m_EnemyObjectViewControls[enemy_index].playAnim("BattleEnemyDamage");
                break;
        }
    }

    /// <summary>
    /// 敵のＨＰゲージ表示位置（足元）を取得
    /// </summary>
    /// <param name="enemy_index"></param>
    /// <returns></returns>
    public Transform getEnemyHpGaugeTransform(int enemy_index)
    {
        return m_EnemyObjectViewControls[enemy_index].m_HitPointLocate.transform;
    }

    /// <summary>
    /// 敵画像中心位置
    /// </summary>
    /// <param name="enemy_index"></param>
    /// <returns></returns>
    public Transform getEnemyCenterTransform(int enemy_index)
    {
        return m_EnemyObjectViewControls[enemy_index].m_CenterObject.transform;
    }

    public Transform getEnemyAreaRootTransform()
    {
        return gameObject.transform;
    }

    // プレイヤーがタッチしている敵を取得.タッチしていなければ -1 が返る
    public int checkTouchEnemy(bool is_long_touch)
    {
        BattleTouchInput battle_touch_input = BattleTouchInput.Instance;
        bool triger = false;
        if (is_long_touch)
        {
            triger = battle_touch_input.isTouching() && battle_touch_input.isDrag() == false && battle_touch_input.getTouchingTime() >= 0.5f;
        }
        else
        {
            triger = battle_touch_input.isTapped();
        }

        if (triger)
        {
            BattleScene.FieldCollision field_collision = GetComponent<BattleScene.FieldCollision>();
            if (field_collision != null)
            {
                if (field_collision.IsMouseOver)
                {
                    Vector2 mouse_position = battle_touch_input.getPosition();
                    float dist_sqr_min = 1.0e+30f;
                    int touch_index = -1;
                    for (int idx = 0; idx < m_EnemyObjectViewControls.Length; idx++)
                    {
                        BattleEnemyObjectViewControl enemy_view_control = m_EnemyObjectViewControls[idx];
                        if (enemy_view_control != null)
                        {
                            Vector3 enemy_screen_position = battle_touch_input.getScreenPosition(enemy_view_control.m_TargetCursorObject.transform.position);
                            Vector2 dir = mouse_position - new Vector2(enemy_screen_position.x, enemy_screen_position.y);
                            float dist_sqr = dir.sqrMagnitude;
                            if (dist_sqr < dist_sqr_min)
                            {
                                dist_sqr_min = dist_sqr;
                                touch_index = idx;
                            }
                        }
                    }
                    return touch_index;
                }
            }
        }

        return -1;
    }


    /// <summary>
    /// 敵領域をタップしたかどうか（カウントダウン短縮の判定で使用）
    /// </summary>
    /// <returns></returns>
    public bool isTappedEnemyArea()
    {
        BattleTouchInput battle_touch_input = BattleTouchInput.Instance;
        if (battle_touch_input.isDeviceTouchTriger()
            && battle_touch_input.isOverrideTouchMode() == false
        )
        {
            BattleScene.FieldCollision field_collision = GetComponent<BattleScene.FieldCollision>();
            if (field_collision != null)
            {
                if (field_collision.IsMouseOver)
                {
                    return true;
                }
            }
        }

        return false;
    }
}

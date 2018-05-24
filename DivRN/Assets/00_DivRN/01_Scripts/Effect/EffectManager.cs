#define REPLACE_SHADER
// エフェクトのシェーダをプラットフォームに適したものに置き換える定義。
// アセットバンドル化したエフェクトデータのシェーダが外れる問題への対策
// Android等向けに作成されたアセットバンドルを UnityEditor 上で使用する場合のみ発生？
// なので UNITY_EDITOR の時のみ有効にすれば良い？（今のところ常に有効にしています）

using UnityEngine;
using System.Collections;

/// <summary>
/// 新エフェクトマネージャ
/// </summary>
public class EffectManager : SingletonComponent<EffectManager>
{
    private const int EFFECT_MAX = 32/*100*/;   // エフェクト同時存在最大数.

    private PoolInfoManager m_PoolInfoManager = new PoolInfoManager();  // プール管理
    private EffectHandle[] m_EffectHandle = new EffectHandle[EFFECT_MAX];     //!< エフェクト関連：エフェクトインスタンス管理

    // MonoBehaviour 関連
    #region <MonoBehaviour>
    protected override void Awake()
    {
        base.Awake();

        for (int idx = 0; idx < m_EffectHandle.Length; idx++)
        {
            m_EffectHandle[idx] = new EffectHandle();
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        updateManager(Time.deltaTime);
    }

    override protected void OnDestroy()
    {
        stopEffectAll();
        unpoolAll();
        base.OnDestroy();
    }
    #endregion //<MonoBehaviour>

    // プール管理
    #region <PoolManagerment>
    /// <summary>
    /// エフェクトをプール
    /// </summary>
    /// <param name="effect_prefab"></param>
    /// <param name="pool_num"></param>
    /// <param name="effect_max">同時に再生できる最大数</param>
    public void poolEffect(GameObject effect_prefab, int pool_num, int effect_max = 0)
    {
        if (effect_prefab != null)
        {
            ParticleSystem key_particle_system = effect_prefab.GetComponent<ParticleSystem>();
            if (key_particle_system != null)
            {
                if (pool_num > 0)
                {
                    if (effect_max < pool_num)
                    {
                        effect_max = pool_num;
                    }
                }

                m_PoolInfoManager.unusePoolInfo(key_particle_system);
                m_PoolInfoManager.createPoolInfo(key_particle_system, pool_num, effect_max);

                // プールした分を一度画面内で再生しておく（シェーダーのコンパイルなどをこの段階で済ませる）
                for (int idx = 0; idx < pool_num; idx++)
                {
                    playEffect(effect_prefab, Vector3.zero, Vector3.zero, null, null, 0.0001f);
                }
            }
        }
    }

    /// <summary>
    /// エフェクトのプールを解除
    /// </summary>
    /// <param name="effect_prefab"></param>
    public void unpoolEffect(GameObject effect_prefab)
    {
        if (effect_prefab != null)
        {
            ParticleSystem key_particle_system = effect_prefab.GetComponent<ParticleSystem>();
            if (key_particle_system != null)
            {
                m_PoolInfoManager.unusePoolInfo(key_particle_system);
            }
        }
    }

    /// <summary>
    /// すべてのエフェクトのプールを解除
    /// </summary>
    public void unpoolAll()
    {
        m_PoolInfoManager.unusePoolInfoAll();
    }
    #endregion //<PoolManagerment>

    /// <summary>
    /// エフェクト再生
    /// </summary>
    /// <param name="effect_prefab">エフェクトのプレハブ</param>
    /// <param name="position">エフェクト生成座標</param>
    /// <param name="base_transform">エフェクト生成座標の基準トランスフォーム（ワールド座標に生成したい場合は null を指定）</param>
    /// <param name="ride_transform">エフェクトのライド先。ここに値が設定された場合エフェクトはそれに追従する</param>
    /// <param name="size_scale">エフェクトのスケーリング</param>
    /// <param name="handle_name">エフェクトハンドリング用の識別名（エフェクト停止などで使用）</param>
    /// <param name="speed_scale">エフェクトの再生速度</param>
    /// <param name="layer">レイヤーの設定(-1の時はbase_transformと同じレイヤーになります)</param>
    /// <returns>再生したエフェクトのインスタンス</returns>
    public GameObject playEffect(
           GameObject effect_prefab,
           Vector3 position,
           Vector3 euler_angles,
           Transform base_transform,
           Transform ride_transform,
           float size_scale = 1.0f,
           string handle_name = null,
           float speed_scale = 1.0f,
           int layer = -1)
    {
        GameObject effect_game_object = null;
        if (effect_prefab != null)
        {
            ParticleSystem key_particle_system = effect_prefab.GetComponent<ParticleSystem>();
            if (key_particle_system != null)
            {
                EffectHandle effect_handle = getFreeEffectHandle();
                if (effect_handle != null)
                {
                    PoolInfo pool_info = m_PoolInfoManager.usePoolInfo(key_particle_system);
                    ParticleSystem instance_particle_system = null;
                    if (pool_info != null)
                    {
                        if (pool_info.isMaxEffect())
                        {
                            // 同一エフェクト最大数を超えるとき

                            EffectHandle old_effect = getOldEffectHandle(pool_info);
                            if (old_effect != null)
                            {
                                // 再生中のエフェクトのゲームオブジェクトをそのまま再利用
                                instance_particle_system = old_effect.m_ParticleSystem;
                                if (instance_particle_system != null)
                                {
                                    // 再生中のエフェクトを停止
                                    instance_particle_system.Stop();
                                }

                                old_effect.m_ParticleSystem = null;
                                old_effect.m_PoolInfo = null;
                                old_effect.m_HandleName = null;
                                old_effect.m_FrameCounter = 0;
                            }
                        }

                        if (instance_particle_system == null)
                        {
                            instance_particle_system = pool_info.instanceParticleSystem();
                        }
                    }

                    if (instance_particle_system != null)
                    {
                        effect_game_object = instance_particle_system.gameObject;

                        effect_handle.m_ParticleSystem = instance_particle_system;
                        effect_handle.m_PoolInfo = pool_info;
                        effect_handle.m_HandleName = handle_name;
                        effect_handle.m_FrameCounter = 0;
                    }

                    if (effect_game_object != null)
                    {
                        if (base_transform != null)
                        {
                            effect_game_object.transform.parent = base_transform;
                        }
                        else
                        {
                            effect_game_object.transform.parent = EffectManager.Instance.transform;
                        }

                        if (layer < 0)
                        {
                            if (effect_game_object.transform.parent != null)
                            {
                                layer = effect_game_object.transform.parent.gameObject.layer;
                            }
                            else
                            {
                                layer = 0;
                            }
                        }

                        pool_info.initialLocalPosture(effect_game_object, size_scale, speed_scale, layer);

                        effect_game_object.transform.localPosition += position;
                        effect_game_object.transform.Rotate(euler_angles);

                        if (ride_transform != null)
                        {
                            effect_game_object.transform.parent = ride_transform;
                        }
                        else
                        {
                            effect_game_object.transform.parent = EffectManager.Instance.transform;
                        }

                        effect_game_object.SetActive(true);
                        instance_particle_system.Play();
                    }
                }
            }
        }

        return effect_game_object;
    }

    /// <summary>
    /// エフェクト再生（playEffect() の指定パラメータが異なるバージョン）
    /// </summary>
    /// <param name="effect_prefab">エフェクトのプレハブ</param>
    /// <param name="position">エフェクト生成座標</param>
    /// <param name="base_game_object">エフェクト生成座標の基準位置（ワールド座標に生成したい場合は null を指定）</param>
    /// <param name="ride_game_object">エフェクトのライド先。ここに値が設定された場合エフェクトはそれに追従する</param>
    /// <param name="size_scale">エフェクトのスケーリング</param>
    /// <param name="handle_name">エフェクトハンドリング用の識別名（エフェクト停止などで使用）</param>
    /// <param name="speed_scale">エフェクトの再生速度</param>
    /// <param name="layer">レイヤーの設定(-1の時はbase_transformと同じレイヤーになります)</param>
    /// <returns>再生したエフェクトのインスタンス</returns>
    public GameObject playEffect2(
           GameObject effect_prefab,
           Vector3 position,
           Vector3 euler_angles,
           GameObject base_game_object,
           GameObject ride_game_object,
           float size_scale = 1.0f,
           string handle_name = null,
           float speed_scale = 1.0f,
           int layer = -1)
    {
        Transform base_transform = null;
        if (base_game_object != null)
        {
            base_transform = base_game_object.transform;
        }

        Transform ride_transform = null;
        if (ride_game_object != null)
        {
            ride_transform = ride_game_object.transform;
        }

        return playEffect(effect_prefab, position, euler_angles, base_transform, ride_transform, size_scale, handle_name, speed_scale, layer);
    }

    /// <summary>
    /// エフェクトが再生中かどうかを取得
    /// </summary>
    /// <param name="handle_name">エフェクトハンドリング用の識別名</param>
    /// <returns>再生中の場合 true</returns>
    public bool isPlayingEffect(string handle_name)
    {
        if (handle_name == null) return false;
        for (int idx = 0; idx < m_EffectHandle.Length; idx++)
        {
            EffectHandle effect_handle = m_EffectHandle[idx];
            if (effect_handle.m_ParticleSystem != null
                   && effect_handle.m_HandleName == handle_name)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// エフェクト停止
    /// </summary>
    /// <param name="handle_name">エフェクトハンドリング用の識別名</param>
    public void stopEffect(string handle_name)
    {
        for (int idx = 0; idx < m_EffectHandle.Length; idx++)
        {
            EffectHandle effect_handle = m_EffectHandle[idx];
            if (effect_handle.m_PoolInfo != null
                   && effect_handle.m_HandleName == handle_name)
            {
                unuseEffectHandle(effect_handle);
            }
        }
    }

    /// <summary>
    /// 全エフェクト停止
    /// </summary>
    public void stopEffectAll()
    {
        for (int idx = 0; idx < m_EffectHandle.Length; idx++)
        {
            EffectHandle effect_handle = m_EffectHandle[idx];
            if (effect_handle.m_PoolInfo != null)
            {
                unuseEffectHandle(effect_handle);
            }
        }
    }

    private EffectHandle getFreeEffectHandle()
    {
        for (int idx = 0; idx < m_EffectHandle.Length; idx++)
        {
            EffectHandle effect_handle = m_EffectHandle[idx];
            if (effect_handle.m_PoolInfo == null)
            {
                return effect_handle;
            }
        }

        return null;
    }

    /// <summary>
    /// 一番過去に再生開始されたエフェクトを取得
    /// </summary>
    /// <param name="pool_info">エフェクト種類指定</param>
    /// <returns></returns>
    private EffectHandle getOldEffectHandle(PoolInfo pool_info)
    {
        int frame_counter_max = -1;
        EffectHandle effect_handle = null;
        for (int idx = 0; idx < m_EffectHandle.Length; idx++)
        {
            EffectHandle wrk_effect_handle = m_EffectHandle[idx];
            if (wrk_effect_handle.m_PoolInfo == pool_info)
            {
                if (wrk_effect_handle.m_FrameCounter > frame_counter_max)
                {
                    effect_handle = wrk_effect_handle;
                }
            }
        }

        return effect_handle;
    }

    private void unuseEffectHandle(EffectHandle effect_handle)
    {
        if (effect_handle.m_PoolInfo != null)
        {
            if (effect_handle.m_ParticleSystem != null)
            {
                effect_handle.m_ParticleSystem.gameObject.SetActive(false);
            }
            effect_handle.m_PoolInfo.destroyInstancedParticleSystem(effect_handle.m_ParticleSystem);

            effect_handle.m_ParticleSystem = null;
            effect_handle.m_PoolInfo = null;
            effect_handle.m_HandleName = null;
            effect_handle.m_FrameCounter = 0;
        }
    }

    private void updateManager(float delta_time)
    {
        // エフェクト終了チェック
        for (int idx = 0; idx < m_EffectHandle.Length; idx++)
        {
            EffectHandle effect_handle = m_EffectHandle[idx];
            if (effect_handle.m_PoolInfo != null)
            {
                ParticleSystem particle_system = effect_handle.m_ParticleSystem;
                if (particle_system != null)
                {
                    effect_handle.m_FrameCounter++;
                    if (particle_system.main.loop == false)
                    {
                        if (particle_system.isPlaying == false)
                        {
                            unuseEffectHandle(effect_handle);
                        }
                    }
                }
                else
                {
                    // ライド先の破棄に巻き込まれてエフェクトも破棄されている場合がある.
                    unuseEffectHandle(effect_handle);
                }
            }
        }
    }


    #region <PoolInfo>
    /// <summary>
    /// エフェクトのプール情報
    /// </summary>
    private class PoolInfo
    {
        private ParticleSystem m_KeyParticleSystem; // エフェクト検索のキー（インスタンスしたゲームオブジェクトの ParticleSystem はプレハブのものとは異なっている可能性があるのでプレハブのものを保存）
        private ParticleSystem[] m_InstancedParticleSystem; // プールしたエフェクト
        private bool[] m_IsUsing;   // プールしたエフェクトの使用状況
        private int m_EffectCount;  // この種類のエフェクトの現在の存在数
        private int m_EffectMax;    // この種類のエフェクトの同時存在数の上限
        private bool m_IsDestroy = false;

        public struct DefaultInfo
        {
            //			public float m_SizeScale;	// 大きさ
            public float m_SpeedScale;  // 再生速度
#if REPLACE_SHADER
            public Material[] m_Materials;  // マテリアル(AssetBundle時にシェーダーが外れる現象対策)
#endif //REPLACE_SHADER
        }

        private TemplateList<DefaultInfo> m_DefaultInfo = null; // プレハブのデフォルト値を保存
        private Vector3 m_InitialLocalPosition; // 初期ローカル位置
        private Quaternion m_InitialLocalRotation;  // 初期ローカル回転

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="key_particle_system"></param>
        /// <param name="pool_num"></param>
        /// <param name="effect_max">同時に再生できる最大数</param>
        public PoolInfo(ParticleSystem key_particle_system, int pool_num, int effect_max)
        {
            if (pool_num > 0)
            {
                if (effect_max < pool_num)
                {
                    effect_max = pool_num;
                }
            }

            m_KeyParticleSystem = key_particle_system;
            m_InstancedParticleSystem = new ParticleSystem[pool_num];
            m_EffectCount = 0;
            m_EffectMax = effect_max;
            m_IsUsing = new bool[pool_num];

            // 拡大縮小情報を生成
            m_DefaultInfo = buildDefaultInfo(m_KeyParticleSystem);

            m_InitialLocalPosition = m_KeyParticleSystem.transform.localPosition;
            m_InitialLocalRotation = m_KeyParticleSystem.transform.localRotation;

            // エフェクトを生成しプール
            if (pool_num > 0)
            {
                for (int idx = 0; idx < m_InstancedParticleSystem.Length; idx++)
                {
                    GameObject game_object = GameObject.Instantiate(key_particle_system.gameObject);
                    game_object.transform.parent = EffectManager.Instance.transform;
                    game_object.SetActive(false);
                    ParticleSystem intanced_particle_system = game_object.GetComponent<ParticleSystem>();
                    m_InstancedParticleSystem[idx] = intanced_particle_system;
                    m_IsUsing[idx] = false;
                }
            }
        }

        public void destroyPoolInfo()
        {
            m_IsDestroy = true;
            for (int idx = 0; idx < m_InstancedParticleSystem.Length; idx++)
            {
                if (m_InstancedParticleSystem[idx] != null)
                {
                    GameObject.Destroy(m_InstancedParticleSystem[idx].gameObject);
                    m_InstancedParticleSystem[idx] = null;
                }
            }
        }

        /// <summary>
        /// キーを取得
        /// </summary>
        /// <returns></returns>
        public ParticleSystem getKey()
        {
            return m_KeyParticleSystem;
        }

        /// <summary>
        /// インスタンスを生成（プールにあればそれを再利用）
        /// </summary>
        /// <returns></returns>
        public ParticleSystem instanceParticleSystem()
        {
            for (int idx = 0; idx < m_IsUsing.Length; idx++)
            {
                if (m_IsUsing[idx] == false)
                {
                    // プールされたエフェクトを再利用.
                    m_IsUsing[idx] = true;
                    ParticleSystem pooled_particle_system = m_InstancedParticleSystem[idx];
                    m_EffectCount++;
                    return pooled_particle_system;
                }
            }

            // 新たにインスタンスを生成.
            GameObject instance_game_object = GameObject.Instantiate(m_KeyParticleSystem.gameObject);
            ParticleSystem instance_particle_system = instance_game_object.GetComponent<ParticleSystem>();

            m_EffectCount++;
            return instance_particle_system;
        }

        /// <summary>
        /// インスタンスを破棄（プール由来のものはプールへ戻す）
        /// </summary>
        /// <param name="instance_particle_system"></param>
        public void destroyInstancedParticleSystem(ParticleSystem instance_particle_system)
        {
            if (m_IsDestroy)
            {
                destroyPoolInfo();
                return;
            }

            if (instance_particle_system != null)
            {
                for (int idx = 0; idx < m_IsUsing.Length; idx++)
                {
                    if (m_InstancedParticleSystem[idx] == instance_particle_system)
                    {
                        // プール由来のものはプールに戻す。
                        m_IsUsing[idx] = false;
                        m_InstancedParticleSystem[idx].gameObject.transform.parent = EffectManager.Instance.transform;
                        m_InstancedParticleSystem[idx].gameObject.SetActive(false);
                        m_EffectCount--;
                        return;
                    }
                }

                // プールされたものでなければ破棄.
                GameObject.Destroy(instance_particle_system.gameObject);
                m_EffectCount--;
            }
            else
            {
                // ライド先の破棄に巻き込まれてエフェクトも破棄されている場合はここに来る.
                for (int idx = 0; idx < m_IsUsing.Length; idx++)
                {
                    if (m_InstancedParticleSystem[idx] == null)
                    {
                        // プールのインスタンスを復元
                        GameObject game_object = GameObject.Instantiate(m_KeyParticleSystem.gameObject);
                        game_object.transform.parent = EffectManager.Instance.transform;
                        game_object.SetActive(false);
                        m_InstancedParticleSystem[idx] = game_object.GetComponent<ParticleSystem>();

                        m_IsUsing[idx] = false;
                        m_EffectCount--;
                    }
                }
            }
        }

        /// <summary>
        /// スケーリング情報を生成
        /// </summary>
        public static TemplateList<DefaultInfo> buildDefaultInfo(ParticleSystem particle_system)
        {
            TemplateList<DefaultInfo> ret_val = null;
            if (particle_system != null)
            {
                ret_val = new TemplateList<DefaultInfo>();
                buildDefaultInfoSub(particle_system.transform, ret_val);
            }
            return ret_val;
        }

        private static void buildDefaultInfoSub(Transform trans, TemplateList<DefaultInfo> scale_info)
        {
            ParticleSystem particle_system = trans.GetComponent<ParticleSystem>();
            if (particle_system != null)
            {
                DefaultInfo default_info;
                //				default_info.m_SizeScale = particle_system.main.startSizeMultiplier;
                default_info.m_SpeedScale = particle_system.main.simulationSpeed;
#if DEBUG
                if (particle_system.main.scalingMode != ParticleSystemScalingMode.Shape)
                {
                    // ここのスケーリング処理は scalingMode が Shape の場合のみ対応しているのでそれ以外の時はどうなるか不明.
                    Debug.LogError("エフェクトデータの scalingMode 要確認：" + particle_system.gameObject.name);
                }
#endif

#if REPLACE_SHADER
                default_info.m_Materials = null;
                ParticleSystemRenderer particle_system_renderer = particle_system.GetComponent<ParticleSystemRenderer>();
                if (particle_system_renderer != null)
                {
                    bool is_replace = false;
                    for (int idx = 0; idx < particle_system_renderer.sharedMaterials.Length; idx++)
                    {
                        Material material = particle_system_renderer.sharedMaterials[idx];
                        if (material != null
                               && material.shader != null
                               && material.shader.isSupported == false
      )
                        {
                            is_replace = true;
                            break;
                        }
                    }

                    if (is_replace)
                    {
                        default_info.m_Materials = new Material[particle_system_renderer.sharedMaterials.Length];
                        for (int idx = 0; idx < default_info.m_Materials.Length; idx++)
                        {
                            Material base_material = particle_system_renderer.sharedMaterials[idx];
                            if (base_material != null)
                            {
                                Material material = new Material(base_material);

                                Shader shader = Shader.Find(base_material.shader.name);
                                if (shader != null)
                                {
                                    material.shader = shader;
                                }

                                default_info.m_Materials[idx] = material;
                            }
                        }
                    }
                }
#endif //REPLACE_SHADER

                scale_info.Add(default_info);
            }

            for (int idx = 0; idx < trans.childCount; idx++)
            {
                Transform child_trans = trans.GetChild(idx);
                buildDefaultInfoSub(child_trans, scale_info);
            }
        }

        /// <summary>
        /// エフェクトをスケーリング
        /// </summary>
        /// <param name="instance_game_object"></param>
        /// <param name="default_info"></param>
        /// <param name="scale"></param>
        public static void scaleEffect(GameObject instance_game_object, TemplateList<DefaultInfo> default_info, float size_scale, float speed_scale, int layer)
        {
            ParticleSystem particle_system = instance_game_object.GetComponent<ParticleSystem>();

            float wrk_scale = 1.0f;
            if (size_scale >= 0.0f)
            {
                wrk_scale = size_scale;
            }
            else
            {
                // 親階層のスケールの影響を受けないモード
                float parent_scale = 1.0f;
                if (particle_system.transform.parent != null)
                {
                    Vector3 parent_scale3 = particle_system.transform.parent.lossyScale;
                    parent_scale = (parent_scale3.x + parent_scale3.y + parent_scale3.z) / 3.0f;
                }

                wrk_scale = -size_scale / parent_scale;
            }

            int scale_info_index = 0;
            scaleEffectSub(particle_system.transform, default_info, speed_scale, layer, ref scale_info_index);

            particle_system.transform.localScale = new Vector3(wrk_scale, wrk_scale, wrk_scale);
        }

        private static void scaleEffectSub(Transform trans, TemplateList<DefaultInfo> default_infos/*, float size_scale*/, float speed_scale, int layer, ref int scale_info_index)
        {
            trans.gameObject.layer = layer;

            ParticleSystem particle_system = trans.GetComponent<ParticleSystem>();
            if (particle_system != null)
            {
                ParticleSystem.MainModule main_module = particle_system.main;

                DefaultInfo default_info = default_infos[scale_info_index++];

                //main_module.startSizeMultiplier = size_scale * scale_info.m_SizeScale;
                main_module.scalingMode = ParticleSystemScalingMode.Hierarchy;
                main_module.simulationSpeed = speed_scale * default_info.m_SpeedScale;
#if REPLACE_SHADER
                if (default_info.m_Materials != null)
                {
                    ParticleSystemRenderer particle_system_renderer = particle_system.GetComponent<ParticleSystemRenderer>();
                    if (particle_system_renderer != null)
                    {
                        particle_system_renderer.materials = default_info.m_Materials;
                    }
                }
#endif //REPLACE_SHADER
            }

            for (int idx = 0; idx < trans.childCount; idx++)
            {
                Transform child_trans = trans.GetChild(idx);
                scaleEffectSub(child_trans, default_infos/*, size_scale*/, speed_scale, layer, ref scale_info_index);
            }
        }

        public void initialLocalPosture(GameObject instance_game_object, float size_scale, float speed_scale, int layer)
        {
            scaleEffect(instance_game_object, m_DefaultInfo, size_scale, speed_scale, layer);
            instance_game_object.transform.localPosition = m_InitialLocalPosition;
            instance_game_object.transform.localRotation = m_InitialLocalRotation;
        }

        public bool isMaxEffect()
        {
            bool ret_val = (m_EffectMax > 0 && m_EffectCount >= m_EffectMax);
            return ret_val;
        }
    }
    #endregion //<PoolInfo>


    #region <PoolInfoManager>
    /// <summary>
    /// エフェクトのプール情報の管理
    /// </summary>
    private class PoolInfoManager
    {
        private PoolInfo[] m_PoolInfos = new PoolInfo[128]; // プール管理
        private int m_PoolInfoCount = 0;    // プール使用個数.

        public PoolInfo searchPoolInfo(ParticleSystem key_particle_system)
        {
            for (int idx = 0; idx < m_PoolInfoCount; idx++)
            {
                PoolInfo pool_info = m_PoolInfos[idx];
                if (pool_info.getKey() == key_particle_system)
                {
                    return pool_info;
                }
            }

            return null;
        }

        public void createPoolInfo(ParticleSystem key_particle_system, int pool_num, int effect_max)
        {
            PoolInfo pool_info = searchPoolInfo(key_particle_system);

            if (pool_info == null)
            {
                if (m_PoolInfoCount < m_PoolInfos.Length)
                {
                    pool_info = new PoolInfo(key_particle_system, pool_num, effect_max);
                    m_PoolInfos[m_PoolInfoCount] = pool_info;
                    m_PoolInfoCount++;
                }
            }
        }

        public PoolInfo usePoolInfo(ParticleSystem key_particle_system)
        {
            PoolInfo pool_info = searchPoolInfo(key_particle_system);

            if (pool_info == null)
            {
                if (m_PoolInfoCount < m_PoolInfos.Length)
                {
                    pool_info = new PoolInfo(key_particle_system, 0, 0);
                    m_PoolInfos[m_PoolInfoCount] = pool_info;
                    m_PoolInfoCount++;
                }
            }

            return pool_info;
        }

        public void unusePoolInfo(ParticleSystem key_particle_system)
        {
            for (int idx = 0; idx < m_PoolInfoCount; idx++)
            {
                PoolInfo pool_info = m_PoolInfos[idx];
                if (pool_info.getKey() == key_particle_system)
                {
                    m_PoolInfos[idx].destroyPoolInfo();
                    m_PoolInfos[idx] = null;

                    m_PoolInfoCount--;

                    if (idx < m_PoolInfoCount)
                    {
                        m_PoolInfos[idx] = m_PoolInfos[m_PoolInfoCount];
                        m_PoolInfos[m_PoolInfoCount] = null;
                    }

                    return;
                }
            }
        }

        public void unusePoolInfoAll()
        {
            for (int idx = 0; idx < m_PoolInfoCount; idx++)
            {
                m_PoolInfos[idx].destroyPoolInfo();
                m_PoolInfos[idx] = null;
            }
            m_PoolInfoCount = 0;
        }
    }
    #endregion //<PoolInfoManager>

    /// <summary>
    /// 再生されたエフェクトの管理
    /// </summary>
    private class EffectHandle
    {
        public ParticleSystem m_ParticleSystem; // 再生中のエフェクト.
        public PoolInfo m_PoolInfo; // どのプールからインスタンスが生成されたか（プールされていないエフェクトもプール個数ゼロのプールがあるとしてそこからインスタンスを生成する）
        public string m_HandleName; // エフェクトグループ名（エフェクトの停止などに使用）
        public int m_FrameCounter; // 再生開始してから何フレーム経過したか
    }
}

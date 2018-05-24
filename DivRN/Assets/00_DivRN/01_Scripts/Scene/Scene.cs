using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using M4u;
using System.Text;

public interface IScene
{
    void OnInitialized();
    SceneType GetSceneType();
}

public class Scene<T> : SingletonComponent<T>, IScene
    where T : Component
{
    protected override void Awake()
    {
        base.Awake();

        InitlaizePrefab();
    }

    protected override void Start()
    {
        base.Start();

        PrepareSEs();
    }

    protected virtual void Update()
    {
        if (UnitIconImageProvider.HasInstance)
        {
            UnitIconImageProvider.Instance.Tick();
        }
    }

    protected virtual void OnEnable()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("SetCurrentScene:" + gameObject.name);
#endif
        if (SceneCommon.Instance != null)
        {
            SceneCommon.Instance.CurrentScene = this;
        }
    }

    public virtual void OnInitialized()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("CALL Scene#OnInitialized:" + gameObject.name);
#endif
    }

    public virtual SceneType GetSceneType()
    {
        return gameObject.name.Replace("_", "").ToEnum<SceneType>();
    }

    protected virtual void InitlaizePrefab()
    {
        JsonMapperSetting.Setup();

#if BUILD_TYPE_DEBUG
        if (DebugOption.Instance == null)
        {
            Instantiate(Resources.Load("Prefab/DebugOption") as GameObject);
        }
#endif

#if UNITY_EDITOR && BUILD_TYPE_DEBUG
        if (DebugOptionInGame.Instance == null)
        {
            Instantiate(Resources.Load("Prefab/DebugOptionInGame") as GameObject);
        }
#endif
        if (UserDataAdmin.Instance == null)
        {
            Instantiate(Resources.Load("Prefab/UserDataAdmin") as GameObject);
        }
        if (ResidentManager.Instance == null)
        {
            Instantiate(Resources.Load("Prefab/ResidentObject") as GameObject);
        }
        if (SceneCommon.Instance == null)
        {
            Instantiate(Resources.Load("Prefab/SceneCommon") as GameObject);
        }
        if (SoundManager.Instance == null)
        {
            Instantiate(Resources.Load("Prefab/SoundManager") as GameObject);
        }
        if (StoreManager.Instance == null)
        {
            Instantiate(Resources.Load("Prefab/StoreManager") as GameObject);
        }
        if (TimeManager.Instance == null)
        {
            Instantiate(Resources.Load("Prefab/TimeManager") as GameObject);
        }

        if (GetSceneType() != SceneType.SceneSplash)
        {
            if (GameObject.FindGameObjectWithTag("ScreenMask") == null)
            {
                Instantiate(Resources.Load("Prefab/ScreenMask") as GameObject);
            }
        }

        if (ReplaceAssetManager.Instance == null)
        {
            Instantiate(Resources.Load("Prefab/ReplaceAssetManager") as GameObject);
        }

        if (AndroidBackKeyManager.Instance == null)
        {
            Instantiate(Resources.Load("Prefab/AndroidBackKeyManager") as GameObject);
        }

        if (WebResource.Instance == null)
        {
            Instantiate(Resources.Load("Prefab/WebResource") as GameObject);
        }

        if (UnitIconImageProvider.Instance == null)
        {
            Instantiate(Resources.Load("Prefab/UnitIconImageProvider") as GameObject);
        }

        if (DebugLogger.Instance == null)
        {
            Instantiate(Resources.Load("Prefab/Debug/DebugLogger") as GameObject);
        }
    }

    protected virtual void PrepareSEs()
    {
        var sceneType = GetSceneType();
        var map = new Dictionary<SceneType, List<SEID>>
        {
            { SceneType.SceneSplash, SESettings.SceneSplashSEs},
            { SceneType.SceneTitle, SESettings.SceneTitleSEs},
            { SceneType.SceneMainMenu, SESettings.SceneMainMenuSEs},
            { SceneType.SceneQuest2, SESettings.SceneQuest2SEs},
        };

        if (!map.ContainsKey(sceneType))
        {
            return;
        }

        SoundManager.Instance.RemoveAllSEs();

        var ses = map[sceneType];

        foreach (var seid in ses)
        {
            SoundManager.Instance.PrepareSE(seid);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ResourceType
{
    NONE,
    Common,
    Menu,
    Battle,
    Title
}

public class ResourceManager
{
    private const int MAX_SHEET_NO_COMMON = 1;
    private const int MAX_SHEET_NO_BATTLE = 2;
    private const int MAX_SHEET_NO_MENU = 3;
    private const int MAX_SHEET_NO_TITLE = 1;

    private static Dictionary<ResourceType, List<Sprite>> cache = new Dictionary<ResourceType, List<Sprite>>();

    private static Dictionary<ResourceType, string> ResourceReferenceName = new Dictionary<ResourceType, string>();

    private static ResourceManager instance = new ResourceManager();

    public static ResourceManager Instance
    {
        get
        {
            return instance;
        }
    }

    private ResourceManager()
    {
        foreach (ResourceType t in Enum.GetValues(typeof(ResourceType)))
        {
            ResourceReferenceName[t] = string.Format("SpriteReference/{0}SpriteReference", t);
        }
    }

    private int GetMaxSheetNo(ResourceType t)
    {
        switch (t)
        {
            case ResourceType.Battle:
                return MAX_SHEET_NO_BATTLE;
            case ResourceType.Menu:
                return MAX_SHEET_NO_MENU;
            case ResourceType.Title:
                return MAX_SHEET_NO_TITLE;
            case ResourceType.Common:
                return MAX_SHEET_NO_COMMON;
        }

        return 0;
    }

    private ResourceType GetResourceType(SceneType sceneType)
    {
        switch (sceneType)
        {
            case SceneType.SceneOutset:
            case SceneType.SceneSplash:
            case SceneType.SceneTitle:
                return ResourceType.Title;

            case SceneType.SceneQuest2:
                return ResourceType.Battle;

            case SceneType.SceneMainMenu:
                return ResourceType.Menu;
        }

        return ResourceType.NONE;
    }

    public void ClearCacheExcept(ResourceType resourceType)
    {
        foreach (ResourceType t in Enum.GetValues(typeof(ResourceType)))
        {
            if (t == resourceType)
            {
                continue;
            }

            if (t == ResourceType.Common)
            {
                continue;
            }

            cache.Remove(t);
        }
    }

    public Sprite Load(string spriteName)
    {
        if (SceneCommon.HasInstance == false)
        {
            return null;
        }

        ResourceType resourceType = GetResourceType(SceneCommon.Instance.CurrentSceneType);

        Sprite result = Load(spriteName, resourceType);

        if (result != null)
        {
            return result;
        }

        if (resourceType != ResourceType.Common)
        {
            result = Load(spriteName, ResourceType.Common);
        }

        if (result != null)
        {
#if BUILD_TYPE_DEBUG
            Debug.LogWarning("ResourceTypeCommon:" + spriteName + " sceneType:" + SceneCommon.Instance.CurrentSceneType);
#endif
            return result;
        }

#if BUILD_TYPE_DEBUG
        Debug.LogError("NotFoundResource:" + spriteName + " sceneType:" + SceneCommon.Instance.CurrentSceneType);
#endif
        return null;
    }

    public List<Sprite> LoadAll(ResourceType resourceType)
    {
        if (cache.ContainsKey(resourceType) == false)
        {
            List<Sprite> list = Resources.Load<SpriteReference>(ResourceReferenceName[resourceType]).sprites;
            cache.Add(resourceType, list);
        }

        return cache[resourceType];
    }

    public Sprite Load(string spriteName, ResourceType resourceType)
    {
#if BUILD_TYPE_DEBUG
        if (spriteName.Contains("/"))
        {
            Debug.LogError("Name is Path:" + spriteName + " sceneType:" + resourceType);
            return null;
        }
#endif

        Sprite result = LoadAll(resourceType).FirstOrDefault(sprite => sprite.name.Equals(spriteName));

        return result;
    }
}

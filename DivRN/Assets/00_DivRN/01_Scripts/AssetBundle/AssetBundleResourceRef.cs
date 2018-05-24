using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssetBundleResourceRef : ScriptableObject
{

    public ResourceType resourceType;

    public List<GameObject> prefabList;

    public GameObject FindPrefab(string prefabName)
    {
        return prefabList.FirstOrDefault(p => p.name.Equals(prefabName));
    }

}

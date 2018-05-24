using System.Collections.Generic;

using UnityEngine;

public class SELoader
{
    class ResourceLoading
    {
        public ResourceRequest resourceRequest = null;
        public System.Action<AudioClip> callback = null;
    }

    private bool _isAssetBundle = false;
    private bool _isAsync = false;

    private List<ResourceLoading> _resourceLoadings = new List<ResourceLoading>();

    public void Tick()
    {
        if (_resourceLoadings.Count == 0)
            return;

        foreach (var resourceLoading in _resourceLoadings)
        {
            if (!resourceLoading.resourceRequest.isDone)
                continue;
            if (resourceLoading.callback == null)
                continue;

            var clip = resourceLoading.resourceRequest.asset as AudioClip;
            resourceLoading.callback(clip);
            resourceLoading.callback = null;
        }
    }

    // callbackの引数はnullable
    public void Load(string dataPath, System.Action<AudioClip> callback)
    {
        if (_isAssetBundle)
            LoadFromAssetBundle(dataPath, callback);
        else if (_isAsync)
            LoadAsync(dataPath, callback);
        else
            LoadSync(dataPath, callback);
    }

    private void LoadSync(string dataPath, System.Action<AudioClip> callback)
    {
        var clip = Resources.Load(dataPath) as AudioClip;
        callback(clip);
    }

    private void LoadAsync(string dataPath, System.Action<AudioClip> callback)
    {
        var resourceLoading = new ResourceLoading
        {
            resourceRequest = Resources.LoadAsync(dataPath),
            callback = callback
        };

        if (resourceLoading.resourceRequest == null)
            return;

        _resourceLoadings.Add(resourceLoading);
    }

    private void LoadFromAssetBundle(string dataPath, System.Action<AudioClip> callback)
    {
        AssetBundler.Create().SetAsAudioClip("pack_se", dataPath,
        (clip) =>
        {
            if (clip == null)
            {
                Debug.LogError("NOT_FOUND_SEID from assetbundle (will try to load from resources) path:" + dataPath);
                LoadSync(dataPath, callback);
            }
            else
            {
                callback(clip);
            }
        }).Load();
    }
}
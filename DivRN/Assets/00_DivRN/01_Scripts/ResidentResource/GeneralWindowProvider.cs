using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneralWindowProvider
{
    private static GeneralWindowProvider m_instance = null;
    public static GeneralWindowProvider Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new GeneralWindowProvider();
            }

            return m_instance;
        }
    }


    /// <summary>
    /// タイトルで汎用ウィンドウのAssetBundle読み込みを行う.
    /// </summary>
    /// <param name="multiplier"></param>
    /// <returns></returns>
    public IEnumerator PreLoad(AssetBundlerMultiplier multiplier)
    {
        if (multiplier != null)
        {
            var assetBundlePathList = MasterFinder<MasterDataAssetBundlePath>.Instance.
                                            SelectWhere(" where category = ? ", MasterDataDefineLabel.ASSETBUNDLE_CATEGORY.GENERALWINDOW);
            yield return null;

            for (int i = 0; i < assetBundlePathList.Count; i++)
            {
                multiplier.Add(
                        AssetBundler.Create().Set(assetBundlePathList[i].name, (result) =>
                        {
#if BUILD_TYPE_DEBUG
                            if (result.AssetBundle != null)
                            {
                                Debug.Log("GeneralWindowProvider:asset download success:" + result.AssetBundle.name);
                            }
#endif //BUILD_TYPE_DEBUG
                        }
                        , null
                        )
                    );

                yield return null;
            }
        }

        yield return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AreaBackgroundProvider
{
    private static readonly string AREA_MAPBG_NAME = "areabackground_";

    private static AreaBackgroundProvider m_instance = null;
    public static AreaBackgroundProvider Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new AreaBackgroundProvider();
            }

            return m_instance;
        }
    }


    /// <summary>
    /// タイトルでエリア背景のAssetBundle読み込みを行う.
    /// </summary>
    /// <param name="multiplier"></param>
    /// <returns></returns>
    public IEnumerator PreLoadAreaBackground(AssetBundlerMultiplier multiplier)
    {
        if (multiplier != null)
        {
            var assetBundlePathList = MasterFinder<MasterDataAssetBundlePath>.Instance.
                                            SelectWhere(" where category = ? ", MasterDataDefineLabel.ASSETBUNDLE_CATEGORY.TITLE);
            yield return null;

            for (int i = 0; i < assetBundlePathList.Count; i++)
            {
                var name = assetBundlePathList[i].name.ToLower();
                if (name.Contains(AREA_MAPBG_NAME) == false) continue;

                multiplier.Add(
                        AssetBundler.Create().Set(assetBundlePathList[i].name, (result) =>
                       {
#if BUILD_TYPE_DEBUG
                           if (result.AssetBundle != null)
                           {
                               Debug.Log("AreaBackgroundProvider:asset download success:" + result.AssetBundle.name);
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

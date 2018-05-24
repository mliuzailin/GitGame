using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;
using DG.Tweening;

public class UnitResultLink : UnitResultBase
{
    private static readonly string AnimationName = "unit_result_link";

    public AssetAutoSetCharaMesh Base = null;
    public AssetAutoSetCharaMesh Link = null;

    private void Update()
    {
        // TODO : AssetAutoSetCharaMeshから処理終了コールバックを受け取る形にしたい
        if (!m_Ready)
            updateLoadWait();
    }

    public void Initialize(
        uint chara_base_id,
        uint chara_link_id,
        System.Action callback)
    {
        Base.SetCharaID(chara_base_id, true);
        Base.m_AssetBundleMeshScaleUp = true;
        Base.m_AssetBundleMeshPosition = true;
        Link.SetCharaID(chara_link_id, true);
        Link.m_AssetBundleMeshScaleUp = true;
        Link.m_AssetBundleMeshPosition = true;

        if (callback != null)
            callback();
    }

    public void Show(System.Action finish)
    {
        SoundUtil.PlaySE(SEID.VOICE_INGAME_MM_LINK_ON);
        PlayAnimation(AnimationName, () =>
        {
            if (finish != null)
                finish();
        });
    }


    private void updateLoadWait()
    {
        if (Base.Ready && Link.Ready)
        {
            m_Ready = true;
        }
    }
}

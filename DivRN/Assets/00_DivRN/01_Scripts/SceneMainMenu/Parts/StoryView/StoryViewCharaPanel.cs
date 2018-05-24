/**
 *  @file   StoryViewCharaPanel.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/03/29
 */

using UnityEngine;
using System.Collections.Generic;
using M4u;
using DG.Tweening;

public class StoryViewCharaPanel : MenuPartsBase
{
    public StoryViewCharaPanelContext Context = new StoryViewCharaPanelContext();

    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = Context;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// キャラ画像の設定
    /// </summary>
    /// <param name="unitID"></param>
    /// <param name="assetLoadCharaImageList"></param>
    public void SetUpCharImage(uint storyCharaID, List<AssetLoadStoryViewResource> assetLoadCharaImageList)
    {
        //-------------------------------------------
        // ユニット画像の設定
        //-------------------------------------------
        if (storyCharaID > 0)
        {
            AssetLoadStoryViewResource assetResource = assetLoadCharaImageList.Find(v => v.m_fix_id == storyCharaID
                                                             && v.Type == AssetLoadStoryViewResource.ResourceType.CHARA);
            Context.CharaTexture = (assetResource != null) ? assetResource.GetTexture() : null;
            Context.CharaTexture_mask = (assetResource != null) ? assetResource.GetTextureMask() : null;

            // UVの設定
            if (assetResource != null && assetResource.CharaMasterData != null)
            {
                MasterDataStoryChara charaMaster = assetResource.CharaMasterData;

                Context.CharaUV = new Rect(charaMaster.img_offset_x * 0.001f
                                            , charaMaster.img_offset_y * 0.001f
                                            , charaMaster.img_tiling * 0.001f
                                            , charaMaster.img_tiling * 0.001f);
            }
            else
            {
                Context.CharaUV = new Rect(0, 0, 1, 1);
            }
        }
    }

    /// <summary>
    /// 画面外に出る位置を設定する
    /// </summary>
    public void SetUpHidePosition()
    {
        RectTransform rect = GetComponent<RectTransform>();
        if (rect.pivot.x >= 0.5f)
        {
            Context.m_CharaHidePosition = rect.sizeDelta;
        }
        else
        {
            Context.m_CharaHidePosition = -rect.sizeDelta;
        }
    }

    /// <summary>
    /// スライドイン・アウトを行う
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="slideType"></param>
    /// <param name="duration"></param>
    /// <param name="key"></param>
    public void DoSlidePanel<T>(MasterDataDefineLabel.BoolType slideType, float duration, T key)
    {
        if (slideType == MasterDataDefineLabel.BoolType.ENABLE)
        {
            Context.CharaImagePosX = Context.m_CharaHidePosition.x;
            DOTween.To(() => Context.CharaImagePosX, (x) => Context.CharaImagePosX = x, 0, duration)
            .SetId(key)
            .SetEase(Ease.Linear);
        }
        else if (slideType == MasterDataDefineLabel.BoolType.DISABLE)
        {
            Context.CharaImagePosX = 0;
            DOTween.To(() => Context.CharaImagePosX, (x) => Context.CharaImagePosX = x, Context.m_CharaHidePosition.x, duration)
            .SetId(key)
            .SetEase(Ease.Linear);
        }
    }
}

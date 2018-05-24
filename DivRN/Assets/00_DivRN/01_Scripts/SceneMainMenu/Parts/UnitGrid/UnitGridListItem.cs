/**
 * 	@file	UnitGridListItem.cs
 *	@brief	ユニット一覧のリストアイテム
 *	@author Developer
 *	@date	2016/10/31
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ServerDataDefine;

public class UnitGridListItem : ListItem<UnitGridContext>
{
    [SerializeField]
    private Image m_iconImage;

    private RectTransform m_recttransform = null;

    void Awake()
    {

    }

    public static string UnitListItemGameObjectNameFormat
    {
        get
        {
            return "UnitListItem_fix_id:{0:D4}:{1:D}";
        }
    }

    void Start()
    {
        SetModel(Context.model);
        m_recttransform = GetRoot().GetComponent<RectTransform>();

        if (TutorialManager.IsExists)
        {
            gameObject.name = string.Format(UnitListItemGameObjectNameFormat,
                                            Context.CharaMasterData.fix_id,
                                            Context.UnitData.unique_id);
        }

        m_listItemModel.OnPositionUpdated += position =>
        {
            m_recttransform.anchoredPosition = position;
        };

        m_iconImage.sprite = Context.UnitImage;
        Context.OnIconImageUpdated += sprite =>
        {
            m_iconImage.sprite = sprite;
        };


        Context.OnUpdateGameObjectName += () =>
        {
            if (TutorialManager.IsExists)
            {
                gameObject.name = string.Format(UnitListItemGameObjectNameFormat,
                                                Context.CharaMasterData.fix_id,
                                                Context.UnitData.unique_id);
            }
        };

        // TODO : 演出あるならしかるべき場所に移動
        m_listItemModel.Appear();
        m_listItemModel.SkipAppearing();

        m_listItemModel.Start();
    }

    void resetM4u(Transform node)
    {
        {
            var cc = node.GetComponents<M4u.M4uImageBinding>();
            foreach (var m4 in cc)
            {
                m4.Start();
            }
        }
        {
            var cc = node.GetComponents<M4u.M4uEnableOnceBinding>();
            foreach (var m4 in cc)
            {
                m4.Start();
            }
        }
        {
            var cc = node.GetComponents<M4u.M4uActiveBinding>();
            foreach (var m4 in cc)
            {
                m4.Start();
            }
        }
        {
            var cc = node.GetComponents<M4u.M4uColorBinding>();
            foreach (var m4 in cc)
            {
                m4.Start();
            }
        }
        {
            var cc = node.GetComponents<M4u.M4uTextBinding>();
            foreach (var m4 in cc)
            {
                m4.Start();
            }
        }
        {
            var cc = node.GetComponents<M4u.M4uTMPTextBinding>();
            foreach (var m4 in cc)
            {
                m4.Start();
            }
        }
        {
            var cc = node.GetComponents<M4u.M4uSelectableEnableBinding>();
            foreach (var m4 in cc)
            {
                m4.Start();
            }
        }
        for (int i = 0; i < node.Children().Length; i++)
        {
            resetM4u(node.GetChild(i));
        }
    }

    public void Refresh()
    {
        Start();
        resetM4u(transform);
    }

}

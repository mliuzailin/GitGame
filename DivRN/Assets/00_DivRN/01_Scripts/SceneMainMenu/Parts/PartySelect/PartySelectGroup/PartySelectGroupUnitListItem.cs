/**
 *  @file   PartySelectGroupUnitListItem.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/02/14
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PartySelectGroupUnitListItem : ListItem<PartySelectGroupUnitContext>
{
    [SerializeField]
    private PartySelectGroupUnitListItemIconView m_icon;
    [SerializeField]
    private PartySelectGroupUnitListItemNameView m_name;

    private PartySelectGroupUnitListItemModel m_model = null;

    [SerializeField]
    Image m_nameBG = null;
    [SerializeField]
    Image m_unitIcon = null;
    [SerializeField]
    Button m_button = null;

	private bool m_iconShowFinish = false;
	public bool isIconShowFinish { get { return m_iconShowFinish; } }

    void Start()
    {
        m_model = Context.model;

        SetModel(m_model);

        m_model.OnShowedIcon += () =>
        {
            m_icon.Show(
                // onNext
                () =>
                {
                    m_model.ShowNextIcon();
                },
				// onFinish
				() =>
				{
					m_iconShowFinish = true;
				});
        };

        m_model.OnShowedName += () =>
        {
            m_name.Show(() =>
            {
                m_model.ShowNextName();
            });
        };

        m_model.OnUpdated += () =>
        {
            if (!m_icon.isShowed)
                return;

            if (m_model.isSelected)
                m_icon.ShowArrow();
            else
                m_icon.HideArrow();


            if (m_model.isSelected)
                m_name.Select();
        };

        // TODO : 演出あるならしかるべき場所に移動
        m_listItemModel.Appear();
        m_listItemModel.SkipAppearing();

        m_model.ViewStarted();

        if (m_button != null)
        {
            // ボタンの選択画像を設定
            if (Context.IsLowerScreen)
            {
                m_button.targetGraphic = m_nameBG;
            }
            else
            {
                m_button.targetGraphic = m_unitIcon;
            }
        }
    }

    public void OnClick()
    {
        m_listItemModel.Click();
    }

	public void iconShowArrow()
	{
		m_icon.ShowArrow();
	}
}

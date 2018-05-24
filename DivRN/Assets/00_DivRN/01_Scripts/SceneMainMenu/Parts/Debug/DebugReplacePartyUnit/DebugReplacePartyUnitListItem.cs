/**
 *  @file   DebugReplacePartyUnitListItem.cs
 *  @brief  
 *  @author Developer
 *  @date   2016/12/12
 */

using UnityEngine;
using System.Collections;
using M4u;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DebugReplacePartyUnitListItem : ListItem<DebugReplacePartyUnitListItemContext>
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region ベースユニットのInputField
    public void OnValueChangedBaseUnitID(string value)
    {
        uint.TryParse(value, out Context.BaseUnitData.id);
    }

    public void OnValueChangedBaseUnitLevel(string value)
    {
        uint.TryParse(value, out Context.BaseUnitData.level);
    }

    public void OnValueChangedBaseUnitAddPow(string value)
    {
        uint.TryParse(value, out Context.BaseUnitData.add_pow);
    }

    public void OnValueChangedBaseUnitAddHP(string value)
    {
        uint.TryParse(value, out Context.BaseUnitData.add_hp);
    }

    public void OnValueChangedBaseUnitSkillLevel(string value)
    {
        uint.TryParse(value, out Context.BaseUnitData.limitbreak_lv);
    }

    public void OnEndEditBaseUnitID(string value)
    {
        MasterDataParamChara charaMasterData = MasterDataUtil.GetCharaParamFromDrawID(Context.BaseUnitData.id); // キャラクターのマスターデータを取得
        if (charaMasterData != null)
        {
            Context.BaseUnitData.id = charaMasterData.fix_id;
            Context.BaseUnitName = charaMasterData.name; // 名前

            // アイコン
            UnitIconImageProvider.Instance.Get(
                charaMasterData.fix_id,
                sprite =>
                {
                    Context.BaseUnitIcon = sprite;
                });
        }
        else
        {
            Context.BaseUnitData.id = 0;
            Context.BaseUnitName = "????"; // 名前

            // アイコン
            UnitIconImageProvider.Instance.GetEmpty(sprite =>
            {
                Context.BaseUnitIcon = sprite;
            });
        }
    }
    #endregion

    #region リンクユニットのInputField
    public void OnValueChangedLinkUnitID(string value)
    {
        uint.TryParse(value, out Context.LinkUnitData.id);
    }

    public void OnValueChangedLinkUnitLevel(string value)
    {
        uint.TryParse(value, out Context.LinkUnitData.level);
    }

    public void OnValueChangedLinkUnitAddPow(string value)
    {
        uint.TryParse(value, out Context.LinkUnitData.add_pow);
    }

    public void OnValueChangedLinkUnitAddHP(string value)
    {
        uint.TryParse(value, out Context.LinkUnitData.add_hp);
    }

    public void OnValueChangedLinkUnitPoint(string value)
    {
        uint.TryParse(value, out Context.LinkUnitData.link_point);
        Context.LinkUnitData.link_point *= 10;
    }

    public void OnEndEditLinkUnitID(string value)
    {
        MasterDataParamChara charaMasterData = MasterDataUtil.GetCharaParamFromDrawID(Context.LinkUnitData.id); // キャラクターのマスターデータを取得
        if (charaMasterData != null)
        {
            Context.LinkUnitData.id = charaMasterData.fix_id;
            Context.LinkUnitName = charaMasterData.name; // 名前

            // アイコン
            UnitIconImageProvider.Instance.Get(
                charaMasterData.fix_id,
                sprite =>
                {
                    Context.LinkUnitIcon = sprite;
                });
        }
        else
        {
            Context.LinkUnitData.id = 0;
            Context.LinkUnitName = "????"; // 名前     

            // アイコン
            UnitIconImageProvider.Instance.GetEmpty(sprite =>
            {
                Context.LinkUnitIcon = sprite;
            });
        }
    }

    #endregion

#if UNITY_EDITOR
    [CustomEditor(typeof(DebugReplacePartyUnitListItem))]
    public class DebugReplacePartyUnitListItemEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DebugReplacePartyUnitListItem item = (DebugReplacePartyUnitListItem)target;

            if (item.Context != null)
            {
                EditorGUILayout.LabelField("BaseUnit==========");
                EditorGUILayout.LabelField("ID", item.Context.BaseUnitData.id.ToString());
                EditorGUILayout.LabelField("Lv", item.Context.BaseUnitData.level.ToString());
                EditorGUILayout.LabelField("+ATK", item.Context.BaseUnitData.add_pow.ToString());
                EditorGUILayout.LabelField("+HP", item.Context.BaseUnitData.add_hp.ToString());
                EditorGUILayout.LabelField("SLv", item.Context.BaseUnitData.limitbreak_lv.ToString());

                EditorGUILayout.LabelField("LinkUnit==========");
                EditorGUILayout.LabelField("ID", item.Context.LinkUnitData.id.ToString());
                EditorGUILayout.LabelField("Lv", item.Context.LinkUnitData.level.ToString());
                EditorGUILayout.LabelField("+ATK", item.Context.LinkUnitData.add_pow.ToString());
                EditorGUILayout.LabelField("+HP", item.Context.LinkUnitData.add_hp.ToString());
                EditorGUILayout.LabelField("LINK", item.Context.LinkUnitData.link_point.ToString());
            }
        }
    }
#endif

}

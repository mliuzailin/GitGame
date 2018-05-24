using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEvolveListItem : ListItem<UnitEvolveContext>
{
    public AssetAutoSetCharaDetail CharaDetail = null;
    public GameObject NameRoot = null;
    public UnitEvolveTouchArea TouchArea = null;
    //public ButtonView SkillButton = null;

    private void Start()
    {
        if (CharaDetail != null)
        {
            CharaDetail.SetCharaTexture(Context.charaMaster, Context.charaTexture, true);
        }

        if (Context.IsViewNamePanel &&
             NameRoot != null)
        {
            UnitNamePanel panel = setupPrefab<UnitNamePanel>("Prefab/UnitNamePanel/UnitNamePanel", NameRoot);
            if (panel != null)
            {
                panel.setup(Context.charaMaster);
                panel.IconSelect = MainMenuUtil.GetElementCircleSprite(Context.charaMaster.element);
            }
        }

        if (TouchArea != null)
        {
            TouchArea.DidPointerEnter = OnMaterialPointerEnter;
            TouchArea.DidPointerExit = OnMaterialPointerExit;
        }
    }
    public T setupPrefab<T>(string _prefabName, GameObject _parent)
    {
        GameObject _tmpObj = Resources.Load(_prefabName) as GameObject;
        if (_tmpObj != null)
        {
            GameObject _newObj = Instantiate(_tmpObj);
            if (_newObj != null)
            {
                _newObj.transform.SetParent(_parent.transform, false);
                return _newObj.GetComponent<T>();
            }
        }
        return default(T);
    }
    public void OnMaterialPointerEnter()
    {
        Context.IsViewFloatWindow = true;
    }
    public void OnMaterialPointerExit()
    {
        Context.IsViewFloatWindow = false;
    }

    public void OnSelectSkill()
    {
        Context.DidSelectItem(Context);
    }
}

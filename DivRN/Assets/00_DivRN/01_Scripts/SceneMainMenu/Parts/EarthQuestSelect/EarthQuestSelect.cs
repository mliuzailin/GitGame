using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening;

public class EarthQuestSelect : MonoBehaviour
{
    public EarthSystem earthObject;
    public EarthQuestButton questButton;
    public RawImage BackGround;
    public float partsOffset = 4500;

    [System.Serializable]
    public class questData
    {
        public string name;
        public float point;
        public float height;
    };

    public questData[] questDataArray;
    public EarthQuestButton[] questButtonArray;

    private int m_SelectQuestIndex = -1;

    void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointDown(BaseEventData _data)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("OnPointDown:" + _data.currentInputModule.name);
#endif
    }

    public void OnDrag(BaseEventData _data)
    {
        if (!EarthQuestSelectFSM.Instance.ActiveStateName.Equals("QuestSelect"))
        {
            return;
        }

        PointerEventData point_data = (PointerEventData)_data;
        earthObject.AddRotate((float)(-point_data.delta.x));
#if BUILD_TYPE_DEBUG
        //		Debug.Log("OnDrag:" +point_data.delta.x);
#endif
    }

    public void OnSelectQuest(int _index)
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("OnSelectQuest:" + _index);
#endif
        m_SelectQuestIndex = _index;
        EarthQuestSelectFSM.Instance.SendFsmEvent("QUEST_SELECTED");
    }

    IEnumerator SceneInitWait()
    {
        while (SceneCommon.Instance.IsLoadingScene)
        {
            yield return null;
        }
        EarthQuestSelectFSM.Instance.SendFsmNextEvent();
    }

    public void Inisialized()
    {

        if (questButton != null)
        {
            questButtonArray = new EarthQuestButton[questDataArray.Length];
            int _index = 0;
            foreach (questData _data in questDataArray)
            {
                GameObject newButton = Instantiate(questButton.gameObject) as GameObject;
                newButton.transform.SetParent(transform.parent, false);
                UnityUtil.SetObjectEnabledOnce(newButton, true);

                Button _button = newButton.GetComponent<Button>();
                int tmpIndex = _index;
                _button.onClick.AddListener(() => { OnSelectQuest(tmpIndex); });
                earthObject.addParts(_button, _data.point + partsOffset, _data.height);

                EarthQuestButton earthQuestButton = newButton.GetComponent<EarthQuestButton>();
                earthQuestButton.Title = _data.name;

                questButtonArray[_index] = earthQuestButton;
                _index++;
            }
        }

        EarthQuestSelectFSM.Instance.SendFsmNextEvent();
    }

    IEnumerator moveQuestCenter()
    {
        float _move = questDataArray[m_SelectQuestIndex].point - earthObject.TotalMoveDistance;
        float _add = _move / 15.0f;
        int _count = 0;
        while (_count != 15)
        {
            earthObject.AddRotate(_add);
            _count++;
            yield return null;
        }
        EarthQuestSelectFSM.Instance.SendFsmNextEvent();
    }

    IEnumerator zoomCamera()
    {
        earthObject.setZoomTarget(questButtonArray[m_SelectQuestIndex].GetComponent<Button>());
        while (!earthObject.updateZoomCamera())
        {
            yield return null;
        }

        earthObject.setRenderTarget(true);
        UnityUtil.SetObjectEnabledOnce(BackGround.gameObject, true);

        EarthQuestSelectFSM.Instance.SendFsmNextEvent();
    }

    public void switchPage()
    {
        DOTween.ToAlpha(
            () => BackGround.color,
            color => BackGround.color = color,
            0f,
            0.5f
            );
        if (SceneNewQuestSelectTest.HasInstance)
        {
            //			GameObject _areaObj = SceneNewQuestSelectTest.Instance.areaQuestSelect.gameObject;
            //			UnityUtil.SetObjectEnabledOnce(_areaObj, true);
        }
    }

    public void startChangeProduct()
    {
    }

    public void updateChangeProduct()
    {

    }
}

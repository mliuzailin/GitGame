using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TutorialDebugOption : SingletonComponent<TutorialDebugOption>
{
    public bool skipMovie;
    public string eventName;


#if UNITY_EDITOR

    [CustomEditor(typeof(TutorialDebugOption))]
    public class TutorialDebugOptionSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayoutOption option = GUILayout.Width(300f);
            DrawDefaultInspector();
            TutorialDebugOption instance = target as TutorialDebugOption;
            if (GUILayout.Button("Carousel_Edit", option))
            {
                TutorialManager.Instance.OnShowCarousel_UnitEdit();
            }
            if (GUILayout.Button("Carousel_BuildUp", option))
            {
                TutorialManager.Instance.OnShowCarousel_UnitBuildUp();
            }
            if (GUILayout.Button("Carousel_Normal02", option))
            {
                TutorialManager.Instance.OnShowCarousel_Scratch();
            }
            if (GUILayout.Button("GoNextMainMenu", option))
            {
                MainMenuManagerFSM.Instance.SendFsmNextEvent();
            }
            if (GUILayout.Button("FinishStory", option))
            {
                TutorialFSM.Instance.SendEvent_FinishStory();
            }
            if (GUILayout.Button("FinishBattle", option))
            {
                TutorialFSM.Instance.SendEvent_FinishBattle();
            }
            if (GUILayout.Button("FinishQuestResult", option))
            {
                TutorialFSM.Instance.SendEvent_FinishQuestResult();
            }
            if (GUILayout.Button("GoOutBattle", option))
            {
                // 中断復帰データの削除
                InGameUtil.RemoveLocalData();
            }
            if (GUILayout.Button("SendEventName", option))
            {
                TutorialFSM.Instance.SendFsmEvent(instance.eventName);
            }
            if (GUILayout.Button("Next", option))
            {
                TutorialFSM.Instance.SendFsmNextEvent();
            }
            if (GUILayout.Button("GetMaster", option))
            {
                MasterDataHero[] master = MasterFinder<MasterDataHero>.Instance.GetAll();
                Debug.LogError("COUNT:" + master.Length);
                foreach (MasterDataHero h in master)
                {
                    Debug.LogError("COUNT:" + h);
                }
            }
            if (GUILayout.Button("HeroDecide", option))
            {
                MasterDataHero[] master = MasterFinder<MasterDataHero>.Instance.GetAll();
                GameObject.FindObjectOfType<TutorialHeroSelect>().Decision(master.FirstOrDefault());
            }
            if (GUILayout.Button("Skip", option))
            {
                TutorialManager.Instance.Skip();
            }
        }
    }
#endif
}

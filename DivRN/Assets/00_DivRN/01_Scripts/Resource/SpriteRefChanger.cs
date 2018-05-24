using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;


public class SpriteRefChanger : MonoBehaviour
{
#if UNITY_EDITOR
    public List<Sprite> spriteList;
#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(SpriteRefChanger))]
public class SpriteRefChangerSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayoutOption option = GUILayout.Width(300f);
        DrawDefaultInspector();
        SpriteRefChanger instance = target as SpriteRefChanger;
        if (GUILayout.Button("Change!", option))
        {
            foreach (Image s in instance.GetComponentsInChildren<Image>(true))
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("s:" + s.gameObject.name);
#endif
                if (s.sprite == null)
                {
#if BUILD_TYPE_DEBUG
                    Debug.Log("NOT_SET_SPRITE:" + s.gameObject.name);
#endif
                    continue;
                }
#if BUILD_TYPE_DEBUG
                Debug.Log("SPRITE:" + s.sprite.name);
#endif

                Sprite sprite = instance.spriteList.FirstOrDefault(ss => ss.name.Equals(s.sprite.name));
                if (sprite != null)
                {
                    s.sprite = sprite;
                    EditorUtility.SetDirty(s);
                }
            }
            foreach (SpriteRenderer s in instance.GetComponentsInChildren<SpriteRenderer>(true))
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("s:" + s.gameObject.name);
#endif
                if (s.sprite == null)
                {
#if BUILD_TYPE_DEBUG
                    Debug.Log("NOT_SET_SPRITE:" + s.gameObject.name);
#endif
                    continue;
                }
#if BUILD_TYPE_DEBUG
                Debug.Log("SPRITE:" + s.sprite.name);
#endif

                Sprite sprite = instance.spriteList.FirstOrDefault(ss => ss.name.Equals(s.sprite.name));
                if (sprite != null)
                {
                    s.sprite = sprite;
                    EditorUtility.SetDirty(s);
                }
            }
        }
        if (GUILayout.Button("ChangeNull!", option))
        {
            foreach (Image s in instance.GetComponentsInChildren<Image>(true))
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("s:" + s.gameObject.name);
#endif
                if (s.sprite == null)
                {
#if BUILD_TYPE_DEBUG
                    Debug.Log("NOT_SET_SPRITE:" + s.gameObject.name);
#endif
                    return;
                }
#if BUILD_TYPE_DEBUG
                Debug.Log("SPRITE:" + s.sprite.name);
#endif

                Sprite sprite = instance.spriteList.FirstOrDefault(ss => ss.name.Equals(s.sprite.name));
                if (sprite != null)
                {
                    s.sprite = null;
                    EditorUtility.SetDirty(s);
                }
            }
            foreach (SpriteRenderer s in instance.GetComponentsInChildren<SpriteRenderer>(true))
            {
#if BUILD_TYPE_DEBUG
                Debug.Log("s:" + s.gameObject.name);
#endif
                if (s.sprite == null)
                {
#if BUILD_TYPE_DEBUG
                    Debug.Log("NOT_SET_SPRITE:" + s.gameObject.name);
#endif
                    return;
                }
#if BUILD_TYPE_DEBUG
                Debug.Log("SPRITE:" + s.sprite.name);
#endif

                Sprite sprite = instance.spriteList.FirstOrDefault(ss => ss.name.Equals(s.sprite.name));
                if (sprite != null)
                {
                    s.sprite = null;
                    EditorUtility.SetDirty(s);
                }
            }

        }
    }
}

#endif

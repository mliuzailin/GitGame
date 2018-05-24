using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.UI;

public class ImageMaterialAssigner : MonoBehaviour
{
    public List<Material> materialList;
    public List<SpriteReference> spriteReferenceList;
    public List<ImageMaterialAssigner> imageMaterialAssigners;


    public ImageMaterialAssigner FindImageMaterialAssigner(Sprite sprite)
    {
        return imageMaterialAssigners.FirstOrDefault(rr =>
            rr.spriteReferenceList.SelectMany(l => l.sprites).Contains(sprite));
    }

    public Material FindMaterial(Sprite sprite, ImageMaterialAssigner assigner = null)
    {
        if (assigner == null)
        {
            assigner = this;
        }

        SpriteReference r = assigner.spriteReferenceList.FirstOrDefault(rr => rr.sprites.Contains(sprite));
        if (r == null)
        {
            return null;
        }

        int index = assigner.spriteReferenceList.IndexOf(r);

        return assigner.materialList[index];
    }

#if UNITY_EDITOR


    [CustomEditor(typeof(ImageMaterialAssigner))]
    public class DebugOptionSettingsEditor : Editor
    {
        private void Up(List<SpriteRenderer> list)
        {
            foreach (SpriteRenderer image in list)
            {
                string path = AssetDatabase.GetAssetPath(image);
                if (image.sprite == null)
                {
                    Debug.Log("IM:" + image.gameObject.name + " ptath:" + path + " null");
                    continue;
                }
                Debug.Log("IM:" + image.gameObject.name + " ptath:" + path);

                ImageMaterialAssigner assigner = instance;

                if (!instance.imageMaterialAssigners.IsNullOrEmpty())
                {
                    assigner = instance.FindImageMaterialAssigner(image.sprite);
                }


                Material m = instance.FindMaterial(image.sprite, assigner);

                if (m != null)
                {
                    image.material = m;
                }

                EditorUtility.SetDirty(image.gameObject);
            }
        }
        private void Up(List<Image> list)
        {
            foreach (Image image in list)
            {
                string path = AssetDatabase.GetAssetPath(image);
                if (image.sprite == null)
                {
                    Debug.Log("IM:" + image.gameObject.name + " ptath:" + path + " null");
                    continue;
                }
                Debug.Log("IM:" + image.gameObject.name + " ptath:" + path);

                ImageMaterialAssigner assigner = instance;

                if (!instance.imageMaterialAssigners.IsNullOrEmpty())
                {
                    assigner = instance.FindImageMaterialAssigner(image.sprite);
                }


                Material m = instance.FindMaterial(image.sprite, assigner);
                if (m != null)
                {
                    image.material = m;
                }
                EditorUtility.SetDirty(image.gameObject);
            }
        }

        private ImageMaterialAssigner instance
        {
            get { return target as ImageMaterialAssigner; }
        }


        public override void OnInspectorGUI()
        {
            GUILayoutOption option = GUILayout.Width(300f);
            DrawDefaultInspector();
            ImageMaterialAssigner instance = target as ImageMaterialAssigner;


            if (GUILayout.Button("Execute_Image", option))
            {
                Debug.Log("EXECUTE");
                foreach (string p in EditorBuildSettings.scenes.Select(s => s.path))
                {
                    EditorSceneManager.OpenScene(p, OpenSceneMode.Additive);
                }

                List<Image> list = Resources.FindObjectsOfTypeAll<Image>().ToList();
                //                list.addran= UnityEngine.Object.FindObjectsOfType<Image>().ToList();
                Debug.Log("COUNT:" + list.Count);
                foreach (string assetPath in AssetDatabase.GetAllAssetPaths())
                {
                    foreach (Image image in AssetDatabase.LoadAllAssetsAtPath(assetPath).Where(p => p as Image)
                        .Where(a => a != null).Select(r => r as Image))
                    {
                        list.Add(image);
                    }
                }

                Up(list);
                EditorSceneManager.MarkAllScenesDirty();
            }

            if (GUILayout.Button("Execute_SpriteRenderer", option))
            {
                Debug.Log("EXECUTE");
                foreach (string p in EditorBuildSettings.scenes.Select(s => s.path))
                {
                    EditorSceneManager.OpenScene(p, OpenSceneMode.Additive);
                }

                List<SpriteRenderer> slist = Resources.FindObjectsOfTypeAll<SpriteRenderer>().ToList();
                foreach (string assetPath in AssetDatabase.GetAllAssetPaths())
                {
                    foreach (SpriteRenderer image in AssetDatabase.LoadAllAssetsAtPath(assetPath).Where(p => p as SpriteRenderer)
                        .Where(a => a != null).Select(r => r as SpriteRenderer))
                    {
                        slist.Add(image);
                    }
                }

                Up(slist);
                EditorSceneManager.MarkAllScenesDirty();
            }

        }
    }

#endif
}

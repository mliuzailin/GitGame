using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AutoUVSetting : MonoBehaviour
{
    private readonly float DefaultCanvasSizeX = 640.0f;
    private readonly float DefaultCanvasSizeY = 960.0f;
    /// <summary>updateUVをUpdate関数で呼ぶかどうか</summary>
    [SerializeField]
    bool isAutoUpdate = true;

    private RawImage targetImage = null;
    RawImage TargetImage
    {
        get
        {
            if (targetImage == null)
            {
                targetImage = GetComponent<RawImage>();
            }
            return targetImage;
        }
    }

    private Canvas mainCanvas = null;
    Canvas MainCanvas
    {
        get
        {
            if (mainCanvas == null)
            {
                mainCanvas = GetComponentInParent<Canvas>();
            }
            return mainCanvas;
        }
    }

    private Camera mainCamera = null;
    Camera MainCamera
    {
        get
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            return mainCamera;
        }
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    //void Update ()
    //{
    //    updateUV();
    //}

    private void LateUpdate()
    {
        if (isAutoUpdate)
        {
            updateUV();
        }
    }

    private void updateUV()
    {
        if (TargetImage.texture == null)
        {
            return;
        }

        //テクスチャサイズ
        float tw = TargetImage.texture.width;
        float th = TargetImage.texture.height;
        //float tw_half = tw * 0.5f;
        float th_half = th * 0.5f;
        //オブジェクト位置
        RectTransform rectObjTrans = GetComponent<RectTransform>();
        Vector2 o_pos = RectTransformUtility.WorldToScreenPoint(MainCamera, rectObjTrans.position);
        //オブジェクトサイズ
        float ow = rectObjTrans.sizeDelta.x;
        float oh = rectObjTrans.sizeDelta.y;
        float ow_half = ow * 0.5f;
        float oh_half = oh * 0.5f;
        //スクリーンサイズ
        float sw = MainCamera.pixelWidth;
        float sh = MainCamera.pixelHeight;
        //float sw_half = sw * 0.5f;
        float sh_half = sh * 0.5f;

        CanvasSetting canvasSetting = MainCanvas.GetComponent<CanvasSetting>();
        if (!canvasSetting.IsHeightReference)
        {
            float rate = DefaultCanvasSizeX / sw;

            o_pos *= rate;

            float tmp0 = (sh_half * rate) - th_half;
            Vector2 o_pos_lu = new Vector2(o_pos.x - ow_half, o_pos.y - oh_half - tmp0);

            TargetImage.uvRect = new Rect(o_pos_lu.x / tw, o_pos_lu.y / th, ow / tw, oh / th);
        }
        else
        {
            float rate = DefaultCanvasSizeY / sh;

            o_pos *= rate;

            float tmp0 = (sh_half * rate) - th_half;
            Vector2 o_pos_lu = new Vector2(o_pos.x - ow_half, o_pos.y - oh_half - tmp0);

            TargetImage.uvRect = new Rect((o_pos_lu.x + (canvasSetting.GetAddWidth() * 0.5f)) / tw, o_pos_lu.y / th, ow / tw, oh / th);
        }
#if UNITY_EDITOR
        TargetImage.OnRebuildRequested();
#endif
    }

    public void UpdateUV()
    {
        // isAutoUpdateがONのときは処理しない
        if (isAutoUpdate) { return; }

        updateUV();
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AutoUVSetting))]
    public class AutoUVSettingEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            AutoUVSetting autoUVSetting = (AutoUVSetting)target;
            if (GUILayout.Button("Update"))
            {
                autoUVSetting.updateUV();
            }
        }
    }
#endif
}

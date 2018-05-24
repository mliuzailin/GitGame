using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasSetting : MonoBehaviour
{
    static readonly float ScreenBaseWidth = 640.0f;
    static readonly float ScreenBaseHeight = 960.0f;

    public enum LayerType
    {
        SCENE_ROOT = 0,
        MAINMENU_SEQ = 1,
        DIALOG = 2,
        WEB_VIEW = 3,
        LOADING = 4,
        INITIALIZE = 5,
        FADE = 6,
        MAINMENU_HEAD = 7,
        GLOBAL_MENU = 8,
        GLOBAL_MENU_SEQ = 9,
        STORY_VIEW = 10,
        TUTORIAL_CAROUSEL = 11,
        GLOBAL_MENU_RETURN = 12,
        INGAME_MENU = 13,
        INGAME_CUTIN = 14,
        INGAME_DIALOG = 15,
        BATTLE = 16,
        SORT_DIALOG = 17,
        MAINMENU_RESULT = 18,
    };

    private int[] layerSortOrder =
    {
        10,				//< SCENE_ROOT
		15,				//< MAINMENU_SEQ
		250,			//< DIALOG
        80,             //< WEB_VIEW
		100,			//< LOADING
		100,			//< INITIALIZE
		200,			//< FADE
		20,				//< MAINMENU_HEAD
		40,				//< GLOBAL_MENU
		41,				//< GLOBAL_MENU_SEQ
		25,				//< STORY_VIEW
		50,				//< TUTORIAL_CAROUSEL
		45,				//< GLOBAL_MENU_RETURN
        10,             //< INGAME_MENU
        20,             //< INGAME_CUTIN
        30,             //< INGAME_DIALOG
		20,				//< BATTLE
        50,             //< SORT_DIALOG
        50,             //< MAINMENU_RESULT
	};

    public LayerType layer = LayerType.SCENE_ROOT;

    private Canvas mainCanvas = null;
    private CanvasScaler canvasScaler = null;
    private int addLayer = 0;
    private bool isHeightReference = false;
    public bool IsHeightReference { get { return isHeightReference; } }

    void Awake()
    {
        mainCanvas = gameObject.GetComponent<Canvas>();
        canvasScaler = gameObject.GetComponent<CanvasScaler>();
        mainCanvas.sortingOrder = layerSortOrder[(int)layer] + addLayer;

        //アスペクト比が足らない場合縦に合わせる
        float base_aspect = ScreenBaseWidth / ScreenBaseHeight;
        float now_aspect = (float)Screen.width / (float)Screen.height;
        isHeightReference = false;
        if (now_aspect > base_aspect)
        {
            isHeightReference = true;
            canvasScaler.matchWidthOrHeight = 1.0f;
        }
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeSortingOrder(int _add)
    {
        addLayer = _add;
    }

    public void ChangeLayerType(LayerType layer_type)
    {
        layer = layer_type;
        mainCanvas.sortingOrder = layerSortOrder[(int)layer] + addLayer;
    }

    public void SetCanvasEnable(bool bFlag)
    {
        mainCanvas.enabled = bFlag;
    }

    public float GetAddWidth()
    {
        float _ret = 0.0f;
        if (!isHeightReference) return _ret;
        RectTransform trans = GetComponent<RectTransform>();
        if (trans == null) return _ret;
        _ret = ScreenBaseWidth - trans.sizeDelta.x;
        return _ret;
    }
}

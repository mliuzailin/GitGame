using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitResult : MonoBehaviour
{
    public enum ResultType
    {
        None = -1,
        Builup = 0,
        Evolve = 1,
        Link = 2,
    }

    private Canvas m_Canvas = null;
    public Canvas MainCanvas { get { return m_Canvas; } }
    private ResultType m_Type = ResultType.None;
    private GameObject m_Parts = null;
    public GameObject Parts { get { return m_Parts; } }
    //private UnitResultBase m_UnitResultBase = null;

    void Awake()
    {
        m_Canvas = GetComponentInChildren<Canvas>();
    }

    void Start()
    {

    }

    void Update()
    {
    }

    void setup(ResultType _type)
    {
        m_Type = _type;
        switch (m_Type)
        {
            case ResultType.Builup:
                m_Parts = loadPrefab("Prefab/UnitResult2/UnitResultBuildup");
                break;
            case ResultType.Evolve:
                m_Parts = loadPrefab("Prefab/UnitResult2/UnitResultEvolve");
                break;
            case ResultType.Link:
                m_Parts = loadPrefab("Prefab/UnitResult2/UnitResultLink");
                break;
        }

        UnityUtil.SetObjectEnabled(m_Parts, true);
    }

    private GameObject loadPrefab(string _name)
    {
        GameObject tmpObj = Resources.Load(_name) as GameObject;
        if (tmpObj == null)
        {
            return null;
        }

        GameObject insObj = Instantiate(tmpObj) as GameObject;
        if (insObj == null)
        {
            return null;
        }

        insObj.transform.SetParent(m_Canvas.transform, false);

        return insObj;
    }

    public void Hide()
    {
        if (m_Type == ResultType.Link)
        {
            LoadingManager.Instance.setOverLayMask(false);
        }

        DestroyObject(gameObject);
    }

    //-------------------------------------------------------------------------------------------------
    //
    //
    //
    //-------------------------------------------------------------------------------------------------
    public static UnitResult Create(ResultType _type)
    {
        //ユニット詳細は１つしか開かない
        if (GetUnitResult() != null)
        {
            return null;
        }

        GameObject _tmpObj = Resources.Load("Prefab/UnitResult2/UnitResult") as GameObject;
        if (_tmpObj == null)
        {
            return null;
        }

        GameObject _insObj = Instantiate(_tmpObj) as GameObject;
        if (_insObj == null)
        {
            return null;
        }

        UnitResult unitResult = _insObj.GetComponent<UnitResult>();
        unitResult.setup(_type);

        if (unitResult.m_Type == ResultType.Link)
        {
            if (unitResult.m_Canvas != null)
            {
                CanvasSetting canvasSetting = unitResult.m_Canvas.GetComponent<CanvasSetting>();
                if (canvasSetting != null)
                {
                    canvasSetting.ChangeLayerType(CanvasSetting.LayerType.DIALOG);
                }

                LoadingManager.Instance.setOverLayMask(true);
            }
        }

        UnityUtil.SetObjectEnabledOnce(_insObj, true);

        return unitResult;
    }

    public static UnitResult GetUnitResult()
    {
        GameObject[] infoArray = GameObject.FindGameObjectsWithTag("UnitResult");
        if (infoArray.Length == 0)
        {
            return null;
        }

        return infoArray[0].GetComponent<UnitResult>();
    }
}

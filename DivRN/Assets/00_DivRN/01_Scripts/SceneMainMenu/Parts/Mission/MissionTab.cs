using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;

public class MissionTab : MenuPartsBase
{
    M4uProperty<string> titleText = new M4uProperty<string>();
    public string TitleText { get { return titleText.Value; } set { titleText.Value = value; } }

    public Toggle m_Toggle;
    public MissionBadge m_MissionBadge;


    void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        m_MissionBadge = GetComponentInChildren<MissionBadge>();
        m_Toggle = GetComponent<Toggle>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

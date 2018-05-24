using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DebugGeneralWindow : MenuPartsBase
{
    [SerializeField]
    InputField m_GroupIDInputField = null;

    uint[] m_GroupIDs = null;

    int m_Index = 0;
    uint m_CurrentID = 0;

    // Use this for initialization
    void Start()
    {
        SetTopAndBottomAjustStatusBar(new Vector2(0, -258));
        List<MasterDataGeneralWindow> generalWindowList = MasterFinder<MasterDataGeneralWindow>.Instance.FindAll();
        List<uint> groupIDList = generalWindowList.Select((v) => v.group_id).ToList();
        m_GroupIDs = groupIDList.Distinct().ToArray();
        SetIndex(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetIndex(int index)
    {
        if (m_GroupIDs.IsNullOrEmpty() == true)
        {
            return;
        }

        if (index >= m_GroupIDs.Length)
        {
            index = 0;
        }
        if (index < 0)
        {
            index = m_GroupIDs.Length - 1;
        }

        m_Index = index;
        m_CurrentID = m_GroupIDs[index];
        m_GroupIDInputField.text = m_CurrentID.ToString();
    }

    void SetID(uint group_id)
    {
        if (m_GroupIDs.IsNullOrEmpty() == true)
        {
            return;
        }

        m_Index = Array.IndexOf(m_GroupIDs, group_id);
        m_CurrentID = group_id;
        m_GroupIDInputField.text = m_CurrentID.ToString();
    }

    public void OnEndEditGroupID(string value)
    {
        uint u;
        uint.TryParse(value, out u);
        SetID(u);
    }

    public void OnClickPrev()
    {
        SetIndex(m_Index - 1);
    }

    public void OnClickNext()
    {
        SetIndex(m_Index + 1);
    }

    public void OnClickOpen()
    {
        GeneralWindowDialog dialog = GeneralWindowDialog.Create();
        dialog.SetGroupID(m_CurrentID);
        dialog.SetDialogEvent(GeneralWindowDialog.ButtonEventType.YES, () =>
        {
            Debug.Log("Send Yes");
        });
        dialog.SetDialogEvent(GeneralWindowDialog.ButtonEventType.NO, () =>
        {
            Debug.Log("Send No");
        });
        dialog.SetDialogEvent(GeneralWindowDialog.ButtonEventType.CLOSE, () =>
        {
            Debug.Log("Send Close");
        });

        dialog.Show();
    }
}

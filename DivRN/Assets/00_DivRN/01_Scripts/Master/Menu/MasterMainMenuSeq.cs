using UnityEngine;
using System.Collections;
using M4u;

public class MasterMainMenuSeq : M4uContextScriptableObject
{
    [System.Serializable]
    public class SequenceObj
    {
        public string object_name = "";
        //[HideInInspector]
        //public GameObject gameObj = null;
    }

    public MAINMENU_SEQ Sequence = MAINMENU_SEQ.SEQ_NONE;
    public SequenceObj[] SequenceObjArray = null;
    public string SequenceName = "";
    public bool HeaderDraw = true;
    public bool HeaderActive = true;
    public bool FooterDraw = true;
    public bool FooterActive = true;
    public bool RefreshResource = false;
    public bool Return = false;
}

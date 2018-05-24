using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class MasterGlobalMenuSeq : M4uContextScriptableObject
{

    [System.Serializable]
    public class SequenceObj
    {
        public string object_name = "";
        public GameObject gameObj = null;
    }

    public GLOBALMENU_SEQ Sequence = GLOBALMENU_SEQ.NONE;
    public SequenceObj[] SequenceObjArray = null;
    public string SequenceName = "";
    public bool Return = false;
}

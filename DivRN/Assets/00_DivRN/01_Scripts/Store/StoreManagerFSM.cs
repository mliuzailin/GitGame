using UnityEngine;
using System.Collections;

public class StoreManagerFSM : FSM<StoreManagerFSM> {

    public bool IsPurchaseWait
    {
        get
        {
            return ActiveStateName.Equals("PurchaseWait");
        }
    }
    
}

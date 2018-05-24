using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 子オブジェクトのGraphicCast以外の全てのRaycastTargetをOffにする
/// </summary>
public class DisalbeAllRayCastTarget : MonoBehaviour
{
    void Awake()
    {
        Graphic[] graphic = GetComponentsInChildren<Graphic>(true);
        for (int i = 0; i < graphic.Length; ++i)
        {
            if (graphic[i].GetType() == typeof(GraphicCast))
            {
                continue;
            }
            graphic[i].raycastTarget = false;
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
}

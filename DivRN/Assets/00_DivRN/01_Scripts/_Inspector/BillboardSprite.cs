using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    public Camera MainCamera = null;

    public void setup( Camera _camera )
    {
        MainCamera = _camera;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (MainCamera == null)
        {
            return;
        }

        Vector3 p = MainCamera.transform.position;
        p.y = transform.position.y;
        transform.LookAt(p);
    }
}

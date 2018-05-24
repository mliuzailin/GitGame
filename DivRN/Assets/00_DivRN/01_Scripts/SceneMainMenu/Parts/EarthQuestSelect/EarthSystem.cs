using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
using DG.Tweening;

public class EarthSystem : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject earthModel;
    public GameObject rotationRoot;
    public GameObject earthParts;
    public RenderTexture renderTexture;
    public float minDistance = 0;
    public float maxDistance = 10000;
    public float rotatePower = 0.02f;
    public EarthParts.MoveType moveType = EarthParts.MoveType.Sphere;

    public Text debugText;

    private float totalMoveDistance = 0;
    public float TotalMoveDistance { get { return totalMoveDistance; } }
    private float earthRotateY = 0.0f;

    //private Vector3 zoomCameraStartPos;
    //private Vector3 zoomCameraEndPos;
    private Vector3 zoomTargetPos;

    private Tweener transAnim;
    private Tweener lookatAnim;

    private List<EarthParts> partsList = new List<EarthParts>();


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void AddRotate(float _add)
    {
        //移動距離
        totalMoveDistance += _add;
        if (totalMoveDistance < minDistance) totalMoveDistance = minDistance;
        if (totalMoveDistance > maxDistance) totalMoveDistance = maxDistance;
        if (debugText != null) debugText.text = "total:" + totalMoveDistance.ToString();

        //地球の回転
        earthRotateY = totalMoveDistance * rotatePower;
        Vector3 angle = earthModel.transform.localEulerAngles;
        if (moveType == EarthParts.MoveType.Sphere)
        {
            earthModel.transform.localRotation = Quaternion.Euler(angle.x, earthRotateY, angle.z);
        }
        else
        {
            earthModel.transform.localRotation = Quaternion.Euler(angle.x, -earthRotateY, angle.z);
        }

        //
        foreach (EarthParts _parts in partsList)
        {
            _parts.SetNowPosition(totalMoveDistance, rotatePower);
            //			Vector3 parts_angle = _parts.transform.localEulerAngles;
            //			_parts.transform.localEulerAngles = new Vector3(parts_angle.x,)
        }

    }

    public void addParts(Button _button, float _point, float _height)
    {
        GameObject newParts = Instantiate(earthParts) as GameObject;
        newParts.transform.SetParent(rotationRoot.transform, false);
        UnityUtil.SetObjectEnabledOnce(newParts, true);

        EarthParts parts = newParts.GetComponent<EarthParts>();
        parts.Setup(_button, moveType, _point, _height);
        parts.SetNowPosition(totalMoveDistance, rotatePower);
        partsList.Add(parts);
    }

    public void setZoomTarget(Button _target)
    {
        Vector3[] path = new Vector3[2];
        //zoomCameraStartPos = mainCamera.transform.position;
        foreach (EarthParts _parts in partsList)
        {
            if (_parts.AttachButton == _target)
            {
                zoomTargetPos = _parts.GetButtonPoint();
                Vector3 dir = _parts.GetButtonPoint() - _parts.GetCameraPoint();
                //zoomCameraEndPos = zoomCameraStartPos + dir * 0.9f;
                path[0] = _parts.GetCameraPoint();
                path[1] = _parts.GetCameraPoint() + dir * 0.8f;
            }
        }
        transAnim = mainCamera.transform.DOPath(path, 1.5f, PathType.CatmullRom);
        //lookatAnim = mainCamera.transform.DOLookAt(zoomTargetPos, 0.5f);
    }

    public bool updateZoomCamera()
    {
        //if( lookatAnim != null && !lookatAnim.IsPlaying() )
        {
            mainCamera.transform.LookAt(zoomTargetPos);
        }

        if (transAnim != null && transAnim.IsPlaying())
        {
            return false;
        }
        return true;
    }

    public void setRenderTarget(bool bSet)
    {
        if (bSet)
        {
            mainCamera.targetTexture = renderTexture;
        }
        else
        {
            mainCamera.targetTexture = null;
        }
    }

}

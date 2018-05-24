using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EarthParts : MonoBehaviour
{
    public enum MoveType
    {
        Sphere = 0,
        Plane,
    }
    public float radius = 20.0f;
    public GameObject buttonPoint;
    public GameObject cameraPoint;
    public Camera mainCamera;

    private Button attachButton;
    public Button AttachButton { get { return attachButton; } }

    private MoveType moveType = MoveType.Sphere;
    private float point = 0.0f;
    public float Point { get { return point; } }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        updateButtonPosition();
    }

    public void Setup(Button _button, MoveType _type, float _point, float _height)
    {
        attachButton = _button;
        moveType = _type;
        point = _point;
        switch (moveType)
        {
            case MoveType.Sphere:
                {
                    Vector3 localpos = transform.localPosition;
                    transform.localPosition = new Vector3(localpos.x, _height, localpos.z);

                    //半径計算
                    if (buttonPoint != null)
                    {
                        float _y = transform.localPosition.y;
                        float _posx = Mathf.Sqrt((radius * radius) - (_y * _y));
                        buttonPoint.transform.localPosition = new Vector3(-_posx, 0, 0);
                    }

                }
                break;
            case MoveType.Plane:
                {
                    if (buttonPoint != null)
                    {
                        buttonPoint.transform.localPosition = new Vector3(-_height, 0, 0);
                    }
                }
                break;
        }
    }

    public void SetNowPosition(float _pos, float _power)
    {
        if (moveType == MoveType.Sphere)
        {
            float pos = _pos - point;
            float rotateY = pos * _power;
            if (rotateY > 0.0f) rotateY = 0.0f;
            if (rotateY < -180.0f) rotateY = -180.0f;
            Vector3 rotate = transform.localEulerAngles;
            transform.localEulerAngles = new Vector3(rotate.x, rotateY, rotate.z);
        }
        else if (moveType == MoveType.Plane)
        {
            float pos = point - _pos;
            float rotateY = pos * _power;
            if (rotateY < 1.0f) rotateY = 1.0f;
            if (rotateY > 180.0f) rotateY = 180.0f;
            Vector3 rotate = transform.localEulerAngles;
            transform.localEulerAngles = new Vector3(rotate.x, rotateY, rotate.z);
        }
    }

    private void updateButtonPosition()
    {
        //スクリーン倍率
        float screenRate = 640.0f / (float)Screen.width;

        //位置計算
        Vector3 position = buttonPoint.transform.position;
        Vector3 screenpos = mainCamera.WorldToScreenPoint(position);

        //位置設定
        RectTransform trans = attachButton.GetComponent<RectTransform>();
        trans.anchoredPosition3D = new Vector3(screenpos.x * screenRate, screenpos.y * screenRate, 0);
    }

    public Vector3 GetButtonPoint()
    {
        return buttonPoint.transform.position;
    }

    public Vector3 GetCameraPoint()
    {
        return cameraPoint.transform.position;
    }
}

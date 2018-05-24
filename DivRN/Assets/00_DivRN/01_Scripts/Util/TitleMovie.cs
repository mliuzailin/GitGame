using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMovie : MonoBehaviour
{
    public GameObject m_SkipImage = null;
    public delegate void OnClick();

    public OnClick onClick;

    public bool m_bPlayMovie = false;

    void Start()
    {
    }

    public void Update()
    {
        if (onClick == null)
        {
            return;
        }
        if (m_bPlayMovie == false)
        {
            return;
        }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        if (Input.GetMouseButtonUp(0) == true)
        {
            onClick();
            return;
        }
#else
		if( Input.touchCount > 0 )
		{
			switch(Input.GetTouch(0).phase)
			{
				case TouchPhase.Ended:
				{
					onClick();
				}
				break;
				default:
					break;
			}
		}
#endif
    }
}


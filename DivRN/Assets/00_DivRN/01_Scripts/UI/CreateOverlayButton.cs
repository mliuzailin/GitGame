using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateOverlayButton : MonoBehaviour
{
    public GameObject prefab;

    void Start()
    {

        transform.GetComponent<Button>().onClick.AddListener(
            () =>
            {
                Instantiate(prefab);
            });

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeBoss : MonoBehaviour
{
    public const float PixelToUnits = 512.0f;
    // ゲーム内解像度 
    public const int BaseScreenWidth = 640;
    public const int BaseScreenHeight = 960;

    public Camera mainCamera = null;
    public Transform Root = null;
    public GameObject BossDataPrefab = null;

    private List<BossSpriteData> bossList = new List<BossSpriteData>();

    private void Awake()
    {
        mainCamera.orthographicSize = BaseScreenHeight / PixelToUnits / 2;

        float baseAspect = (float)BaseScreenHeight / (float)BaseScreenWidth;
        float nowAspect = (float)Screen.height / (float)Screen.width;
        float changeAspect;

        if (baseAspect > nowAspect)
        {
            changeAspect = nowAspect / baseAspect;
            mainCamera.rect = new Rect((1.0f - changeAspect) * 0.5f, 0.0f, changeAspect, 1.0f);
        }
        else
        {
            changeAspect = baseAspect / nowAspect;
            mainCamera.rect = new Rect(0.0f, (1.0f - changeAspect) * 0.5f, 1.0f, changeAspect);
        }
    }

    public void AddBossData( uint[] boss_ids )
    {
        if( boss_ids == null ||
            boss_ids.Length == 0 ||
            BossDataPrefab == null)
        {
            return;
        }

        bossList.Clear();

        float interval = 360.0f / boss_ids.Length;
        float angle = 0.0f;

        for(int i = 0; i < boss_ids.Length; i++)
        {
            var newObj = GameObject.Instantiate(BossDataPrefab);
            newObj.transform.SetParent(Root, false);

            BossSpriteData bossData = newObj.GetComponent<BossSpriteData>();
            bossData.setup(boss_ids[i], mainCamera, angle);
            bossList.Add(bossData);

            angle += interval;
        }
    }
}

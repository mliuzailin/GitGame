using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpriteData : MonoBehaviour
{
    private SpriteRenderer render = null;
    private bool bLog = false;
    private void Awake()
    {
        render = GetComponentInChildren<SpriteRenderer>();
    }

    public void setup( uint boss_id, Camera mainCamera, float angle )
    {
        //
        var assetBundler = AssetBundler.Create();
        assetBundler.SetAsUnitTexture(
            boss_id,
            (asset) =>
            {
                var tex2D = asset.GetUncompressedTexture2D();
                if (render != null && tex2D != null)
                {
                    render.sprite = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f,0.5f));
                }
            })
            .Load();

        //
        GetComponentInChildren<BillboardSprite>().setup(mainCamera);

        //
        transform.localRotation = Quaternion.Euler(0, angle, 0);

        if (angle == 0) bLog = true;
    }

    private void Update()
    {
    }
}

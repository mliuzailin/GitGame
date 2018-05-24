using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebResource : SingletonComponent<WebResource>
{
    private readonly string ADDR_BANNER = "/banner/";
    private readonly string ADDR_SCRATCH = "/scratch/";
    private readonly string ADDR_MASTER = "/master/";
    private readonly string KEY_BANNER = "banner";
    private readonly string KEY_SCRATCH = "scratch";
    private readonly string KEY_MASTER = "master";
    private readonly string KEY_OTHRE = "othre";

    /// <summary>
    /// Sprite取得
    /// 対象ドメインが1箇所のみしか想定されていないので注意
    /// 取得先が複数ある場合は状況に応じて修正の必要がある
    /// </summary>
    /// <param name="url"></param>
    /// <param name="success"></param>
    /// <param name="error"></param>
    public void GetSprite(string url, System.Action<Sprite> success, System.Action error)
    {
        //ローカルストレージから取得
        LoadTexture(url,
            (textrue) =>
            {
                //成功
                success(textrue.CreateSprite());
            },
            () =>
            {
                //失敗したらWebから取得
                StartCoroutine(
                    GetWebTexture(url, (textrue) =>
                    {
                        success(textrue.CreateSprite());
                    },
                    error)
                );
            });
    }

    /// <summary>
    /// Texture2D取得
    /// </summary>
    /// <param name="url"></param>
    /// <param name="success"></param>
    /// <param name="error"></param>
    public void GetTexture2D(string url, System.Action<Texture2D> success, System.Action error)
    {
        //ローカルストレージから取得
        LoadTexture(url,
            (texture) =>
            {
                //成功
                success(texture);
            },
            () =>
            {
                //失敗したらWebから取得
                StartCoroutine(
                    GetWebTexture(url, (textrue) =>
                    {
                        success(textrue);
                    },
                    error)
                );
            });
    }

    public void RemoveAll()
    {
        LocalSaveUtilToInstallFolder.RemoveWebResource();
    }

    /// <summary>
    /// ローカルストレージからテクスチャをロードする
    /// </summary>
    /// <param name="url"></param>
    /// <param name="success"></param>
    /// <param name="error"></param>
    private void LoadTexture(string url, System.Action<Texture2D> success, System.Action error)
    {
        string key = makeKeyFromURL(url);
        if (key == string.Empty)
        {
            error();
            return;
        }

        byte[] data = LocalSaveUtilToInstallFolder.LoadWebResource(key);
        if (data == null ||
            data.Length == 0)
        {
            error();
            return;
        }

        Texture2D texture = GetTexture2DFromBinary(data);
        if (texture != null)
        {
            success(texture);
            return;
        }

        error();
        return;
    }

    /// <summary>
    /// バイナリデータから幅・高さを計算してテクスチャを生成
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private Texture2D GetTexture2DFromBinary(byte[] data)
    {
        // PNGのみ対応

        // 16バイトから開始
        int pos = 16;

        int width = 0;
        for (int i = 0; i < 4; i++)
        {
            width = width * 256 + data[pos++];
        }

        int height = 0;
        for (int i = 0; i < 4; i++)
        {
            height = height * 256 + data[pos++];
        }

        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        texture.LoadImage(data);
        texture.wrapMode = TextureWrapMode.Clamp;

        return texture;
    }

    /// <summary>
    /// テクスチャをローカルストレージに保存(PNG形式)
    /// </summary>
    /// <param name="url"></param>
    /// <param name="texture"></param>
    private Texture2D SaveTexture(string url, Texture2D texture)
    {
        if (texture == null)
        {
            return null;
        }

        string key = makeKeyFromURL(url);
        if (key == string.Empty)
        {
            return texture;
        }

        byte[] data = texture.EncodeToPNG();
        if (data == null)
        {
            return texture;
        }

        LocalSaveUtilToInstallFolder.SaveWebResource(key, data);

        //保存したデータからテクスチャを生成して返す
        return GetTexture2DFromBinary(data);
    }

    /// <summary>
    /// URLから保存キーを生成する
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private string makeKeyFromURL(string url)
    {
        if (url == null ||
            url == string.Empty)
        {
            return string.Empty;
        }

        string[] datas = url.Split('/');
        if (datas.Length < 2)
        {
            return string.Empty;
        }

        //アドレスキー名
        string addr_key = KEY_OTHRE;
        if (url.Contains(ADDR_BANNER))
        {
            addr_key = KEY_BANNER;
        }
        else if (url.Contains(ADDR_SCRATCH))
        {
            addr_key = KEY_SCRATCH;
        }
        else if (url.Contains(ADDR_MASTER))
        {
            addr_key = KEY_MASTER;
        }
        //ファイル名
        string last1 = System.IO.Path.GetFileNameWithoutExtension(datas[datas.Length - 1]);
        //１つ上のフォルダ名
        string last2 = datas[datas.Length - 2];

        string key = addr_key + "_" + last2 + "_" + last1;

        return key;
    }

    /// <summary>
    /// Webからテクスチャを取得
    /// </summary>
    /// <param name="url"></param>
    /// <param name="success"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    private IEnumerator GetWebTexture(string url, System.Action<Texture2D> success, System.Action error = null)
    {
        WWW cWWW = new WWW(url);
        yield return cWWW;

        if (cWWW != null
        && cWWW.isDone == true
        && cWWW.error == null
        && cWWW.texture != null
        && cWWW.texture.width > 10  //※ダミーテクスチャが勝手に読まれた場合、8x8のテクスチャが適用されるので弾く
        )
        {
            //保存処理
            Texture2D tex = SaveTexture(url, cWWW.texture);

            success(tex);
        }
        else
        {
            Debug.LogError("Load Image Error![" + url + "]");
            error();
        }

        cWWW.Dispose();
        cWWW = null;
    }

}

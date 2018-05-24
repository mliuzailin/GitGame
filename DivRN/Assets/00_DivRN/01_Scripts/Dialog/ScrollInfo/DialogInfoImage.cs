using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class DialogInfoImage : M4uContextMonoBehaviour
{
    M4uProperty<Sprite> image = new M4uProperty<Sprite>();
    public Sprite Image { get { return image.Value; } set { image.Value = value; } }

    M4uProperty<Color> imageColor = new M4uProperty<Color>();
    public Color ImageColor { get { return imageColor.Value; } set { imageColor.Value = value; } }

    private string m_URL;

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
        Image = null;
        ImageColor = new Color(1, 1, 1, 0);
    }

    public void setup(string _url)
    {
        m_URL = _url;
        LoadImage();
    }

    private void LoadImage()
    {
        WebResource.Instance.GetSprite(m_URL,
                        (Sprite sprite) =>
                        {
                            Image = sprite;
                            ImageColor = new Color(1, 1, 1, 1);
                        },
                        () =>
                        {
                            UnityUtil.SetObjectEnabled(this.gameObject, false);
                        });

    }
}

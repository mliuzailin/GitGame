using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class UnitCatalogItemContext : M4uContext
{
    public long requestId = 0;

    M4uProperty<int> index = new M4uProperty<int>();
    public int Index { get { return index.Value; } set { index.Value = value; } }

    private bool m_isIconImageLoaded = false;
    M4uProperty<Sprite> iconImage = new M4uProperty<Sprite>();
    public Sprite IconImage
    {
        get { return iconImage.Value; }
        set
        {
            iconImage.Value = value;

            while (m_onIconImageUpdatedQueue.Count > 0)
            {
                m_onIconImageUpdatedQueue.Dequeue()(value);
            }

            m_isIconImageLoaded = true;
        }
    }

    private bool m_isIconColorSet = false;
    M4uProperty<Color> iconColor = new M4uProperty<Color>(new Color(255, 255, 255, 255));
    public Color IconColor
    {
        get { return iconColor.Value; }
        set
        {
            iconColor.Value = value;

            while (m_onIconColorUpdatedQueue.Count > 0)
                m_onIconColorUpdatedQueue.Dequeue()(value);

            m_isIconColorSet = true;
        }
    }

    public MasterDataParamChara master = null;

    public System.Action<uint> DidSelectItem = delegate { };
    public System.Action<uint> DidLongPressItem = delegate { };


    private Queue<System.Action<Sprite>> m_onIconImageUpdatedQueue = new Queue<System.Action<Sprite>>();
    public void RegisteronIconImageUpdatedOnce(System.Action<Sprite> callback)
    {
        m_onIconImageUpdatedQueue.Enqueue(callback);
    }
    private Queue<System.Action<Color>> m_onIconColorUpdatedQueue = new Queue<System.Action<Color>>();
    public void RegisteronIconColorUpdatedOnce(System.Action<Color> callback)
    {
        m_onIconColorUpdatedQueue.Enqueue(callback);
    }

    public delegate void EventHandler();
    public event EventHandler OnDestructed;
    ~UnitCatalogItemContext()
    {
        if (OnDestructed != null)
            OnDestructed();
    }


    public void Copy(UnitCatalogItemContext _data)
    {
        _data.Index = Index;
        _data.master = master;
        _data.DidSelectItem = DidSelectItem;
        _data.DidLongPressItem = DidLongPressItem;

        if (m_isIconColorSet)
        {
            _data.IconColor = IconColor;
        }
        else
        {
            SetIconColorLater(_data);
        }

        if (m_isIconImageLoaded)
        {
            _data.IconImage = IconImage;
        }
        else
        {
            SetIconImageLater(_data);
        }
    }

    private void SetIconColorLater(UnitCatalogItemContext _data)
    {
        // コピー元のアイコンロード完了時にコピー先がいなくなっていた場合の対策
        bool isAlive = true;
        _data.OnDestructed += () =>
        {
            isAlive = false;
        };

        RegisteronIconColorUpdatedOnce(color =>
        {
            if (!isAlive)
                return;

            _data.IconColor = color;
        });
    }

    private void SetIconImageLater(UnitCatalogItemContext _data)
    {
        // コピー元のアイコンロード完了時にコピー先がいなくなっていた場合の対策
        bool isAlive = true;
        _data.OnDestructed += () =>
        {
            isAlive = false;
        };

        RegisteronIconImageUpdatedOnce(sprite =>
        {
            if (!isAlive)
                return;

            _data.IconImage = sprite;
        });
    }
}

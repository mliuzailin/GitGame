using UnityEngine;
using System.Collections;

public class PresentRecordListItem : ListItem<PresentRecordListItemContex>
{
    public GameObject categoryPresentGO;
    public GameObject categoryPresentLogGO;

    public Present.Category Category
    {
        get
        {
            return (Present.Category)Context.Category;
        }
    }

    void Awake()
    {
        categoryPresentGO.SetActive(false);
        categoryPresentLogGO.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {
        if (Category == Present.Category.Present)
        {
            categoryPresentGO.SetActive(true);
        }
        else
        {
            categoryPresentLogGO.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    { }


    // クリック時のフィードバック
    public void OnClicked()
    {
        Context.DidSelectItem(Context);
#if BUILD_TYPE_DEBUG
        //Debug.Log("[Debug:RecordListItem - OnClicked]" + Context.CaptionText01);
#endif
    }
}
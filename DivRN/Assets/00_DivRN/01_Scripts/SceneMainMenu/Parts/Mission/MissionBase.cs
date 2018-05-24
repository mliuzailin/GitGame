using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class MissionBase : M4uContextMonoBehaviour
{
    // カラーパレット
    // "#E03F6CFF", "#FFBF00FF", "#82B312FF"
    public Color[] IconColor;
    // "#FFA6C1C0", "#F1E990C0", "#BDF291C0"
    public Color[] FrameColor;

    // ボタン画像
    public Sprite[] RecordButtonImage;

    // ナビゲーションメニューの選択 選択中のカテゴリ
    protected M4uProperty<int> categoryId = new M4uProperty<int>(0);
    public int CategoryId { get { return categoryId.Value; } set { categoryId.Value = value; } }

    // 完了カテゴリ
    public int DoneCategory { get; set; }

    // ナビゲーションメニュー文字列
    protected M4uProperty<string> naviText0 = new M4uProperty<string>("");
    protected M4uProperty<string> naviText1 = new M4uProperty<string>("");
    protected M4uProperty<string> naviText2 = new M4uProperty<string>("");
    protected M4uProperty<string> naviText3 = new M4uProperty<string>("");
    public string NaviText0 { get { return naviText0.Value; } set { naviText0.Value = value; } }
    public string NaviText1 { get { return naviText1.Value; } set { naviText1.Value = value; } }
    public string NaviText2 { get { return naviText2.Value; } set { naviText2.Value = value; } }
    public string NaviText3 { get { return naviText3.Value; } set { naviText3.Value = value; } }

    // ナビゲーションメニューバッジ文字列
    protected M4uProperty<int> naviBadgeText0 = new M4uProperty<int>(0);
    protected M4uProperty<int> naviBadgeText1 = new M4uProperty<int>(0);
    protected M4uProperty<int> naviBadgeText2 = new M4uProperty<int>(0);
    protected M4uProperty<int> naviBadgeText3 = new M4uProperty<int>(0);
    public int NaviBadgeText0 { get { return naviBadgeText0.Value; } set { naviBadgeText0.Value = value; } }
    public int NaviBadgeText1 { get { return naviBadgeText1.Value; } set { naviBadgeText1.Value = value; } }
    public int NaviBadgeText2 { get { return naviBadgeText2.Value; } set { naviBadgeText2.Value = value; } }
    public int NaviBadgeText3 { get { return naviBadgeText3.Value; } set { naviBadgeText3.Value = value; } }

    // レコード数
    protected M4uProperty<int> recordCount0 = new M4uProperty<int>(0);
    protected M4uProperty<int> recordCount1 = new M4uProperty<int>(0);
    protected M4uProperty<int> recordCount2 = new M4uProperty<int>(0);
    protected M4uProperty<int> recordCount3 = new M4uProperty<int>(0);
    public int RecordCount0 { get { return recordCount0.Value; } set { recordCount0.Value = value; } }
    public int RecordCount1 { get { return recordCount1.Value; } set { recordCount1.Value = value; } }
    public int RecordCount2 { get { return recordCount2.Value; } set { recordCount2.Value = value; } }
    public int RecordCount3 { get { return recordCount3.Value; } set { recordCount3.Value = value; } }

    // レコードの追加先
    protected M4uProperty<List<RecordListItemContex>> records0 = new M4uProperty<List<RecordListItemContex>>(new List<RecordListItemContex>());
    protected M4uProperty<List<RecordListItemContex>> records1 = new M4uProperty<List<RecordListItemContex>>(new List<RecordListItemContex>());
    protected M4uProperty<List<RecordListItemContex>> records2 = new M4uProperty<List<RecordListItemContex>>(new List<RecordListItemContex>());
    protected M4uProperty<List<RecordListItemContex>> records3 = new M4uProperty<List<RecordListItemContex>>(new List<RecordListItemContex>());
    public List<RecordListItemContex> Records0 { get { return records0.Value; } set { records0.Value = value; } }
    public List<RecordListItemContex> Records1 { get { return records1.Value; } set { records1.Value = value; } }
    public List<RecordListItemContex> Records2 { get { return records2.Value; } set { records2.Value = value; } }
    public List<RecordListItemContex> Records3 { get { return records3.Value; } set { records3.Value = value; } }

    // 選択中カテゴリと一致するレコード
    public List<RecordListItemContex> CurrentRecords
    {
        get { return GetRecords(CategoryId); }
        set { this.SetRecords(CategoryId, value); }
    }

    // 完了レコード
    public List<RecordListItemContex> DoneRecords
    {
        get { return GetRecords(DoneCategory); }
        set { this.SetRecords(DoneCategory, value); }
    }

    // ページ関連内部データ 最初に表示するページで初期化（レコード増減に影響）
    protected List<int> pagesSelect = new List<int> { { 0 }, { 0 }, { 0 }, { 0 } };
    protected List<int> pagesMax = new List<int> { { 0 }, { 0 }, { 0 }, { 0 } };

    // １ページ中の最大レコード数 30件
    public int PageCapacity = 30;
    //public const int pageCapacity = 6;

    // 選択中のページの表示文字列
    protected M4uProperty<string> pageText = new M4uProperty<string>("");
    public string PageText { get { return pageText.Value; } set { pageText.Value = value; } }

    // 選択中のページの表示フラグ
    protected M4uProperty<bool> pageTextView = new M4uProperty<bool>(false);
    public bool PageTextView { get { return pageTextView.Value; } set { pageTextView.Value = value; } }

    // 選択中のページの変更ボタン表示（<<減算）
    protected M4uProperty<bool> pageLeftView = new M4uProperty<bool>(false);
    public bool PageLeftView { get { return pageLeftView.Value; } set { pageLeftView.Value = value; } }

    // 選択中のページの変更ボタン表示（加算>>）
    protected M4uProperty<bool> pageRightView = new M4uProperty<bool>(false);
    public bool PageRightView { get { return pageRightView.Value; } set { pageRightView.Value = value; } }


    public MissionBase()
    {
        // 初期化 C#バージョンの都合上、宣言時に初期化は行えない
        // public string StrFld { get; set; } = "";
        //  ↑こういうのがエラーになります
        CategoryId = 0;
    }

    protected void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    protected void Start()
    {
    }

    // SceneStart後に実行
    protected void PostSceneStart()
    {
        // 全レコードのソート
        sortAllRecords();

        // 初期選択カテゴリ
        setCategory(0);

        // 全レコード数の更新 バッジやEmpty表示で使用
        updateRecordCount();

        updateBadgeText();
    }

    // カテゴリの選択
    protected void setCategory(int category)
    {
        // 選択から外れるレコードに対し、処理を行う
        foreach (RecordListItemContex context in CurrentRecords)
        {
            // アイコン上の「New」バッジを非表示化
            context.IsNewBadge = false;
        }

        // カテゴリの選択
        CategoryId = category;

        // 選択中のページの更新
        updateCategoryPage(CategoryId);

        // 全レコード数の更新 EMPTY 表示で使用
        //updateRecordCount();
    }

    // 全レコード数の更新
    protected void updateRecordCount()
    {
        //　カテゴリ毎のレコード数の再集計
        RecordCount0 = (int)Records0.Count();
        RecordCount1 = (int)Records1.Count();
        RecordCount2 = (int)Records2.Count();
        RecordCount3 = (int)Records3.Count();
    }

    // 受領バッジの更新
    protected void updateBadgeText()
    {
        //　受取待ちレコードの再集計　バッジ表示で使用　
        NaviBadgeText0 = (int)Records0.Count(p => p.Stats == RecordListItemContex.STATS.CLEARD);
        NaviBadgeText1 = (int)Records1.Count(p => p.Stats == RecordListItemContex.STATS.CLEARD);
        NaviBadgeText2 = (int)Records2.Count(p => p.Stats == RecordListItemContex.STATS.CLEARD);
        NaviBadgeText3 = (int)Records3.Count(p => p.Stats == RecordListItemContex.STATS.CLEARD);

    }

    // 選択中のページの変更UIの更新
    protected void updatePageSelector()
    {
        int page = pagesSelect[CategoryId];
        int pageMax = pagesMax[CategoryId];

        // 移動先ページにレコードが存在しない場合、移動ボタンを表示しない
        PageLeftView = (0 < page) ? true : false;

        // 移動先ページにレコードが存在しない場合、移動ボタンを表示しない
        PageRightView = (page < pageMax) ? true : false;

        // 選択中のページ数が複数構成の場合は、「選択中のページ/最終ページ」文字列を表示する
        PageTextView = (0 < pageMax) ? true : false;

        // 選択中のページ/最終ページ文字列
        PageText = (page + 1) + "/" + (pageMax + 1);
    }

    // 選択中のページを更新
    // ・更新後に選択中のページが存在する場合　→　更新前の現在のページ＝＞更新後の現在のページ
    // ・更新後に選択中のページが存在しない場合　→　更新後の最終ページ＝＞更新後の現在のページ
    protected void updateCategoryPage(int category)
    {
        // 最終ページの更新
        pagesMax[category] = (int)(CurrentRecords.Count - 1) / this.PageCapacity;

        // 選択中のページの更新
        pagesSelect[category] = pagesSelect[category] <= pagesMax[category]
                                    ? pagesSelect[category] : pagesMax[category];

        // 不要なレコードを非表示化
        for (int i = 0; i < CurrentRecords.Count; i++)
        {
            // 選択中のページの先頭レコード
            int top = pagesSelect[category] * this.PageCapacity;

            // 選択中のページの最終レコード
            int tail = top + this.PageCapacity - 1;

            // 選択中のページの先頭レコードから、最終レコードのみをアクティブに
            CurrentRecords[i].Active = (top <= i && i <= tail) ? true : false;
        }

        // 選択中のページの変更UIの更新
        updatePageSelector();
    }

    // 選択中のページの変更ボタンのフィードバック
    public void OnClickedPageButton(int inc_page)
    {
        if (0 <= (pagesSelect[CategoryId] + inc_page)
            && (pagesSelect[CategoryId] + inc_page) <= pagesMax[CategoryId])
        {
            pagesSelect[CategoryId] += inc_page;
        }

        // 選択中のページを更新
        updateCategoryPage(CategoryId);
    }

    // ナビゲーションバーのカテゴリボタンのフィードバック
    protected void OnClickedNavigationButton(int category)
    {
        // カテゴリの選択
        setCategory(category);
    }

    // 戻るボタン
    protected void OnClickedBackButton()
    {
#if BUILD_TYPE_DEBUG
        Debug.Log("OnClickedBackButton");
#endif
        Destroy(gameObject);
    }


    // 「すべて受領」ボタンのフィードバック
    // 　現在選択中のページ内にある、アイテムすべてが対象
    protected int OnClickeTakeAllButton()
    {
        // アイテム受領個数の初期化
        int num = 0;

        // ループ中で要素数を変更しているため、foreachの動作は保証されない
        for (int i = CurrentRecords.Count - 1; i >= 0; i--)
        {
            // クリアフラグが有効なレコードを対象に処理を行う
            if (CurrentRecords[i].Stats == RecordListItemContex.STATS.CLEARD)
            {
                // 選択中のレコード内にある、対象IDと一致するレコードを、終了レコードに移動する
                moveRecords(CurrentRecords[i].Id, DoneRecords);

                // 受領個数を加算
                num += 1;
            }
        }

        // 受取個数が1個以上存在する場合
        if (0 < num)
        {
            // ページを更新
            updateCategoryPage(CategoryId);

            // 全レコード数の更新
            updateRecordCount();
        }

        return num;
    }

    // レコード内ボタンのフィードバック
    protected void moveToDoneRecords(RecordListItemContex context)
    {
#if BUILD_TYPE_DEBUG
        //Debug.Log("RecordListItemContex");
#endif
        // レコードの移動
        moveRecords(context.Id, DoneRecords);

        // 移動元のレコードのソート
        sortRecords(CurrentRecords);

        // 移動先のレコードのソート
        sortRecords(DoneRecords);

        // 選択中のページを更新
        updateCategoryPage(CategoryId);

        // 全レコード数の更新
        updateRecordCount();
    }

    // データ入力
    // 各種違いがあるので、ここでは定義しない
    protected void AddRecord() { }

    // レコードの移動
    // 移動元からID基に検索を行い、マッチする1レコードのみを移動先に移動する
    // 移動元にIDのレコードがが存在しない場合、何も行わない
    // foreach内の使用は未定義
    protected bool moveRecords(int id, List<RecordListItemContex> context)
    {
        var contex = CurrentRecords.Find(p => p.Id == id);
        if (contex == null)
            return false;

        contex.IconColor = IconColor[DoneCategory];			// 色変更
        contex.FrameColor = FrameColor[DoneCategory];

        // この行は分離した方がいいかも
        contex.Stats = RecordListItemContex.STATS.DONE;     // 状態遷移 ->受領済

        contex.Category = DoneCategory; // カテゴリ変更

        contex.CaptionText02 = "";  // 残り時間非表示
        context.Add(contex);

        CurrentRecords.Remove(contex);
        return true;
    }

    // レコードのソート
    protected void sortRecords(List<RecordListItemContex> context)
    {
        // 受取可能なものを上部に表示し残りはIDでソート
        context.Sort((a, b) =>
        {
            int result = (int)(b.Stats - a.Stats);
            return result != 0 ? result : (int)(a.Id - b.Id);
        });
    }

    // 全レコードのソート
    protected void sortAllRecords()
    {
        // レコードのソート
        sortRecords(Records0);
        sortRecords(Records1);
        sortRecords(Records2);
        sortRecords(Records3);
    }

    // Recordを配列のように扱う（レコード増減に影響）
    public List<RecordListItemContex> GetRecords(int id)
    {
        switch (id)
        {
            case 0:
                return Records0;
            case 1:
                return Records1;
            case 2:
                return Records2;
            case 3:
                return Records3;
            default:
                return null;
        }
    }

    protected void SetRecords(int id, List<RecordListItemContex> records)
    {
        switch (id)
        {
            case 0:
                Records0 = records;
                break;
            case 1:
                Records1 = records;
                break;
            case 2:
                Records2 = records;
                break;
            case 3:
                Records3 = records;
                break;
            default:
                break;
        }
    }
}

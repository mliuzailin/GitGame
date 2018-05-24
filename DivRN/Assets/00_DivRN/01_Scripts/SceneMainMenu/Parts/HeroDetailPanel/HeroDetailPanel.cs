using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M4u;
using ServerDataDefine;

public class HeroDetailPanel : MenuPartsBase
{
    M4uProperty<string> heroNameText = new M4uProperty<string>();
    /// <summary>名前</summary>
    public string HeroNameText
    {
        get
        {
            return heroNameText.Value;
        }
        set
        {
            heroNameText.Value = value;
        }
    }

    M4uProperty<string> heroPhoneticText = new M4uProperty<string>();
    /// <summary>ふりがな</summary>
    public string HeroPhoneticText
    {
        get
        {
            return heroPhoneticText.Value;
        }
        set
        {
            heroPhoneticText.Value = value;
        }
    }

    M4uProperty<Sprite> heroImage = new M4uProperty<Sprite>();
    public Sprite HeroImage
    {
        get
        {
            return heroImage.Value;
        }
        set
        {
            heroImage.Value = value;
        }
    }

    M4uProperty<Texture> heroImage_mask = new M4uProperty<Texture>();
    public Texture HeroImage_mask
    {
        get
        {
            return heroImage_mask.Value;
        }
        set
        {
            heroImage_mask.Value = value;
        }
    }

    M4uProperty<string> messageText = new M4uProperty<string>();
    /// <summary>詳細テキスト</summary>
    public string MessageText
    {
        get
        {
            return messageText.Value;
        }
        set
        {
            messageText.Value = value;
        }
    }
    /*
        M4uProperty<string> illustratorText = new M4uProperty<string>();
        /// <summary>イラストレーター名</summary>
        public string IllustratorText
        {
            get
            {
                return illustratorText.Value;
            }
            set
            {
                illustratorText.Value = value;
            }
        }
    */
    M4uProperty<List<UnitSkillContext>> skills = new M4uProperty<List<UnitSkillContext>>(new List<UnitSkillContext>());
    public List<UnitSkillContext> Skills
    {
        get
        {
            return skills.Value;
        }
        set
        {
            skills.Value = value;
        }
    }

    M4uProperty<float> contentPosY = new M4uProperty<float>(0);
    public float ContentPosY
    {
        get
        {
            return contentPosY.Value;
        }
        set
        {
            contentPosY.Value = value;
        }
    }

    int gradeNum = 0;
    /// <summary>GRADEの値</summary>
    public int m_GradeNum
    {
        get { return gradeNum; }
        set
        {
            gradeNum = value;
            SetUpGradeImage(value);
        }
    }

    int nextGradeNum = 0;
    /// <summary>次のGRADEまでの値</summary>
    public int m_NextGradeNum
    {
        get { return nextGradeNum; }
        set
        {
            nextGradeNum = value;
            SetUpNextGradeImage(value);
        }
    }

    /// <summary>GRADEの数字画像</summary>
    [SerializeField]
    Image[] m_GradeImages;

    /// <summary>次のGRADEまでの数字画像</summary>
    [SerializeField]
    Image[] m_NextGradeImages;

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }

    /// <summary>
    /// GRADEの数字を描画する
    /// </summary>
    /// <param name="grade"></param>
    void SetUpGradeImage(int grade)
    {
        List<uint> number = GetNumberList(grade);
        if (m_GradeImages == null) { return; }
        for (int i = 0; i < m_GradeImages.Length; ++i)
        {
            if (number.IsRange(i))
            {
                string path = string.Format("number_{0}", number[i]);
                Sprite sprite = ResourceManager.Instance.Load(path, ResourceType.Common);
                m_GradeImages[i].sprite = sprite;
                m_GradeImages[i].gameObject.SetActive(sprite != null);
            }
            else
            {
                m_GradeImages[i].sprite = null;
                m_GradeImages[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 次のGRADEまでの数字を描画する
    /// </summary>
    /// <param name="nextGrade"></param>
    void SetUpNextGradeImage(int nextGrade)
    {
        List<uint> number = GetNumberList(nextGrade);
        if (m_NextGradeImages == null) { return; }
        for (int i = 0; i < m_NextGradeImages.Length; ++i)
        {
            if (number.IsRange(i))
            {
                string path = string.Format("number_{0}", number[i]);
                Sprite sprite = ResourceManager.Instance.Load(path, ResourceType.Common);
                m_NextGradeImages[i].sprite = sprite;
                m_NextGradeImages[i].gameObject.SetActive(sprite != null);
            }
            else
            {
                m_NextGradeImages[i].sprite = null;
                m_NextGradeImages[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 数値を文字配列に変換する
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    List<uint> GetNumberList(int num)
    {
        int digit = num;
        //要素数0には１桁目の値が格納
        List<uint> number = new List<uint>();
        if (digit > 0)
        {
            while (digit != 0)
            {
                number.Add((uint)(digit % 10));
                digit = digit / 10;
            }
        }
        else
        {
            number.Add(0);
        }

        return number;
    }

    public void SetUpHeroName(int hero_id)
    {
        switch (hero_id)
        {
            case 1:
                HeroNameText = GameTextUtil.GetText("tutorial_selecttext1");
                HeroPhoneticText = GameTextUtil.GetText("tutorial_selecttext7");
                break;
            case 2:
                HeroNameText = GameTextUtil.GetText("tutorial_selecttext2");
                HeroPhoneticText = GameTextUtil.GetText("tutorial_selecttext8");
                break;
            case 3:
                HeroNameText = GameTextUtil.GetText("tutorial_selecttext3");
                HeroPhoneticText = GameTextUtil.GetText("tutorial_selecttext9");
                break;
            case 4:
                HeroNameText = GameTextUtil.GetText("tutorial_selecttext4");
                HeroPhoneticText = GameTextUtil.GetText("tutorial_selecttext10");
                break;
            case 5:
                HeroNameText = GameTextUtil.GetText("tutorial_selecttext5");
                HeroPhoneticText = GameTextUtil.GetText("tutorial_selecttext11");
                break;
            case 6:
                HeroNameText = GameTextUtil.GetText("tutorial_selecttext6");
                HeroPhoneticText = GameTextUtil.GetText("tutorial_selecttext12");
                break;
            default:
                HeroNameText = "";
                HeroPhoneticText = "";
                break;
        }
    }

    public void ClearView()
    {
        HeroImage = null;
        HeroImage_mask = null;
        HeroNameText = "";
        HeroPhoneticText = "";
        MessageText = "";
        m_GradeNum = 0;
        m_NextGradeNum = 0;
        //		IllustratorText = "";
        Skills.Clear();
        ContentPosY = 0;
    }
    public void SetDetail(PacketStructHero heroData)
    {
        ClearView();
        if (heroData == null || heroData.hero_id == 0) { return; }
        MasterDataHero heroMaster = MasterFinder<MasterDataHero>.Instance.Find((int)heroData.hero_id);
        if (heroMaster == null) { return; }

        SetUpHeroName(heroData.hero_id); // 名前
        MessageText = heroMaster.detail; // 詳細テキスト

        AssetBundler.Create().Set("hero_" + heroData.hero_id.ToString("0000"), "hero_" + heroData.hero_id.ToString("0000"), (o) =>
        {
            Sprite[] sprite = o.AssetBundle.LoadAssetWithSubAssets<Sprite>("hero_" + heroData.hero_id.ToString("0000"));
            HeroImage = sprite[4];
        }).Load();
        //------------------------------------------------------------------
        // グレードの設定
        //------------------------------------------------------------------
        m_GradeNum = heroData.level;

        MasterDataHeroLevel nextHeroLevelMaster = MasterFinder<MasterDataHeroLevel>.Instance.Find((int)heroData.level + 1);
        if (nextHeroLevelMaster != null)
        {
            m_NextGradeNum = nextHeroLevelMaster.exp_next_total - heroData.exp;
        }
        else
        {
            m_NextGradeNum = 0;
        }

        //------------------------------------------------------------------
        // イラストレーター名
        //------------------------------------------------------------------
        /*
                MasterDataIllustrator illustratorMaster = MasterFinder<MasterDataIllustrator>.Instance.Find((int)heroMaster.illustrator_id);
                if (illustratorMaster != null)
                {
                    IllustratorText = illustratorMaster.name;
                }
        */
        //------------------------------------------------------------------
        // スキルの設定
        //------------------------------------------------------------------
        List<UnitSkillContext> skillList = new List<UnitSkillContext>();
        UnitSkillContext skill = new UnitSkillContext();
        skill.setupHeroSkill((uint)heroMaster.default_skill_id, 0, 0, true);
        skillList.Add(skill);
        Skills = skillList;

    }
}

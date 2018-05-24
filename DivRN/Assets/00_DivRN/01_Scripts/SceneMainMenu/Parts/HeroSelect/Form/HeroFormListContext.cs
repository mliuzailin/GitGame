/**
 *  @file   HeroFormListContext.cs
 *  @brief  
 *  @author Developer
 *  @date   2017/02/24
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using M4u;

public class HeroFormListContext : M4uContext
{
    public enum JobType
    {
        NONE,
        TEACHER,
        STUDENT,
    }

    public Toggle Toggle = null;

    /// <summary>顔写真が押されたときのアクション</summary>
    public Action<HeroFormListContext> OnClickFaceImageAction = delegate { };

    public int HeroIndex { get; set; }

    M4uProperty<Sprite> faceImage = new M4uProperty<Sprite>();
    /// <summary>顔写真</summary>
    public Sprite FaceImage
    {
        get
        {
            return faceImage.Value;
        }
        set
        {
            faceImage.Value = value;
        }
    }

    M4uProperty<Texture2D> faceImage_mask = new M4uProperty<Texture2D>();
    /// <summary>顔写真</summary>
    public Texture2D FaceImage_mask
    {
        get
        {
            return faceImage_mask.Value;
        }
        set
        {
            faceImage_mask.Value = value;
        }
    }

    M4uProperty<Rect> faceImageUV = new M4uProperty<Rect>(new Rect(0, 0, 1, 1));
    /// <summary>顔写真のUV設定</summary>
    public Rect FaceImageUV { get { return faceImageUV.Value; } set { faceImageUV.Value = value; } }

    M4uProperty<Sprite> numberImage = new M4uProperty<Sprite>();
    /// <summary>学生番号</summary>
    public Sprite NumberImage
    {
        get
        {
            return numberImage.Value;
        }
        set
        {
            numberImage.Value = value;
        }
    }

    M4uProperty<Sprite> schoolYearImage = new M4uProperty<Sprite>();
    /// <summary>学年</summary>
    public Sprite SchoolYearImage
    {
        get
        {
            return schoolYearImage.Value;
        }
        set
        {
            schoolYearImage.Value = value;
        }
    }

    M4uProperty<Sprite> nameImage = new M4uProperty<Sprite>();
    /// <summary>氏名</summary>
    public Sprite NameImage
    {
        get
        {
            return nameImage.Value;
        }
        set
        {
            nameImage.Value = value;
        }
    }

    M4uProperty<Sprite> birthdayImage = new M4uProperty<Sprite>();
    /// <summary>誕生日</summary>
    public Sprite BirthdayImage
    {
        get
        {
            return birthdayImage.Value;
        }
        set
        {
            birthdayImage.Value = value;
        }
    }


    M4uProperty<JobType> job = new M4uProperty<JobType>();
    /// <summary>職業</summary>
    public JobType Job
    {
        get
        {
            return job.Value;
        }
        set
        {
            // 職業によって画像を変更する
            switch (value)
            {
                case JobType.TEACHER:
                    BackGroundImage = ResourceManager.Instance.Load("card_bg7");
                    BackGroundTilingImage = ResourceManager.Instance.Load("card_bg10");
                    BackGroundEmblemColor = new Color(0.513f, 0.87f, 0.639f);
                    TopBarImage = ResourceManager.Instance.Load("card_bg8");
                    break;
                case JobType.STUDENT:
                    BackGroundImage = ResourceManager.Instance.Load("card_bg2");
                    BackGroundTilingImage = ResourceManager.Instance.Load("card_bg6");
                    BackGroundEmblemColor = new Color(0.5f, 0.843f, 1.0f);
                    TopBarImage = ResourceManager.Instance.Load("card_bg3");
                    break;
            }

            job.Value = value;
        }
    }

    M4uProperty<Sprite> backGroundImage = new M4uProperty<Sprite>();
    /// <summary>背景</summary>
    public Sprite BackGroundImage
    {
        get
        {
            return backGroundImage.Value;
        }
        set
        {
            backGroundImage.Value = value;
        }
    }

    M4uProperty<Sprite> backGroundTilingImage = new M4uProperty<Sprite>();
    /// <summary>背景のタイリング画像</summary>
    public Sprite BackGroundTilingImage
    {
        get
        {
            return backGroundTilingImage.Value;
        }
        set
        {
            backGroundTilingImage.Value = value;
        }
    }

    M4uProperty<Color> backGroundEmblemColor = new M4uProperty<Color>();
    /// <summary>背景の校章の画像</summary>
    public Color BackGroundEmblemColor
    {
        get
        {
            return backGroundEmblemColor.Value;
        }
        set
        {
            backGroundEmblemColor.Value = value;
        }
    }

    M4uProperty<Sprite> topBarImage = new M4uProperty<Sprite>();
    /// <summary>上部バーの背景</summary>
    public Sprite TopBarImage
    {
        get
        {
            return topBarImage.Value;
        }
        set
        {
            topBarImage.Value = value;
        }
    }

    M4uProperty<Texture> numberImage_mask = new M4uProperty<Texture>();
    public Texture NumberImage_mask { get { return numberImage_mask.Value; } set { numberImage_mask.Value = value; } }

    M4uProperty<Texture> nameImage_mask = new M4uProperty<Texture>();
    public Texture NameImage_mask { get { return nameImage_mask.Value; } set { nameImage_mask.Value = value; } }

    M4uProperty<Texture> birthdayImage_mask = new M4uProperty<Texture>();
    public Texture BirthdayImage_mask { get { return birthdayImage_mask.Value; } set { birthdayImage_mask.Value = value; } }
}

/**
 * 	@file	HSBColor.cs
 *	@brief	HSBカラーを扱う
 *	@author Developer
 *	@date	2015/10/23
 */

using UnityEngine;

[System.Serializable]
public struct HSBColor
{
    public float h;
    public float s;
    public float b;
    public float a;

    public HSBColor(float h, float s, float b, float a)
    {
        this.h = h;
        this.s = s;
        this.b = b;
        this.a = a;
    }

    public HSBColor(float h, float s, float b)
    {
        this.h = h;
        this.s = s;
        this.b = b;
        this.a = 1f;
    }

    public HSBColor(Color col)
    {
        HSBColor temp = FromColor(col);
        h = temp.h;
        s = temp.s;
        b = temp.b;
        a = temp.a;
    }

    /**
	 * <summary>RGBカラーをHSBカラーに変換する.</summary>
     * <returns>HSBカラー</returns>
	 */
    public static HSBColor FromColor(Color color)
    {
        HSBColor hsbcolor = new HSBColor(0f, 0f, 0f, color.a);

        float r = color.r;
        float g = color.g;
        float b = color.b;

        float max = Mathf.Max(r, Mathf.Max(g, b));

        if (max <= 0)
        {
            return hsbcolor;
        }

        float min = Mathf.Min(r, Mathf.Min(g, b));
        float dif = max - min;

        if (max > min)
        {
            if (g == max)
            {
                hsbcolor.h = (b - r) / dif * 60f + 120f;
            }
            else if (b == max)
            {
                hsbcolor.h = (r - g) / dif * 60f + 240f;
            }
            else if (b > g)
            {
                hsbcolor.h = (g - b) / dif * 60f + 360f;
            }
            else
            {
                hsbcolor.h = (g - b) / dif * 60f;
            }
            if (hsbcolor.h < 0)
            {
                hsbcolor.h = hsbcolor.h + 360f;
            }
        }
        else
        {
            hsbcolor.h = 0;
        }

        hsbcolor.h *= 1f / 360f;
        hsbcolor.s = (dif / max) * 1f;
        hsbcolor.b = max;

        return hsbcolor;
    }

    /**
	 * <summary>HSBカラーをRGBカラーに変換する.</summary>
     * <param name="hsbColor">HSBカラー</param>
     * <returns>RGBカラー</returns>
	 */
    public static Color ToColor(HSBColor hsbColor)
    {
        float r = hsbColor.b;
        float g = hsbColor.b;
        float b = hsbColor.b;
        if (hsbColor.s != 0)
        {
            float max = hsbColor.b;
            float dif = hsbColor.b * hsbColor.s;
            float min = hsbColor.b - dif;

            float h = hsbColor.h * 360f;

            if (h < 60f)
            {
                r = max;
                g = h * dif / 60f + min;
                b = min;
            }
            else if (h < 120f)
            {
                r = -(h - 120f) * dif / 60f + min;
                g = max;
                b = min;
            }
            else if (h < 180f)
            {
                r = min;
                g = max;
                b = (h - 120f) * dif / 60f + min;
            }
            else if (h < 240f)
            {
                r = min;
                g = -(h - 240f) * dif / 60f + min;
                b = max;
            }
            else if (h < 300f)
            {
                r = (h - 240f) * dif / 60f + min;
                g = min;
                b = max;
            }
            else if (h <= 360f)
            {
                r = max;
                g = min;
                b = -(h - 360f) * dif / 60 + min;
            }
            else
            {
                r = g = b = 0;
            }
        }

        return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsbColor.a);
    }

    /**
	 * <summary>HSBカラーをRGBカラーに変換する.</summary>
     * <returns>RGBカラー</returns>
	 */
    public Color ToColor()
    {
        return ToColor(this);
    }

    /**
	 * <summary>HSBカラーを文字列にする.</summary>
     * <returns>HSBカラーの文字列</returns>
	 */
    public override string ToString()
    {
        return "H:" + h + " S:" + s + " B:" + b;
    }

    /**
	 * <summary>2色を補間する.</summary>
     * <param name="a">HSBカラー1</param>
     * <param name="b">HSBカラー2</param>
     * <param name="t">補間値 </param>
     * <returns>HSBカラー</returns>
	 */
    public static HSBColor Lerp(HSBColor a, HSBColor b, float t)
    {
        float h, s;

        //check special case black (color.b==0): interpolate neither hue nor saturation!
        //check special case grey (color.s==0): don't interpolate hue!
        if (a.b == 0)
        {
            h = b.h;
            s = b.s;
        }
        else if (b.b == 0)
        {
            h = a.h;
            s = a.s;
        }
        else
        {
            if (a.s == 0)
            {
                h = b.h;
            }
            else if (b.s == 0)
            {
                h = a.h;
            }
            else
            {
                // works around bug with LerpAngle
                float angle = Mathf.LerpAngle(a.h * 360f, b.h * 360f, t);
                while (angle < 0f)
                    angle += 360f;
                while (angle > 360f)
                    angle -= 360f;
                h = angle / 360f;
            }
            s = Mathf.Lerp(a.s, b.s, t);
        }
        return new HSBColor(h, s, Mathf.Lerp(a.b, b.b, t), Mathf.Lerp(a.a, b.a, t));
    }

#if false
	public static void Test () {
		HSBColor color;

		color = new HSBColor(Color.red);
		Debug.Log("red: " + color);

		color = new HSBColor(Color.green);
		Debug.Log("green: " + color);

		color = new HSBColor(Color.blue);
		Debug.Log("blue: " + color);

		color = new HSBColor(Color.grey);
		Debug.Log("grey: " + color);

		color = new HSBColor(Color.white);
		Debug.Log("white: " + color);

		color = new HSBColor(new Color(0.4f, 1f, 0.84f, 1f));
		Debug.Log("0.4, 1f, 0.84: " + color);

		Debug.Log("164,82,84   .... 0.643137f, 0.321568f, 0.329411f  :" + ToColor(new HSBColor(new Color(0.643137f, 0.321568f, 0.329411f))));
	}
#endif
}
/*==========================================================================*/
/*==========================================================================*/
/*!
	@file	InGamePartyUnit.cs
	@brief	非アクティブオブジェクト一元化クラス
	@author Developer
	@date 	2012/10/04
*/
/*==========================================================================*/
/*==========================================================================*/

/*==========================================================================*/
/*		Using																*/
/*==========================================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M4u;

public class ContinuousBattleWave : M4uContextMonoBehaviour
{
    /*==========================================================================*/
    /*		var																	*/
    /*==========================================================================*/

    public Sprite[] m_Number;

    M4uProperty<Sprite> wave_num0 = new M4uProperty<Sprite>();
    public Sprite Wave_num0 { get { return wave_num0.Value; } set { wave_num0.Value = value; } }

    M4uProperty<Sprite> wave_num1 = new M4uProperty<Sprite>();
    public Sprite Wave_num1 { get { return wave_num1.Value; } set { wave_num1.Value = value; } }

    M4uProperty<Sprite> wave_max0 = new M4uProperty<Sprite>();
    public Sprite Wave_max0 { get { return wave_max0.Value; } set { wave_max0.Value = value; } }

    M4uProperty<Sprite> wave_max1 = new M4uProperty<Sprite>();
    public Sprite Wave_max1 { get { return wave_max1.Value; } set { wave_max1.Value = value; } }


    void Awake()
    {
        gameObject.GetComponent<M4uContextRoot>().Context = this;
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void setWaveNum(int num)
    {
        int num0 = num % 10;
        int num1 = num / 10;
        Wave_num0 = m_Number[num0];
        Wave_num1 = m_Number[num1];
    }

    public void setupWave(int num, int max)
    {
        setWaveNum(num);
        int max0 = max % 10;
        int max1 = max / 10;
        Wave_max0 = m_Number[max0];
        Wave_max1 = m_Number[max1];
    }
}

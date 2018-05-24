using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;

public class SceneQuestResultTest : Scene<SceneQuestResultTest>
{
    public QuestResult Bind;
    public int GetCoin = 0;
    public int GetTicket = 0;
    public int GetExp = 0;
    public int[] GetUnit = null;

    private int m_Status = 0;
    private Tweener m_Tweener = null;

    protected override void Start()
    {
        base.Start();
    }

    public override void OnInitialized()
	{
        Bind.AreaCategoryName = "エリアカテゴリ名";
        Bind.AreaNameText = "チュートリアルエリア";
        Bind.QuestNameText = "はじまりの赤";
        Bind.Coin = 0;
        Bind.Ticket = 0;
        Bind.Exp = 0;
        Bind.ExpLabel = "EXP";
        Bind.NextRank = 15;
        Bind.NextRankLabel = "あと";

        Bind.IsClearStatus = true;
        Bind.IsRankUp = false;
        Bind.IsMissionClear = false;
        Bind.IsMasterGradeUp = false;

        m_Tweener = DOTween.To(() => Bind.Coin, coin => Bind.Coin = coin, GetCoin, 0.5f);

        if (GetUnit != null)
        {
            for (int i = 0; i < GetUnit.Length; i++)
            {
                MasterDataParamChara _charMaster = MasterFinder<MasterDataParamChara>.Instance.Find(GetUnit[i]);
                if (_charMaster == null)
		        {
		            continue;
		        }

                int index = i;
                var model = new DropIconListItemModel((uint)index);

                model.OnShowedNext += () => 
                {
                    if (index >= GetUnit.Length - 1)
                        return;

                    Bind.GetUnitAt(index + 1).Appear();
                };

                model.OnAppeared += () =>
                {
                    if (index < GetUnit.Length - 1)
                        return;

                    Bind.LoopUnits();
                };

                var contex = new DropIconContex(_charMaster, false, false, model);
                Bind.DropIcons.Add(contex);
                Bind.AddUnit(model);
            }
        }
    }

    private void Update()
    {
        if (m_Tweener == null)
            return;

        switch (m_Status)
        {
            case 0:
                Bind.Show(()=> { m_Status++; });
                m_Status++;
                break;
            case 1:
                break;
            case 2:
                if (!m_Tweener.IsPlaying())
                {
                    m_Tweener.Kill();
                    m_Tweener = DOTween.To(() => Bind.Ticket, ticket => Bind.Ticket = ticket, GetTicket, 0.5f);
                    m_Status++;
                }
                break;
            case 3:
                if (!m_Tweener.IsPlaying())
                {
                    m_Tweener.Kill();
                    m_Tweener = DOTween.To(() => Bind.Exp, exp => Bind.Exp = exp, GetExp, 0.5f);
                    m_Status++;
                }
                break;
            case 4:
                if (!m_Tweener.IsPlaying())
                {
                    m_Tweener.Kill();
                    m_Status++;

                    Bind.IsDrops = true;
                }
                break;
        }
    }
}

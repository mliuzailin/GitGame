using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PartyParamListItemModel : ListItemModel
{
    private List<PartyMemberUnitListItemModel> m_units = new List<PartyMemberUnitListItemModel>();
    private List<ListItemModel> m_skills = new List<ListItemModel>();


    private System.Action _onUnitsAppeared = null;


    public PartyParamListItemModel(uint index) : base(index)
    {
        m_index = index;
    }


    public void AddUnit(PartyMemberUnitListItemModel unit)
    {
        unit.OnShowedNext += () =>
        {
            if (unit.index >= m_units.Count - 1)
                return;

            m_units[(int)unit.index + 1].Appear();
        };

        unit.OnAppeared += () =>
        {
            if (unit.index < m_units.Count - 1)
                return;

            foreach (var unitModel in m_units)
                unitModel.ShowStatus();

            if (_onUnitsAppeared != null)
                _onUnitsAppeared();
        };

        m_units.Add(unit);
    }
    public void AddSkill(ListItemModel skill)
    {
        m_skills.Add(skill);
    }

    public void ShowUnits(System.Action callback)
    {
        m_units[0].Appear();

        _onUnitsAppeared = callback;
    }

    public void ShowSkills(System.Action callback)
    {
        foreach (var skill in m_skills)
            skill.Appear();

        if (callback != null)
            callback();
    }

    public bool isUnitAppearingBegan()
    {
        if (m_units.Count == 0) return false;
        if (m_units[0].isAppearingBegan() == false) return false;
        return true;
    }
}

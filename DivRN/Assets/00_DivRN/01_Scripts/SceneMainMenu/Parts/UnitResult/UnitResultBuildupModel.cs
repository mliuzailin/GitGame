using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitResultBuildupModel
{
    private List<ListItemModel> m_materials = new List<ListItemModel>();

    public void AddMaterial(ListItemModel material)
    {
        material.Appear();
        material.SkipAppearing(); // TODO : 演出を入れるなら消す

        m_materials.Add(material);
    }

    public void RemoveLastMaterial()
    {
        int lastIndex = m_materials.Count - 1;
        m_materials.RemoveAt(lastIndex);
    }

    public void DisappearMaterial()
    {
        int lastIndex = m_materials.Count - 1;
        m_materials[lastIndex].Close();
        m_materials.RemoveAt(lastIndex);
    }

    public int LeftMaterialCount()
    {
        return m_materials.Count;
    }
}

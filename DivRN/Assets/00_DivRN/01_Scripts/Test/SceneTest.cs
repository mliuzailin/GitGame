using UnityEngine;
using System.Collections;

public class SceneTest<T> : Scene<T> where T : Component
{
    /// <summary>データの表示にダミーデータを使うかどうか</summary>
    [SerializeField]
    protected bool IsUseDummyData = true;
}

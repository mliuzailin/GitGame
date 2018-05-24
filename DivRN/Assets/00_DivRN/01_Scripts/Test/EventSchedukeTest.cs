using UnityEngine;
using M4u;

class EventSchedukeTest : MonoBehaviour
{
    User user = new User();

    void Start()
    {
        int id = 1;
        string name = "てんぷら";

        // データ更新
        user.Id = id;
        user.Name = name;

        // Viewへの反映必要なし！
    }
}

class User
{
    M4uProperty<int> id = new M4uProperty<int>();
    M4uProperty<string> name = new M4uProperty<string>();

    public int Id { get { return id.Value; } set { id.Value = value; } }
    public string Name { get { return name.Value; } set { name.Value = value; } }
}


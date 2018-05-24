using M4u;

public class DialogInfoText : M4uContextMonoBehaviour
{
    M4uProperty<string> message = new M4uProperty<string>();
    public string Message { get { return message.Value; } set { message.Value = value; } }

    private void Awake()
    {
        GetComponent<M4uContextRoot>().Context = this;
    }
}

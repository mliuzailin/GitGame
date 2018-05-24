using System.Collections;
using System.Collections.Generic;

public class SerialProcess
{
    private Queue<System.Action<System.Action>> _processes = new Queue<System.Action<System.Action>>();

    public SerialProcess Add(System.Action<System.Action> process)
    {
        _processes.Enqueue(process);
        return this;
    }

    public void Flush()
    {
        Next();
    }

    private void Next()
    {
        if (_processes.Count == 0)
            return;

        var process = _processes.Dequeue();
        process(() =>
        {
            Next();
        });
    }
}

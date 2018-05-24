using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEditor;
using NUnit.Framework;


public class ButtonBlockerTest
{
    [Test]
    public void Test()
    {
        Assert.False(ButtonBlocker.Instance.IsActive());
        ButtonBlocker.Instance.Block();
        Assert.True(ButtonBlocker.Instance.IsActive());
        ButtonBlocker.Instance.Unblock();
        Assert.False(ButtonBlocker.Instance.IsActive());


        ButtonBlocker.Instance.Block("test1");
        ButtonBlocker.Instance.Block("test2");
        ButtonBlocker.Instance.Block("test3");
        ButtonBlocker.Instance.Block("test4");
        Assert.True(ButtonBlocker.Instance.IsActive());
        ButtonBlocker.Instance.Unblock("test2");
        Assert.True(ButtonBlocker.Instance.IsActive());
        ButtonBlocker.Instance.Unblock("test3");
        Assert.True(ButtonBlocker.Instance.IsActive());
        ButtonBlocker.Instance.Unblock("test1");
        Assert.True(ButtonBlocker.Instance.IsActive());
        ButtonBlocker.Instance.Unblock("test2");
        Assert.True(ButtonBlocker.Instance.IsActive());
        ButtonBlocker.Instance.Unblock("test4");
        Assert.False(ButtonBlocker.Instance.IsActive());
    }
}

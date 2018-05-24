using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using UnityEngine;
using UnityEditor;
using NUnit.Framework;


public class SemaphoreTest
{
    [Test]
    public void Test()
    {
        bool isTestOver = false;
        bool isFinishSuccessfully = false;

        var semaphore = new Semaphore();

        new Thread(
            () =>
            {
                while (!isTestOver)
                {
                    semaphore.Tick();
                    Thread.Sleep(1);
                }
            }).Start();

        semaphore.Lock(() => { isFinishSuccessfully = true; });
        new Thread(
            () =>
            {
                semaphore.Unlock();
            }).Start();

        Thread.Sleep(16);

        Assert.True(isFinishSuccessfully);

        isTestOver = true;
    }
}

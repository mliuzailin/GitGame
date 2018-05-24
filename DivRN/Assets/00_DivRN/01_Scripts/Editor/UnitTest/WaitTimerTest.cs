using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEditor;
using NUnit.Framework;


public class WaitTimerTest
{
    [Test]
    public void Test()
    {
        bool isTestOver = false;
        bool isFinishSuccessfully = false;


        var timer = new WaitTimer(
            1.0f,
            () =>
            {
                isFinishSuccessfully = true;
            });


        new Thread(
            () =>
            {
                int oldMilsec = System.DateTime.Now.Millisecond;
                int deltaMilsec = 0;
                while (!isTestOver
                    && !isFinishSuccessfully)
                {
                    int nowMilsec = System.DateTime.Now.Millisecond;
                    if (nowMilsec < oldMilsec)
                        nowMilsec += 1000;

                    deltaMilsec = nowMilsec - oldMilsec;

                    timer.Tick(1.0f * deltaMilsec / 1000);

                    oldMilsec = nowMilsec;

                    Thread.Sleep(1);
                }
            }).Start();

        Thread.Sleep(3);
        Assert.False(isFinishSuccessfully);

        Thread.Sleep(1333);
        Assert.True(isFinishSuccessfully);

        isTestOver = true;
    }
}

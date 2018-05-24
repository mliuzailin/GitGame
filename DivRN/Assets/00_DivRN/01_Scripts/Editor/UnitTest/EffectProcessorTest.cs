using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using NUnit.Framework;


public class EffectProcessorTest
{
    [Test]
    public void Test()
    {
        var testCases = new List<TestCase>
        {
            new TestCase
            {
                title = "ordered test 1",
                raw = new List<int>
                {
                    1, 2, 3, 4
                },
                expected = new List<int>
                {
                    1, 2, 3, 4
                }
            },
            new TestCase
            {
                title = "reverse ordered test 1",
                raw = new List<int>
                {
                    5, 4, 3, 2, 1
                },
                expected = new List<int>
                {
                    5, 4, 3, 2, 1
                }
            }
        };

        foreach (var testCase in testCases)
        {
            int size = testCase.raw.Count;

            var result = new List<int>();

            for (int i = 0; i < size; i++)
            {
                int index = i;

                EffectProcessor.Instance.Register("order" + index, (System.Action finish) =>
                {
                    result.Add(testCase.raw[index]);
                    finish();
                });
            }

            EffectProcessor.Instance.Register("Last", (System.Action finish) =>
            {
                if (!IsSameList(result, testCase.expected))
                    Assert.Fail("Test : " + testCase.title + " failed.");

                finish();
            });

            var order = new List<string>();
            for (int i = 0; i < size; i++)
                order.Add("order" + i);
            order.Add("Last");

            EffectProcessor.Instance.PlayOrderly(order);
        }

        Assert.Pass();
    }

    private bool IsSameList(List<int> a, List<int> b)
    {
        if (a.Count != b.Count)
            return false;

        for (int i = 0; i < a.Count; i++)
            if (a[i] != b[i])
                return false;

        return true;
    }

    class TestCase
    {
        public string title;
        public List<int> raw;
        public List<int> expected;
    }
}

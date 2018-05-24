using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEditor;
using NUnit.Framework;


public class UnitGridManagerTest
{
    [Test]
    public void InitialConditionTest()
    {
        foreach (var testCase in m_initialConditionTestCases)
        {
            var unitGaridManager = new UnitGridManager
                (testCase.gridWidth,
                testCase.gridHeight,
                testCase.horizontalCount,
                testCase.verticalCount);
            unitGaridManager.UpdateElementCount(testCase.elementCount);

            Assert.True(unitGaridManager.GetVerticalOffset() == testCase.expectedVerticalOffset);
            Assert.True(unitGaridManager.GetScrollRectHeight() == testCase.expectedScrollRectHeight);
            foreach (var initialPosition in testCase.initialPositions)
                Assert.True(unitGaridManager.GetInitialPosition(initialPosition.index) == initialPosition.expected);
        }
    }

    [Test]
    public void ShiftTest()
    {
        foreach (var testCase in m_shiftTestCases)
        {
            var unitGaridManager = new UnitGridManager
                (testCase.gridWidth,
                testCase.gridHeight,
                testCase.horizontalCount,
                testCase.verticalCount);
            unitGaridManager.UpdateElementCount(testCase.elementCount);

            foreach (var verticalOffset in testCase.verticalOffsets)
            {
                unitGaridManager.Shift(verticalOffset.scrollValue, null, null);
                Assert.True(unitGaridManager.GetVerticalOffset() == verticalOffset.expected);
            }
        }
    }

    [Test]
    public void ShiftCallbackTest()
    {
        foreach (var testCase in m_shiftCallbackTestCases)
        {
            var unitGaridManager = new UnitGridManager
                (testCase.gridWidth,
                testCase.gridHeight,
                testCase.horizontalCount,
                testCase.verticalCount);
            unitGaridManager.UpdateElementCount(testCase.elementCount);

            foreach (var shiftCallback in testCase.shiftCallbacks)
            {
                var movedIndexes = new List<int>();
                var dataUpdatedIndexes = new List<int>();
                unitGaridManager.Shift(shiftCallback.scrollValue,
                    (index, delta) =>
                    {
                        movedIndexes.Add(index);
                    },
                    (index, delta) =>
                    {
                        dataUpdatedIndexes.Add(index);
                    });

                Assert.True(IsSamePrimitiveList(movedIndexes, shiftCallback.expectedMovedIndexes));
                Assert.True(IsSamePrimitiveList(dataUpdatedIndexes, shiftCallback.expectedUpdatedIndexes));
            }
        }
    }

    private bool IsSamePrimitiveList<T>(List<T> listA, List<T> listB)
    {
        if (listA.Count != listB.Count)
            return false;

        foreach (var element in listA)
            if (!listB.Contains(element))
                return false;

        return true;
    }


    // ===================================================
    class InitialConditionTestCase
    {
        public float gridWidth;
        public float gridHeight;
        public int horizontalCount;
        public int verticalCount;
        public int elementCount;

        public int expectedVerticalOffset;
        public float expectedScrollRectHeight;

        public class InitialPosition
        {
            public int index;
            public Vector2 expected;
        }
        public List<InitialPosition> initialPositions;
    }

    List<InitialConditionTestCase> m_initialConditionTestCases = new List<InitialConditionTestCase>
    {
        new InitialConditionTestCase
        {
            gridWidth = 100,
            gridHeight = 200,
            horizontalCount = 10,
            verticalCount = 20,
            elementCount = 1000,

            expectedVerticalOffset = 0,
            expectedScrollRectHeight = 20000,

            initialPositions = new List<InitialConditionTestCase.InitialPosition>
            {
                new InitialConditionTestCase.InitialPosition
                {
                    index = 0,
                    expected = new Vector2(400, 8000)
                }
            }
        }
    };



    // =================================================
    class ShiftTestCase
    {
        public float gridWidth;
        public float gridHeight;
        public int horizontalCount;
        public int verticalCount;
        public int elementCount;


        public class VerticalOffset
        {
            public float scrollValue;
            public int expected;
        }
        public List<VerticalOffset> verticalOffsets;
    }

    List<ShiftTestCase> m_shiftTestCases = new List<ShiftTestCase>
    {
        new ShiftTestCase
        {
            gridWidth = 100,
            gridHeight = 200,
            horizontalCount = 10,
            verticalCount = 20,
            elementCount = 1000,

            verticalOffsets = new List<ShiftTestCase.VerticalOffset>
            {
                new ShiftTestCase.VerticalOffset
                {
                    scrollValue = 1 * 200,
                    expected = 1
                },
                new ShiftTestCase.VerticalOffset
                {
                    scrollValue = 4 * 200,
                    expected = 4
                },
                new ShiftTestCase.VerticalOffset
                {
                    scrollValue = 9 * 200,
                    expected = 9
                },
                new ShiftTestCase.VerticalOffset
                {
                    scrollValue = 5 * 200,
                    expected = 5
                },
            }
        }
    };



    // ==============================================
    class ShiftCallbackTestCase
    {
        public float gridWidth;
        public float gridHeight;
        public int horizontalCount;
        public int verticalCount;
        public int elementCount;


        public class ShiftCallback
        {
            public float scrollValue;
            public List<int> expectedMovedIndexes;
            public List<int> expectedUpdatedIndexes;
        }
        public List<ShiftCallback> shiftCallbacks;
    }

    List<ShiftCallbackTestCase> m_shiftCallbackTestCases = new List<ShiftCallbackTestCase>
    {
        new ShiftCallbackTestCase
        {
            gridWidth = 100,
            gridHeight = 200,
            horizontalCount = 10,
            verticalCount = 20,
            elementCount = 1000,

            shiftCallbacks = new List<ShiftCallbackTestCase.ShiftCallback>
            {
                new ShiftCallbackTestCase.ShiftCallback
                {
                    scrollValue = 1 * 200,
                    expectedMovedIndexes = new List<int> { 199, 198, 197, 196, 195, 194, 193, 192, 191, 190 },
                    expectedUpdatedIndexes = new List<int> { 199, 198, 197, 196, 195, 194, 193, 192, 191, 190 },
                },
                new ShiftCallbackTestCase.ShiftCallback
                {
                    scrollValue = 0 * 200,
                    expectedMovedIndexes = new List<int> { 199, 198, 197, 196, 195, 194, 193, 192, 191, 190 },
                    expectedUpdatedIndexes = new List<int> { 199, 198, 197, 196, 195, 194, 193, 192, 191, 190 },
                },
                new ShiftCallbackTestCase.ShiftCallback
                {
                    scrollValue = 6 * 200,
                    expectedMovedIndexes = new List<int>
                    {
                        199, 198, 197, 196, 195, 194, 193, 192, 191, 190,
                        189, 188, 187, 186, 185, 184, 183, 182, 181, 180,
                        179, 178, 177, 176, 175, 174, 173, 172, 171, 170,
                        169, 168, 167, 166, 165, 164, 163, 162, 161, 160,
                        159, 158, 157, 156, 155, 154, 153, 152, 151, 150,
                        149, 148, 147, 146, 145, 144, 143, 142, 141, 140,
                    },
                    expectedUpdatedIndexes = new List<int>
                    {
                        199, 198, 197, 196, 195, 194, 193, 192, 191, 190,
                        189, 188, 187, 186, 185, 184, 183, 182, 181, 180,
                        179, 178, 177, 176, 175, 174, 173, 172, 171, 170,
                        169, 168, 167, 166, 165, 164, 163, 162, 161, 160,
                        159, 158, 157, 156, 155, 154, 153, 152, 151, 150,
                        149, 148, 147, 146, 145, 144, 143, 142, 141, 140,
                    },
                }
            }
        }
    };
}

using System.Collections;
using NUnit.Framework;

[TestFixture]
public class UnitIconAtlasLocatorTest
{
    [Test, TestCaseSource(typeof(UnitIconAtlasLocatorTestCase), "Name")]
    public string Name(string name, int index)
    {
        return new UnitIconAtlasLocator(name).name;
    }

    [Test, TestCaseSource(typeof(UnitIconAtlasLocatorTestCase), "InRange")]
    public bool InRange(string name, int index)
    {
        return new UnitIconAtlasLocator(name).InRange(index);
    }
}

class UnitIconAtlasLocatorTestCase
{
    public static IEnumerable Name
    {
        get
        {
            yield return new TestCaseData("iconpackunit_100_199", 111).Returns("iconpackunit_100_199")
                .SetName("Simple test");
        }
    }

    public static IEnumerable InRange
    {
        get
        {
            yield return new TestCaseData("iconpackunit_100_199", 111).Returns(true)
                .SetName("sets (iconpackunit_100_199, 111) expects (true)");
            yield return new TestCaseData("iconpackunit_100_199", 200).Returns(false)
                .SetName("sets (iconpackunit_100_199, 200) expects (false)");
        }
    }
}
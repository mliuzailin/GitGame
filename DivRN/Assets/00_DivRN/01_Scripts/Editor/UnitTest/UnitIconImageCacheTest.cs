using System.Collections.Generic;
using NUnit.Framework;

using UnityEngine;

[TestFixture]
public class UnitIconImageCacheTest
{
    [Test, TestCaseSource(typeof(UnitIconImageCacheTestCase), "GetAtlas")]
    public void GetAtlas(List<UnitIconImageCacheTestCase.AtlasCache> registerAtlases, List<string> removeAtlases, string argKey, UIAtlas expected)
    {
        var cache = new UnitIconImageCache();

        foreach (var registerAtlas in registerAtlases)
            cache.Register(registerAtlas.key, registerAtlas.atlas);

        foreach (var removeAtlasName in removeAtlases)
            cache.RemoveAtlas(removeAtlasName);

        var result = cache.GetAtlas(argKey);

        if (expected == null)
            Assert.True(result == null);
        else
            Assert.True(
                result != null
                && result == expected);
    }

    [Test]
    public void GetSprite()
    {
        foreach (var testCase in UnitIconImageCacheTestCase.GetSprite)
        {
            UnitIconImageCacheTestCase.InitializeSprites();

            var registerSprites = (List<UnitIconImageCacheTestCase.SpriteCache>)testCase.Arguments[0];
            var removeSprites = (List<string>)testCase.Arguments[1];
            var argKey = (string)testCase.Arguments[2];
            var expected = (Sprite)testCase.Arguments[3];

            var cache = new UnitIconImageCache();

            foreach (var registerSprite in registerSprites)
                cache.Register(registerSprite.key, registerSprite.sprite);

            foreach (var removeSpriteName in removeSprites)
                cache.RemoveSprite(removeSpriteName);

            var result = cache.GetSprite(argKey);

            if (expected == null)
                Assert.True(result == null, "Failed at " + testCase.TestName);
            else
                Assert.True(
                    result != null
                    && result == expected
                    , "Failed at " + testCase.TestName);
        }
    }

    [Test]
    public void Clear()
    {
        foreach (var testCase in UnitIconImageCacheTestCase.GetAtlas)
        {
            var registerAtlases = (List<UnitIconImageCacheTestCase.AtlasCache>)testCase.Arguments[0];

            var cache = new UnitIconImageCache();

            foreach (var registerAtlas in registerAtlases)
                cache.Register(registerAtlas.key, registerAtlas.atlas);

            cache.Clear();

            foreach (var registerAtlas in registerAtlases)
            {
                var result = cache.GetAtlas(registerAtlas.key);
                Assert.True(result == null);
            }

            break;
        }

        foreach (var testCase in UnitIconImageCacheTestCase.GetSprite)
        {
            UnitIconImageCacheTestCase.InitializeSprites();

            var registerSprites = (List<UnitIconImageCacheTestCase.SpriteCache>)testCase.Arguments[0];

            var cache = new UnitIconImageCache();

            foreach (var registerSprite in registerSprites)
                cache.Register(registerSprite.key, registerSprite.sprite);

            cache.Clear();

            foreach (var registerSprite in registerSprites)
            {
                var result = cache.GetSprite(registerSprite.key);
                Assert.True(result == null);
            }

            break;
        }

    }
}

public class UnitIconImageCacheTestCase
{
    public class AtlasCache
    {
        public string key;
        public UIAtlas atlas;
    }

    static UIAtlas atlas1 = new UIAtlas();
    static UIAtlas atlas2 = new UIAtlas();
    static UIAtlas atlas3 = new UIAtlas();
    static UIAtlas atlas4 = new UIAtlas();
    static UIAtlas atlas5 = new UIAtlas();

    public static IEnumerable<TestCaseData> GetAtlas
    {
        get
        {
            yield return new TestCaseData(
                new List<AtlasCache>
                {
                    new AtlasCache
                    {
                        key = "test 1",
                        atlas = atlas1
                    },
                    new AtlasCache
                    {
                        key = "test 2",
                        atlas = atlas2
                    },
                    new AtlasCache
                    {
                        key = "test 3",
                        atlas = atlas3
                    },
                    new AtlasCache
                    {
                        key = "test 4",
                        atlas = atlas4
                    },
                    new AtlasCache
                    {
                        key = "test 5",
                        atlas = atlas5
                    },
                },
                new List<string>
                {

                },
                "test 1",
                atlas1)
                .SetName("Simple test");
            yield return new TestCaseData(
                new List<AtlasCache>
                {
                    new AtlasCache
                    {
                        key = "test 1",
                        atlas = atlas1
                    },
                    new AtlasCache
                    {
                        key = "test 2",
                        atlas = atlas2
                    },
                    new AtlasCache
                    {
                        key = "test 3",
                        atlas = atlas3
                    },
                    new AtlasCache
                    {
                        key = "test 4",
                        atlas = atlas4
                    },
                    new AtlasCache
                    {
                        key = "test 5",
                        atlas = atlas5
                    },
                },
                new List<string>
                {
                    "test 1",
                    "test 2",
                    "test 4"
                },
                "test 3",
                atlas3)
                .SetName("Simple Remove test");
            yield return new TestCaseData(
                new List<AtlasCache>
                {
                    new AtlasCache
                    {
                        key = "test 1",
                        atlas = atlas1
                    },
                    new AtlasCache
                    {
                        key = "test 2",
                        atlas = atlas2
                    },
                    new AtlasCache
                    {
                        key = "test 3",
                        atlas = atlas3
                    },
                    new AtlasCache
                    {
                        key = "test 4",
                        atlas = atlas4
                    },
                    new AtlasCache
                    {
                        key = "test 5",
                        atlas = atlas5
                    },
                },
                new List<string>
                {
                    "test 1",
                    "test 2",
                    "test 4"
                },
                "test 4",
                null)
                .SetName("Simple Remove Expecting Null test");
        }
    }


    public class SpriteCache
    {
        public string key;
        public Sprite sprite;
    }

    public static Sprite sprite1;
    public static Sprite sprite2;
    public static Sprite sprite3;
    public static Sprite sprite4;
    public static Sprite sprite5;

    public static void InitializeSprites()
    {
        sprite1 = Sprite.Create(new Texture2D(100, 100), new Rect(0, 0, 100, 100), Vector2.zero);
        sprite2 = Sprite.Create(new Texture2D(100, 100), new Rect(0, 0, 100, 100), Vector2.zero);
        sprite3 = Sprite.Create(new Texture2D(100, 100), new Rect(0, 0, 100, 100), Vector2.zero);
        sprite4 = Sprite.Create(new Texture2D(100, 100), new Rect(0, 0, 100, 100), Vector2.zero);
        sprite5 = Sprite.Create(new Texture2D(100, 100), new Rect(0, 0, 100, 100), Vector2.zero);
    }

    public static IEnumerable<TestCaseData> GetSprite
    {
        get
        {
            yield return new TestCaseData(
                new List<SpriteCache>
                {
                    new SpriteCache
                    {
                        key = "test 1",
                        sprite = sprite1
                    },
                    new SpriteCache
                    {
                        key = "test 2",
                        sprite = sprite2
                    },
                    new SpriteCache
                    {
                        key = "test 3",
                        sprite = sprite3
                    },
                    new SpriteCache
                    {
                        key = "test 4",
                        sprite = sprite4
                    },
                    new SpriteCache
                    {
                        key = "test 5",
                        sprite = sprite5
                    },
                },
                new List<string>
                {

                },
                "test 1",
                sprite1)
                .SetName("Simple test");
            yield return new TestCaseData(
                new List<SpriteCache>
                {
                    new SpriteCache
                    {
                        key = "test 1",
                        sprite = sprite1
                    },
                    new SpriteCache
                    {
                        key = "test 2",
                        sprite = sprite2
                    },
                    new SpriteCache
                    {
                        key = "test 3",
                        sprite = sprite3
                    },
                    new SpriteCache
                    {
                        key = "test 4",
                        sprite = sprite4
                    },
                    new SpriteCache
                    {
                        key = "test 5",
                        sprite = sprite5
                    },
                },
                new List<string>
                {
                    "test 1",
                    "test 2",
                    "test 4"
                },
                "test 3",
                sprite3)
                .SetName("Simple Remove test");
            yield return new TestCaseData(
                new List<SpriteCache>
                {
                    new SpriteCache
                    {
                        key = "test 1",
                        sprite = sprite1
                    },
                    new SpriteCache
                    {
                        key = "test 2",
                        sprite = sprite2
                    },
                    new SpriteCache
                    {
                        key = "test 3",
                        sprite = sprite3
                    },
                    new SpriteCache
                    {
                        key = "test 4",
                        sprite = sprite4
                    },
                    new SpriteCache
                    {
                        key = "test 5",
                        sprite = sprite5
                    },
                },
                new List<string>
                {
                    "test 1",
                    "test 2",
                    "test 4"
                },
                "test 4",
                null)
                .SetName("Simple Remove Expecting Null test");
        }
    }
}
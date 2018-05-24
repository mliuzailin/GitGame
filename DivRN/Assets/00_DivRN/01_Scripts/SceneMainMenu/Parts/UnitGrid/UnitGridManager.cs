using System.Collections.Generic;

using UnityEngine;

public class UnitGridManager
{
    private float GRID_WIDTH = 110;
    private float GRID_HEIGHT = 130;
    private int HORIZONTAL_COUNT = 5;
    private int VIRTICAL_COUNT = 16;
    private float GRIDS_RECT_HEIGHT = 0;
    private int GRID_COUNT = 0;
    private int GRID_SHIFT_LINE = 0;
    private float GRID_SHIFT_HEIGHT = 0;

    private float m_scrollHeight = 0;
    private float m_oldScrollHeight = 0;
    private int m_elementCount = 0;
    private float m_currentScrolledValue = 0;
    private float m_oldScrolledValue = 0;

    public UnitGridManager(
        float gridWidth,
        float gridHeight,
        int horizontalCount,
        int verticalCount)
    {
        GRID_WIDTH = gridWidth;
        GRID_HEIGHT = gridHeight;
        HORIZONTAL_COUNT = horizontalCount;
        VIRTICAL_COUNT = verticalCount;
        GRIDS_RECT_HEIGHT = VIRTICAL_COUNT * GRID_HEIGHT;
        GRID_COUNT = HORIZONTAL_COUNT * VIRTICAL_COUNT;

        GRID_SHIFT_LINE = 8;
        GRID_SHIFT_HEIGHT = (GRID_HEIGHT * GRID_SHIFT_LINE) + (GRID_HEIGHT / 2);
    }

    // ================================================ APIs

    /// <summary>
    /// スクロールに伴ってグリッドの配置を変更するときに呼ぶ
    /// </summary>
    /// <param name="scrollValue">
    /// 現在のスクロール量
    /// </param>
    /// <param name="onGridMoved">
    /// 表示しているグリッドの位置を変更する必要が出たときに呼ばれるコールバック、引数はグリッドのインデックスと変化量
    /// </param>
    /// <param name="onGridDataUpdated">
    /// 表示しているグリッドの表示内容を変更する必要が出たときに呼ばれるコールバック、引数はグリッドのインデックスと変化量
    /// </param>
    public void Shift(
        float scrollValue,
        System.Action<int, int> onGridMoved,
        System.Action<int, int> onGridDataUpdated)
    {
        m_currentScrolledValue = scrollValue;

        int currentVirticalIndexOffset = GetVerticalOffset(m_currentScrolledValue);
        int oldVirticalIndexOffset = GetVerticalOffset(m_oldScrolledValue);
        int delta = currentVirticalIndexOffset - oldVirticalIndexOffset;

        if (delta > 0)
        {
            RepositionTopListItems(
                currentVirticalIndexOffset,
                delta,
                onGridMoved,
                onGridDataUpdated);
        }
        else if (delta < 0)
        {
            RepositionBottomListItems(
                currentVirticalIndexOffset,
                delta,
                onGridMoved,
                onGridDataUpdated);
        }

        m_oldScrolledValue = m_currentScrolledValue;
    }

    /// <summary>
    /// いまどれだけグリッドを下方向に再配置しているか
    /// </summary>
    /// <returns></returns>
    public int GetVerticalOffset()
    {
        return GetVerticalOffset(m_currentScrolledValue);
    }

    /// <summary>
    /// スククロールビューに配置した要素の数が変更されたときに呼ぶ
    /// </summary>
    /// <param name="elementCount">
    /// 変更後の要素の数
    /// </param>
    public void UpdateElementCount(int elementCount)
    {
        m_elementCount = elementCount;
        m_scrollHeight = Mathf.Ceil((float)elementCount / HORIZONTAL_COUNT) * GRID_HEIGHT;
    }

    /// <summary>
    /// スクロールビューの大きさ(グリッドの配置によって決まる)
    /// </summary>
    /// <returns></returns>
    public float GetScrollRectHeight()
    {
        return m_scrollHeight;
    }

    /// <summary>
    /// 配置しているグリッド全体の高さ合計
    /// </summary>
    /// <returns></returns>
    public float GetGridsRectHeight()
    {
        return GRIDS_RECT_HEIGHT;
    }

    /// <summary>
    /// 配置しているグリッドの数
    /// </summary>
    /// <returns></returns>
    public int GetGridCount()
    {
        return GRID_COUNT;
    }

    /// <summary>
    /// スクロールや再配置しない状態でのグリッドの縦何列目か
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int GetVirticalIndex(int index)
    {
        return (index - GetHorizontalIndex(index)) / HORIZONTAL_COUNT;
    }

    private int GetHorizontalIndex(int index)
    {
        return index % HORIZONTAL_COUNT;
    }

    /// <summary>
    /// 配置するグリッドの初期位置
    /// </summary>
    /// <param name="index">
    /// グリッドのインデックス(0～グリッドの数-1)
    /// </param>
    /// <returns></returns>
    public Vector2 GetInitialPosition(int index)
    {
        m_oldScrollHeight = m_scrollHeight;
        float x = ((HORIZONTAL_COUNT - 1) - GetHorizontalIndex(index)) * GRID_WIDTH - (int)(HORIZONTAL_COUNT / 2) * GRID_WIDTH;
        float y = GetVirticalIndex(index) * GRID_HEIGHT + (m_scrollHeight / 2) - GRIDS_RECT_HEIGHT + GRID_SHIFT_HEIGHT;

        return new Vector2(x, y);
    }

    // スクロールに合わせて場所を入れ替えた後のindex
    public int GetModifiedIndex(int index)
    {
        // indexをリバースする
        int maxIndex = VIRTICAL_COUNT * HORIZONTAL_COUNT - 1;
        int gridsCount = VIRTICAL_COUNT * HORIZONTAL_COUNT;
        int loopCount = GetVirticalIndexOffsetLoopCount(index);

        int reversindex = maxIndex + gridsCount * loopCount - index;

        // スタート位置をずらす
        return reversindex - (HORIZONTAL_COUNT * GRID_SHIFT_LINE);
    }

    // ================================================

    private int GetVerticalOffset(float scrolledValue)
    {
        int loopCount = 0;
        float targetScrollValue = scrolledValue;
        while (targetScrollValue > GRIDS_RECT_HEIGHT)
        {
            targetScrollValue -= GRIDS_RECT_HEIGHT;
            loopCount++;
        }

        for (int i = VIRTICAL_COUNT; i > 0; i--)
        {
            float top = i * GRID_HEIGHT;
            float bottom = (i - 1) * GRID_HEIGHT;

            if (top >= targetScrollValue &&
                targetScrollValue > bottom)
            {
                return i + loopCount * VIRTICAL_COUNT;
            }
        }

        return 0;
    }

    private void RepositionTopListItems(
        int virticalIndexOffset,
        int delta,
        System.Action<int, int> onGridMoved,
        System.Action<int, int> onGridDataUpdated,
        int loopCount = 0)
    {
        Debug.Assert(delta > 0, "call RepositionTopListItems with no difference.");

        if (delta > VIRTICAL_COUNT)
        {
            if (onGridMoved != null)
            {
                for (int i = 0; i < GRID_COUNT; i++)
                {
                    onGridMoved(i, delta);
                }
            }

            RepositionTopListItems(
                virticalIndexOffset,
                delta - VIRTICAL_COUNT,
                onGridMoved,
                onGridDataUpdated,
                loopCount + 1);

            return;
        }

        var targetVirticalIndex = VIRTICAL_COUNT * GetVirticalLoopCount(virticalIndexOffset) - virticalIndexOffset;
        var trimmed = TrimIndex(targetVirticalIndex, VIRTICAL_COUNT);

        for (int i = 0; i < GRID_COUNT; i++)
        {
            var index = i;
            var virticalIndex = GetVirticalIndex(index);

            if (IsInRange(virticalIndex, trimmed.index, delta) == false)
            {
                if (loopCount > 0)
                {
                    if (onGridDataUpdated != null)
                    {
                        onGridDataUpdated(index, delta);
                    }
                }

                continue;
            }

            if (onGridMoved != null)
            {
                onGridMoved(index, delta);
            }

            if (onGridDataUpdated != null)
            {
                onGridDataUpdated(index, delta);
            }
        }
    }

    private void RepositionBottomListItems(
        int virticalIndexOffset,
        int delta,
        System.Action<int, int> onGridMoved,
        System.Action<int, int> onGridDataUpdated,
        int loopCount = 0)
    {
        Debug.Assert(delta < 0, "call RepositionTopListItems with no difference.");

        if (delta < -1 * VIRTICAL_COUNT)
        {
            if (onGridMoved != null)
            {
                for (int i = 0; i < GRID_COUNT; i++)
                {
                    onGridMoved(i, delta);
                }
            }

            RepositionBottomListItems(
                virticalIndexOffset,
                delta + VIRTICAL_COUNT,
                onGridMoved,
                onGridDataUpdated,
                loopCount + 1);

            return;
        }

        var targetVirticalIndex = -1 * (virticalIndexOffset + 1);
        var trimmed = TrimIndex(targetVirticalIndex, VIRTICAL_COUNT);

        for (int i = 0; i < GRID_COUNT; i++)
        {
            var index = i;
            var virticalIndex = GetVirticalIndex(index);

            if (IsInRange(virticalIndex, trimmed.index, delta) == false)
            {
                if (loopCount > 0)
                {
                    if (onGridDataUpdated != null)
                    {
                        onGridDataUpdated(index, delta);

                    }
                }

                continue;
            }

            if (onGridMoved != null)
            {
                onGridMoved(index, delta);
            }

            if (onGridDataUpdated != null)
            {
                onGridDataUpdated(index, delta);
            }
        }
    }

    private int GetVirticalLoopCount(int virticalIndexOffset)
    {
        int loopCount = 0;

        while (virticalIndexOffset > VIRTICAL_COUNT)
        {
            virticalIndexOffset -= VIRTICAL_COUNT;
            loopCount++;
        }

        return loopCount;
    }

    private int GetVirticalIndexOffset(float scrollValue)
    {
        int loopCount = 0;
        float targetScrollValue = scrollValue;
        while (targetScrollValue > GRIDS_RECT_HEIGHT)
        {
            targetScrollValue -= GRIDS_RECT_HEIGHT;
            loopCount++;
        }

        for (int i = VIRTICAL_COUNT; i > 0; i--)
        {
            var top = i * GRID_HEIGHT;
            var bottom = (i - 1) * GRID_HEIGHT;

            if (top >= targetScrollValue &&
                targetScrollValue > bottom)
            {
                return i + loopCount * VIRTICAL_COUNT;
            }
        }

        return 0;
    }

    private int GetVirticalIndexOffsetLoopCount(int index)
    {
        int loopCount = 0;
        int tempIndex = index;
        int virticalIndexOffset = GetVirticalIndexOffset(m_currentScrolledValue);

        loopCount += GetVirticalLoopCount(virticalIndexOffset);
        if (virticalIndexOffset > 0 &&
            virticalIndexOffset % VIRTICAL_COUNT == 0 ||
            tempIndex > (VIRTICAL_COUNT - virticalIndexOffset % VIRTICAL_COUNT) * HORIZONTAL_COUNT - 1)
        {
            loopCount++;
        }

        return loopCount;
    }

    private class TrimResult
    {
        public int index;
        public int offset;
    }

    private TrimResult TrimIndex(int index, int limit)
    {
        int trimmedIndex = index;
        int trimmingOffset = 0;

        while (trimmedIndex < 0)
        {
            trimmedIndex += limit;
            trimmingOffset++;
        }

        while (trimmedIndex >= limit)
        {
            trimmedIndex -= limit;
            trimmingOffset--;
        }

        return new TrimResult
        {
            index = trimmedIndex,
            offset = trimmingOffset
        };
    }

    private bool IsInRange(int index, int rangeOriginal, int rangeOffset)
    {
        int range = rangeOriginal + rangeOffset;

        if (range >= VIRTICAL_COUNT)
        {
            if (IsInRange(index, rangeOriginal - VIRTICAL_COUNT, rangeOffset))
            {
                return true;
            }
        }

        if (range < 0)
        {
            if (IsInRange(index, rangeOriginal + VIRTICAL_COUNT, rangeOffset))
            {
                return true;
            }
        }

        if (rangeOffset > 0)
        {
            if (index < rangeOriginal ||
                index >= range)
            {
                return false;
            }
        }
        else if (rangeOffset < 0)
        {
            if (index > rangeOriginal ||
                index <= range)
            {
                return false;
            }
        }

        return true;
    }
}

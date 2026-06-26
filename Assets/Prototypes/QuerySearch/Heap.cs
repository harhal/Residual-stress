using System;

public class Heap<T> where T : IComparable<T>
{
    private T[] items;
    public int Count { get; private set; }

    public Heap()
    {
        items = new T[64];
        Count = 0;
    }

    public void Clear()
    {
        Count = 0;
    }

    public void Push(T item)
    {
        if (Count == items.Length)
        {
            Array.Resize(ref items, Count * 2);
        }

        int idx = Count;
        Count++;

        while (idx > 0)
        {
            int parentIdx = getParentIdx(idx);
            if (items[parentIdx].CompareTo(item) >= 0)
            {
                break;
            }

            items[idx] = items[parentIdx];
            idx = parentIdx;
        }

        items[idx] = item;
    }

    public T Pop()
    {
        if (Count <= 0)
        {
            throw new IndexOutOfRangeException();
        }

        int idx = 0;
        T result = items[idx];
        Count--;

        while (idx < Count)
        {
            int minItemIdx = Count;

            int lChildIdx = getLChildIdx(idx);
            for (int i = 0; i < 2; i++)
            {
                int ChildIdx = lChildIdx + i;
                if (ChildIdx >= Count)
                {
                    continue;
                }

                if (items[minItemIdx].CompareTo(items[ChildIdx]) > 0)
                {
                    minItemIdx = ChildIdx;
                }
            }

            items[idx] = items[minItemIdx];
            idx = minItemIdx;
        }

        return result;
    }

    public T Peek()
    {
        if (Count <= 0)
        {
            throw new IndexOutOfRangeException();
        }
        return items[0];
    }

    private static int getParentIdx(int idx)
    {
        return (idx - 1) >> 1;
    }

    private static int getLChildIdx(int idx)
    {
        return idx << 1 + 1;
    }
}
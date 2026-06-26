using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SquaresSumSearch
{
    struct SqaresPair : IComparable<SqaresPair>
    {
        public readonly Vector2Int v;
        readonly ulong comparationKey;

        public SqaresPair(Vector2Int pair)
        {
            v = pair;
            comparationKey = (uint)pair.sqrMagnitude << 32 | (uint)pair.x;
        }

        public int CompareTo(SqaresPair obj)
        {
            if (comparationKey == obj.comparationKey)
            {
                return 0;
            }            
            else if (comparationKey > obj.comparationKey)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }

    static Heap<SqaresPair> SearchHeap = new Heap<SqaresPair>();
    static List<SqaresPair> CashedValues = new List<SqaresPair>();

    static public Vector2Int GetNextPair(Vector2Int pair)
    {
        Vector2Int swapedPair = new Vector2Int(pair.y, pair.x);
        if (pair.x < pair.y)
        {
            return swapedPair;
        }

        SqaresPair searchAfter = new SqaresPair(swapedPair);
        int cachedIdx = CashedValues.BinarySearch(searchAfter);
        if (cachedIdx >= 0 && cachedIdx + 1 < CashedValues.Count)
        {
            return CashedValues[cachedIdx + 1].v;
        }

        if (SearchHeap.Count <= 0)
        {
            SearchHeap.Push(new SqaresPair(Vector2Int.zero));
        }
        
        do
        {
            SqaresPair nextPair = SearchHeap.Pop();
            SearchHeap.Push(new SqaresPair(nextPair.v + Vector2Int.up));
            if (nextPair.v.x == nextPair.v.y)
            {
                SearchHeap.Push(new SqaresPair(nextPair.v + Vector2Int.one));
            }

        } while (CashedValues.Last().CompareTo(searchAfter) <= 0);

        return CashedValues.Last().v;
    }

    static public IEnumerable<Vector2Int> Enumerate()
    {
        Vector2Int nextPair = Vector2Int.zero;
        while (true)
        {
            yield return nextPair;
            nextPair = GetNextPair(nextPair);
        }
    }
}
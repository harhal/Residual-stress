using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct QueryObjsBucketAdress
{
    public ulong ChunkId;
    public int BuckedId;

    public QueryObjsBucketAdress(ulong ChunkId, int BuckedId)
    {
        this.ChunkId = ChunkId;
        this.BuckedId = BuckedId;
    }

    public QueryObjsBucketAdress(Vector2 GridSpacePos, int ChunkBucketsCountSqrt)
    {
        Vector2Int ChunkAdress = VHelper.Floor(GridSpacePos);
        Vector2 ChunkSpaceBucketPos = VHelper.TopFraction(GridSpacePos) * ChunkBucketsCountSqrt;
        Vector2Int BucketAdress = VHelper.Floor(ChunkSpaceBucketPos);
        ChunkId = (uint)ChunkAdress.x << 32 | (uint)ChunkAdress.y;
        BuckedId = BucketAdress.x * ChunkBucketsCountSqrt + BucketAdress.y;
    }

    public static bool operator==(QueryObjsBucketAdress FirstAdress, QueryObjsBucketAdress SecondAdress)
    {
        return FirstAdress.BuckedId == SecondAdress.BuckedId &&
            FirstAdress.ChunkId == SecondAdress.ChunkId;
    }

    public static bool operator!=(QueryObjsBucketAdress FirstAdress, QueryObjsBucketAdress SecondAdress)
    {
        return FirstAdress.BuckedId != SecondAdress.BuckedId ||
            FirstAdress.ChunkId != SecondAdress.ChunkId;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public struct BucketPolarResult
{
    public QueryObjsBucket Bucket { get; private set; }
    public float MinDist { get; private set; }
    public float MaxDist { get; private set; }
    public float MinAngle { get; private set; }
    public float MaxAngle { get; private set; }

    public BucketPolarResult(QueryObjsBucket bucket, float minDist, float maxDist, float minAngle, float maxAngle, int quarter)
    {
        Bucket = bucket;
        MinDist = minDist;
        MaxDist = maxDist;
        float quarterOffset = Mathf.PI * 0.5f * quarter;
        MinAngle = quarterOffset + minAngle;
        MaxAngle = quarterOffset + maxAngle;
    }
}

public class QueryObjsBucket: List<SearchableObj>
{
    public static QueryObjsBucket EmptyBucket = new QueryObjsBucket();
}

class QueryObjsChunk
{
    QueryObjsBucket[] Buckets;

    public QueryObjsChunk(int BucketsCountSqrt)
    {
        int BucketsCount = BucketsCountSqrt * BucketsCountSqrt;
        Buckets = new QueryObjsBucket[BucketsCount];
        for (int i = 0; i < BucketsCount; i++)
        {
            Buckets[i] = new QueryObjsBucket();
        }
    }

    public QueryObjsBucket GetBucket(int BuckedId)
    {
        if (BuckedId < 0 || BuckedId >= Buckets.Count())
        {
            return QueryObjsBucket.EmptyBucket;
        }

        return Buckets[BuckedId];
    }
}

public static class QueryObjsRegister
{
    private const float ChunkSize = 100f;
    private const int ChunkBucketsCountSqrt = 10;
    private const int SearchMaxBudget = 10000;
    private static int SearchBudget = SearchMaxBudget;
    private static Dictionary<ulong, QueryObjsChunk> Chunks = new Dictionary<ulong, QueryObjsChunk>();
    private static List<ObjectsSearch> PassesInProgress = new List<ObjectsSearch>();

    private static Vector2 ToChunkGridSpace(Vector3 pos)
    {
        return new Vector2(pos.x, pos.z) / ChunkSize;
    }

    private static QueryObjsBucketAdress GetChunkBucketAdress(Vector3 pos)
    {
        Vector2 GridSpaceChunkPos = ToChunkGridSpace(pos);
        return new QueryObjsBucketAdress(GridSpaceChunkPos, ChunkBucketsCountSqrt);
    }

    private static QueryObjsBucket GetBucket(QueryObjsBucketAdress adress)
    {
        QueryObjsChunk Chunk;
        if (!Chunks.TryGetValue(adress.ChunkId, out Chunk))
        {
            return QueryObjsBucket.EmptyBucket;
        }

        return Chunk.GetBucket(adress.BuckedId);
    }

    public static void Clear()
    {
        Chunks = new Dictionary<ulong, QueryObjsChunk>();
        PassesInProgress = new List<ObjectsSearch>();
    }

    public static void Update()
    {
        SearchBudget = SearchMaxBudget;
        for (int idx = 0; idx < PassesInProgress.Count; idx++)
        {
            int allocatedBudget = SearchBudget / PassesInProgress.Count;
            ObjectsSearch filter = PassesInProgress[idx];
            for (; allocatedBudget > 0; allocatedBudget--)
            {
                if (!filter.Passes.MoveNext())
                {
                    break;
                }
            }
        }

        PassesInProgress.RemoveAll((ObjectsSearch filter) => filter.Complete);
    }

    public static void RegisterObject(SearchableObj obj)
    {
        QueryObjsBucketAdress BucketAdress = GetChunkBucketAdress(obj.transform.position);

        if (!Chunks.ContainsKey(BucketAdress.ChunkId))
        {
            Chunks.Add(BucketAdress.ChunkId, new QueryObjsChunk(ChunkBucketsCountSqrt));
        }

        QueryObjsChunk Chunk = Chunks[BucketAdress.ChunkId];
        Chunk.GetBucket(BucketAdress.BuckedId).Add(obj);
        obj.BucketAdress = BucketAdress;
    }

    public static void UpdateObjectBucket(SearchableObj obj)
    {
        QueryObjsBucketAdress BucketAdress = GetChunkBucketAdress(obj.transform.position);

        if (BucketAdress == obj.BucketAdress)
        {
            return;
        }

        GetBucket(obj.BucketAdress).Remove(obj);

        RegisterObject(obj);
    }

    public static IEnumerable<BucketPolarResult> EnumerateNearBuckets(Vector3 center)
    {
        Vector2 offsetToCenter = Vector2.one * 0.5f;

        Vector2 centerBucket = ToChunkGridSpace(center) / ChunkBucketsCountSqrt;
        Vector2 inBucketPos = VHelper.TopFraction(centerBucket);

        Vector2Int zeroBucket = VHelper.Floor(centerBucket) + VHelper.Round(inBucketPos);
        Vector2Int[] zeroBucketQuarterOffset = {Vector2Int.zero, Vector2Int.down, -Vector2Int.one, Vector2Int.left};

        foreach (Vector2Int bucketOffset in SquaresSumSearch.Enumerate())
        {
            float minDist = bucketOffset.magnitude * ChunkSize;
            float maxDist = (bucketOffset + Vector2Int.one).magnitude * ChunkSize;
            float minAngle = Mathf.Atan2(bucketOffset.x, bucketOffset.y + 1);
            float maxAngle = Mathf.Atan2(bucketOffset.x + 1, bucketOffset.y);

            for (int quarter = 0; quarter < 4; quarter++)
            {
                Vector2 bucketCenter = zeroBucket + zeroBucketQuarterOffset[quarter] + VHelper.Rotate90(bucketOffset, quarter) + offsetToCenter;
                QueryObjsBucket bucket = GetBucket(new QueryObjsBucketAdress(bucketCenter, ChunkBucketsCountSqrt));
                BucketPolarResult polarResult = new BucketPolarResult(bucket, minDist, maxDist, minAngle, maxAngle, quarter);
                yield return polarResult;
            }
        }
    }

    public static void RequestSearch(ObjectsSearch search, Vector3 center)
    {
        ObjectsSearch.StartSearch(search, center, EnumerateNearBuckets(center));
        if (!search.Complete)
        {
            PassesInProgress.Add(search);
        }
    }
}

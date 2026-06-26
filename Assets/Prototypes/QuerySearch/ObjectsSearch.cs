using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectsSearch
{
    protected enum BucketPreCheckResult
    {
        Proceede,
        Skip,
        Complete
    }

    public ObjectsSearch()
    {
        Complete = false;
    }

    public bool Complete { get; private set; }
    public IEnumerator Passes { get; private set; }

    protected abstract BucketPreCheckResult BucketPreCheck(BucketPolarResult bucket, Vector3 zeroPoint);

    protected abstract bool FilterObject(SearchableObj obj, Vector3 zeroPoint);

    public static void StartSearch(ObjectsSearch search, Vector3 zeroPoint, IEnumerable<BucketPolarResult> buckets)
    {
        search.Passes = GetSearchEnumeration(search, zeroPoint, buckets);
    }

    public static IEnumerator GetSearchEnumeration(ObjectsSearch search, Vector3 zeroPoint, IEnumerable<BucketPolarResult> buckets)
    {
        foreach (BucketPolarResult bucket in buckets)
        {
            BucketPreCheckResult precheck = search.BucketPreCheck(bucket, zeroPoint);
            yield return null;

            if (precheck == BucketPreCheckResult.Proceede)
            {
                foreach (SearchableObj obj in bucket.Bucket)
                {
                    search.FilterObject(obj, zeroPoint);
                    yield return null;
                }
            }
            else if (precheck == BucketPreCheckResult.Complete)
            {
                search.Complete = true;
                break;
            }
        }
    }
}
using System;
using System.Collections;
using UnityEditor.Search;
using UnityEngine;

public class FindClosestEnemy: ObjectsSearch
{
    public FindClosestEnemy(): base()
    {
        
    }

    protected override BucketPreCheckResult BucketPreCheck(BucketPolarResult bucket, Vector3 zeroPoint)
    {
        throw new NotImplementedException();
    }

    protected override bool FilterObject(SearchableObj obj, Vector3 zeroPoint)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class BrainTask_FindEnemy : BrainTask
{
    FindClosestEnemy search;

    public BrainTask_FindEnemy()
    {
        search = new FindClosestEnemy();
    }

    protected override IEnumerator Execute(MateBrain brain)
    {
        QueryObjsRegister.RequestSearch(search, brain.transform.position);
        while (!search.Complete)
        {
            yield return null;
        }

        
    }
}
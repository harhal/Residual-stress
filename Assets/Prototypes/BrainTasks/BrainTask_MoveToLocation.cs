using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class BrainTask_MoveToLocation : BrainTask
{
    Vector3 MoveTo;

    public BrainTask_MoveToLocation(Vector3 moveTo)
    {
        MoveTo = moveTo;
    }

    protected override IEnumerator Execute(MateBrain brain)
    {
        brain.CachedAgent.SetDestination(MoveTo);
        while (brain.CachedAgent.remainingDistance <= 0)
        {
            yield return null;
        }

        yield return null;
    }
}
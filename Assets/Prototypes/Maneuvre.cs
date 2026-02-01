

using System.Collections;
using UnityEngine;

//[CreateAssetMenu(fileName = "Maneuvre", menuName = "ScriptableObjects/CreateManeuvre", order = 1)]
public abstract class Maneuvre
{
    public abstract IEnumerator Execute(Squad squad);
}

public abstract class PlayerManeuvre : Maneuvre
{
}

public class PlayerManeuvre_ExploreArea: PlayerManeuvre
{
    Vector3 AreaLocation;

    public PlayerManeuvre_ExploreArea(Vector3 areaLocation)
    {
        AreaLocation = areaLocation; 
    }
    public override IEnumerator Execute(Squad squad)
    {
        MateBrain frontMan = squad.GetMate(SquaddieRole.FrontMan);
        frontMan.RunTask(new BrainTask_MoveToLocation(AreaLocation + Vector3.forward));

        MateBrain bass = squad.GetMate(SquaddieRole.Bass);
        bass.RunTask(new BrainTask_MoveToLocation(AreaLocation + Vector3.left));

        MateBrain sax = squad.GetMate(SquaddieRole.Sax);
        sax.RunTask(new BrainTask_MoveToLocation(AreaLocation + Vector3.right));

        MateBrain drum = squad.GetMate(SquaddieRole.Drum);
        drum.RunTask(new BrainTask_MoveToLocation(AreaLocation + Vector3.back));

        while (frontMan.HasRunningTask()
            || bass.HasRunningTask()
            || sax.HasRunningTask()
            || drum.HasRunningTask())
        {
            yield return null;
        }
        yield return null;
    }
}
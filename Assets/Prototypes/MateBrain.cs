using System.Collections;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class MateBrain : MonoBehaviour
{    
    BrainTask RunningTask;
    public NavMeshAgent CachedAgent
    {
        private set; 
        get;
    }

    public void RunTask(BrainTask Task)
    {
        if (RunningTask != null)
        {
            StopRunningTask();
        }
        RunningTask = Task;
        StartCoroutine(CoroutineRunTask());
    }

    public bool HasRunningTask()
    {
        return RunningTask != null;
    }

    public void StopRunningTask()
    {
        RunningTask.StopExecuteCoroutine();
        RunningTask = null;
    }

    IEnumerator CoroutineRunTask()
    {
        yield return RunningTask.StartExecuteCoroutine(this);
        RunningTask = null;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        CachedAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

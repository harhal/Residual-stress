using System;
using System.Collections;
using UnityEngine;

[Serializable]
public abstract class BrainTask
{
    protected abstract IEnumerator Execute(MateBrain brain);

    private Coroutine _execute_coroutine;
    private MateBrain _brain;

    public IEnumerator StartExecuteCoroutine(MateBrain brain)
    {
        _brain = brain;
        _execute_coroutine = brain.StartCoroutine(Execute(brain));
        yield return _execute_coroutine;
    }

    public bool IsActive()
    {
        return _execute_coroutine != null;
    }

    public void StopExecuteCoroutine()
    {
        _brain.StopCoroutine(_execute_coroutine);
        _execute_coroutine = null;
    }
}
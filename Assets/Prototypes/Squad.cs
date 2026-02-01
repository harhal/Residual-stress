using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum SquaddieRole
{
    FrontMan,
    Bass,
    Sax,
    Drum
}

public class Squad : MonoBehaviour
{
    Dictionary<SquaddieRole, MateBrain> Quartet = new Dictionary<SquaddieRole, MateBrain>
        {
            {SquaddieRole.FrontMan, null}, 
            {SquaddieRole.Bass, null}, 
            {SquaddieRole.Sax, null}, 
            {SquaddieRole.Drum, null}
        };

    public MateBrain GetMate(SquaddieRole role)
    {
        return Quartet.GetValueOrDefault(role, null);
    }

    public void InitQuartet(MateBrain frontMan, MateBrain bass, MateBrain sax, MateBrain drum)
    {
        Quartet[SquaddieRole.FrontMan] = frontMan;
        Quartet[SquaddieRole.Bass] = bass;
        Quartet[SquaddieRole.Sax] = sax;
        Quartet[SquaddieRole.Drum] = drum;
    }

    public void ExecuteManeuvre(Maneuvre maneuvre)
    {
        StartCoroutine(maneuvre.Execute(this));
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

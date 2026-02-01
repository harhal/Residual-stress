using UnityEngine;

[RequireComponent(typeof(Squad))]
public class Squad_TestInit : MonoBehaviour
{
    [SerializeField]
    MateBrain FrontMan;

    [SerializeField]
    MateBrain Bass;

    [SerializeField]
    MateBrain Sax;

    [SerializeField]
    MateBrain Drum;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Squad squad = GetComponent<Squad>();
        squad.InitQuartet(FrontMan, Bass, Sax, Drum);
    }
}

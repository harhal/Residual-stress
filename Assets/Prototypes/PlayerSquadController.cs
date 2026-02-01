using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Squad))]
public class PlayerSquadController : MonoBehaviour
{
    [SerializeField]
    WorldCoursor coursor;

    [SerializeField]
    string ActionName_MoveCoursor;

    [SerializeField]
    string ActionName_Select;
    Squad squad;

    void Awake()
    {
        squad = GetComponent<Squad>();
        GetComponent<PlayerInput>().onActionTriggered += ProcessInput;
    }

    public void ProcessInput(InputAction.CallbackContext context)
    {
        if (context.action.name == ActionName_MoveCoursor)
        {
            OnCoursor(context);
        } else if (context.action.name == ActionName_Select)
        {
            OnSelect(context);
        }
    }

    public void OnCoursor(InputAction.CallbackContext context)
    {
        Debug.Log("Coursor");
        Vector2 coursorScreenLocation = context.ReadValue<Vector2>();        
        Ray ray = Camera.main.ScreenPointToRay(coursorScreenLocation);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            coursor.SetLocation(hit.point, ray);
        }
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        Debug.Log("Select");
        squad.ExecuteManeuvre(new PlayerManeuvre_ExploreArea(coursor.GetLocation()));
    }
}

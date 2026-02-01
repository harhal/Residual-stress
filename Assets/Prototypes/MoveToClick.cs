using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MoveToClick : MonoBehaviour
{
    private NavMeshAgent agent;
    private Camera cam;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        cam = Camera.main;
        InputAction click = new InputAction("Click", InputActionType.Button);
        click.AddBinding("<Mouse>/leftButton");
        click.Enable();
        click.performed += OnClick;
    }

    private void OnClick(InputAction.CallbackContext callback)
    {
        Debug.Log("Click");
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            agent.SetDestination(hit.point);
        }
    }
}

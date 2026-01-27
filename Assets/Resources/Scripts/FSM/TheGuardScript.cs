using UnityEngine;
using Unity.InputSystem;

public class TheGuardScript : MonoBehaviour
{
    [SerializeField] BaseStateClass wanderState,attackState,persueState;
    [SerializeField] BaseStateClass startState;
    BaseStateClass currentState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = startState;
    }

    // Update is called once per frame
    void Update()
    {
        currentState.isActive = false;
        //Testing
        if(Keyboard.current.f1Key.isPressed)
        {
            SwitchState(wanderState);
        }
        else if(Keyboard.current.f2Key.isPressed)
        {
            SwitchState(attackState);
        }
        else if(Keyboard.current.f3Key.isPressed)
        {
            SwitchState(persueState);
        }

    }
    void SwitchState(BaseStateClass newState)
    {
        currentState.isActive = false;
        currentState = newState;
        currentState.isActive = true;
    }
}

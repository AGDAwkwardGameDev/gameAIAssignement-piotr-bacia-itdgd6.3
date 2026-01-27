using UnityEngine;

public class BaseStateClass : MonoBehaviour
{
    public bool isActive = false;
    protected TheGuardScript guardScript;

    // Track previous frame active state to detect enter/exit
    bool wasActive = false;

    protected virtual void Start()
    {
        guardScript = GetComponent<TheGuardScript>();
        wasActive = isActive;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // Detect transitions
        if (isActive && !wasActive)
        {
            OnEnter();
        }
        else if (!isActive && wasActive)
        {
            OnExit();
        }

        wasActive = isActive;

        if (!isActive)
            return;

        stateUpdate();
    }

    // Hook for states to run once when becoming active
    protected virtual void OnEnter() { }

    // Hook for states to run once when being deactivated
    protected virtual void OnExit() { }

    public virtual void stateUpdate()
    {

    }
}

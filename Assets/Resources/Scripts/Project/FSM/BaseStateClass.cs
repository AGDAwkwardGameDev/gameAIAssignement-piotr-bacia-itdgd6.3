using UnityEngine;

public class BaseStateClass : MonoBehaviour
{
    public bool isActive = false;
    public bool allowExternalTransitions = true;

    protected TheGuardScript guardScript;

    bool wasActive = false;

    public void Initialize(TheGuardScript guard)
    {
        guardScript = guard;
    }

    protected virtual void Start()
    {
        wasActive = isActive;
    }

    protected virtual void Update()
    {
        if (isActive && !wasActive)
        {
            OnEnter();
        }
        else if (!isActive && wasActive)
        {
            OnExit();
        }

        wasActive = isActive;

        if (isActive)
            stateUpdate();
    }

    public virtual void OnEnter()
    {
        Debug.Log($"{GetType().Name} ENTER");
    }

    public virtual void OnExit()
    {
        Debug.Log($"{GetType().Name} EXIT");
    }

    public virtual void stateUpdate() { }
}

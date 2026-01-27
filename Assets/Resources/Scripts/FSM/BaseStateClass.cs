using UnityEngine;

public class BaseStateClass : MonoBehaviour
{
    public bool isActive = false;

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!isActive)
            return;
        else
        {
            stateUpdate();
        }
    }
    public virtual void stateUpdate()
    {

    }
}

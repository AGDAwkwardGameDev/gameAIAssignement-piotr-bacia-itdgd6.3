using UnityEngine;

public class BaseStateClass : MonoBehaviour
{
    public bool isActive = false;

    // Update is called once per frame
    void Update()
    {
        if(!isActive)
            return;
    }
}

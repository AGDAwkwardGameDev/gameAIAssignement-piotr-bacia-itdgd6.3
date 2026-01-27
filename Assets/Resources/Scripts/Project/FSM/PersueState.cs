using UnityEngine;

public class PersueState : BaseStateClass
{
    private Vector3 lastKnownPlayerPosition;
    private float timeSinceLost = 0;

    private const float LOST_SIGHT_TIME = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public override void stateUpdate()
    {
        print("Persuing");
    }

    void chaseOpponent()
    {

    }
}

using UnityEngine;

public class PersueState : BaseStateClass
{
    private Vector3 lastKnownPlayerPosition;
    private float timeSinceLost = 0f;

    private const float LOST_SIGHT_TIME = 5f;

    public override void Start()
    {
        base.Start();

        // Start chasing current known target if any
        var currentTarget = guardScript.GetCurrentTarget();
        if (currentTarget != null)
        {
            guardScript.GetAIDestinationSetter().target = currentTarget;
            lastKnownPlayerPosition = currentTarget.position;
        }
        else
        {
            lastKnownPlayerPosition = guardScript.transform.position;
        }

        guardScript.GetAIPath().canMove = true;
        timeSinceLost = 0f;
    }

    public override void stateUpdate()
    {
       Debug.Log("Chasing Opponent");
        chaseOpponent();
    }

    void chaseOpponent()
    {
        var currentTarget = guardScript.GetCurrentTarget();

        if (currentTarget != null)
        {
            // Player/opponent currently assigned -> update destination and reset timer
            guardScript.GetAIDestinationSetter().target = currentTarget;
            lastKnownPlayerPosition = currentTarget.position;
            timeSinceLost = 0f;
        }
        else
        {
            // Lost sight: keep moving to last known position for a period
            timeSinceLost += Time.deltaTime;
            Debug.Log("Time Since Lost: " + timeSinceLost);

            bool reachedLastKnown = Vector3.Distance(guardScript.transform.position,
                                            lastKnownPlayerPosition) < 1f;
            if (timeSinceLost >= LOST_SIGHT_TIME || reachedLastKnown)
            {
                // stop chasing and hand control back to guard to resume wandering
                guardScript.GetAIDestinationSetter().target = null;
                guardScript.GetAIPath().canMove = false;

                // instruct guard to switch back to wander state
                guardScript.ReturnToWander();

                // disable this state update until reactivated by the guard
                isActive = false;
            }
        }
    }
}

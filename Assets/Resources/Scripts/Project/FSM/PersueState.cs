using UnityEngine;
using UnityEngine.AI;

public class PersueState : BaseStateClass
{
    [SerializeField] float persueSpeed = 6f;

    Vector3 lastKnownPlayerPosition;
    float timeSinceLost = 0f;
    const float LOST_SIGHT_TIME = 5f;

    NavMeshAgent agent;

    public override void OnEnter()
    {
        base.OnEnter();

        // Allow GuardScript to switch to Attack while pursuing
        allowExternalTransitions = true;

        agent = guardScript.GetNavMeshAgent();
        if (agent != null)
        {
            agent.speed = persueSpeed;
            agent.isStopped = false;

            // Make sure we can actually get into attack radius
            agent.stoppingDistance = guardScript.AttackRadius * 0.5f;
            agent.autoBraking = false;
        }

        var target = guardScript.GetCurrentTarget();
        if (target != null)
        {
            lastKnownPlayerPosition = target.position;
            agent?.SetDestination(lastKnownPlayerPosition);
        }

        timeSinceLost = 0f;
    }

    public override void stateUpdate()
    {
        var target = guardScript.GetCurrentTarget();

        if (target != null)
        {
            lastKnownPlayerPosition = target.position;
            timeSinceLost = 0f;
            agent?.SetDestination(lastKnownPlayerPosition);
            return;
        }

        timeSinceLost += Time.deltaTime;

        if (agent != null && Vector3.Distance(transform.position, lastKnownPlayerPosition) > 1f)
            agent.SetDestination(lastKnownPlayerPosition);

        if (timeSinceLost >= LOST_SIGHT_TIME)
        {
            allowExternalTransitions = true;
            guardScript.ReturnToWander();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        allowExternalTransitions = true;
    }
}

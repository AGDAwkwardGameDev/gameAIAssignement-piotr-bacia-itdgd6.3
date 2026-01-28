using UnityEngine;
using UnityEngine.AI;

public class WanderState : BaseStateClass
{
    [SerializeField] float wanderSpeed = 2.5f;
    [SerializeField] float wanderRadius = 15f;
    [SerializeField] float goalDistance = 0.5f;

    NavMeshAgent agent;
    GameObject wanderTarget;

    public override void OnEnter()
    {
        base.OnEnter();

        agent = guardScript.agent;
        if (agent == null)
        {
            Debug.LogError("WanderState: NavMeshAgent not found!");
            return;
        }

        if (wanderTarget == null)
        {
            wanderTarget = new GameObject($"{gameObject.name}_WanderTarget");
            wanderTarget.transform.SetParent(transform.parent);
        }

        agent.speed = wanderSpeed;
        agent.isStopped = false;

        GenerateNewGoalPosition();
        agent.SetDestination(wanderTarget.transform.position);
    }

    public override void stateUpdate()
    {
        if (agent == null || wanderTarget == null)
            return;

        if (!agent.pathPending && agent.remainingDistance <= goalDistance)
        {
            GenerateNewGoalPosition();
            agent.SetDestination(wanderTarget.transform.position);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        agent?.ResetPath();
    }

    void GenerateNewGoalPosition()
    {
        Vector3 randomPoint = guardScript.transform.position + Random.insideUnitSphere * wanderRadius;
        randomPoint.y = guardScript.transform.position.y;

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            randomPoint = hit.position;

        wanderTarget.transform.position = randomPoint;
    }
}

using UnityEngine;
using UnityEngine.AI;

public class UtilityFleeThreat : IUtilityAction
{
    private readonly TheStrategistScript strategist;
    private readonly NavMeshAgent agent;

    public string Name => "Flee";

    public UtilityFleeThreat(TheStrategistScript strategist, NavMeshAgent agent)
    {
        this.strategist = strategist;
        this.agent = agent;
    }

    public float Evaluate()
    {
        if (!strategist.TryFindThreat(out Transform threat, out float distance))
            return 0f;

        float healthFactor = 1f - (strategist.CurrentHP / strategist.MaxHP);
        float distanceFactor = Mathf.Clamp01(1f - (distance / 20f));

        return Mathf.Clamp01(0.6f * healthFactor + 0.4f * distanceFactor);
    }

    public void Execute()
    {
        if (!strategist.TryFindThreat(out Transform threat, out float distance))
            return;

        Vector3 away = (strategist.transform.position - threat.position).normalized;
        Vector3 targetPos = strategist.transform.position + away * 10f;

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            strategist.UpdateActionLabel("Fleeing");
            agent.SetDestination(hit.position);
        }
    }
}

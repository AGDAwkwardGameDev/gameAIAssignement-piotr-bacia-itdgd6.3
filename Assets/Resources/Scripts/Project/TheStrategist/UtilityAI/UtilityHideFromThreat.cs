using UnityEngine;
using UnityEngine.AI;

public class UtilityHideFromThreat : IUtilityAction
{
    private readonly TheStrategistScript strategist;
    private readonly NavMeshAgent agent;

    public string Name => "Hide";

    public UtilityHideFromThreat(TheStrategistScript strategist, NavMeshAgent agent)
    {
        this.strategist = strategist;
        this.agent = agent;
    }

    public float Evaluate()
    {
        if (!strategist.TryFindThreat(out Transform threat, out float distance))
            return 0f;

        if (!strategist.TryFindCover(out Vector3 coverPos))
            return 0f;

        float healthFactor = 1f - (strategist.CurrentHP / strategist.MaxHP);
        float distanceFactor = Mathf.Clamp01(1f - (distance / 25f));

        return Mathf.Clamp01(0.5f * healthFactor + 0.5f * distanceFactor);
    }

    public void Execute()
    {
        if (!strategist.TryFindCover(out Vector3 coverPos))
            return;

        strategist.UpdateActionLabel("Hiding");
        agent.SetDestination(coverPos);
    }
}

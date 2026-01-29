using UnityEngine;
using UnityEngine.AI;

public class UtilityMeleeEngage : IUtilityAction
{
    private readonly TheStrategistScript strategist;
    private readonly NavMeshAgent agent;

    public string Name => "MeleeAttack";

    public UtilityMeleeEngage(TheStrategistScript strategist, NavMeshAgent agent)
    {
        this.strategist = strategist;
        this.agent = agent;
    }

    public float Evaluate()
    {
        if (!strategist.TryFindThreat(out Transform threat, out float distance))
            return 0f;

        if (strategist.IsInCriticalState)
            return 0f; // let flee/health/hide dominate

        float distanceFactor = Mathf.Clamp01(1f - (distance / 10f));
        float healthFactor = strategist.CurrentHP / strategist.MaxHP;

        return Mathf.Clamp01(0.6f * distanceFactor + 0.4f * healthFactor);
    }

    public void Execute()
    {
        if (!strategist.TryFindThreat(out Transform threat, out float distance))
            return;

        strategist.UpdateActionLabel("Melee Engage");

        if (distance > 1.5f)
        {
            agent.SetDestination(threat.position);
        }
        else
        {
            strategist.PerformMeleeStrike(threat);
        }
    }
}

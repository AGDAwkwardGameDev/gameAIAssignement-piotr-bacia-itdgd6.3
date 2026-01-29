using UnityEngine;
using UnityEngine.AI;

public class UtilityRetrieveHealthPickup : IUtilityAction
{
    private readonly TheStrategistScript strategist;
    private readonly NavMeshAgent agent;

    public string Name => "SeekHealth";

    public UtilityRetrieveHealthPickup(TheStrategistScript strategist, NavMeshAgent agent)
    {
        this.strategist = strategist;
        this.agent = agent;
    }

    public float Evaluate()
    {
        if (strategist.CurrentHP > strategist.MaxHP * 0.8f)
            return 0f;

        var packs = GameObject.FindGameObjectsWithTag("HealthPack");
        if (packs.Length == 0)
            return 0f;

        float best = float.MaxValue;
        foreach (var p in packs)
        {
            float d = Vector3.Distance(strategist.transform.position, p.transform.position);
            if (d < best)
                best = d;
        }

        float healthFactor = 1f - (strategist.CurrentHP / strategist.MaxHP);
        float distanceFactor = Mathf.Clamp01(1f - (best / 30f));

        return Mathf.Clamp01(0.7f * healthFactor + 0.3f * distanceFactor);
    }

    public void Execute()
    {
        var packs = GameObject.FindGameObjectsWithTag("HealthPack");
        if (packs.Length == 0)
            return;

        Transform closest = null;
        float best = float.MaxValue;

        foreach (var p in packs)
        {
            float d = Vector3.Distance(strategist.transform.position, p.transform.position);
            if (d < best)
            {
                best = d;
                closest = p.transform;
            }
        }

        if (closest != null)
        {
            strategist.UpdateActionLabel("Seeking Health");
            agent.SetDestination(closest.position);
        }
    }
}

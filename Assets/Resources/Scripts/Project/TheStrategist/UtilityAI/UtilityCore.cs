using System.Collections.Generic;
using UnityEngine;

public interface IUtilityAction
{
    float Evaluate();   // Return a score 0–1 based on current context
    void Execute();     // Perform the action (or set up NavMeshAgent, etc.)
    string Name { get; } // For debugging / history
}

public class UtilityAISystem
{
    private readonly List<IUtilityAction> actions = new List<IUtilityAction>();
    private readonly List<string> recentSequence = new List<string>();
    private readonly int sequenceMemory = 5;
    private float decisionInterval = 0.5f;
    private float timeSinceLastDecision = 0f;

    private System.Random rng = new System.Random();

    public void RegisterAction(IUtilityAction action)
    {
        if (!actions.Contains(action))
            actions.Add(action);
    }

    public void Update(float deltaTime)
    {
        timeSinceLastDecision += deltaTime;
        if (timeSinceLastDecision < decisionInterval)
            return;

        timeSinceLastDecision = 0f;

        if (actions.Count == 0)
            return;

        IUtilityAction best = null;
        float bestScore = float.NegativeInfinity;

        foreach (var action in actions)
        {
            float score = action.Evaluate();

            // Small random jitter to avoid deterministic ties
            score += (float)(rng.NextDouble() * 0.05f);

            // Penalize repeating the same action sequence
            if (recentSequence.Count > 0 && recentSequence[recentSequence.Count - 1] == action.Name)
                score -= 0.1f;

            if (score > bestScore)
            {
                bestScore = score;
                best = action;
            }
        }

        if (best != null && bestScore > 0f)
        {
            best.Execute();
            recentSequence.Add(best.Name);
            if (recentSequence.Count > sequenceMemory)
                recentSequence.RemoveAt(0);
        }
    }
}

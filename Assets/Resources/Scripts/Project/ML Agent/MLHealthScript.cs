using System;
using UnityEngine;


public class MLHealthScript : HealthScript
{
    // Fired when this agent takes damage
    public event Action<float> OnDamaged;

    // Fired when this agent is healed
    public event Action<float> OnHealed;

    // Fired when this agent dies
    public event Action OnDied;

    // ---------------------------------------------------------
    // DAMAGE OVERRIDE
    // ---------------------------------------------------------
    public override void DealDamage(float damageAmount)
    {
        float before = currentHealth;
        base.DealDamage(damageAmount);

        float applied = before - currentHealth;
        if (applied > 0f)
            OnDamaged?.Invoke(applied);

        if (currentHealth <= 0f)
            OnDied?.Invoke();
    }

    // ---------------------------------------------------------
    // HEAL OVERRIDE
    // ---------------------------------------------------------
    public override void Heal(float healAmount)
    {
        float before = currentHealth;
        base.Heal(healAmount);

        float applied = currentHealth - before;
        if (applied > 0f)
            OnHealed?.Invoke(applied);
    }

    // ---------------------------------------------------------
    // RESET SUPPORT
    // ---------------------------------------------------------
    public void ResetToFull()
    {
        currentHealth = maxHealth;
    }

    public bool IsDeadML => currentHealth <= 0f;
    public float HealthPercentML => currentHealth / maxHealth;
}

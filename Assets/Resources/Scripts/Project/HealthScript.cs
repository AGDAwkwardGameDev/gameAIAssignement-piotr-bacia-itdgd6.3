using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public float maxHealth = 100f;
    float currentHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void DealDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Debug.Log(transform.gameObject.name+  " has died.");
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

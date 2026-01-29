using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private float healAmount = 40f;

    private void OnTriggerEnter(Collider other)
    {
        var health = other.GetComponent<HealthScript>();
        if (health == null)
            return;

        health.Heal(healAmount);

        // Destroy or disable the pickup
        Destroy(this.transform.gameObject);
    }
}

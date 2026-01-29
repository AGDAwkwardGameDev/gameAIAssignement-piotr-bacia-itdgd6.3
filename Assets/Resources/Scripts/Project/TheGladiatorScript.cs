using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class TheGladiatorScript : Agent
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float turnSpeed = 140f;

    [Header("Combat")]
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private float meleeDamage = 20f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Awareness")]
    [SerializeField] private float detectionRadius = 25f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private string[] targetTags = { "Strategist", "Guardian" };

    private Rigidbody rb;
    private MLHealthScript health;
    private Transform target;
    private float lastAttackTime;
    private float previousDistance;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<MLHealthScript>();
        lastAttackTime = -attackCooldown;
    }

    public override void OnEpisodeBegin()
    {
        if (health != null)
            health.ResetToFull();

        transform.position += new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        AcquireTarget();
        previousDistance = target != null
            ? Vector3.Distance(transform.position, target.position)
            : 0f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(rb.linearVelocity.normalized);
        sensor.AddObservation(Time.time - lastAttackTime >= attackCooldown ? 1f : 0f);

        AcquireTarget();
        if (target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            Vector3 localDir = transform.InverseTransformDirection(dir);
            float dist = Vector3.Distance(transform.position, target.position);

            sensor.AddObservation(localDir);
            sensor.AddObservation(dist / detectionRadius);
            sensor.AddObservation(dist <= meleeRange ? 1f : 0f);

            var tHealth = target.GetComponent<HealthScript>();
            sensor.AddObservation(tHealth != null && tHealth.currentHealth != 0 ? 1f : 0f);
        }
        else
        {
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
        }

        sensor.AddObservation(CastRay(Vector3.forward));
        sensor.AddObservation(CastRay(Quaternion.Euler(0, 45, 0) * Vector3.forward));
        sensor.AddObservation(CastRay(Quaternion.Euler(0, -45, 0) * Vector3.forward));
    }

    private float CastRay(Vector3 direction)
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, 5f, obstacleMask)
            ? 1f
            : 0f;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveInput = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float turnInput = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        bool wantsToAttack = actions.DiscreteActions[0] == 1;

        Vector3 move = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        Quaternion turn = Quaternion.Euler(0, turnInput * turnSpeed * Time.fixedDeltaTime, 0);
        rb.MoveRotation(rb.rotation * turn);

        if (target != null)
        {
            float dist = Vector3.Distance(transform.position, target.position);

            float delta = previousDistance - dist;
            AddReward(delta * 0.01f);
            previousDistance = dist;

            if (wantsToAttack && Time.time - lastAttackTime >= attackCooldown)
            {
                if (dist <= meleeRange)
                {
                    var tHealth = target.GetComponent<HealthScript>();
                    if (tHealth != null && tHealth.currentHealth != 0f)
                    {
                        tHealth.DealDamage(meleeDamage);

                        if (tHealth.currentHealth == 0)
                        {
                            AddReward(1.0f);
                            EndEpisode();
                        }
                        else
                        {
                            AddReward(0.2f);
                        }
                    }
                }
                else
                {
                    AddReward(-0.05f);
                }

                lastAttackTime = Time.time;
            }
        }

        AddReward(0.0005f);

        if (health.IsDeadML)
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

    private void AcquireTarget()
    {
        float best = float.MaxValue;
        Transform bestT = null;

        foreach (string tag in targetTags)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
            foreach (var o in objs)
            {
                var h = o.GetComponent<MLHealthScript>();
                if (h != null && h.IsDeadML)
                    continue;

                float d = Vector3.Distance(transform.position, o.transform.position);
                if (d < best && d <= detectionRadius)
                {
                    best = d;
                    bestT = o.transform;
                }
            }
        }

        target = bestT;
    }
}

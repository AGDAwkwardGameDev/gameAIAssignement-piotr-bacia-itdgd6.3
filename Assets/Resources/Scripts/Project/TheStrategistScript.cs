using UnityEngine;
using UnityEngine.AI;
using CrashKonijn.Goap.Runtime;
using CrashKonijn.Agent.Runtime;
using StrategistGOAP;

public class TheStrategistScript : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float criticalHealthThreshold = 40f;
    public float CriticalHealthThreshold => criticalHealthThreshold;

    [Header("Melee Combat")]
    [SerializeField] private float meleeRange = 2.2f;
    [SerializeField] private float meleeCooldown = 1.0f;
    [SerializeField] private float meleeDamage = 30f;
    private float lastMeleeTime = -10f;

    [Header("Awareness")]
    [SerializeField] private float threatScanRadius = 20f;
    [SerializeField] private LayerMask coverMask;

    [Header("Debug")]
    [SerializeField] private bool showDebug = true;

    private NavMeshAgent agent;
    private HealthScript health;
    private AgentBehaviour agentBehaviour;
    private GoapActionProvider goapProvider;

    private UtilityAISystem utilityAI;
    private bool goapActive = false;
    private string currentActionLabel = "Idle";

    public float CurrentHP => health.currentHealth;
    public float MaxHP => health.maxHealth;
    public bool IsInCriticalState => CurrentHP <= CriticalHealthThreshold;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<HealthScript>();
        agentBehaviour = GetComponent<AgentBehaviour>();
        goapProvider = GetComponent<GoapActionProvider>();

        InitializeGOAP();
        InitializeUtilityAI();
    }

    private void Update()
    {
        HandleGoapOverride();

        if (!goapActive && utilityAI != null)
            utilityAI.Update(Time.deltaTime);
    }

    // ---------------------------------------------------------
    // GOAP INITIALIZATION
    // ---------------------------------------------------------
    private void InitializeGOAP()
    {
        // Build AgentTypeConfig from your factory
        var agentTypeConfig = new StrategistAgentProfile().Create();

        // Assign to both GOAP components
        agentBehaviour.SetConfig(agentTypeConfig);
        goapProvider.SetConfig(agentTypeConfig);
    }

    // ---------------------------------------------------------
    // UTILITY AI INITIALIZATION
    // ---------------------------------------------------------
    private void InitializeUtilityAI()
    {
        utilityAI = new UtilityAISystem();

        utilityAI.RegisterAction(new UtilityFleeThreat(this, agent));
        utilityAI.RegisterAction(new UtilityRetrieveHealthPickup(this, agent));
        utilityAI.RegisterAction(new UtilityHideFromThreat(this, agent));
        utilityAI.RegisterAction(new UtilityMeleeEngage(this, agent));
    }

    // ---------------------------------------------------------
    // GOAP OVERRIDE LOGIC
    // ---------------------------------------------------------
    private void HandleGoapOverride()
    {
        if (IsInCriticalState && !goapActive)
        {
            goapActive = true;
            goapProvider.RequestGoal<SurvivalGoal>(true);
            UpdateActionLabel("GOAP: Survival Override");
        }
        else if (!IsInCriticalState && goapActive)
        {
            goapActive = false;
            goapProvider.ClearGoal();
            UpdateActionLabel("Utility AI");
        }
    }

    // ---------------------------------------------------------
    // THREAT DETECTION
    // ---------------------------------------------------------
    public bool TryFindThreat(out Transform threat, out float distance)
    {
        threat = null;
        distance = float.MaxValue;

        Collider[] hits = Physics.OverlapSphere(transform.position, threatScanRadius);
        foreach (var h in hits)
        {
            if (!h.CompareTag("Gladiator") && !h.CompareTag("Guard"))
                continue;

            float d = Vector3.Distance(transform.position, h.transform.position);
            if (d < distance)
            {
                distance = d;
                threat = h.transform;
            }
        }

        return threat != null;
    }

    // ---------------------------------------------------------
    // MELEE COMBAT
    // ---------------------------------------------------------
    public void PerformMeleeStrike(Transform target)
    {
        if (Time.time - lastMeleeTime < meleeCooldown)
            return;

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist > meleeRange)
            return;

        lastMeleeTime = Time.time;

        Vector3 dir = (target.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir);

        var enemyHealth = target.GetComponent<HealthScript>();
        if (enemyHealth != null)
            enemyHealth.DealDamage(meleeDamage);
    }

    // ---------------------------------------------------------
    // COVER / HIDING
    // ---------------------------------------------------------
    public bool TryFindCover(out Vector3 coverPosition)
    {
        coverPosition = transform.position;

        Collider[] hits = Physics.OverlapSphere(transform.position, threatScanRadius, coverMask);
        float best = float.MaxValue;
        bool found = false;

        foreach (var h in hits)
        {
            float d = Vector3.Distance(transform.position, h.transform.position);
            if (d < best)
            {
                best = d;
                coverPosition = h.transform.position;
                found = true;
            }
        }

        return found;
    }

    // ---------------------------------------------------------
    // DEBUG LABEL
    // ---------------------------------------------------------
    public void UpdateActionLabel(string label)
    {
        currentActionLabel = label;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebug)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, threatScanRadius);

#if UNITY_EDITOR
        string mode = goapActive ? "GOAP: Survival" : "Utility AI";
        string text = $"Mode: {mode}\nAction: {currentActionLabel}\nHP: {CurrentHP}/{MaxHP}";
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2.5f, text);
#endif
    }
}

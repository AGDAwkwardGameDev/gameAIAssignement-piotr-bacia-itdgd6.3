using UnityEngine;
using UnityEngine.AI;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using StrategistGOAP; // For SurvivalGoal, sensors, etc.

public class TheStrategistScript : MonoBehaviour
{
    // ---------------------------------------------------------
    // HEALTH / STATS
    // ---------------------------------------------------------
    [Header("Health")]
    [SerializeField] private float criticalHealthThreshold = 40f;

    private HealthScript health;

    public float CurrentHP => health != null ? health.currentHealth : 0f;
    public float MaxHP => health != null ? health.maxHealth : 100f;
    public bool IsInCriticalState => CurrentHP <= criticalHealthThreshold;

    // ---------------------------------------------------------
    // MELEE COMBAT
    // ---------------------------------------------------------
    [Header("Melee Combat")]
    [SerializeField] private float meleeRange = 2.2f;
    [SerializeField] private float meleeCooldown = 1.0f;
    [SerializeField] private float meleeDamage = 30f;
    private float lastMeleeTime = -10f;

    // ---------------------------------------------------------
    // AWARENESS
    // ---------------------------------------------------------
    [Header("Awareness")]
    [SerializeField] private float threatScanRadius = 20f;
    [SerializeField] private LayerMask coverMask;

    // ---------------------------------------------------------
    // AI SYSTEMS
    // ---------------------------------------------------------
    [Header("AI Systems")]
    [SerializeField] private bool enableGoapOverride = true;
    [SerializeField] private bool showDebugInfo = true;

    private NavMeshAgent navAgent;
    private AgentBehaviour agentBehaviour;
    private GoapActionProvider goapProvider;

    private UtilityAISystem utilityAI;
    private bool isGoapOverrideActive = false;
    private string currentActionName = "Initializing";

    // ---------------------------------------------------------
    // UNITY LIFECYCLE
    // ---------------------------------------------------------
    private void Start()
    {
        // Health
        health = GetComponent<HealthScript>();
        if (health == null)
        {
            Debug.LogWarning("[TheStrategist] No HealthScript found. Adding one...");
            health = gameObject.AddComponent<HealthScript>();
        }

        // NavMeshAgent
        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            Debug.LogWarning("[TheStrategist] No NavMeshAgent found. Adding one...");
            navAgent = gameObject.AddComponent<NavMeshAgent>();
        }

        // GOAP components
        agentBehaviour = GetComponent<AgentBehaviour>();
        goapProvider = GetComponent<GoapActionProvider>();

        InitializeUtilityAI();
        InitializeGOAP();

        Debug.Log("[TheStrategist] AI systems initialized successfully.");
    }

    private void Update()
    {
        CheckGoapOverride();

        if (!isGoapOverrideActive && utilityAI != null)
            utilityAI.Update(Time.deltaTime);
    }

    // ---------------------------------------------------------
    // UTILITY AI INITIALIZATION
    // ---------------------------------------------------------
    private void InitializeUtilityAI()
    {
        utilityAI = new UtilityAISystem();

        utilityAI.RegisterAction(new UtilityFleeThreat(this, navAgent));
        utilityAI.RegisterAction(new UtilityRetrieveHealthPickup(this, navAgent));
        utilityAI.RegisterAction(new UtilityHideFromThreat(this, navAgent));
        utilityAI.RegisterAction(new UtilityMeleeEngage(this, navAgent));

        Debug.Log("[TheStrategist] Utility AI initialized with melee actions.");
    }

    // ---------------------------------------------------------
    // GOAP INITIALIZATION (v4-style auto config)
    // ---------------------------------------------------------
    private void InitializeGOAP()
    {
        if (!enableGoapOverride)
            return;

        if (agentBehaviour == null || goapProvider == null)
        {
            Debug.LogWarning("[TheStrategist] GOAP components missing. GOAP override disabled.");
            enableGoapOverride = false;
            return;
        }

        // v4+ style: config is driven by ScriptableObjects / inspector setup.
        Debug.Log("[TheStrategist] GOAP system initialized (auto-config).");
    }

    // ---------------------------------------------------------
    // GOAP OVERRIDE LOGIC
    // ---------------------------------------------------------
    private void CheckGoapOverride()
    {
        if (!enableGoapOverride || goapProvider == null || health == null)
        {
            isGoapOverrideActive = false;
            return;
        }

        bool shouldOverride = IsInCriticalState;

        if (shouldOverride && !isGoapOverrideActive)
        {
            Debug.LogWarning($"[TheStrategist] CRITICAL HEALTH! GOAP taking control at {CurrentHP}/{MaxHP}");
            isGoapOverrideActive = true;
            goapProvider.RequestGoal<SurvivalGoal>(true);
            UpdateActionLabel("GOAP: Survival Override");
        }
        else if (!shouldOverride && isGoapOverrideActive)
        {
            Debug.Log($"[TheStrategist] Health stabilized at {CurrentHP}/{MaxHP}. Returning to Utility AI.");
            isGoapOverrideActive = false;
            goapProvider.ClearGoal();
            UpdateActionLabel("Utility AI");
        }
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
    // COVER SEARCH
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
    // DEBUG LABEL API (USED BY UTILITY / GOAP)
    // ---------------------------------------------------------
    public void UpdateActionLabel(string label)
    {
        currentActionName = label;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebugInfo)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, threatScanRadius);

#if UNITY_EDITOR
        string mode = isGoapOverrideActive ? "GOAP: Survival" : "Utility AI";
        string text = $"Mode: {mode}\nAction: {currentActionName}\nHP: {CurrentHP}/{MaxHP}";
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2.5f, text);
#endif
    }
}

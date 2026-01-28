using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class TheGuardScript : MonoBehaviour
{
    [Header("States")]
    [SerializeField] BaseStateClass wanderState;
    [SerializeField] BaseStateClass attackState;
    [SerializeField] BaseStateClass persueState;

    [Header("Starting State")]
    [SerializeField] BaseStateClass startState;

    [Header("Detection Settings")]
    [SerializeField] float sightRange = 20f;
    [SerializeField] float attackRadius = 5f;

    public NavMeshAgent agent;
    private GameObject opponentObject;
    public Transform currentTarget;

    BaseStateClass currentState;

    public NavMeshAgent GetNavMeshAgent() => agent;
    public Transform GetCurrentTarget() => currentTarget;
    public GameObject GetOpponentObject() => opponentObject;
    public float AttackRadius => attackRadius;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        wanderState.Initialize(this);
        persueState.Initialize(this);
        attackState.Initialize(this);

        currentState = startState;
    }

    void Start()
    {
        currentState.isActive = true; // BaseStateClass will call OnEnter on first Update
    }

    void Update()
    {
        // Manual debug switching
        if (Keyboard.current.f1Key.wasPressedThisFrame)
            SwitchState(wanderState);
        if (Keyboard.current.f2Key.wasPressedThisFrame)
            SwitchState(attackState);
        if (Keyboard.current.f3Key.wasPressedThisFrame)
            SwitchState(persueState);

        // If current state blocks transitions, do nothing
        if (!currentState.allowExternalTransitions)
            return;

        // Attack takes priority
        if (IsInAttackRange(attackRadius))
        {
            SwitchState(attackState);
            return;
        }

        // Sight detection
        if (IsInSightRange(sightRange))
        {
            SwitchState(persueState);
            return;
        }

        // Otherwise wander
        SwitchState(wanderState);
    }

    void SwitchState(BaseStateClass newState)
    {
        if (newState == null || newState == currentState)
            return;

        currentState.isActive = false;
        currentState = newState;
        currentState.isActive = true;
    }

    public void ReturnToWander() => SwitchState(wanderState);
    public void ReturnToPersue() => SwitchState(persueState);

    bool IsInAttackRange(float radius)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        foreach (var hit in hits)
        {
            Transform root = hit.transform.root;

            if (root.CompareTag("Strategist") || root.CompareTag("Gladiator"))
            {
                opponentObject = root.gameObject;
                currentTarget = root;
                return true;
            }
        }

        return false;
    }

    bool IsInSightRange(float range)
    {
        Vector3 origin = transform.position + Vector3.up * 1.1f;

        Collider[] hits = Physics.OverlapSphere(origin, range);

        foreach (var hit in hits)
        {
            Transform root = hit.transform.root;
            string rootTag = root.tag;

            if (rootTag != "Strategist" && rootTag != "Gladiator")
                continue;

            Vector3 targetPos = root.position + Vector3.up * 1.1f;
            Vector3 dir = (targetPos - origin).normalized;

            float dot = Vector3.Dot(transform.forward, dir);
            if (dot < 0.5f)
                continue;

            if (Physics.Raycast(origin, dir, out RaycastHit sightHit, sightRange))
            {
                Debug.DrawRay(origin, dir * sightRange, Color.red);

                if (sightHit.collider.transform.root == root)
                {
                    opponentObject = root.gameObject;
                    currentTarget = root;
                    return true;
                }
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        // Sight range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 1.1f, sightRange);

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}

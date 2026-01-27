using System.Collections.Generic;
using System.Transactions;
using Pathfinding;
using UnityEngine;
using UnityEngine.InputSystem;

public class TheGuardScript : MonoBehaviour
{
    [Header("States")]
    [SerializeField] BaseStateClass wanderState, attackState, persueState;
    [Header("Starting State")]
    [SerializeField] BaseStateClass startState;
    [Header("Detection Settings")]
    [SerializeField]
    float sightRange = 20f;
    private AIDestinationSetter aiDestSetter;
    private AIPath aiPath;
    private GameObject opponentObject;
    private Transform currentTarget;

    [SerializeField]
    float attackRadius = 5f;
    BaseStateClass currentState;

    // Public getters so FSM states can interact with TheGuardScript internals.
    public AIDestinationSetter GetAIDestinationSetter() => aiDestSetter;
    public AIPath GetAIPath() => aiPath;
    public Transform GetCurrentTarget() => currentTarget;
    public GameObject GetOpponentObject() => opponentObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = startState;
        currentState.isActive = true;
        aiDestSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug switching
        if (Keyboard.current.f1Key.isPressed)
        {
            SwitchState(wanderState);
        }
        else if (Keyboard.current.f2Key.isPressed)
        {
            SwitchState(attackState);
        }
        else if (Keyboard.current.f3Key.isPressed)
        {
            SwitchState(persueState);
        }

        //Actual logic
        if (IsInAttackRange(attackRadius))
        {
            SwitchState(attackState);
        }
        else
        {
            if (IsInSightRange(sightRange))
            {
                if (currentTarget == null)
                {
                    if (opponentObject != null)
                    {
                        currentTarget = opponentObject.transform;
                        aiDestSetter.target = currentTarget;
                    }
                }
                SwitchState(persueState);
            }
            else
            {
                currentTarget = null;
                aiDestSetter.target = null;
                SwitchState(wanderState);
            }
        }

    }
    void SwitchState(BaseStateClass newState)
    {
        if (currentState == newState
            || newState == null)
            return;
        currentState.isActive = false;
        currentState = newState;
        currentState.isActive = true;
    }

    // Public getters so FSM states can interact with TheGuardScript internals.
    public AIDestinationSetter GetAIDestinationSetter() => aiDestSetter;
    public AIPath GetAIPath() => aiPath;
    public Transform GetCurrentTarget() => currentTarget;
    public GameObject GetOpponentObject() => opponentObject;

    // Allow states to ask the guard to return to patrol/wander state.
    public void ReturnToWander()
    {
        SwitchState(wanderState);
    }
    public void ReturnToPersue()
    {
        SwitchState(persueState);
    }

    bool IsInAttackRange(float radius)
    {
        bool isInRange = false;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.CompareTag("Strategist") || hitCollider.transform.CompareTag("Gladiator"))
            {

                isInRange = true;
                break;
            }
        }
        return isInRange;

    }
    bool IsInSightRange(float range)
    {
        // Safely attempt to raycast toward the opponentObject (if any).
        if (opponentObject == null)
            return false;

        Vector3 origin = transform.position + Vector3.up * 1.1f;
        Vector3 target = opponentObject.transform.position + Vector3.up * 1.1f;

        Vector3 direction = (target - origin).normalized;

        Debug.DrawRay(origin, direction * 50f, Color.red);

        if (Physics.Raycast(origin, direction,
                        out RaycastHit hit, range))
        {
            Debug.Log("Ray hit: " + hit.collider.name);
            if (hit.collider.CompareTag("Strategist") || hit.collider.CompareTag("Gladiator"))
            {
                // capture the found opponent for targeting
                opponentObject = hit.collider.gameObject;
                return true;
            }
        }

        return false;
    }



    private void OnDrawGizmos()
    {
        // Set the color with custom alpha.
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f); // Red with custom alpha

        // Draw the sphere.
        Gizmos.DrawSphere(transform.position, attackRadius);

        // Draw wire sphere outline.
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

}

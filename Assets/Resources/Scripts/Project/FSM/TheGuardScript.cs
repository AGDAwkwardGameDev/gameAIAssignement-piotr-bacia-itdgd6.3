using System.Collections.Generic;
using System.Transactions;
using Pathfinding;
using UnityEngine;
using UnityEngine.InputSystem;

public class TheGuardScript : MonoBehaviour
{
    [Header("States")]
    [SerializeField] BaseStateClass wanderState,attackState,persueState;
    [Header("Starting State")]
    [SerializeField] BaseStateClass startState;
    [Header("Detection Settings")]
    [SerializeField]
    float sightRange = 20f;
    private AIDestinationSetter aiDestSetter;
    private AIPath aiPath;
    private Transform playerTarget;
    [SerializeField]
    float attackRadius = 5f;
    BaseStateClass currentState;
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
        if(Keyboard.current.f1Key.isPressed)
        {
            SwitchState(wanderState);
        }
        else if(Keyboard.current.f2Key.isPressed)
        {
            SwitchState(attackState);
        }
        else if(Keyboard.current.f3Key.isPressed)
        {
            SwitchState(persueState);
        }
        //Actual logic
        if (aiDestSetter.target != null)
        {
            playerTarget = aiDestSetter.target;
            if (IsInAttackRange(attackRadius))
            {
                SwitchState(attackState);
            }
            else
            {
                SwitchState(persueState);
            }
        }
        else
        {
            playerTarget = null;
            SwitchState(wanderState);
        }

    }
    void SwitchState(BaseStateClass newState)
    {
        if(currentState == newState
            || newState == null)
            return;
        currentState.isActive = false;
        currentState = newState;
        currentState.isActive = true;
    }
    bool IsInAttackRange(float radius)
    {
        bool isInRange = false;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.CompareTag("Strategist")|| hitCollider.transform.CompareTag("Gladiator"))
            {
                isInRange = true;
                break;
            }
        }
        return isInRange;
  
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

using UnityEngine;
using UnityEngine.AI;

public class AttackState : BaseStateClass
{
    [SerializeField] float attackRadius = 5f;
    [SerializeField] float damagePerHit = 10f;
    [SerializeField] float attackInterval = 1f;

    float lastAttackTime = -999f;
    NavMeshAgent agent;

    public override void OnEnter()
    {
        base.OnEnter();
        allowExternalTransitions = false;

        agent = guardScript.GetNavMeshAgent();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    public override void stateUpdate()
    {
        GameObject opponent = guardScript.GetOpponentObject();

        if (opponent == null)
        {
            allowExternalTransitions = true;
            guardScript.ReturnToWander();
            return;
        }

        float dist = Vector3.Distance(transform.position, opponent.transform.position);

        if (dist > attackRadius)
        {
            allowExternalTransitions = true;
            guardScript.ReturnToPersue();
            return;
        }

        Vector3 dir = opponent.transform.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion desired = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, desired, Time.deltaTime * 8f);
        }

        if (Time.time - lastAttackTime >= attackInterval)
        {
            lastAttackTime = Time.time;

            var health = opponent.GetComponent<HealthScript>();
            if (health != null)
                health.DealDamage(damagePerHit);

            if (opponent == null || !opponent.activeInHierarchy)
            {
                allowExternalTransitions = true;
                guardScript.ReturnToWander();
            }
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        allowExternalTransitions = true;

        if (agent != null)
            agent.isStopped = false;
    }
}

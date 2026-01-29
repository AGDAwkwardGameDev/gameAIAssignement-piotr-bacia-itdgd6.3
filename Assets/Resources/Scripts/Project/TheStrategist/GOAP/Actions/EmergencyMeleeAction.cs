using CrashKonijn.Goap.Runtime;
using CrashKonijn.Goap.Core;
using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using UnityEngine;

namespace StrategistGOAP
{
    [GoapId("EmergencyCQSurvivalAction-unique-002")]
    public class EmergencyCloseQuartersSurvivalAction : GoapActionBase<EmergencyCloseQuartersSurvivalAction.Data>
    {
        private TheStrategistScript strategist;

        public override void Created() { }
        public override void Start(IMonoAgent agent, Data data)
        {
            strategist = agent.Transform.GetComponent<TheStrategistScript>();
            strategist.UpdateActionLabel("Emergency Survival");
        }

        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context)
        {
            if (!strategist.IsInCriticalState)
                return ActionRunState.Completed;

            if (data.Target != null)
            {
                var nav = strategist.GetComponent<UnityEngine.AI.NavMeshAgent>();
                nav.SetDestination(data.Target.Position);
            }

            if (strategist.TryFindThreat(out Transform threat, out float dist) && dist <= 2.5f)
                strategist.PerformMeleeStrike(threat);

            return ActionRunState.Continue;
        }

        public override void End(IMonoAgent agent, Data data)
        {
            strategist.UpdateActionLabel("Idle");
        }

        public class Data : IActionData
        {
            public ITarget Target { get; set; }
        }
    }
}

using CrashKonijn.Goap.Runtime;
using CrashKonijn.Goap.Core;
using CrashKonijn.Agent.Core;

namespace StrategistGOAP
{
    [GoapId("VitalitySensor-unique-003")]
    public class VitalityStatusSensor : LocalWorldSensorBase
    {
        public override void Created() { }
        public override void Update() { }

        public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
        {
            var s = agent.Transform.GetComponent<TheStrategistScript>();
            return new SenseValue(s.IsInCriticalState);
        }
    }
}

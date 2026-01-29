using CrashKonijn.Goap.Runtime;
using CrashKonijn.Goap.Core;
using CrashKonijn.Agent.Core;
using UnityEngine;

namespace StrategistGOAP
{
    [GoapId("ClosestHealthPickupSensor-unique-005")]
    public class ClosestHealthPickupSensor : LocalTargetSensorBase
    {
        public override void Created() { }
        public override void Update() { }

        public override ITarget Sense(IActionReceiver receiver, IComponentReference references, ITarget current)
        {
            var packs = GameObject.FindGameObjectsWithTag("HealthPack");
            if (packs.Length == 0)
                return null;

            Transform nearest = null;
            float best = float.MaxValue;

            foreach (var p in packs)
            {
                float d = Vector3.Distance(receiver.Transform.position, p.transform.position);
                if (d < best)
                {
                    best = d;
                    nearest = p.transform;
                }
            }

            return nearest != null ? new TransformTarget(nearest) : null;
        }
    }
}

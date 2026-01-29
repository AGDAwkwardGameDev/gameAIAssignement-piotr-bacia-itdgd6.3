using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;
using System;

namespace StrategistGOAP
{
    /// <summary>
    /// Target sensor for finding the nearest health pack.
    /// Used by GOAP system to navigate to health pickups.
    /// </summary>
    [GoapId("NearestHealthPackTargetSensor-b8c9d0e1-f2a3-4567-1234-678901234567")]
    public class NearestHealthPackTargetSensor : LocalTargetSensorBase
    {
        public override void Created() { }

        public override void Update() { }

        public override ITarget Sense(IActionReceiver receiver, IComponentReference references, ITarget target)
        {
            GameObject[] healthPacks = GameObject.FindGameObjectsWithTag("HealthPack");

            if (healthPacks.Length == 0)
                return null;

            Transform nearest = null;
            float closestDistance = float.MaxValue;

            foreach (var pack in healthPacks)
            {
                if (pack == null)
                    continue;

                float distance = Vector3.Distance(receiver.Transform.position, pack.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearest = pack.transform;
                }
            }

            if (nearest == null)
                return null;

            return new TransformTarget(nearest);
        }
    }
}
using CrashKonijn.Goap.Runtime;
using CrashKonijn.Goap.Core;
using UnityEngine;

namespace StrategistGOAP
{
    [GoapId("HealthPickupScanner-unique-004")]
    public class HealthPickupScanner : GlobalWorldSensorBase
    {
        public override void Created() { }

        public override SenseValue Sense()
        {
            return new SenseValue(GameObject.FindGameObjectsWithTag("HealthPack").Length > 0);
        }
    }
}

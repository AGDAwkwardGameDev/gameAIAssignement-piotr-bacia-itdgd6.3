using CrashKonijn.Goap.Runtime;

namespace StrategistGOAP
{
    // Health is below a critical threshold
    [GoapId("LowVitalityFlag-unique-001")]
    public class LowVitalityFlag : WorldKeyBase { }

    // At least one health pickup exists in the world
    [GoapId("HealthPickupPresent-unique-002")]
    public class HealthPickupPresent : WorldKeyBase { }

    // Agent has reached a relatively safe position
    [GoapId("RetreatZoneReached-unique-003")]
    public class RetreatZoneReached : WorldKeyBase { }
}

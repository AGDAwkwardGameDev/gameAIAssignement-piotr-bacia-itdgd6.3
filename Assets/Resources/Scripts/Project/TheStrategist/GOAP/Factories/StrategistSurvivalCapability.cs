using CrashKonijn.Goap.Runtime;
using CrashKonijn.Goap.Core;

namespace StrategistGOAP
{
    public class StrategistSurvivalCapability : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("StrategistSurvivalCapability");

            // Survival goal
            builder.AddGoal<SurvivalGoal>()
                .AddCondition<LowVitalityFlag>(Comparison.GreaterThanOrEqual, 1)
                .SetBaseCost(1f);

            // Emergency melee survival action
            builder.AddAction<EmergencyCloseQuartersSurvivalAction>()
                .SetTarget<ClosestHealthPickupTarget>()
                .AddCondition<LowVitalityFlag>(Comparison.GreaterThanOrEqual, 1)
                .AddCondition<HealthPickupPresent>(Comparison.GreaterThanOrEqual, 1)
                .AddEffect<RetreatZoneReached>(EffectType.Increase)
                .SetStoppingDistance(1.5f);

            // Sensors
            builder.AddWorldSensor<VitalityStatusSensor>()
                .SetKey<LowVitalityFlag>();

            builder.AddWorldSensor<HealthPickupScanner>()
                .SetKey<HealthPickupPresent>();

            builder.AddTargetSensor<ClosestHealthPickupSensor>()
                .SetTarget<ClosestHealthPickupTarget>();

            return builder.Build(); // returns ICapabilityConfig
        }
    }
}

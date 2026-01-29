using CrashKonijn.Goap.Runtime;

namespace StrategistGOAP
{
    public class StrategistAgentProfile : AgentTypeFactoryBase
    {
        public override IAgentTypeConfig Create()
        {
            var builder = this.CreateBuilder("StrategistAgentProfile");

            // Add the Strategist's survival capability
            builder.AddCapability<StrategistSurvivalCapability>();

            return builder.Build(); // returns IAgentTypeConfig
        }
    }
}

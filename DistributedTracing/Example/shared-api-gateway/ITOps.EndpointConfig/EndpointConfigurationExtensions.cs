using System;
using NServiceBus;
using NServiceBus.Configuration.AdvancedExtensibility;

namespace ITOps.EndpointConfig
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration Configure(
            this EndpointConfiguration endpointConfiguration,
            Action<RoutingSettings<LearningTransport>> configureRouting = null)
        {
            endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
            endpointConfiguration.Recoverability().Delayed(c => c.NumberOfRetries(0));

            var transport = endpointConfiguration.UseTransport<LearningTransport>();

            var routing = transport.Routing();

            endpointConfiguration.UsePersistence<LearningPersistence>();

            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Commands") && t.Name.EndsWith("Command"));
            conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Events") && t.Name.EndsWith("Event"));

            endpointConfiguration.EnableInstallers();
            endpointConfiguration.EnableOpenTelemetry();

            configureRouting?.Invoke(routing);

            return endpointConfiguration;
        }
    }
}

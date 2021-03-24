using System;
using NServiceBus;

class ConfigureHeartBeats : INeedInitialization
{
    public void Customize(EndpointConfiguration configuration)
    {
        var serviceControlQueue = "Particular.Heartbeats.Servicecontrol";

        var transport = configuration.UseTransport<SqlServerTransport>();
        transport.UseSchemaForQueue(serviceControlQueue, "dbo");
        var heartBeatsInterval = TimeSpan.FromSeconds(5);
        configuration.SendHeartbeatTo(serviceControlQueue, frequency: heartBeatsInterval);
    }
}
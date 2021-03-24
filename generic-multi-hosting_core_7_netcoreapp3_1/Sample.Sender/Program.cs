﻿using System;

namespace Sample.Sender
{
    using System.Threading.Tasks;
    using NServiceBus;

    class Program
    {
        static async Task Main(string[] args)
        {
            var sendOnlyInstance = await StartSendOnlyEndpoint();
            
            try
            {
                Console.WriteLine("Press '1' to send a message from this endpoint to Instance1");
                Console.WriteLine("Press '2' to send a message from this endpoint to Instance2");
                Console.WriteLine("Press any key to exit");

                while (true)
                {
                    var key = Console.ReadKey();
                    Console.WriteLine();
                    var message = new MyMessage();
                    if (key.Key == ConsoleKey.D1)
                    {
                        await sendOnlyInstance.Send("Instance1", message)
                            .ConfigureAwait(false);
                        continue;
                    }
                    if (key.Key == ConsoleKey.D2)
                    {
                        await sendOnlyInstance.Send("Instance2", message)
                            .ConfigureAwait(false);
                        continue;
                    }
                    return;
                }
            }
            finally
            {
                if (sendOnlyInstance != null)
                {
                    await sendOnlyInstance.Stop()
                        .ConfigureAwait(false);
                }
            }
        }
        
        static Task<IEndpointInstance> StartSendOnlyEndpoint()
        {
            var endpointConfiguration = new EndpointConfiguration("Samples.MultiHosting.SendOnly");
            var scanner = endpointConfiguration.AssemblyScanner();

            endpointConfiguration.UsePersistence<LearningPersistence>();
            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            var connection = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=Heartbeats;Integrated Security=True;Max Pool Size=100";
            transport.ConnectionString(connection);
            transport.Transactions(TransportTransactionMode.SendsAtomicWithReceive);
            endpointConfiguration.EnableInstallers();

            return Endpoint.Start(endpointConfiguration);
        }
    }

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
}
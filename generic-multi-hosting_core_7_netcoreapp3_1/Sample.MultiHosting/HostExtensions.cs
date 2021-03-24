using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

public static class HostExtensions
{
    public static async Task RunWithExceptionHandlingAsync(this IHost host, CancellationToken token = default)
    {
        try
        {
            await host.StartAsync(token);

            await host.WaitForShutdownAsync(token);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            if (host is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else
            {
                host.Dispose();
            }
        }
    }
}
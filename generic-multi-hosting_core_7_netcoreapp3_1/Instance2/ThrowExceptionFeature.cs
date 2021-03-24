using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;

public class ThrowExceptionFeature : Feature
{
    public ThrowExceptionFeature()
    {
        EnableByDefault();
    }

    protected override void Setup(FeatureConfigurationContext context)
    {
        context.RegisterStartupTask(() => new InitializeThrowExceptionFeature());
    }
}

public class InitializeThrowExceptionFeature : FeatureStartupTask
{
    protected override Task OnStart(IMessageSession session)
    {
        return Task.FromException(new Exception("Exceptin on Feature initialization"));
    }

    protected override Task OnStop(IMessageSession session)
    {
        throw new NotImplementedException();
    }
}
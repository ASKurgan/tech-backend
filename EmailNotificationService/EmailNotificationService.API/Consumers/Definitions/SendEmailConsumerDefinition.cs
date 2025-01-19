using MassTransit;

namespace EmailNotificationService.API.Consumers.Definitions;

/// <summary>
/// Consumer setup.
/// </summary>
public class SendEmailConsumerDefinition : ConsumerDefinition<SendEmailConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<SendEmailConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(
            c => c.Incremental(3, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5)));
    }
}
 
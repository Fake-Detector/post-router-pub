using Common.Library.Kafka.Consumer.Interfaces;
using Common.Library.Kafka.Producer.Interfaces;
using Confluent.Kafka;
using Fake.Detection.Post.Bridge.Contracts;
using Fake.Detection.Post.Monitoring.Client.Services.Interfaces;
using Fake.Detection.Post.Monitoring.Messages;
using Newtonsoft.Json;

namespace Fake.Detection.Post.Router.Services;

public class ConsumerHandler : IConsumerHandler<Bridge.Contracts.Post>
{
    private readonly TargetTopicSelector _targetTopicSelector;
    private readonly IProducerHandler<Item> _itemProducer;
    private readonly IMonitoringClient _monitoringClient;
    private readonly ILogger<ConsumerHandler> _logger;

    public ConsumerHandler(
        TargetTopicSelector targetTopicSelector,
        IProducerHandler<Item> itemProducer,
        IMonitoringClient monitoringClient,
        ILogger<ConsumerHandler> logger)
    {
        _targetTopicSelector = targetTopicSelector;
        _itemProducer = itemProducer;
        _monitoringClient = monitoringClient;
        _logger = logger;
    }

    public async Task HandleMessage(ConsumeResult<string, Bridge.Contracts.Post> message,
        CancellationToken cancellationToken)
    {
        var post = message.Message.Value;

        try
        {
            foreach (var items in post.Items.GroupBy(it => it.Type))
            {
                var topic = _targetTopicSelector.Select(items.Key);

                foreach (var item in items)
                {
                    try
                    {
                        await _itemProducer.Produce(topic, item, cancellationToken);
                        await _monitoringClient.SendToMonitoring(MonitoringType.Info, JsonConvert.SerializeObject(item));
                    }
                    catch (Exception e)
                    {
                        await _monitoringClient.SendToMonitoring(
                            MonitoringType.Error,
                            JsonConvert.SerializeObject(item),
                            JsonConvert.SerializeObject(e));

                        _logger.LogError(e, "Error while handling: {Message}", JsonConvert.SerializeObject(item));
                    }
                }
            }
        }
        catch (Exception e)
        {
            await _monitoringClient.SendToMonitoring(
                MonitoringType.Error,
                JsonConvert.SerializeObject(post),
                JsonConvert.SerializeObject(e));

            _logger.LogError(e, "Error while handling: {Message}", JsonConvert.SerializeObject(post));
        }
    }
}
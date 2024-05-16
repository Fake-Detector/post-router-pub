using Common.Library.Kafka.Common.Extensions;
using Common.Library.Kafka.Consumer.Extensions;
using Common.Library.Kafka.Producer.Extensions;
using Fake.Detection.Post.Bridge.Contracts;
using Fake.Detection.Post.Monitoring.Client.Extensions;
using Fake.Detection.Post.Monitoring.Messages;
using Fake.Detection.Post.Router.Configure;
using Fake.Detection.Post.Router.Services;

namespace Fake.Detection.Post.Router;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<PostAudioProduceOptions>(_configuration.GetSection(nameof(PostAudioProduceOptions)));
        services.Configure<PostVideoProduceOptions>(_configuration.GetSection(nameof(PostVideoProduceOptions)));
        services.Configure<PostTextProduceOptions>(_configuration.GetSection(nameof(PostTextProduceOptions)));
        services.Configure<PostPhotoProduceOptions>(_configuration.GetSection(nameof(PostPhotoProduceOptions)));

        services.AddCommonKafka(_configuration);
        services.AddConsumerHandler<Bridge.Contracts.Post, PostConsumerOptions, ConsumerHandler>(_configuration);
        services.AddProducerHandler<Item>();
        services.AddProducerHandler<MonitoringMessage>();

        services.AddSingleton<TargetTopicSelector>();

        services.AddMonitoring(_configuration);
    }

    public void Configure()
    {
    }
}
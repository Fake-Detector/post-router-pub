using Fake.Detection.Post.Bridge.Contracts;
using Fake.Detection.Post.Router.Configure;
using Microsoft.Extensions.Options;

namespace Fake.Detection.Post.Router.Services;

public sealed class TargetTopicSelector
{
    private readonly IOptionsMonitor<PostAudioProduceOptions> _postAudioOptions;
    private readonly IOptionsMonitor<PostPhotoProduceOptions> _postPhotoOptions;
    private readonly IOptionsMonitor<PostTextProduceOptions> _postTextOptions;
    private readonly IOptionsMonitor<PostVideoProduceOptions> _postVideoOptions;

    public TargetTopicSelector(
        IOptionsMonitor<PostAudioProduceOptions> postAudioOptions,
        IOptionsMonitor<PostPhotoProduceOptions> postPhotoOptions,
        IOptionsMonitor<PostTextProduceOptions> postTextOptions,
        IOptionsMonitor<PostVideoProduceOptions> postVideoOptions)
    {
        _postAudioOptions = postAudioOptions;
        _postPhotoOptions = postPhotoOptions;
        _postTextOptions = postTextOptions;
        _postVideoOptions = postVideoOptions;
    }

    public string Select(ItemType type) =>
        type switch
        {
            ItemType.Text => _postTextOptions.CurrentValue.TopicName,
            ItemType.Image => _postPhotoOptions.CurrentValue.TopicName,
            ItemType.ImageUrl => _postPhotoOptions.CurrentValue.TopicName,
            ItemType.Audio => _postAudioOptions.CurrentValue.TopicName,
            ItemType.AudioUrl => _postAudioOptions.CurrentValue.TopicName,
            ItemType.Video => _postVideoOptions.CurrentValue.TopicName,
            ItemType.VideoUrl => _postVideoOptions.CurrentValue.TopicName,
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Incorrect post type")
        };
}
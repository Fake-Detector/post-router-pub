namespace Fake.Detection.Post.Router.Messages;

public record PostMessage(
    long ChatId,
    int MessageId,
    PostType PostType,
    string? Text,
    string? FileId,
    DateTime CreatedAt
);
using System.Diagnostics.CodeAnalysis;

namespace DiamondRush.MonoGame.Core.Messages.Abstractions;

public interface IMessenger
{
    public void SendMessage(
        string messageName,
        IMessage message);

    public void SendMessage(
        IMessage message);

    public bool TryReadMessage<TMessage>(
        string messageName,
        [MaybeNullWhen(false)] out TMessage message)
        where TMessage : IMessage;

    public bool TryReadMessage<TMessage>(
        [MaybeNullWhen(false)] out TMessage message)
        where TMessage : IMessage;

    public bool RemoveMessage(
        string messageName);

    public bool RemoveMessage<TMessage>();
}

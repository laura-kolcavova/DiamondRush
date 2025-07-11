using DiamondRush.MonoGame.Core.Messages.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace DiamondRush.MonoGame.Core.Messages;

public sealed class Messenger :
    IMessenger
{
    private readonly Dictionary<string, IMessage> _messagesByName;

    public Messenger()
    {
        _messagesByName = [];
    }

    public void ClearMessages()
    {
        _messagesByName.Clear();
    }

    public void SendMessage(
        string messageName,
        IMessage message)
    {
        if (!_messagesByName.TryAdd(messageName, message))
        {
            throw new InvalidOperationException(
                $"Message with name {messageName} already exists in the message box.");
        }
    }

    public void SendMessage(
        IMessage message)
    {
        var messageName = message
            .GetType()
            .Name;

        SendMessage(
            messageName,
            message);
    }

    public bool TryReadMessage<TMessage>(
        string messageName,
        [MaybeNullWhen(false)] out TMessage message)
        where TMessage : IMessage
    {
        if (!_messagesByName.TryGetValue(
            messageName,
            out var value))
        {
            message = default;

            return false;
        }

        if (value is not TMessage typedMessage)
        {
            throw new InvalidCastException(
                $"Message with name {messageName} is not of type {typeof(TMessage).Name}.");
        }

        message = typedMessage;

        return true;
    }

    public bool TryReadMessage<TMessage>(
        [MaybeNullWhen(false)] out TMessage message)
        where TMessage : IMessage
    {
        var messageName = typeof(TMessage)
            .Name;

        return TryReadMessage(
            messageName,
            out message);
    }
}

using DiamondRush.MonoGame.Core.Messages.Abstractions;
using DiamondRush.MonoGame.Core.Systems.Abstractions;
using DiamondRush.MonoGame.Play.Messages;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class PlayerControlSystem :
    IUpdateSystem
{
    private readonly IMessenger _messenger;

    private readonly PlayContext _playContext;

    public PlayerControlSystem(
        IMessenger messenger,
        PlayContext playContext)
    {
        _messenger = messenger;
        _playContext = playContext;
    }

    private bool _pressStarted = false;

    private Vector2 _pressStartPosition = Vector2.Zero;

    private Vector2 _pressMovePosition = Vector2.Zero;

    public void Update(GameTime gameTime)
    {
        if (!IsUpdateEnabled())
        {
            return;
        }

        var mouseState = Mouse.GetState();

        HandlePressStarted(mouseState);

        HandlePressMove(mouseState);

        HandlePressReleased(mouseState);
    }

    private bool IsUpdateEnabled() =>
       _playContext.PlayState == PlayState.WaitingForInput;

    private void HandlePressStarted(
        MouseState mouseState)
    {
        if (!(!_pressStarted && mouseState.LeftButton == ButtonState.Pressed))
        {
            return;
        }

        var pressStartPosition = new Vector2(
            mouseState.X,
            mouseState.Y);

        _pressStarted = true;

        _pressStartPosition = pressStartPosition;

        _messenger.SendMessage(
            new PlayerPressStartedMessage()
            {
                Position = pressStartPosition
            });
    }

    private void HandlePressMove(
        MouseState mouseState)
    {
        if (!(_pressStarted && mouseState.LeftButton == ButtonState.Pressed))
        {
            return;
        }

        var pressMovePosition = new Vector2(
            mouseState.X,
            mouseState.Y);

        if (pressMovePosition == _pressStartPosition)
        {
            return;
        }

        if (pressMovePosition == _pressMovePosition)
        {
            return;
        }

        _pressMovePosition = pressMovePosition;

        _messenger.SendMessage(
            new PlayerPressMovedMessage()
            {
                StartPosition = _pressStartPosition,
                CurrentPosition = pressMovePosition,
            });
    }

    private void HandlePressReleased(
        MouseState mouseState)
    {
        if (!(_pressStarted && mouseState.LeftButton == ButtonState.Released))
        {
            return;
        }

        var pressEndPosition = new Vector2(
            mouseState.X,
            mouseState.Y);

        ResetPressState();

        _messenger.SendMessage(
            new PlayerPressReleasedMessage()
            {
                StartPosition = _pressStartPosition,
                EndPosition = pressEndPosition
            });
    }

    private void ResetPressState()
    {
        _pressStarted = false;
        _pressStartPosition = Vector2.Zero;
        _pressMovePosition = Vector2.Zero;
    }
}

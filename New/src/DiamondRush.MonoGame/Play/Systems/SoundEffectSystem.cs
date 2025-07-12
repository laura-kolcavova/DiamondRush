using DiamondRush.MonoGame.Core.Messages.Abstractions;
using DiamondRush.MonoGame.Core.Systems.Abstractions;
using DiamondRush.MonoGame.Play.Content.Abstractions;
using DiamondRush.MonoGame.Play.Messages;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class SoundEffectSystem
    : IUpdateSystem
{
    private readonly IMessenger _messenger;

    private readonly IPlaySceneContentProvider _playSceneContentProvider;

    public SoundEffectSystem(
        IMessenger messenger,
        IPlaySceneContentProvider playSceneContentProvider)
    {
        _messenger = messenger;

        _playSceneContentProvider = playSceneContentProvider;
    }

    public void Update(GameTime gameTime)
    {
        if (_messenger.TryReadMessage<GemCollectingStartedMessage>(
            out var _))
        {
            _playSceneContentProvider
                .GemCollectSoundEffect
                .Play();

            _messenger.RemoveMessage<GemCollectingStartedMessage>();
        }
    }
}

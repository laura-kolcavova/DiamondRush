using DiamondRush.MonoGame.Core.Systems;
using DiamondRush.MonoGame.Play.Content.Abstractions;
using DiamondRush.MonoGame.Play.Messages;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;

namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class SoundEffectSystem
    : IUpdateSystem
{
    private readonly IEntityContext _entityContext;

    private readonly IPlaySceneContentProvider _playSceneContentProvider;

    public SoundEffectSystem(
        IEntityContext entityContext,
        IPlaySceneContentProvider playSceneContentProvider)
    {
        _entityContext = entityContext;
        _playSceneContentProvider = playSceneContentProvider;
    }

    public void Update(GameTime gameTime)
    {
        if (_entityContext.State.Contains(GemCollectingStartedMessage.Name))
        {
            _playSceneContentProvider.GemCollectSoundEffect.Play();

            _entityContext.State.Remove(GemCollectingStartedMessage.Name);
        }
    }
}

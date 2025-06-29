using DiamondRush.MonoGame.Core.Systems;
using DiamondRush.MonoGame.Core.Textures;
using DiamondRush.MonoGame.Play.Components;
using DiamondRush.MonoGame.Play.Renderers;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class RenderSystem :
    IDrawSystem
{
    private readonly IEntityContext _entityContext;
    private readonly SpriteBatch _spriteBatch;

    private readonly BackgroundRenderer _backgroundRenderer;

    private readonly IEntityView _entityView;

    private readonly IComponentStore<Identity> _identityStore;
    private readonly IComponentStore<Sprite> _spriteStore;
    private readonly IComponentStore<Vector2> _positionStore;

    public RenderSystem(
        IEntityContext entityContext,
        SpriteBatch spriteBatch,
        GraphicsDevice graphicsDevice)
    {
        _entityContext = entityContext;
        _spriteBatch = spriteBatch;

        _backgroundRenderer = new BackgroundRenderer(graphicsDevice);

        _identityStore = _entityContext.UseStore<Identity>();
        _spriteStore = _entityContext.UseStore<Sprite>();
        _positionStore = _entityContext.UseStore<Vector2>();

        _entityView = _entityContext
            .UseQuery()
            .With<Identity>()
            .With<Sprite>()
            .With<Vector2>()
            .AsView();
    }

    public void Draw(GameTime gameTime)
    {
        foreach (var entity in _entityView.AsEnumerable())
        {
            var identity = _identityStore.Get(entity);
            var sprite = _spriteStore.Get(entity);
            var position = _positionStore.Get(entity);

            switch (identity.EntityType)
            {
                case EntityType.Background:
                    _backgroundRenderer.Render(_spriteBatch, sprite, position);
                    break;
            }
        }
    }
}

using DiamondRush.MonoGame.Core.Systems;
using DiamondRush.MonoGame.Play.Components;
using DiamondRush.MonoGame.Play.Renderers;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame.Play.Systems;

internal sealed class RenderSystem :
    IDrawSystem
{
    private readonly IEntityView _entityView;

    private readonly IComponentStore<Identity> _identityStore;

    private readonly EntitySpriteRenderer _entitySpriteRenderer;

    private readonly GameBoardEntityRenderer _gameBoardEntityRenderer;

    public RenderSystem(
        IEntityContext entityContext,
        SpriteBatch spriteBatch)
    {
        _entityView = entityContext
            .UseQuery()
            .With<Identity>()
            .AsView();

        _identityStore = entityContext.UseStore<Identity>();

        _entitySpriteRenderer = new EntitySpriteRenderer(
            entityContext,
            spriteBatch);

        _gameBoardEntityRenderer = new GameBoardEntityRenderer(
            entityContext,
            spriteBatch);
    }

    public void Draw(GameTime gameTime)
    {
        foreach (var entity in _entityView.AsEnumerable())
        {
            var identity = _identityStore.Get(entity);

            switch (identity.EntityType)
            {
                case EntityType.GameBoard:
                    _gameBoardEntityRenderer.Render(entity);
                    break;
                default:
                    _entitySpriteRenderer.Render(entity);
                    break;
            }
        }
    }
}

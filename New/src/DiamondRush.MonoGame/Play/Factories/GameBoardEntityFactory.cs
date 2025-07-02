using DiamondRush.MonoGame.Core;
using DiamondRush.MonoGame.Core.Textures;
using DiamondRush.MonoGame.Play.Components;
using DiamondRush.MonoGame.Play.Content.Abstractions;
using LightECS;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame.Play.Factories;

internal sealed class GameBoardEntityFactory
{
    private readonly IPlaySceneContentProvider _playSceneContentProvider;
    private readonly GraphicsDevice _graphicsDevice;

    public GameBoardEntityFactory(
        IPlaySceneContentProvider playSceneContentProvider,
        GraphicsDevice graphicsDevice)
    {
        _playSceneContentProvider = playSceneContentProvider;
        _graphicsDevice = graphicsDevice;
    }

    public Entity Create(
        IEntityContext entityContext,
        int rows,
        int columns)
    {
        var entity = entityContext.CreateEntity();

        var identity = new Identity
        {
            EntityType = EntityType.GameBoard,
        };

        var gameBoard = new GameBoard
        {
            Rows = rows,
            Columns = columns,
        };

        var gameBoardStyles = new GameBoardStyles
        {
            BoardSprite = new Sprite
            {
                TextureRegion = new TextureRegion(
                    _playSceneContentProvider.BlankTexture),
                Color = Constants.GameBoardColor,
            },

            BorderSprite = new Sprite
            {
                TextureRegion = new TextureRegion(
                    _playSceneContentProvider.BlankTexture),
                Color = Constants.GameBoardBorderColor,
            },

            SpacingSprite = new Sprite
            {
                TextureRegion = new TextureRegion(
                    _playSceneContentProvider.BlankTexture),
                Color = Constants.GameBoardSpacingColor,
            }
        };

        var spacingWidthSum = (columns - 1) * Constants.GameBoardSpacingWidth;
        var spacingHeightSum = (rows - 1) * Constants.GameBoardSpacingWidth;

        var width = columns * Constants.GameBoardFieldSize + spacingWidthSum;
        var height = rows * Constants.GameBoardFieldSize + spacingHeightSum;

        var rectTransform = new RectTransform
        {
            Position = new Vector2(
                (_graphicsDevice.Viewport.Width - width) / 2,
                (_graphicsDevice.Viewport.Height - height) / 2),
            Width = width,
            Height = height
        };

        entityContext.Set(entity, identity);
        entityContext.Set(entity, gameBoard);
        entityContext.Set(entity, gameBoardStyles);
        entityContext.Set(entity, rectTransform);

        return entity;
    }
}

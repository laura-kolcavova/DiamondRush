﻿using DiamondRush.MonoGame.Core;
using DiamondRush.MonoGame.Core.Textures;
using DiamondRush.MonoGame.Play.Components;
using LightECS;
using LightECS.Abstractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DiamondRush.MonoGame.Play.Renderers;

internal sealed class GameBoardEntityRenderer
{
    private readonly IEntityContext _entityContext;

    private readonly SpriteBatch _spriteBatch;

    private readonly IComponentStore<GameBoard> _gameBoardStore;

    private readonly IComponentStore<GameBoardStyles> _gameBoardStylesStore;

    private readonly IComponentStore<RectTransform> _rectTransformStore;

    public GameBoardEntityRenderer(
        IEntityContext entityContext,
        SpriteBatch spriteBatch)
    {
        _entityContext = entityContext;
        _spriteBatch = spriteBatch;

        _gameBoardStore = _entityContext.UseStore<GameBoard>();
        _gameBoardStylesStore = _entityContext.UseStore<GameBoardStyles>();
        _rectTransformStore = _entityContext.UseStore<RectTransform>();
    }

    public void Render(
        Entity entity)
    {
        var gameBoard = _gameBoardStore.Get(entity);
        var gameBoardStyles = _gameBoardStylesStore.Get(entity);
        var rectTransform = _rectTransformStore.Get(entity);

        var destinationRectangle = new Rectangle(
            (int)rectTransform.Position.X,
            (int)rectTransform.Position.Y,
            (int)rectTransform.Width,
            (int)rectTransform.Height);

        _spriteBatch.DrawSprite(
            gameBoardStyles.BoardSprite,
            destinationRectangle);

        RenderSpacing(
            gameBoard,
            gameBoardStyles,
            rectTransform);

        RenderBorder(
            gameBoardStyles,
            rectTransform);
    }

    private void RenderSpacing(
        GameBoard gameBoard,
        GameBoardStyles gameBoardStyles,
        RectTransform rectTransform)
    {
        foreach (var rowIndex in Enumerable.Range(1, gameBoard.Rows - 1))
        {
            var rowPositionY = rowIndex * Constants.GameBoard.FieldSize
                + (rowIndex - 1) * Constants.GameBoard.SpacingWidth;

            var rowDestinationRectangle = new Rectangle(
                (int)rectTransform.Position.X,
                (int)rectTransform.Position.Y + rowPositionY,
                (int)rectTransform.Width,
                Constants.GameBoard.SpacingWidth);

            _spriteBatch.DrawSprite(
                gameBoardStyles.SpacingSprite,
                rowDestinationRectangle);
        }

        foreach (var colIndex in Enumerable.Range(1, gameBoard.Columns - 1))
        {
            var colPositionX = colIndex * Constants.GameBoard.FieldSize
                + (colIndex - 1) * Constants.GameBoard.SpacingWidth;

            var colDestinationRectangle = new Rectangle(
                (int)rectTransform.Position.X + colPositionX,
                (int)rectTransform.Position.Y,
                Constants.GameBoard.SpacingWidth,
                (int)rectTransform.Height);

            _spriteBatch.DrawSprite(
                gameBoardStyles.SpacingSprite,
                colDestinationRectangle);
        }
    }

    private void RenderBorder(
        GameBoardStyles gameBoardStyles,
        RectTransform rectTransform)
    {
        var leftBorderDestinationRectangle = new Rectangle(
            (int)rectTransform.Position.X - Constants.GameBoard.BorderWidth,
            (int)rectTransform.Position.Y - Constants.GameBoard.BorderWidth,
            Constants.GameBoard.BorderWidth,
            (int)rectTransform.Height + Constants.GameBoard.BorderWidth);

        _spriteBatch.DrawSprite(
            gameBoardStyles.BorderSprite,
            leftBorderDestinationRectangle);

        var topBorderDestinationRectangle = new Rectangle(
            (int)rectTransform.Position.X,
            (int)rectTransform.Position.Y - Constants.GameBoard.BorderWidth,
            (int)rectTransform.Width + Constants.GameBoard.BorderWidth,
            Constants.GameBoard.BorderWidth);

        _spriteBatch.DrawSprite(
            gameBoardStyles.BorderSprite,
            topBorderDestinationRectangle);

        var rightBorderDestinationRectangle = new Rectangle(
            (int)(rectTransform.Position.X + rectTransform.Width),
            (int)rectTransform.Position.Y,
            Constants.GameBoard.BorderWidth,
            (int)rectTransform.Height + Constants.GameBoard.BorderWidth);

        _spriteBatch.DrawSprite(
            gameBoardStyles.BorderSprite,
            rightBorderDestinationRectangle);

        var bottomBorderDestinationRectangle = new Rectangle(
            (int)rectTransform.Position.X - Constants.GameBoard.BorderWidth,
            (int)(rectTransform.Position.Y + rectTransform.Height),
            (int)rectTransform.Width + Constants.GameBoard.BorderWidth,
            Constants.GameBoard.BorderWidth);

        _spriteBatch.DrawSprite(
            gameBoardStyles.BorderSprite,
            bottomBorderDestinationRectangle);
    }
}

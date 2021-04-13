using DiamondRush.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoECS.Engine.Graphics;
using MonoECS.Engine.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiamondRush.Models
{
    public static class GameBoard
    {
        public static void Render(BoardAppearanceComponent boardAppearance, BoardFieldComponent boardField, RendererComponent renderer, Transform2DComponent transform2D, SpriteBatch sb)
        {
            if (!renderer.IsVisible)
                return;

            var bounding = renderer.GetBoundingRectangle(transform2D);

            DrawBorder(boardAppearance, bounding, sb);
            DrawBoardField(boardAppearance, boardField, bounding, sb);
            DrawGrid(boardAppearance, boardField, bounding, sb);
        }

        private static void DrawBorder(BoardAppearanceComponent appearance, Rectangle bounding, SpriteBatch sb)
        {
            var borderRect = bounding;

            borderRect.X -= 1;
            borderRect.Width += appearance.Theme.BorderWidth;
            borderRect.Y -= appearance.Theme.BorderWidth;
            borderRect.Height += appearance.Theme.BorderWidth;

            sb.DrawRectangle(borderRect, appearance.Theme.BorderColor, appearance.Theme.BorderWidth);
        }

        private static void DrawBoardField(BoardAppearanceComponent appearance, BoardFieldComponent boardField, Rectangle bounding, SpriteBatch sb)
        {
            int fieldWidth = appearance.FieldWidth;
            int fieldHeight = appearance.FieldHeight;
            int fieldSpace = appearance.FieldSpace;

            for (int row = 0; row < boardField.Rows; row++)
            {
                for (int col = 0; col < boardField.Cols; col++)
                {
                    int posX = (col * fieldWidth) + (col * fieldSpace);
                    int posY = (row * fieldHeight) + (row * fieldSpace);

                    var rect = new Rectangle
                    (
                        bounding.Location.X + posX,
                        bounding.Location.Y + posY,
                        fieldWidth,
                        fieldHeight
                    );

                    sb.FillRectangle(rect, appearance.Theme.FieldColor);
                }
            }
        }

        private static void DrawGrid(BoardAppearanceComponent appearance, BoardFieldComponent boardField, Rectangle bounding, SpriteBatch sb)
        {
            int fieldWidth = appearance.FieldWidth;
            int fieldHeight = appearance.FieldHeight;
            int fieldSpace = appearance.FieldSpace;
            Color gridColor = appearance.Theme.FieldSpaceColor;

            Vector2 point1;
            Vector2 point2;

            for (int col = 0; col < boardField.Cols; col++)
            {
                if (col != 0)
                {

                    point1 = new Vector2(
                         bounding.X + (col * fieldWidth) + (col * fieldSpace),
                         bounding.Y);

                    point2 = new Vector2(
                        bounding.X + (col * fieldWidth) + (col * fieldSpace),
                        bounding.Bottom);

                    sb.DrawLine(point1, point2, gridColor, fieldSpace);
                }
            }

            for (int row = 0; row < boardField.Rows; row++)
            {
                if (row != 0)
                {
                    point1 = new Vector2(
                        bounding.X,
                        bounding.Y + row * fieldHeight + row * fieldSpace - fieldSpace);
                    point2 = new Vector2(
                        bounding.Right,
                        bounding.Y + row * fieldHeight + row * fieldSpace - fieldSpace);

                    sb.DrawLine(point1, point2, gridColor, fieldSpace);
                }
            }
        }
    }
}

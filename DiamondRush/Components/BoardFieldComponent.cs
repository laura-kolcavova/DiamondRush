using DiamondRush.Data.Classes;
using DiamondRush.Data.Structs;
using MonoECS.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiamondRush.Components
{
    public class BoardFieldComponent : IEntityComponent
    {
        public GemField[,] Field { get; private set; }

        public int Rows { get; private set; }

        public int Cols { get; private set; }

        public int MinCountForGemGroup { get; set; }

        public void SetField(GemField[,] field) 
        {
            Field = field;

            Rows = field.GetLength(0);
            Cols = field.GetLength(1);
        }

        public GemField GetFieldAt(int row, int col)
        {
            if (row >= Rows || row < 0 || col >= Cols || col < 0)
                return null;
               //throw new ArgumentOutOfRangeException("Parametres are out of field");

            return Field[row, col];
        }

        public GemField GetFieldLeftOf(GemField field)
        {
            return GetFieldAt(field.Row, field.Col - 1);
        }

        public GemField GetFieldRightOf(GemField field)
        {
            return GetFieldAt(field.Row, field.Col + 1);
        }

        public GemField GetFieldBottomOf(GemField field)
        {
            return GetFieldAt(field.Row + 1, field.Col);
        }

        public GemField GetFieldUpOf(GemField field)
        {
            return GetFieldAt(field.Row - 1, field.Col);
        }

        public GemField[] GetEmptyFields()
        {
            return Field
                .Cast<GemField>()
                .Where(f => f.IsEmpty)
                .ToArray();               
        }   
    } 
}
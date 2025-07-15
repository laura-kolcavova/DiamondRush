using DiamondRush.MonoGame.Play.Components;

namespace DiamondRush.MonoGame.Play;

internal sealed class GameBoardFields
{
    private readonly GameBoard _gameBoard;

    private readonly GameBoardField[,] _fields;

    public GameBoardFields(GameBoard gameBoard)
    {
        _gameBoard = gameBoard;

        _fields = CreateFields(_gameBoard);
    }

    public int Rows => _gameBoard.Rows;

    public int Columns => _gameBoard.Columns;

    public GameBoardField GetField(
        int rowIndex,
        int columnIndex)
    {
        AssertRowIndexInBounds(rowIndex);

        AssertColumnIndexInBounds(columnIndex);

        return _fields[rowIndex, columnIndex];
    }

    public IEnumerable<GameBoardField> FieldsInRow(
        int rowIndex)
    {
        AssertRowIndexInBounds(rowIndex);

        for (var columnIndex = 0; columnIndex < _gameBoard.Columns; columnIndex++)
        {
            yield return _fields[rowIndex, columnIndex];
        }
    }

    public IEnumerable<GameBoardField> FieldsInColumn(
        int columnIndex)
    {
        AssertColumnIndexInBounds(columnIndex);

        for (var rowIndex = 0; rowIndex < _gameBoard.Rows; rowIndex++)
        {
            yield return _fields[rowIndex, columnIndex];
        }
    }

    public IEnumerable<GameBoardField> AsEnumerable()
    {
        for (var row = 0; row < _gameBoard.Rows; row++)
        {
            for (var column = 0; column < _gameBoard.Columns; column++)
            {
                yield return _fields[row, column];
            }
        }
    }

    private void AssertRowIndexInBounds(
        int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= _gameBoard.Rows)
        {
            throw new ArgumentOutOfRangeException(nameof(rowIndex), "Row index is out of bounds.");
        }
    }

    private void AssertColumnIndexInBounds(
        int columnIndex)
    {
        if (columnIndex < 0 || columnIndex >= _gameBoard.Columns)
        {
            throw new ArgumentOutOfRangeException(nameof(columnIndex), "Column index is out of bounds.");
        }
    }

    private static GameBoardField[,] CreateFields(
        GameBoard gameBoard)
    {
        var fields = new GameBoardField[
            gameBoard.Rows,
            gameBoard.Columns];

        for (var rowIndex = 0; rowIndex < gameBoard.Rows; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < gameBoard.Columns; columnIndex++)
            {
                fields[rowIndex, columnIndex] = new GameBoardField(
                    rowIndex,
                    columnIndex);
            }
        }

        return fields;
    }
}

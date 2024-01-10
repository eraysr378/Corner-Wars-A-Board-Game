using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
public enum Players
{
    Player1,
    Player2,
    None
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private Players turn;
    [SerializeField] private bool firstInputTaken;
    [SerializeField] private List<CellProperties> cells;
    [SerializeField] private List<PieceProperties> pieces;
    [SerializeField] private PieceProperties selectedPiece;
    [SerializeField] private List<List<CellProperties>> moveableCellsList = new List<List<CellProperties>>();
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private TMP_Text loserText;
    [SerializeField] private Players winner = Players.None;
    [SerializeField] private bool isBotActive = true;
    [SerializeField] private float pieceHighlight;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private float pieceMoveSpeed;
    [SerializeField] private float botWaitTime;
    [SerializeField] private GameObject menuBtn;
    List<Vector2> winCellsp1 = new List<Vector2>
        {
            new Vector2(5, 5),
            new Vector2(5, 6),
            new Vector2(5, 7),
            new Vector2(6, 5),
            new Vector2(6, 6),
            new Vector2(6, 7),
            new Vector2(7, 5),
            new Vector2(7, 6),
            new Vector2(7, 7)
        };



    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(audioManager.PlayGameStartSound());
        turn = Players.Player2;

    }

    // Update is called once per frame
    void Update()
    {
        HighlightSelectedPiece();
        if (!isBotActive || turn == Players.Player2)
        {
            PieceProperties hitPiece = PieceProperties.GetClickedPiece();
            CellProperties hitCell = CellProperties.GetClickedCell();
            if (hitPiece != null || hitCell != null)
            {

                if (hitCell == null)
                    Debug.Log("cell null");
                if (hitPiece == null)
                    Debug.Log("piece null");

                if (hitPiece != null)
                {
                    firstInputTaken = false;
                    if (selectedPiece != null)
                    {
                        selectedPiece.image.color = new Color(selectedPiece.image.color.r, selectedPiece.image.color.g, selectedPiece.image.color.b, 1);
                        selectedPiece = null;
                    }

                }
                else
                {
                    firstInputTaken = true;
                }

                if (!firstInputTaken)
                {
                    SelectPiece(hitPiece);
                }
                else
                {
                    StartCoroutine(MovePiece(hitCell));
                }
            }
        }
        //if (turn == Players.Player2)
        //{
        //    StartCoroutine(MovePieceBot2());
        //    turn = Players.None;
        //}
        else if (turn == Players.Player1)
        {

            StartCoroutine(MovePieceBot1());
            turn = Players.None;

        }

    }

    private void SelectPiece(PieceProperties hitPiece)
    {

        RectifyHighlightedCells();
        ResetMoveableCells();

        if (hitPiece == null)
        {
            Debug.Log("Hit piece null");
        }
        if (hitPiece != null && hitPiece.GetOwner() == turn)
        {
            foreach (var piece in pieces)
            {
                if (hitPiece.col == piece.col && hitPiece.row == piece.row)
                {
                    selectedPiece = piece;
                }
            }
            firstInputTaken = true;
            FillMoveableCellList();
            HighlightValidMoves();
        }
        else
        {

            firstInputTaken = false;
        }


    }

    private IEnumerator MovePiece(CellProperties hitCell)
    {

        if (hitCell != null)
        {

            foreach (var cellList in moveableCellsList)
            {
                for (int i = 0; i < cellList.Count; i++)
                {
                    if (cellList[i].GetRow() == hitCell.GetRow() && cellList[i].GetCol() == hitCell.GetCol())
                    {
                        Debug.Log("cell moveable");
                        CellProperties oldCell = FindInCells(cells, selectedPiece.row, selectedPiece.col);
                        oldCell.SetPiece(Pieces.None);

                        selectedPiece.col = cellList[i].GetCol();
                        selectedPiece.row = cellList[i].GetRow();
                        RectifyHighlightedCells();

                        if (turn == Players.Player1)
                        {
                            cellList[i].SetPiece(Pieces.P1);
                            Debug.Log(cellList[i].gameObject.name + " p1");
                        }
                        else
                        {
                            cellList[i].SetPiece(Pieces.P2);
                            Debug.Log(cellList[i].gameObject.name + " p2");
                        }
                        Players nextTurn;
                        if (turn == Players.Player2)
                        {
                            nextTurn = Players.Player1;
                        }
                        else
                        {
                            nextTurn = Players.Player2;
                        }

                        turn = Players.None;
                        PieceProperties selPiece = selectedPiece;
                        selectedPiece.image.color = new Color(selectedPiece.image.color.r, selectedPiece.image.color.g, selectedPiece.image.color.b, 1);
                        selectedPiece = null;
                        for (int j = 0; j <= i; j++)
                        {
                            audioManager.PlayMoveSound();
                            selPiece.transform.position = cellList[j].transform.position;
                            yield return new WaitForSeconds(pieceMoveSpeed);
                        }

                        winner = CheckWinner();
                        if (winner != Players.None)
                        {
                            menuBtn.SetActive(true);
                            nextTurn = Players.None;
                            if (winner == Players.Player1)
                            {
                                audioManager.PlayLoseSound();
                                loserText.gameObject.SetActive(true);
                            }
                            else
                            {
                                audioManager.PlayWinSound();
                                winnerText.gameObject.SetActive(true);
                            }
                        }
                        turn = nextTurn;
                        firstInputTaken = false;
                        RectifyHighlightedCells();
                        ResetMoveableCells();
                        yield break;
                    }
                }
            }
            if (selectedPiece != null)
            {
                selectedPiece.image.color = new Color(selectedPiece.image.color.r, selectedPiece.image.color.g, selectedPiece.image.color.b, 1);
                selectedPiece = null;
            }
            firstInputTaken = false;
            RectifyHighlightedCells();
            ResetMoveableCells();
        }
        else
        {
            if (selectedPiece != null)
            {
                selectedPiece.image.color = new Color(selectedPiece.image.color.r, selectedPiece.image.color.g, selectedPiece.image.color.b, 1);
                selectedPiece = null;
            }
        }

    }
    private void CalculateValidMoves(List<CellProperties> copyCells, int startRow, int startCol, bool isJumped, int index)
    {
        int selRow = selectedPiece.row;
        int selCol = selectedPiece.col;

        if (moveableCellsList.Count <= index)
        {
            moveableCellsList.Add(new List<CellProperties>());
        }
        // make sure it does not calculate going back to start position.
        if (isJumped && selCol == startCol && selRow == startRow)
        {
            return;
        }
        if (turn == Players.Player1)
        {
            // double jump left
            if (selCol < 6 && copyCells[selRow * 8 + selCol + 1].GetPiece() != Pieces.None && copyCells[selRow * 8 + selCol + 2].GetPiece() == Pieces.None)
            {
                // make sure cell is not visited previously
                if (FindInCells(moveableCellsList[index], selRow, selCol + 2) == null)
                {
                    moveableCellsList[index].Add(cells[selRow * 8 + selCol + 2]);
                    copyCells[selRow * 8 + selCol].SetPiece(Pieces.None);
                    selectedPiece.row = selRow;
                    selectedPiece.col = selCol + 2;
                    copyCells[selRow * 8 + selCol + 2].SetPiece(Pieces.P1);
                    CalculateValidMoves(copyCells, startRow, startCol, true, index);
                    int oldIndex = index;
                    index = moveableCellsList.Count;
                    moveableCellsList.Add(new List<CellProperties>());
                    // if a move has been found recursively we should get one move back and search if another move exists.
                    // so that copy the steps to the found move excluding the last move because we will find alternative of it if exists.
                    if (isJumped)
                    {
                        for (int i = 0; i < moveableCellsList[oldIndex].Count - 1; i++)
                            moveableCellsList[index].Add(moveableCellsList[oldIndex].ElementAt(i));
                    }
                }
            }
            // double jump right
            if (selCol > 1 && copyCells[selRow * 8 + selCol - 1].GetPiece() != Pieces.None && copyCells[selRow * 8 + selCol - 2].GetPiece() == Pieces.None)
            {
                // make sure cell is not visited previously
                if (FindInCells(moveableCellsList[index], selRow, selCol - 2) == null)
                {
                    moveableCellsList[index].Add(cells[selRow * 8 + selCol - 2]);
                    copyCells[selRow * 8 + selCol].SetPiece(Pieces.None);
                    selectedPiece.row = selRow;
                    selectedPiece.col = selCol - 2;
                    copyCells[selRow * 8 + selCol - 2].SetPiece(Pieces.P1);
                    CalculateValidMoves(copyCells, startRow, startCol, true, index);
                    int oldIndex = index;
                    index = moveableCellsList.Count;
                    moveableCellsList.Add(new List<CellProperties>());

                    if (isJumped)
                    {
                        for (int i = 0; i < moveableCellsList[oldIndex].Count - 1; i++)
                            moveableCellsList[index].Add(moveableCellsList[oldIndex].ElementAt(i));
                    }

                }

            }
            // double jump up
            if (selRow < 6 && copyCells[(selRow + 1) * 8 + selCol].GetPiece() != Pieces.None && copyCells[(selRow + 2) * 8 + selCol].GetPiece() == Pieces.None)
            {
                if (FindInCells(moveableCellsList[index], selRow + 2, selCol) == null)
                {
                    moveableCellsList[index].Add(cells[(selRow + 2) * 8 + selCol]);
                    copyCells[selRow * 8 + selCol].SetPiece(Pieces.None);
                    selectedPiece.row = selRow + 2;
                    selectedPiece.col = selCol;
                    copyCells[(selRow + 2) * 8 + selCol].SetPiece(Pieces.P1);
                    CalculateValidMoves(copyCells, startRow, startCol, true, index);
                    int oldIndex = index;
                    index = moveableCellsList.Count;
                    moveableCellsList.Add(new List<CellProperties>());

                    if (isJumped)
                    {
                        for (int i = 0; i < moveableCellsList[oldIndex].Count - 1; i++)
                            moveableCellsList[index].Add(moveableCellsList[oldIndex].ElementAt(i));
                    }
                }

            }
            // If the piece in win square, then it can go back as well
            if (selRow == 7 && selCol > 4 && selCol < 8 && copyCells[(selRow - 1) * 8 + selCol].GetPiece() != Pieces.None && copyCells[(selRow - 2) * 8 + selCol].GetPiece() == Pieces.None)
            {
                if (FindInCells(moveableCellsList[index], selRow + 2, selCol) == null)
                {
                    moveableCellsList[index].Add(cells[(selRow - 2) * 8 + selCol]);
                    copyCells[selRow * 8 + selCol].SetPiece(Pieces.None);
                    selectedPiece.row = selRow - 2;
                    selectedPiece.col = selCol;
                    copyCells[(selRow - 2) * 8 + selCol].SetPiece(Pieces.P1);
                    CalculateValidMoves(copyCells, startRow, startCol, true, index);
                    int oldIndex = index;
                    index = moveableCellsList.Count;
                    moveableCellsList.Add(new List<CellProperties>());

                    if (isJumped)
                    {
                        for (int i = 0; i < moveableCellsList[oldIndex].Count - 1; i++)
                            moveableCellsList[index].Add(moveableCellsList[oldIndex].ElementAt(i));
                    }

                }

            }
            // back on win square
            if (!isJumped && selRow > 5 && selRow < 8 && selCol > 4 && selCol < 8 && copyCells[(selRow - 1) * 8 + selCol].GetPiece() == Pieces.None)
            {
                moveableCellsList[index].Add(cells[(selRow - 1) * 8 + selCol]);
                index = moveableCellsList.Count;
                moveableCellsList.Add(new List<CellProperties>());

            }
            // left
            if (!isJumped && selCol < 7 && copyCells[selRow * 8 + selCol + 1].GetPiece() == Pieces.None)
            {
                moveableCellsList[index].Add(cells[selRow * 8 + selCol + 1]);
                index = moveableCellsList.Count;
                moveableCellsList.Add(new List<CellProperties>());
            }
            // right
            if (!isJumped && selCol > 0 && copyCells[selRow * 8 + selCol - 1].GetPiece() == Pieces.None)
            {
                moveableCellsList[index].Add(cells[selRow * 8 + selCol - 1]);
                index = moveableCellsList.Count;
                moveableCellsList.Add(new List<CellProperties>());

            }
            // up
            if (!isJumped && selRow < 7 && copyCells[(selRow + 1) * 8 + selCol].GetPiece() == Pieces.None)
            {
                moveableCellsList[index].Add(cells[(selRow + 1) * 8 + selCol]);
                index = moveableCellsList.Count;
                moveableCellsList.Add(new List<CellProperties>());
            }
        }
        else
        {
            // double jump left
            if (selCol > 1 && copyCells[selRow * 8 + selCol - 1].GetPiece() != Pieces.None && copyCells[selRow * 8 + selCol - 2].GetPiece() == Pieces.None)
            {
                if (FindInCells(moveableCellsList[index], selRow, selCol - 2) == null)
                {
                    moveableCellsList[index].Add(cells[selRow * 8 + selCol - 2]);
                    copyCells[selRow * 8 + selCol].SetPiece(Pieces.None);
                    selectedPiece.row = selRow;
                    selectedPiece.col = selCol - 2;
                    copyCells[selRow * 8 + selCol - 2].SetPiece(Pieces.P2);
                    CalculateValidMoves(copyCells, startRow, startCol, true, index);
                    int oldIndex = index;
                    index = moveableCellsList.Count;
                    moveableCellsList.Add(new List<CellProperties>());

                    if (isJumped)
                    {
                        for (int i = 0; i < moveableCellsList[oldIndex].Count - 1; i++)
                            moveableCellsList[index].Add(moveableCellsList[oldIndex].ElementAt(i));
                    }
                }
            }
            // double jump right
            if (selCol < 6 && copyCells[selRow * 8 + selCol + 1].GetPiece() != Pieces.None && copyCells[selRow * 8 + selCol + 2].GetPiece() == Pieces.None)
            {
                if (FindInCells(moveableCellsList[index], selRow, selCol + 2) == null)
                {
                    moveableCellsList[index].Add(cells[selRow * 8 + selCol + 2]);
                    copyCells[selRow * 8 + selCol].SetPiece(Pieces.None);
                    selectedPiece.row = selRow;
                    selectedPiece.col = selCol + 2;
                    copyCells[selRow * 8 + selCol + 2].SetPiece(Pieces.P2);
                    CalculateValidMoves(copyCells, startRow, startCol, true, index);
                    int oldIndex = index;
                    index = moveableCellsList.Count;
                    moveableCellsList.Add(new List<CellProperties>());

                    if (isJumped)
                    {
                        for (int i = 0; i < moveableCellsList[oldIndex].Count - 1; i++)
                            moveableCellsList[index].Add(moveableCellsList[oldIndex].ElementAt(i));
                    }
                }
            }
            // double jump up
            if (selRow > 1 && copyCells[(selRow - 1) * 8 + selCol].GetPiece() != Pieces.None && copyCells[(selRow - 2) * 8 + selCol].GetPiece() == Pieces.None)
            {
                if (FindInCells(moveableCellsList[index], selRow - 2, selCol) == null)
                {
                    moveableCellsList[index].Add(cells[(selRow - 2) * 8 + selCol]);
                    copyCells[selRow * 8 + selCol].SetPiece(Pieces.None);
                    selectedPiece.row = selRow - 2;
                    selectedPiece.col = selCol;
                    copyCells[(selRow - 2) * 8 + selCol].SetPiece(Pieces.P2);
                    CalculateValidMoves(copyCells, startRow, startCol, true, index);
                    int oldIndex = index;
                    index = moveableCellsList.Count;
                    moveableCellsList.Add(new List<CellProperties>());

                    if (isJumped)
                    {
                        for (int i = 0; i < moveableCellsList[oldIndex].Count - 1; i++)
                            moveableCellsList[index].Add(moveableCellsList[oldIndex].ElementAt(i));
                    }
                }
            }
            //  if the piece in win square
            if (selRow == 0 && selCol < 3 && copyCells[(selRow + 1) * 8 + selCol].GetPiece() != Pieces.None && copyCells[(selRow + 2) * 8 + selCol].GetPiece() == Pieces.None)
            {
                if (FindInCells(moveableCellsList[index], selRow + 2, selCol) == null)
                {
                    moveableCellsList[index].Add(cells[(selRow + 2) * 8 + selCol]);
                    copyCells[selRow * 8 + selCol].SetPiece(Pieces.None);
                    selectedPiece.row = selRow + 2;
                    selectedPiece.col = selCol;
                    copyCells[(selRow + 2) * 8 + selCol].SetPiece(Pieces.P2);
                    CalculateValidMoves(copyCells, startRow, startCol, true, index);
                    int oldIndex = index;
                    index = moveableCellsList.Count;
                    moveableCellsList.Add(new List<CellProperties>());

                    if (isJumped)
                    {
                        for (int i = 0; i < moveableCellsList[oldIndex].Count - 1; i++)
                            moveableCellsList[index].Add(moveableCellsList[oldIndex].ElementAt(i));
                    }
                }
            }
            // back on wib square
            if (!isJumped && selRow < 2 && selCol < 3 && copyCells[(selRow + 1) * 8 + selCol].GetPiece() == Pieces.None)
            {
                moveableCellsList[index].Add(cells[(selRow + 1) * 8 + selCol]);
                index = moveableCellsList.Count;
                moveableCellsList.Add(new List<CellProperties>());
            }

            // left
            if (!isJumped && selCol > 0 && copyCells[selRow * 8 + selCol - 1].GetPiece() == Pieces.None)
            {
                moveableCellsList[index].Add(cells[selRow * 8 + selCol - 1]);
                index = moveableCellsList.Count;
                moveableCellsList.Add(new List<CellProperties>());
            }
            // right
            if (!isJumped && selCol < 7 && copyCells[selRow * 8 + selCol + 1].GetPiece() == Pieces.None)
            {
                moveableCellsList[index].Add(cells[selRow * 8 + selCol + 1]);
                index = moveableCellsList.Count;
                moveableCellsList.Add(new List<CellProperties>());
            }
            // up
            if (!isJumped && selRow > 0 && copyCells[(selRow - 1) * 8 + selCol].GetPiece() == Pieces.None)
            {
                moveableCellsList[index].Add(cells[(selRow - 1) * 8 + selCol]);
                index = moveableCellsList.Count;
                moveableCellsList.Add(new List<CellProperties>());
            }
        }

    }
    private void FillMoveableCellList()
    {
        int selectedRow = selectedPiece.row;
        int selectedCol = selectedPiece.col;
        List<CellProperties> copyCells = new();
        for (int i = 0; i < cells.Count; i++)
        {
            CellProperties newCell = Instantiate(cells[i], new Vector3(0f, 0f, 0f), Quaternion.identity);
            copyCells.Add(newCell);
        }

        CalculateValidMoves(copyCells, selectedRow, selectedCol, false, 0);
        foreach (var cell in copyCells)
        {
            cell.KillSelf();
        }

        selectedPiece.row = selectedRow;
        selectedPiece.col = selectedCol;

        foreach (var cellList in moveableCellsList)
        {
            int removeIndex = -1;
            for (int i = 0; i < cellList.Count; i++)
            {
                if (cellList[i].GetRow() == selectedPiece.row && cellList[i].GetCol() == selectedPiece.col)
                {
                    removeIndex = i;
                    continue;
                }
            }
            if (removeIndex != -1)
            {
                cellList.RemoveAt(removeIndex);
            }
        }
    }
    private void HighlightValidMoves()
    {

        foreach (var cellList in moveableCellsList)
        {
            // when the piece is moved, it shouldn't be able to move back to the start position
            int removeIndex = -1;
            for (int i = 0; i < cellList.Count; i++)
            {
                if (cellList[i].GetRow() == selectedPiece.row && cellList[i].GetCol() == selectedPiece.col)
                {
                    removeIndex = i;
                    continue;
                }
                cellList[i].HighlightCell();
            }
            if (removeIndex != -1)
            {
                cellList.RemoveAt(removeIndex);
            }
        }


        if (moveableCellsList.Count == 0)
        {
            firstInputTaken = false;
        }

    }
    private void RectifyHighlightedCells()
    {
        foreach (var cellList in moveableCellsList)
        {
            foreach (var cell in cellList)
            {
                cell.Rectify();
            }
        }

    }
    public void ResetMoveableCells()
    {
        PieceProperties.ResetClicked();
        CellProperties.ResetClicked();
        moveableCellsList = new List<List<CellProperties>>();
    }
    public CellProperties FindInCells(List<CellProperties> cells, int row, int col)
    {
        foreach (var cell in cells)
        {
            if (cell.GetRow() == row && cell.GetCol() == col)
            {
                return cell;
            }
        }
        return null;

    }
    public Players CheckWinner()
    {

        if ((cells[7 * 8 + 7].GetPiece() == Pieces.P1) &&
             (cells[7 * 8 + 6].GetPiece() == Pieces.P1) &&
             (cells[7 * 8 + 5].GetPiece() == Pieces.P1) &&
             (cells[6 * 8 + 7].GetPiece() == Pieces.P1) &&
             (cells[6 * 8 + 6].GetPiece() == Pieces.P1) &&
             (cells[6 * 8 + 5].GetPiece() == Pieces.P1) &&
             (cells[5 * 8 + 7].GetPiece() == Pieces.P1) &&
             (cells[5 * 8 + 6].GetPiece() == Pieces.P1) &&
             (cells[5 * 8 + 5].GetPiece() == Pieces.P1)
            )
        {
            return Players.Player1;
        }

        else if ((cells[0 * 8 + 0].GetPiece() == Pieces.P2) &&
             (cells[0 * 8 + 1].GetPiece() == Pieces.P2) &&
             (cells[0 * 8 + 2].GetPiece() == Pieces.P2) &&
             (cells[1 * 8 + 0].GetPiece() == Pieces.P2) &&
             (cells[1 * 8 + 1].GetPiece() == Pieces.P2) &&
             (cells[1 * 8 + 2].GetPiece() == Pieces.P2) &&
             (cells[2 * 8 + 0].GetPiece() == Pieces.P2) &&
             (cells[2 * 8 + 1].GetPiece() == Pieces.P2) &&
             (cells[2 * 8 + 2].GetPiece() == Pieces.P2)
            )
        {
            return Players.Player2;
        }

        return Players.None;
    }
    private IEnumerator MovePieceBot1()
    {
        List<CellProperties> cellList = new();
        List<int> winPieceIndex = new();
        int pieceIndex = -1;
        int maxMoveIndex = -1;
        int maxMoveCount = 0;

        // check if a piece that is not in one of the win cells can move
        for (int i = 0; i < 9; i++)
        {

            if (winCellsp1.Contains(new Vector2(pieces[i].row, pieces[i].col)))
            {
                winPieceIndex.Add(i);
                continue;
            }
            selectedPiece = pieces[i];
            RectifyHighlightedCells();
            ResetMoveableCells();
            FillMoveableCellList();
            // find the move that covers the most distance
            for (int j = 0; j < moveableCellsList.Count; j++)
            {
                if (moveableCellsList[j].Count > maxMoveCount &&
                   (moveableCellsList[j][^1].GetRow() >= selectedPiece.row && moveableCellsList[j][^1].GetCol() >= selectedPiece.col))
                {
                    pieceIndex = i;
                    maxMoveIndex = j;
                    maxMoveCount = moveableCellsList[j].Count;
                }
            }

        }
        //***

        if (maxMoveIndex == -1)
        {
            bool exit = false;
            foreach (var index in winPieceIndex)
            {
                selectedPiece = pieces[index];
                RectifyHighlightedCells();
                ResetMoveableCells();
                FillMoveableCellList();
                var copyMoveableCellsList = moveableCellsList;
                for (int i = 0; i < copyMoveableCellsList.Count; i++)
                {
                    if (copyMoveableCellsList[i].Count == 0)
                        continue;
                    copyMoveableCellsList[i][^1].SetPiece(Pieces.P1);
                    cells[selectedPiece.row * 8 + selectedPiece.col].SetPiece(Pieces.None);
                    if (CheckOuterPieceMove())
                    {
                        copyMoveableCellsList[i][^1].SetPiece(Pieces.None);
                        cells[selectedPiece.row * 8 + selectedPiece.col].SetPiece(Pieces.P1);
                        maxMoveIndex = i;
                        pieceIndex = index;
                        exit = true;
                        break;
                    }
                    else
                    {
                        copyMoveableCellsList[i][^1].SetPiece(Pieces.None);
                        cells[selectedPiece.row * 8 + selectedPiece.col].SetPiece(Pieces.P1);
                    }
                }
                if (exit) { break; }

            }
        }




        //***
        int count = 0;

        while (maxMoveIndex == -1)
        {
            count++;
            // check if the pieces that are already placed in win cells can be moved 
            do
            {
                pieceIndex = Random.Range(0, 9);
                selectedPiece = pieces[pieceIndex];
                RectifyHighlightedCells();
                ResetMoveableCells();
                FillMoveableCellList();
            } while (!winCellsp1.Contains(new Vector2(selectedPiece.row, selectedPiece.col)) || moveableCellsList.Count == 0);


            // find the move that covers the most distance
            for (int j = 0; j < moveableCellsList.Count; j++)
            {
                if (moveableCellsList[j].Count > maxMoveCount)
                {
                    maxMoveIndex = j;
                    maxMoveCount = moveableCellsList[j].Count;
                }

            }
            if (count == 1000)
            {
                Debug.Log("Bot1 couldnt find valid move");
                yield break;
            }
        }


        selectedPiece = pieces[pieceIndex];
        ResetMoveableCells();
        FillMoveableCellList();
        //cellList = moveableCellsList[maxMoveIndex];
        cellList = new List<CellProperties>();

        for (int i = 0; i < moveableCellsList[maxMoveIndex].Count; i++)
        {
            if (i == 0)
            {
                cellList.Add(moveableCellsList[maxMoveIndex][i]);
            }
            else if (!winCellsp1.Contains(new Vector2(selectedPiece.row, selectedPiece.col)) &&
                (moveableCellsList[maxMoveIndex][i].GetRow() >= moveableCellsList[maxMoveIndex][i - 1].GetRow() &&
                 moveableCellsList[maxMoveIndex][i].GetCol() >= moveableCellsList[maxMoveIndex][i - 1].GetCol())
                )
            {
                cellList.Add(moveableCellsList[maxMoveIndex][i]);
            }
        }





        //////

        CellProperties oldCell = FindInCells(cells, selectedPiece.row, selectedPiece.col);
        oldCell.SetPiece(Pieces.None);

        selectedPiece.col = cellList[^1].GetCol();
        selectedPiece.row = cellList[^1].GetRow();
        RectifyHighlightedCells();

        cellList[^1].SetPiece(Pieces.P1);

        yield return new WaitForSeconds(botWaitTime); // to make player see the move

        PieceProperties selPiece = selectedPiece;
        selectedPiece.image.color = new Color(selectedPiece.image.color.r, selectedPiece.image.color.g, selectedPiece.image.color.b, 1);
        selectedPiece = null;
        foreach (var cell in cellList)
        {
            audioManager.PlayMoveSound();
            selPiece.transform.position = cell.transform.position;
            yield return new WaitForSeconds(pieceMoveSpeed);
        }
        Players nextTurn = Players.Player2;
        winner = CheckWinner();
        if (winner != Players.None)
        {
            menuBtn.SetActive(true);
            nextTurn = Players.None;
            if (winner == Players.Player1)
            {
                audioManager.PlayLoseSound();
                loserText.gameObject.SetActive(true);
            }
            else
            {
                audioManager.PlayWinSound();
                winnerText.gameObject.SetActive(true);
            }

        }
        ResetMoveableCells();
        turn = nextTurn;

    }
    private IEnumerator MovePieceBot2()
    {
        List<Vector2> winCells = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(0, 2),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(1, 2),
            new Vector2(2, 0),
            new Vector2(2, 1),
            new Vector2(2, 2)
        };
        List<CellProperties> cellList = new();
        int pieceIndex = -1;
        int maxMoveIndex = -1;
        int maxMoveCount = 0;

        // check if a piece that is not in one of the win cells can move
        for (int i = 9; i < 18; i++)
        {

            if (winCells.Contains(new Vector2(pieces[i].row, pieces[i].col)))
            {
                continue;
            }
            selectedPiece = pieces[i];
            RectifyHighlightedCells();
            ResetMoveableCells();
            FillMoveableCellList();
            // find the move that covers the most distance
            for (int j = 0; j < moveableCellsList.Count; j++)
            {
                if (moveableCellsList[j].Count > maxMoveCount &&
                   (moveableCellsList[j][^1].GetRow() <= selectedPiece.row && moveableCellsList[j][^1].GetCol() <= selectedPiece.col))
                {
                    pieceIndex = i;
                    maxMoveIndex = j;
                    maxMoveCount = moveableCellsList[j].Count;
                }
            }

        }
        int count = 0;
        while (maxMoveIndex == -1)
        {
            count++;
            // check if the pieces that are already placed in win cells can be moved 
            do
            {
                pieceIndex = Random.Range(9, 18);
                selectedPiece = pieces[pieceIndex];
                RectifyHighlightedCells();
                ResetMoveableCells();
                FillMoveableCellList();
            } while (!winCells.Contains(new Vector2(selectedPiece.row, selectedPiece.col)) || moveableCellsList.Count == 0);


            // find the move that covers the most distance
            for (int j = 0; j < moveableCellsList.Count; j++)
            {
                if (moveableCellsList[j].Count > maxMoveCount)
                {
                    maxMoveIndex = j;
                    maxMoveCount = moveableCellsList[j].Count;
                }

            }
            if (count == 1000)
            {
                Debug.Log("Bot2 couldnt find valid move");
                yield break;
            }
        }


        selectedPiece = pieces[pieceIndex];
        ResetMoveableCells();
        FillMoveableCellList();
        cellList = moveableCellsList[maxMoveIndex];




        //////

        CellProperties oldCell = FindInCells(cells, selectedPiece.row, selectedPiece.col);
        oldCell.SetPiece(Pieces.None);

        selectedPiece.col = cellList[cellList.Count - 1].GetCol();
        selectedPiece.row = cellList[cellList.Count - 1].GetRow();
        RectifyHighlightedCells();

        cellList[cellList.Count - 1].SetPiece(Pieces.P2);
        PieceProperties selPiece = selectedPiece;
        selectedPiece.image.color = new Color(selectedPiece.image.color.r, selectedPiece.image.color.g, selectedPiece.image.color.b, 1);
        selectedPiece = null;
        foreach (var cell in cellList)
        {
            selPiece.transform.position = cell.transform.position;
            yield return new WaitForSeconds(pieceMoveSpeed);
        }
        Players nextTurn = Players.Player1;

        winner = CheckWinner();
        if (winner != Players.None)
        {
            menuBtn.SetActive(true);
            nextTurn = Players.None;
            if (winner == Players.Player1)
            {
                audioManager.PlayLoseSound();
                loserText.gameObject.SetActive(true);
            }
            else
            {
                audioManager.PlayWinSound();
                winnerText.gameObject.SetActive(true);
            }

        }
        ResetMoveableCells();
        turn = nextTurn;

    }
    public void HighlightSelectedPiece()
    {
        if (selectedPiece != null)
        {
            if (selectedPiece.image.color.a < 0.3f && pieceHighlight < 0)
            {
                pieceHighlight *= -1;

            }
            else if (selectedPiece.image.color.a > 0.98f && pieceHighlight > 0)
            {
                pieceHighlight *= -1;

            }

            selectedPiece.image.color = new Color(selectedPiece.image.color.r, selectedPiece.image.color.g, selectedPiece.image.color.b, selectedPiece.image.color.a + pieceHighlight * Time.deltaTime);

        }
    }
    bool CheckOuterPieceMove()
    {
        PieceProperties selPiece = selectedPiece;
        for (int i = 0; i < 9; i++)
        {

            if (winCellsp1.Contains(new Vector2(pieces[i].row, pieces[i].col)))
            {
                continue;
            }
            selectedPiece = pieces[i];
            RectifyHighlightedCells();
            ResetMoveableCells();
            FillMoveableCellList();
            // find the move that covers the most distance
            for (int j = 0; j < moveableCellsList.Count; j++)
            {
                if (moveableCellsList[j].Count > 0 &&
                   (moveableCellsList[j][^1].GetRow() >= selPiece.row && moveableCellsList[j][^1].GetCol() >= selPiece.col))
                {
                    selectedPiece = selPiece;
                    return true;
                }
            }

        }
        selectedPiece = selPiece;
        return false;
    }
}

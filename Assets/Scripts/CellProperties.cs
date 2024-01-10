using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum Pieces
{
    P1,
    P2,
    None
}
public class CellProperties : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private int row;
    [SerializeField] private int col;
    [SerializeField] private Pieces piece;
    [SerializeField] private int colorNum;
    [SerializeField] static private CellProperties clickedCell;
    private Color color;
    private Image image;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
       
        col = gameObject.name[0] - 'a';
        row = gameObject.name[1] - '1';
        if (colorNum == 0)
        {
            color = new Color32(103,71,51,255);
        }
        else
        {
            color = new Color32(246, 248, 176, 255);
        }
        image.color = color;
    }



    public int GetRow() { return row; }
    public int GetCol() { return col; }
    public void SetRow(int row) { this.row = row;}
    public void SetCol(int col) {  this.col = col;}

    public Pieces GetPiece() { return piece; }
    public void HighlightCell()
    {
        image.color = Color.yellow;
    }
    public void Rectify()
    {
        image.color = color;
    }
    public void SetPiece(Pieces newPiece)
    {
        piece = newPiece;
    }
    public void KillSelf()
    {
        Destroy(gameObject);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        clickedCell = this;
        Debug.Log("celll clicked " + this.gameObject.name);
        PieceProperties.ResetClicked();
    }
    public static void ResetClicked()
    {
        clickedCell = null;
    }
    public static CellProperties GetClickedCell()
    {
        return clickedCell;
    }
}

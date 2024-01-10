using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PieceProperties : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] public int row;
    [SerializeField] public int col;
    [SerializeField] public Players owner;
    [SerializeField] static private PieceProperties clickedPiece;
    [SerializeField] public RawImage image;

    // Start is called before the first frame update
    private void Start()
    {
        image = GetComponent<RawImage>();
    }
    public Players GetOwner() { return owner; }
    public void OnPointerClick(PointerEventData eventData)
    {
        clickedPiece = this;
        
        Debug.Log("piece clicked " +gameObject.name);
        CellProperties.ResetClicked();
    }
    public static void ResetClicked()
    {
        clickedPiece = null;
    }
    public static PieceProperties GetClickedPiece()
    {
        return clickedPiece;
    }
}

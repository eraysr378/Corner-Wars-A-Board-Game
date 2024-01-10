using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TutorialWinAnim : MonoBehaviour
{
    [SerializeField] private List<CellProperties> cells;
    [SerializeField] private List<PieceProperties> pieces;
    [SerializeField] private float moveSpeed;
    float time = 0;
    int index = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;


        if (time > 1 && index < pieces.Count)
        {
            if (cells[index].transform.position != pieces[index].transform.position)
            {
                pieces[index].transform.position = Vector3.MoveTowards(pieces[index].transform.position, cells[index].transform.position, moveSpeed * Time.deltaTime);
            }
            if (Vector3.Distance(cells[index].transform.position, pieces[index].transform.position) < 0.001f)
            {
                // Swap the position of the cylinder.
                index++;
            }
        }


        if (time > 12)
        {
            time = 0;
            index = 0;
            for (int i = 0; i < pieces.Count; i++)
            {
                pieces[i].transform.position = cells[i + 9].transform.position;
            }
        }

    }
}

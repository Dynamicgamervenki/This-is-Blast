using System.Linq;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public Vector2Int posIndex;
    [HideInInspector]
    public Board board;
    public enum GemType {blue ,green,red,yellow,purple}
    public GemType type;

    private void Update()
    {
        if (Vector2.Distance(transform.position, posIndex) > 0.1f &&  gameObject.layer != 6)
        {
            transform.position = Vector2.Lerp(transform.position,posIndex,board.gemSpeed * Time.deltaTime);
        }
    }
    
    public void SetupGem(Vector2Int pos, Board theboard)
    {
        posIndex = pos;
        board = theboard;
    }
    
    

}

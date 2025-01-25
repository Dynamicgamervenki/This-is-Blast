using System.Linq;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int posIndex;
    [HideInInspector]
    public Board board;

    public InteractBoard interactBoard;
    

    public bool mousePressed;
    
    public enum GemType {blue ,green,red,yellow,purple}
    public GemType type;
    
    public void SetupGem(Vector2Int pos, Board theboard)
    {
        posIndex = pos;
        board = theboard;
    }
    
    public void SetupGem(Vector2Int pos, InteractBoard theboard)
    {
        posIndex = pos;
        interactBoard = theboard;
    }

    
    private void OnMouseDown()
    {
        if (gameObject.layer == 6)
        {
            mousePressed = true;
        }
    }
}

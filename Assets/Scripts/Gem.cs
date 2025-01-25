using UnityEngine;

public class Gem : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int posIndex;
    [HideInInspector]
    public Board board;
    

    public void SetupGem(Vector2Int pos, Board theboard)
    {
        posIndex = pos;
        board = theboard;
    }
}

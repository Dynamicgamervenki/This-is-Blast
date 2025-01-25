using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    
    public GameObject bgTilePrefab;
    
    void Start()
    {
        SetUp();
    }
    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 position = new Vector2(i, j);
                GameObject bgTile = Instantiate(bgTilePrefab,position,Quaternion.identity);
                bgTile.transform.SetParent(transform);
                bgTile.name = "BgTile_" + i + "," + j;
            }
        }
    }

}

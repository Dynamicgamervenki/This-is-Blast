using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GemPattern
{
    SplitHalf,   // Left half one color, right half another color
    StarShape,   // Star shape pattern (you can customize this)
    StripedRows   
}


public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GemPattern gemPattern;
    
    public float gemSpeed = 5.0f;
    public GameObject bgTilePrefab;
    public Gem[] gems;
    
    
    public Gem[,] allGems;
    
    public List<Gem> bottomGems = new List<Gem>();
    public List<Gem> DesroyeedGems = new List<Gem>();
    void Start()
    {
        allGems = new Gem[width, height];
        SetUp();
    }

    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 position = new Vector2(i, j);
                GameObject bgTile = Instantiate(bgTilePrefab, position, Quaternion.identity);
                bgTile.transform.SetParent(transform);
                bgTile.name = "BgTile_" + i + "," + j;

                // Get gem index based on the selected pattern
                int gemToUse = GetGemBasedOnPattern(i, j);
                SpawnGem(new Vector2Int(i, j), gems[gemToUse]);
            }
        }

        UpdateBottomGemsList();
    }

    private int GetGemBasedOnPattern(int i, int j)
    {
        switch (gemPattern)
        {
            case GemPattern.SplitHalf:
                // Left half one color, right half another color
                if (i < width / 2)
                    return Random.Range(0, gems.Length / 2); // First half
                else
                    return Random.Range(gems.Length / 2, gems.Length); // Second half

            case GemPattern.StarShape:
                // Star shape pattern (e.g. gems near the center form a star)
                if (Mathf.Abs(i - width / 2) + Mathf.Abs(j - height / 2) <= 3)  // Simple star-like logic
                    return Random.Range(0, gems.Length);  // Central area gets random gems
                else
                    return Random.Range(gems.Length / 2, gems.Length);  // Outer area gets different gems
            
            case GemPattern.StripedRows:
                // Alternate colors based on row index, considering width
                int rowBlock = i / 2; // Divide the width into blocks of two columns

                // Alternate color sets for each row (alternating between two colors)
                if (rowBlock % 2 == 0)
                    return Random.Range(0, gems.Length / 2);  // First color set (left half)
                else
                    return Random.Range(gems.Length / 2, gems.Length);  // Second color set (right half)


            default:
                return Random.Range(0, gems.Length); // Default to random if no pattern is selected
        }
    }
    
    private void SpawnGem(Vector2Int pos,Gem gemToSpawn)
    {
        Gem gem =  Instantiate(gemToSpawn, new Vector3(pos.x,pos.y,0f), Quaternion.identity);
        gem.transform.SetParent(transform);
        gem.name = "Gem - " + pos.x + "," + pos.y + "," + gemToSpawn;
        allGems[pos.x, pos.y] = gem;

        gem.SetupGem(pos, this);
    }
    
    
    public IEnumerator DecreaseRowCo()
    {
        yield return new WaitForSecondsRealtime(.2f);
        Debug.Log("Entering Coroutine");
        int nullCounter = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x,y] == null)
                {
                    nullCounter++;
                }
                else if(nullCounter > 0)
                {
                    allGems[x,y].posIndex.y -= nullCounter;
                    allGems[x,y - nullCounter] = allGems[x, y];
                    allGems[x, y] = null;
                }

                if (y == 0)
                {
                    bottomGems.Add(allGems[x, y]);
                    bottomGems = bottomGems.Distinct().ToList();
                }
            }
            
            nullCounter = 0;
        }
    }
    
    
    public void UpdateBottomGemsList()
    {
        //bottomGems.Clear();
        // Add gems from the bottom row (y = 0)
        for (int x = 0; x < width; x++)
        {
                bottomGems.Add(allGems[x, 0]);
        }
        bottomGems = bottomGems.Distinct().ToList();
    }

}

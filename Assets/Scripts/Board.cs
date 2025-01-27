using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GemPattern
{
    SplitHalf,   // Left half one color, right half another color
    StripedRows ,
    DiagonalStripes,
    TwoGemCheckerboard,
    ThreeGemTriangles,
    FourGemQuadrants,
    FiveGemSpiral
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
       // Camera.main.transform.position = new Vector3(width / 2f, height / 2f, Camera.main.transform.position.z);
        
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
            
            case GemPattern.StripedRows :
               //logic for stripe rows
               if (j % 2 == 0) // Even rows
                   return Random.Range(0, gems.Length / 2); 
               else // Odd rows
                   return Random.Range(gems.Length / 2, gems.Length); 
            case GemPattern.DiagonalStripes:
                // Diagonal Stripes
                if ((i + j) % 2 == 0) // Even diagonals
                    return Random.Range(0, gems.Length / 2);
                else // Odd diagonals
                    return Random.Range(gems.Length / 2, gems.Length); 
            case GemPattern.FourGemQuadrants:
                // Divide the board into four quadrants
                if (i < width / 2 && j < height / 2) // Top-Left quadrant
                    return 0; 
                else if (i >= width / 2 && j < height / 2) // Top-Right quadrant
                    return 1;
                else if (i < width / 2 && j >= height / 2) // Bottom-Left quadrant
                    return 2;
                else // Bottom-Right quadrant
                    return 3;
            case GemPattern.ThreeGemTriangles:
                // Divide the board into three triangles
                if (j >= i) // Top-Left and Middle Triangle
                    return 0;
                else if (j < i && j >= -i + width) // Top-Right and Middle Triangle
                    return 1;
                else // Bottom Triangle
                    return 2;
            case GemPattern.TwoGemCheckerboard:
                // Simple Checkerboard with two gems
                if ((i + j) % 2 == 0) 
                    return 0; // First gem type
                else
                    return 1; // Second gem type
            case GemPattern.FiveGemSpiral:
                // Create a spiral pattern with five gem types
                int distanceFromCenter = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow(i - width / 2, 2) + Mathf.Pow(j - height / 2, 2)));
                return distanceFromCenter % 5; // Cycle through five gem types

            
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

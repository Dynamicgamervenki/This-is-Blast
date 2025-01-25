using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    public float gemSpeed = 5.0f;
    public GameObject bgTilePrefab;
    public Gem[] gems;
    
    public Gem[,] allGems;
    
    public List<Gem> bottomGems = new List<Gem>();
    void Start()
    {
        allGems = new Gem[width, height];
        SetUp();
    }

    private void Update()
    {
        UpdateBottomGems();
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
                
                int gemToUse;               //temporory spawn logic for basic testing
                if (i < width / 2) // Left half of the board
                {
                    gemToUse = Random.Range(0, gems.Length / 2);
                }
                else // Right half of the board
                {
                    gemToUse = Random.Range(gems.Length / 2, gems.Length);
                }
            
                SpawnGem(new Vector2Int(i, j), gems[gemToUse]);
            }
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

    private void UpdateBottomGems()
    {
      //  bottomGems.Clear();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (j == 0)
                {
                    bottomGems.Add(allGems[i,j]);
                }
            }
        }

        if(bottomGems.Count > 0)
        bottomGems = bottomGems.Distinct().ToList();
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
            }
            
            nullCounter = 0;
        }
        UpdateBottomGemsList();

        Debug.Log("Bottom list updated!");
    }
    
    
    private void UpdateBottomGemsList()
    {
        bottomGems.Clear(); 

        // Add gems from the bottom row (y = 0)
        for (int x = 0; x < width; x++)
        {
            if (allGems[x, 0] != null) // Ensure there's a gem in the bottom row
            {
                bottomGems.Add(allGems[x, 0]);
            }
        }
    }

}

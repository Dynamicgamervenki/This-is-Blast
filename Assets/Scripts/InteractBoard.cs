using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteractBoard : MonoBehaviour
{
    public int width;
    public int height;
    
    public GameObject bgTilePrefab;
    public Board board;
    public Gem[] gems;
    public List<Gem> interactGems = new List<Gem>();
    private ToMoveBoard moveBoard;
    private void Awake()
    {
        board = FindObjectOfType<Board>();
        moveBoard = FindObjectOfType<ToMoveBoard>();
    }

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
                Vector3 position = transform.position + new Vector3(i, j,0);
                GameObject bgTile = Instantiate(bgTilePrefab,position,Quaternion.identity);
                bgTile.transform.SetParent(transform);
                bgTile.name = "BgTile - " + i + "," + j;
                
                int gemToUse;               //temporory spawn logic for basic testing
                if (i < width / 2) 
                {
                    gemToUse = Random.Range(0, gems.Length / 2);
                }
                else
                {
                    gemToUse = Random.Range(gems.Length / 2, gems.Length);
                }
                Gem gem = Instantiate(gems[gemToUse],position,Quaternion.identity);
                gem.transform.SetParent(transform);
                gem.name = "Gem - " + i + "," + j;
                gem.gameObject.layer = 6;
                interactGems.Add(gem);
                gem.shootId = 10;
            }
        }
    }

    void Update()
    {
        Check();
    }

    private void Check()
    {
        foreach (Gem iGem in interactGems)
        {
            if (iGem.mousePressed)
            {
                foreach (Gem bGem in board.bottomGems)
                {
                    if (iGem.type == bGem.type && bGem != null)
                    {
                        StartCoroutine(MoveToBoard(iGem, bGem));
                        //break;
                    }
                }
                StartCoroutine(board.DecreaseRowCo());
            }
        }
    }

    private IEnumerator MoveToBoard(Gem iGem, Gem bGem)
    {
        // Wait for 0.1 seconds before starting movement
        yield return new WaitForSeconds(0.1f);

        // Move gem to the board
        float elapsedTime = 0f;
        Vector3 startingPos = iGem.transform.position;
        Vector3 targetPos = moveBoard.bgTilesTransform[0].position;
            

        // Move the gem using Lerp
        while (elapsedTime < 1f)
        {
            iGem.transform.position = Vector2.Lerp(startingPos, targetPos, elapsedTime);
            elapsedTime += Time.deltaTime * board.gemSpeed;
            yield return null; // Wait until the next frame
        }

        // Ensure the gem is exactly at the target position
        iGem.transform.position = targetPos;

        // Destroy the bottom gem and increment the counter for destroyed gems
        Destroy(bGem.gameObject);
        iGem.bGemDestoryed++;

        // Check if enough gems are destroyed, then proceed to the next step
        if (iGem.bGemDestoryed >= iGem.shootId)
        {
            iGem.mousePressed = false;
            iGem.bGemDestoryed = 0;
            Destroy(iGem.gameObject); // Destroy the interacting gem
            interactGems.Remove(iGem); // Remove it from the list
              
        }
        
    }

}

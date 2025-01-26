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
    public InteractGems[] gems;
    public List<InteractGems> interactGems = new List<InteractGems>();
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
                InteractGems gem = Instantiate(gems[gemToUse],position,Quaternion.identity);
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
        foreach (InteractGems iGem in interactGems)
        {
            if (iGem.mousePressed)
            {
                foreach (Gem bGem in board.bottomGems)
                {
                    if ((int)iGem.type == (int)bGem.type && bGem != null)
                    {
                        StartCoroutine(MoveToBoard(iGem, bGem));
                        break;
                    }
                }
                
                StartCoroutine(board.DecreaseRowCo());
            }
        }
    }

    private IEnumerator MoveToBoard(InteractGems iGem, Gem bGem)
    {
        // Wait for 0.1 seconds before starting movement
        yield return new WaitForSeconds(0.1f);

        // Move gem to the board
        float elapsedTime = 0f;
        Vector3 startingPos = iGem.transform.position;
        Vector3 targetPos = moveBoard.bgTilesTransform[0].position;
        
        iGem.transform.position = targetPos;
        Destroy(bGem.gameObject);
        board.bottomGems.Remove(bGem);
        Debug.Log("Destroyed " + bGem.name + "current count :  " + iGem.bGemDestoryed );
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

using System;
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
    private void Awake()
    {
        board = FindObjectOfType<Board>();
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
                    if (iGem.type == bGem.type &&  bGem != null)
                    {
                            Destroy(bGem.gameObject);
                            iGem.bGemDestoryed++;
                            

                            if (iGem.bGemDestoryed >= iGem.shootId)
                            {
                                iGem.mousePressed = false;
                                iGem.bGemDestoryed = 0;
                                Destroy(iGem.gameObject);
                                interactGems.Remove(iGem);
                                break; 
                            }
                    }
                }
                StartCoroutine(board.DecreaseRowCo());
            }
        }
    }
}

using System;
using System.Collections.Generic;
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
                
                int gemToUse = Random.Range(0, gems.Length);
                Gem gem = Instantiate(gems[gemToUse],position,Quaternion.identity);
                gem.transform.SetParent(transform);
                gem.name = "Gem - " + i + "," + j;
                gem.gameObject.layer = 6;
                interactGems.Add(gem);
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
                        iGem.mousePressed = false;
                        Destroy(bGem.gameObject);
                    }
                }

                StartCoroutine(board.DecreaseRowCo());
            }
        }
    }
}

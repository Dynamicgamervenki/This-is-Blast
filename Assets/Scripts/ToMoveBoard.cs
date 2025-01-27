using System;
using System.Collections.Generic;
using UnityEngine;

public class ToMoveBoard : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject bgTilePrefab;
    public GameObject[] bgTiles;
    public List<Transform> bgTilesTransform;

    private void Start()
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
                bgTilesTransform.Add(bgTile.transform);
            }
        }
    }
    
    
    
}

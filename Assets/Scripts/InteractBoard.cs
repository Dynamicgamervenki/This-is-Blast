using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class InteractBoard : MonoBehaviour
{
    public int width;
    public int height;

    public GameObject bgTilePrefab;
    public GameObject BulletPrefab;
    private Board board;
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
                Vector3 position = transform.position + new Vector3(i, j, 0);
                GameObject bgTile = Instantiate(bgTilePrefab, position, Quaternion.identity);
                bgTile.transform.SetParent(transform);
                bgTile.name = "BgTile - " + i + "," + j;

                int gemToUse; // Temporary spawn logic for basic testing
                if (i < width / 2)
                {
                    gemToUse = Random.Range(0, gems.Length / 2);
                }
                else
                {
                    gemToUse = Random.Range(gems.Length / 2, gems.Length);
                }

                InteractGems gem = Instantiate(gems[i], position, Quaternion.identity);
                gem.transform.SetParent(transform);
                gem.name = "Gem - " + i + "," + j;
                gem.gameObject.layer = 6;
                gem.pos = moveBoard.bgTilesTransform[i];
                gem.isMoving = false; // Initialize isMoving for each gem
                interactGems.Add(gem);
            }
        }
    }

    private void Update()
    {
        Check();

        if (board.bottomGems.Count == 0)
        {
            Debug.Log("Level Complete");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void Check()
    {
        foreach (InteractGems iGem in interactGems)
        {
            // Check if the gem is pressed, not moving, and has not reached its shoot limit
            if (iGem.mousePressed && !iGem.isMoving && iGem.bGemDestoryed < iGem.shootId)
            {
                foreach (Gem bGem in board.bottomGems)
                {
                    if ((int)iGem.type == (int)bGem.type && bGem != null)
                    {
                        iGem.isMoving = true; // Mark this gem as moving
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
        // Ensure the gem moves to the target position
        Vector3 targetPos = iGem.pos.position;
        iGem.transform.position = targetPos;

        // Instantiate a bullet at the gem's position
        GameObject bullet = Instantiate(BulletPrefab, iGem.pos.position, Quaternion.identity);

        // Move the bullet towards the target gem
        while (bGem != null && Vector3.Distance(bullet.transform.position, bGem.transform.position) > 0.1f)
        {
            // Smoothly move the bullet towards the gem
            bullet.transform.position = Vector3.MoveTowards(
                bullet.transform.position,
                bGem.transform.position,
                iGem.shootSpeed * Time.deltaTime
            );

            yield return null; // Wait for the next frame
        }

        // Align the bullet with the target gem's position, if it still exists
        if (bGem != null)
        {
            bullet.transform.position = bGem.transform.position;

            // Wait briefly before destroying the gem and bullet
            yield return new WaitForSeconds(0.1f);
            Destroy(bGem.gameObject); // Destroy the target gem
        }

        // Destroy the bullet after the gem is destroyed
        Destroy(bullet);

        // Update the gem's destruction count
        iGem.bGemDestoryed++;
        Debug.Log("Destroyed " + bGem?.name + " | Current Count: " + iGem.bGemDestoryed);

        // Check if the gem has reached its shooting limit
        if (iGem.bGemDestoryed >= iGem.shootId)
        {
            Debug.Log(iGem.name + " has reached its shooting limit.");

            // Deactivate or destroy the gem
            interactGems.Remove(iGem); // Remove it from the list
            Destroy(iGem.gameObject); // Destroy the gem
        }
        else
        {
            // Reset the isMoving flag if the gem is still active
            iGem.isMoving = false;
        }
    }
}

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
    
    public GemData gemData; 

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

    List<InteractGems> availableGems = new List<InteractGems>(gems);
    List<Transform> availablePositions = new List<Transform>(moveBoard.bgTilesTransform);

    for (int i = 0; i < width; i++)
    {
        for (int j = 0; j < height; j++)
        {
            Vector3 position = transform.position + new Vector3(i, j, 0);
            GameObject bgTile = Instantiate(bgTilePrefab, position, Quaternion.identity);
            bgTile.transform.SetParent(transform);
            bgTile.name = "BgTile - " + i + "," + j;

 
            InteractGems selectedGem;
            if (availableGems.Count > 0)
            {
                int randomIndex = Random.Range(0, availableGems.Count);
                selectedGem = availableGems[randomIndex];
                availableGems.RemoveAt(randomIndex); 
            }
            else
            {

                availableGems = new List<InteractGems>(gems);
                int randomIndex = Random.Range(0, availableGems.Count);
                selectedGem = availableGems[randomIndex];
                availableGems.RemoveAt(randomIndex);
            }


            Transform selectedPos;
            if (availablePositions.Count > 0)
            {
                int randomIndex = Random.Range(0, availablePositions.Count);
                selectedPos = availablePositions[randomIndex];
                availablePositions.RemoveAt(randomIndex); 
            }
            else
            {
                availablePositions = new List<Transform>(moveBoard.bgTilesTransform);
                int randomIndex = Random.Range(0, availablePositions.Count);
                selectedPos = availablePositions[randomIndex];
                availablePositions.RemoveAt(randomIndex);
            }


            InteractGems gem = Instantiate(selectedGem, position, Quaternion.identity);
            gem.transform.SetParent(transform);
            gem.name = "Gem - " + i + "," + j;
            gem.gameObject.layer = 6;
            gem.pos = selectedPos;  
            gem.isMoving = false; 
            gem.shootId = gem.GetShootIdBasedOnType(gem.type, gemData);
            interactGems.Add(gem);
        }
    }
}






    private void Update()
    {
        Check();
        if (IsBoardEmpty(board))
        {
            int index = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(index);
        }

    }

    private bool IsBoardEmpty(Board board)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allGems[i, j] != null)
                {
                    return false; 
                }
            }
        }

        return true; 
    }

    public void Check()
    {
        foreach (InteractGems iGem in interactGems)
        {
            if (iGem.mousePressed && !iGem.isMoving)
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
        Vector3 targetPos = iGem.pos.position;
        float journeyLength = Vector3.Distance(iGem.transform.position, targetPos);
        float startTime = Time.time;

 
        while (Vector3.Distance(iGem.transform.position, targetPos) > 0.1f)
        {
            float distanceCovered = (Time.time - startTime) * iGem.shootSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;


            iGem.transform.position = Vector3.Lerp(iGem.transform.position, targetPos, fractionOfJourney);

            yield return null; 
        }
        
        iGem.transform.position = targetPos;


        GameObject bullet = Instantiate(BulletPrefab, iGem.transform.position, Quaternion.identity);


        TriggerVibration();

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

            yield return new WaitForSeconds(0.1f);

            Destroy(bGem.gameObject); 
            
            iGem.bGemDestoryed++;
            Debug.Log("Destroyed " + bGem?.name + " | Current Count: " + iGem.bGemDestoryed);
        }

    
        Destroy(bullet);
        
        iGem.DecreaseTempShootId();

     
        if (iGem.bGemDestoryed == iGem.shootId)
        {
            Debug.Log(iGem.name + " has reached its destruction limit.");
            interactGems.Remove(iGem); 
            Destroy(iGem.gameObject); 
        }
        else
        {
  
            iGem.isMoving = false;
        }
    }


    private void TriggerVibration()
    {
        #if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
        #endif
    }
}


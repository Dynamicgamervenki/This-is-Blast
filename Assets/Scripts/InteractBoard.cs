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
    // Create a list to track available gems and available positions for unique placement
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

            // Pick a unique gem
            InteractGems selectedGem;
            if (availableGems.Count > 0)
            {
                int randomIndex = Random.Range(0, availableGems.Count);
                selectedGem = availableGems[randomIndex];
                availableGems.RemoveAt(randomIndex); // Remove the selected gem to ensure uniqueness
            }
            else
            {
                // If all gems have been used once, reset the list to allow duplicates
                availableGems = new List<InteractGems>(gems);
                int randomIndex = Random.Range(0, availableGems.Count);
                selectedGem = availableGems[randomIndex];
                availableGems.RemoveAt(randomIndex);
            }

            // Pick a unique position
            Transform selectedPos;
            if (availablePositions.Count > 0)
            {
                int randomIndex = Random.Range(0, availablePositions.Count);
                selectedPos = availablePositions[randomIndex];
                availablePositions.RemoveAt(randomIndex); // Remove the selected position to ensure uniqueness
            }
            else
            {
                // If all positions have been used once, reset the list to allow duplicates
                availablePositions = new List<Transform>(moveBoard.bgTilesTransform);
                int randomIndex = Random.Range(0, availablePositions.Count);
                selectedPos = availablePositions[randomIndex];
                availablePositions.RemoveAt(randomIndex);
            }

            // Instantiate the gem and assign the position
            InteractGems gem = Instantiate(selectedGem, position, Quaternion.identity);
            gem.transform.SetParent(transform);
            gem.name = "Gem - " + i + "," + j;
            gem.gameObject.layer = 6;
            gem.pos = selectedPos;  // Assign the unique position
            gem.isMoving = false; // Initialize isMoving for each gem
            interactGems.Add(gem);
        }
    }
}



    private void Update()
    {
        Check();
        if (IsBoardEmpty(board))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
                    return false; // Not empty if a gem is found
                }
            }
        }

        return true; // Empty if no gems are found on the board
    }

    public void Check()
    {
        foreach (InteractGems iGem in interactGems)
        {
            // Check if the gem is pressed, not moving, and has not reached its shoot limit
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
        // Ensure the gem moves to the target position
        Vector3 targetPos = iGem.pos.position;
        float journeyLength = Vector3.Distance(iGem.transform.position, targetPos);
        float startTime = Time.time;

        // Smoothly move iGem to target position over time
        while (Vector3.Distance(iGem.transform.position, targetPos) > 0.1f)
        {
            float distanceCovered = (Time.time - startTime) * iGem.shootSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            // Move the gem smoothly using Lerp
            iGem.transform.position = Vector3.Lerp(iGem.transform.position, targetPos, fractionOfJourney);

            yield return null; // Wait for the next frame
        }

        // Ensure the final position is exactly the target position
        iGem.transform.position = targetPos;

        // Instantiate a bullet at the gem's position
        GameObject bullet = Instantiate(BulletPrefab, iGem.transform.position, Quaternion.identity);

        // **Trigger vibration when the bullet is fired**
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

            // **Trigger vibration when the gem is hit**
            TriggerVibration();

            Destroy(bGem.gameObject); // Destroy the target gem

            // Increment the bGemDestoryed count
            iGem.bGemDestoryed++;
            Debug.Log("Destroyed " + bGem?.name + " | Current Count: " + iGem.bGemDestoryed);
        }

        // Destroy the bullet after the gem is destroyed
        Destroy(bullet);

        // Call the method to decrease the temporary shootId for the UI update
        iGem.DecreaseTempShootId();

        // Check if the gem has reached its destruction limit
        if (iGem.bGemDestoryed == iGem.shootId)
        {
            Debug.Log(iGem.name + " has reached its destruction limit.");
            interactGems.Remove(iGem); // Remove it from the list
            Destroy(iGem.gameObject); // Destroy the gem
        }
        else
        {
            // Reset the isMoving flag if the gem is still active
            iGem.isMoving = false;
        }
    }

// Vibration or Haptic Feedback Method
    private void TriggerVibration()
    {
        #if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
        #endif
    }
}


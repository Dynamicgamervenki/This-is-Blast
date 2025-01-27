using TMPro;
using UnityEngine;

public class InteractGems : MonoBehaviour
{
    public bool mousePressed;
    public int shootId = 0;
    public int bGemDestoryed = 0;

    public Transform pos;
    public enum IGemType { blue, green, red, yellow, purple }
    public IGemType type;
    public bool isMoving;

    public float shootSpeed = 20f;

    // TextMeshPro reference to display the shootId
    public TextMeshPro textField;
    
    // Temporary shootId for UI updates
    private int tempShootId;

    private void Start()
    {
        // Create a new TextMeshPro object dynamically
        GameObject textObject = new GameObject("ShootIdText");
        textObject.transform.SetParent(transform);  // Make sure the text is a child of the gem
        textObject.transform.localPosition = Vector3.zero;  // Position it at the center of the gem (relative to the gem)

        // Add the TextMeshPro component to the object
        textField = textObject.AddComponent<TextMeshPro>();

        textField.autoSizeTextContainer = true;
        textField.sortingOrder = 10;
        
        //  text to show the shootId
        textField.text = shootId.ToString();

        //  text appearance (optional)
        textField.fontSize = 6;
        textField.color = Color.black;
        textField.alignment = TextAlignmentOptions.Center;

        // Initialize the temporary shootId with the original shootId
        tempShootId = shootId;
    }

    private void OnMouseDown()
    {
        mousePressed = true;
    }

    private void Update()
    {
        // Handle touch input for Android
        if (Input.touchCount > 0) // Check if there is at least one touch
        {
            Touch touch = Input.GetTouch(0); // Get the first touch

            if (touch.phase == TouchPhase.Began) // Touch started
            {
                OnTouchDown(touch);
            }
        }
    }

    private void OnTouchDown(Touch touch)
    {
        // Convert the touch position to a world point
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
        RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject) // Check if this object was touched
        {
            mousePressed = true; // Perform the same action as OnMouseDown
        }
    }

    // You can update the text dynamically as well
    public void UpdateShootIdText()
    {
        if (textField != null)
        {
            // Update the UI text with the temporary shootId
            textField.text = tempShootId.ToString();
        }
    }

    // Call this function to decrease the temporary shootId when a gem is destroyed
    public void DecreaseTempShootId()
    {
        // Only decrease if the temporary shootId is greater than 0
        if (tempShootId > 0)
        {
            tempShootId--;
        }
        UpdateShootIdText(); // Update the UI text after the decrease
    }
}

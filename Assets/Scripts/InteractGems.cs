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
    
    public TextMeshPro textField;
    

    private int tempShootId;
    

    private void Start()
    {
        // Create a new TextMeshPro object dynamically
        GameObject textObject = new GameObject("ShootIdText");
        textObject.transform.SetParent(transform);  
        textObject.transform.localPosition = Vector3.zero;  


        textField = textObject.AddComponent<TextMeshPro>();
        textField.autoSizeTextContainer = true;
        textField.sortingOrder = 10;
        textField.text = shootId.ToString();

        textField.fontSize = 6;
        textField.color = Color.black;
        textField.alignment = TextAlignmentOptions.Center;
        
        tempShootId = shootId;
    }
    
    public int GetShootIdBasedOnType(IGemType gemType,GemData gemData)
    {
        switch (gemType)
        {
            case IGemType.blue:
                return gemData.blueShootId;
            case IGemType.green:
                return gemData.greenShootId;
            case IGemType.red:
                return gemData.redShootId;
            case IGemType.yellow:
                return gemData.yellowShootId;
            case IGemType.purple:
                return gemData.purpleShootId;
            default:
                return 0; 
        }
    }
    
    private void OnMouseDown()
    {
        mousePressed = true;
    }

    private void Update()
    {
        // Handle touch input for Android
        if (Input.touchCount > 0) 
        {
            Touch touch = Input.GetTouch(0); 

            if (touch.phase == TouchPhase.Began) 
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

        if (hit.collider != null && hit.collider.gameObject == gameObject) 
        {
            mousePressed = true; 
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

    //  function to decrease the temporary shootId when a gem is destroyed
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

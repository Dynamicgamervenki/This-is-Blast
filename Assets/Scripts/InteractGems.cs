using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractGems : MonoBehaviour
{
    public bool mousePressed;
    public int shootId = 0;
    public int bGemDestoryed = 0;
    public enum IGemType {blue ,green,red,yellow,purple}
    public IGemType type;

    
    private void OnMouseDown()
    { 
        mousePressed = true;
    }
}

using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public bool hasRed, hasGreen, hasBlue;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CanvasManager.Instance.ClearKeys();
    }

}

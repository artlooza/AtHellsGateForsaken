using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public bool hasRed, hasGreen, hasBlue;

    void Start()
    {
        CanvasManager.Instance.ClearKeys();
    }

    public void GiveKey(string keyColor)
    {
        switch (keyColor.ToLower())
        {
            case "red":
                hasRed = true;
                break;
            case "green":
                hasGreen = true;
                break;
            case "blue":
                hasBlue = true;
                break;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CanvasManager : MonoBehaviour
{
    public TextMeshProUGUI health;
    public TextMeshProUGUI armor;
    public TextMeshProUGUI ammo;

    public Image healthIndicator;

    public Sprite health1; //healthy
    public Sprite health2;
    public Sprite health3;
    public Sprite health4;
    public Sprite health5; //dead


    public GameObject redKey;
    public GameObject blueKey;
    public GameObject greenKey;

    public Reticle reticle; // reference to reticle script


    private static CanvasManager _instance;
    public static CanvasManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }


    public void UpdateHealth(int healthValue)
    {
        health.text = healthValue.ToString() + "%";
        UpdateHealthIndicator(healthValue);
    }
    public void UpdateArmor(int armorValue)
    {
        armor.text = armorValue.ToString() + "%";
    }
    public void UpdateAmmo(int ammoValue)
    {
        ammo.text = ammoValue.ToString();

    }

    public void UpdateHealthIndicator(int healthValue)
    {
        if (healthValue >= 66)
        {
            healthIndicator.sprite = health1; // healthy face
        }
        else if (healthValue >= 33)
        {
            healthIndicator.sprite = health2; //
        }
        else if (healthValue > 0)
        {
            healthIndicator.sprite = health3;
        }
        else
        {
            healthIndicator.sprite = health5; // dead face
        }
    }

    public void UpdateKeys(string keyColor)
    {
        if(keyColor == "red")
        {
            redKey.SetActive(true);
        }
        else if(keyColor == "blue")
        {
            blueKey.SetActive(true);
        }
        else if(keyColor == "green")
        {
            greenKey.SetActive(true);
        }
    }

    public void ClearKeys()
    {
        redKey.SetActive(false);
        blueKey.SetActive(false);
        greenKey.SetActive(false);
    }

    public void ShowHitMarker()
    {
        if(reticle != null)
        {
            reticle.ShowHitMarker();
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


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

    [Header("Boss Health Bar")]
    public GameObject bossHealthBarPanel;
    public Image bossHealthBarFill;
    public Image bossHealthBarBackground;
    public TextMeshProUGUI bossNameText;
    public TextMeshProUGUI phaseText;
    public RectTransform healthBarRect;  // For dynamic sizing

    [Header("Health Bar Sizes")]
    public float phase1BarWidth = 200f;   // Small bar for "Pablo"
    public float phase2BarWidth = 600f;   // Large dramatic bar for "Pablo the Destroyer"
    public float barResizeSpeed = 2f;     // How fast the bar grows

    private Boss currentBoss;
    private float bossMaxHealth;
    private float targetBarWidth;
    private Coroutine resizeCoroutine;


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

    public void ShowBossHealthBar(Boss boss, string bossName)
    {
        currentBoss = boss;
        bossHealthBarPanel.SetActive(true);
        bossNameText.text = bossName;

        // Set initial size for Phase 1
        bossMaxHealth = boss.phase1Health;
        targetBarWidth = phase1BarWidth;
        if (healthBarRect != null)
            healthBarRect.sizeDelta = new Vector2(phase1BarWidth, healthBarRect.sizeDelta.y);

        UpdateBossPhase(boss.currentPhase);
    }

    public void HideBossHealthBar()
    {
        bossHealthBarPanel.SetActive(false);
        currentBoss = null;
        if (resizeCoroutine != null)
            StopCoroutine(resizeCoroutine);
    }

    public void UpdateBossHealth(float currentHealth)
    {
        if (bossHealthBarFill != null && bossMaxHealth > 0)
        {
            float fillAmount = Mathf.Clamp01(currentHealth / bossMaxHealth);
            bossHealthBarFill.fillAmount = fillAmount;
        }
    }

    public void UpdateBossPhase(int phase, string newName = null)
    {
        if (phaseText != null)
        {
            phaseText.text = $"PHASE {phase}";
            phaseText.color = phase == 2 ? Color.red : Color.white;
        }

        // Update boss name if provided
        if (newName != null && bossNameText != null)
            bossNameText.text = newName;

        // Update max health and bar size when phase changes
        if (currentBoss != null)
        {
            bossMaxHealth = phase == 1 ? currentBoss.phase1Health : currentBoss.phase2Health;

            // Dramatic resize for Phase 2
            if (phase == 2)
            {
                targetBarWidth = phase2BarWidth;
                if (resizeCoroutine != null)
                    StopCoroutine(resizeCoroutine);
                resizeCoroutine = StartCoroutine(AnimateBarResize());
            }
        }
    }

    private IEnumerator AnimateBarResize()
    {
        float currentWidth = healthBarRect.sizeDelta.x;

        while (Mathf.Abs(currentWidth - targetBarWidth) > 1f)
        {
            currentWidth = Mathf.Lerp(currentWidth, targetBarWidth, barResizeSpeed * Time.deltaTime);
            healthBarRect.sizeDelta = new Vector2(currentWidth, healthBarRect.sizeDelta.y);
            yield return null;
        }

        healthBarRect.sizeDelta = new Vector2(targetBarWidth, healthBarRect.sizeDelta.y);
    }
}

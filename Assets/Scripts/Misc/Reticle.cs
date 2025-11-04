using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{
    public Image reticleImage; // reference to the UI Image component
    public Sprite defaultReticle; // default crosshair sprite
    public Sprite hitMarkerReticle; // crosshair when hitting enemy (optional)

    public Color defaultColor = Color.white;
    public Color hitColor = Color.red;

    public float hitMarkerDuration = 0.1f; // how long the hit marker shows
    private float hitMarkerTimer = 0f;

    void Start()
    {
        // If no image is assigned, try to get it from this GameObject
        if(reticleImage == null)
        {
            reticleImage = GetComponent<Image>();
        }

        // Set default appearance
        if(reticleImage != null)
        {
            reticleImage.color = defaultColor;
            if(defaultReticle != null)
            {
                reticleImage.sprite = defaultReticle;
            }
        }
    }

    void Update()
    {
        // Handle hit marker timer
        if(hitMarkerTimer > 0f)
        {
            hitMarkerTimer -= Time.deltaTime;
            if(hitMarkerTimer <= 0f)
            {
                ResetReticle();
            }
        }
    }

    // Call this when the player hits an enemy
    public void ShowHitMarker()
    {
        if(reticleImage != null)
        {
            reticleImage.color = hitColor;
            if(hitMarkerReticle != null)
            {
                reticleImage.sprite = hitMarkerReticle;
            }
            hitMarkerTimer = hitMarkerDuration;
        }
    }

    private void ResetReticle()
    {
        if(reticleImage != null)
        {
            reticleImage.color = defaultColor;
            if(defaultReticle != null)
            {
                reticleImage.sprite = defaultReticle;
            }
        }
    }
}

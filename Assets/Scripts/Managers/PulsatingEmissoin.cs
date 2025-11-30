using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PulsatingEmission : MonoBehaviour
{
    public Color emissionColor = Color.green;  // base color of your portal
    public float minIntensity = 0.5f;          // how dim it gets
    public float maxIntensity = 3f;            // how bright it gets
    public float pulseSpeed = 2f;              // how fast it pulses

    Material _material;
    static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    void Start()
    {
        var renderer = GetComponent<Renderer>();
        // Use a unique instance of the material, so you don’t affect other objects
        _material = renderer.material;
        _material.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        // 0 → 1 → 0 ping-pong
        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, t);

        Color finalColor = emissionColor * intensity;
        _material.SetColor(EmissionColorID, finalColor);
    }
}

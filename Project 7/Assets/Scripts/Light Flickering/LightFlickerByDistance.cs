using UnityEngine;

public class LightFlickerByDistance : MonoBehaviour
{
    public float maxDistance;
    public float minDistance;

    private Transform _monsterTransform;
    private LightFlickerEffect _flickerEffect;
    private float _maxLight;
    private int _maxSmoothing;

    // Start is called before the first frame update
    private void Start()
    {
        _monsterTransform = GameObject.FindWithTag("Monster").transform;

        _flickerEffect = gameObject.GetComponent<LightFlickerEffect>();
        _maxLight = _flickerEffect.maxIntensity;
        _maxSmoothing = _flickerEffect.smoothing;
    }

    // Update is called once per frame
    private void Update()
    {
        if(!_monsterTransform) return;
        float distance = Vector3.Distance(transform.position, _monsterTransform.position);

        float intensity = (distance - minDistance) / maxDistance;
        intensity = Mathf.Clamp(intensity, 0, 1);

        _flickerEffect.maxIntensity = _maxLight * intensity;
        _flickerEffect.smoothing = Mathf.Clamp(Mathf.RoundToInt(_maxSmoothing * intensity), 1, 50);
    }
}
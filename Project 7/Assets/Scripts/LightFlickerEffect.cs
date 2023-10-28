using System.Collections.Generic;
using UnityEngine;

// Written by Steve Streeting 2017
// License: CC0 Public Domain http://creativecommons.org/publicdomain/zero/1.0/

/// <summary>
/// Component which will flicker a linked light while active by changing its
/// intensity between the min and max values given. The flickering can be
/// sharp or smoothed depending on the value of the smoothing parameter.
///
/// Just activate / deactivate this component as usual to pause / resume flicker
/// </summary>
public class LightFlickerEffect : MonoBehaviour
{
    [Tooltip("External light to flicker; you can leave this null if you attach script to a light")]
    public new Light light;

    [Tooltip("Emissive material to go with the light")]
    public Renderer emissive;

    [Tooltip("Minimum random light intensity multiplier")]
    public float minIntensity;

    [Tooltip("Maximum random light intensity multiplier")]
    public float maxIntensity = 1f;

    [Tooltip("How much to smooth out the randomness; lower values = sparks, higher = lantern")] [Range(1, 50)]
    public int smoothing = 5;

    // Continuous average calculation via FIFO queue
    // Saves us iterating every time we update, we just change by the delta
    private Queue<float> _smoothQueue;
    private float _lastSum;

    private float _lightIntensity;

    private Color _emissiveColor;
    private static readonly int EmissiveColor = Shader.PropertyToID("_EmissionColor");


    /// <summary>
    /// Reset the randomness and start again. You usually don't need to call
    /// this, deactivating/reactivating is usually fine but if you want a strict
    /// restart you can do.
    /// </summary>
    public void Reset()
    {
        _smoothQueue.Clear();
        _lastSum = 0;
    }

    private void Start()
    {
        _smoothQueue = new Queue<float>(smoothing);
        // External or internal light?
        if(!light)
            light = GetComponent<Light>();

        if(!emissive)
            emissive = GetComponent<Renderer>();

        _lightIntensity = light.intensity;
        _emissiveColor = emissive.material.GetColor(EmissiveColor);
    }

    private void Update()
    {
        if(!light && !emissive) return;

        // pop off an item if too big
        while (_smoothQueue.Count >= smoothing)
        {
            _lastSum -= _smoothQueue.Dequeue();
        }

        // Generate random new item, calculate new average
        float newVal = Random.Range(minIntensity, maxIntensity);
        _smoothQueue.Enqueue(newVal);
        _lastSum += newVal;

        // Calculate new smoothed average
        float intensity = _lastSum / _smoothQueue.Count;
        if(light) light.intensity = intensity * _lightIntensity;
        if(emissive) emissive.material.SetColor(EmissiveColor, _emissiveColor * intensity);
    }
}
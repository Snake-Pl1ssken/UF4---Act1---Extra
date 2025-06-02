using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drum : MonoBehaviour
{
    public float vibrateDuration = 0.5f;
    public float vibrateAmplitude = 0.2f;
    public float vibrateFrequency = 3.0f;

    public bool test;

    AudioSource source;

    Vector3 basePosition;

    float vibrateTimer;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        basePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(test) { Hit(); }

        if(vibrateTimer > 0)
        {
            vibrateTimer -= Time.deltaTime;
        }
        else
        {
            vibrateTimer = 0;
        }

        transform.position = basePosition + Vector3.right * Mathf.Sin(vibrateTimer / vibrateDuration * 360.0f * Mathf.Deg2Rad * vibrateFrequency) * vibrateAmplitude;

        test = false;
    }

    public void Hit()
    {
        source.Stop();
        source.Play();
        vibrateTimer = vibrateDuration;
    }
}

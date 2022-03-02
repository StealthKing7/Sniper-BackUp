using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera shake;
    private float ShakeTimer;
    public static CameraShake Instance { get; private set; }
    private CinemachineBasicMultiChannelPerlin channelPerlin;
    void Awake()
    {
        Instance = this;
        shake = GetComponent<CinemachineVirtualCamera>();
        channelPerlin = shake.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    public void Shake(float Intensity,float time)
    {
        ShakeTimer = time;
        channelPerlin.m_AmplitudeGain = Intensity;
    }
    void Update()
    {
        if (ShakeTimer < 0)
        {
            ShakeTimer -= Time.deltaTime;
            if (ShakeTimer <= 0)
            {
                channelPerlin.m_AmplitudeGain = 0;
            }
        }
    }

}

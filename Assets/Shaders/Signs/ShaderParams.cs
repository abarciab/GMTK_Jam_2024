using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderParams : MonoBehaviour
{
    public static int fadeOut = Shader.PropertyToID("_FadeOut");
    public static int inAnim = Shader.PropertyToID("_InAnim");
    public static int outAnim = Shader.PropertyToID("_OutAnim");
    public static int isNotActiveButton = Shader.PropertyToID("_isNotActiveButton");
    public static int particleHueOffset = Shader.PropertyToID("_ParticleHueOffset");
    public static int hue_Offset = Shader.PropertyToID("_Hue_Offset");
    public static int bgHue_Offset = Shader.PropertyToID("_BGHue_Offset");
    public static int sat = Shader.PropertyToID("_Sat");
    public static int val = Shader.PropertyToID("_Val");
    public static int particleSat = Shader.PropertyToID("_ParticleSat");
    public static int particleVal = Shader.PropertyToID("_ParticleVal");
    public static int lastHitTime = Shader.PropertyToID("_lastHitTime");
    public static int idle_Rotation = Shader.PropertyToID("_Idle_Rotation");
    public static int alpha = Shader.PropertyToID("_Alpha");
    public static int bgSat = Shader.PropertyToID("_BGSat");
    public static int bgVal = Shader.PropertyToID("_BGVal");

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode,ImageEffectAllowedInSceneView]
public class UnderwaterEff : MonoBehaviour
{
    public Material _mat;
    [Range(0.001f, 0.1f)]
    public float _pixelOffest;
    [Range(0.1f, 20f)]
    public float _noiseScale;
    [Range(0.1f, 20f)]
    public float _noiseFrequency;

    [Range(0.1f, 30f)]
    public float _noiseSpeed;


    public float _DepthStart;
    public float _DepthDis;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _mat.SetFloat("_NoiseScale", _noiseScale);
        _mat.SetFloat("_NoiseFrequency", _noiseFrequency);
        _mat.SetFloat("_NoiseSpeed", _noiseSpeed);
        _mat.SetFloat("_PixelOffest", _pixelOffest);
        _mat.SetFloat("_DepthStart", _DepthStart);
        _mat.SetFloat("_DepthDis", _DepthDis);
    }

    private void OnRenderImage(RenderTexture soured,RenderTexture destination)
    {
        Graphics.Blit(soured, destination,_mat);
        
    }
}

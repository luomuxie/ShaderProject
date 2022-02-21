using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode,ImageEffectAllowedInSceneView]
public class FogEff : MonoBehaviour
{
    public Material _mat;
    public Color _fogColor;

    [Range(0.1f, 20f)]
    public float _DepthStart;

    [Range(0.1f, 30f)]
    public float _DepthDis;

    void Start()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }

    // Update is called once per frame
    void Update()
    {
        _mat.SetColor("_FogColor",_fogColor);
        _mat.SetFloat("_DepthStart", _DepthStart);
        _mat.SetFloat("_DepthDis", _DepthDis);
    }

    private void OnRenderImage(RenderTexture soure,RenderTexture destination)
    {
        Graphics.Blit(soure, destination,_mat);
    }
}

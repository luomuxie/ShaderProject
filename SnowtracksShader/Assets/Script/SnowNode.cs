using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowNode : MonoBehaviour
{
    public Shader _snowFallShader;
    private Material snowFallMat;
    private MeshRenderer meshRenderer;
    [Range(0.001f, 1f)]
    public float _flackAmount;
    [Range(0f, 1f)]
    public float _flackOpacity;
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        snowFallMat = new Material(_snowFallShader);
    }

    // Update is called once per frame
    void Update()
    {
        snowFallMat.SetFloat("_FlackAmount", _flackAmount);
        snowFallMat.SetFloat("_FlackOpacity", _flackOpacity);
        RenderTexture snow = (RenderTexture)meshRenderer.material.GetTexture("_splat");
        RenderTexture temp = RenderTexture.GetTemporary(snow.width, snow.height, 0, RenderTextureFormat.ARGBFloat);
        Graphics.Blit(snow, temp, snowFallMat);
        Graphics.Blit(temp, snow);
        meshRenderer.material.SetTexture("_splat", snow);
        RenderTexture.ReleaseTemporary(temp);

    }


}

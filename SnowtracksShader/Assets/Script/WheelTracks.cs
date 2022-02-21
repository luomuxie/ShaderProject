using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTracks : MonoBehaviour
{
    
    public Shader _drawShader;
    public GameObject _terrain;
    public Transform[] wheels;

    [Range(0f, 20f)]
    public float _brushSize = 1;
    [Range(0f, 2f)]
    public float _brushStrength = 1;

    private RenderTexture _splatMap;
    private Material snowMaterial;
    private Material drawMaterial;
    private RaycastHit groundHit;
    private int layerMask;

    // Start is called before the first frame update
    void Start()
    {
        layerMask = LayerMask.GetMask("Ground");
        drawMaterial = new Material(_drawShader);
        drawMaterial.SetVector("_Color", Color.red);
        snowMaterial = _terrain.GetComponent<MeshRenderer>().material;
        _splatMap = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        snowMaterial.SetTexture("_splat", _splatMap);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            if(Physics.Raycast(wheels[i].position,-Vector3.up,out groundHit, 1f, layerMask))
            {
                drawMaterial.SetFloat("_Size", _brushSize);
                drawMaterial.SetFloat("_Strength", _brushStrength);
                //设置碰撞位置处的 UV 纹理坐标
                drawMaterial.SetVector("_Coordinate", new Vector4(groundHit.textureCoord.x, groundHit.textureCoord.y, 0, 0));
                //获取临时渲染纹理
                RenderTexture temp = RenderTexture.GetTemporary(_splatMap.width, _splatMap.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(_splatMap, temp);
                Graphics.Blit(temp, _splatMap, drawMaterial);
                RenderTexture.ReleaseTemporary(temp);
            }
        }
    }

}

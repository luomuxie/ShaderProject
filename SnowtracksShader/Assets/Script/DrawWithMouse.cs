using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawWithMouse : MonoBehaviour
{
    public Camera _camera;
    public Shader _drawShader;
    
    [Range(0f, 20f)]
    public float _brushSize = 1;
    [Range(0f,2f)]
    public float _brushStrength = 1;

    private RenderTexture _splatMap;
    private Material _snowMaterial, _drawMaterial;
    private RaycastHit _raycastHit;

    // Start is called before the first frame update
    void Start()
    {
        _drawMaterial = new Material(_drawShader);
        _drawMaterial.SetVector("_Color", Color.red);
        _snowMaterial = GetComponent<MeshRenderer>().material;
        _splatMap = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        _snowMaterial.SetTexture("_splat",_splatMap);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Mouse0)){
            if(Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),out _raycastHit)){
                _drawMaterial.SetFloat("_Size", _brushSize);
                _drawMaterial.SetFloat("_Strength", _brushStrength);
                //设置碰撞位置处的 UV 纹理坐标
                _drawMaterial.SetVector("_Coordinate", new Vector4(_raycastHit.textureCoord.x, _raycastHit.textureCoord.y, 0, 0));
                //获取临时渲染纹理
                RenderTexture temp = RenderTexture.GetTemporary(_splatMap.width, _splatMap.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(_splatMap, temp);
                Graphics.Blit(temp, _splatMap, _drawMaterial);
                RenderTexture.ReleaseTemporary(temp);

            }
        }
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, 256, 256), _splatMap, ScaleMode.ScaleToFit, false, 1);
    }
}

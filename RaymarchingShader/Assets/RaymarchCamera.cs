using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class RaymarchCamera : SceneViewFilter
{
    
    public Shader shader;
    public Material _raymarchMaterial
    {
        get
        {
            if (!_raymarchMat && shader)
            {
                _raymarchMat = new Material(shader); ;
                _raymarchMat.hideFlags = HideFlags.HideAndDontSave; ;
            }
            return _raymarchMat;
        }
    }
    private Material _raymarchMat;
    

    public Camera _camera
    {
        get 
        {
            if (!_cam)
            {
                _cam = GetComponent<Camera>();
                return _cam;
            }
            return _cam;
                
        }
    }
    private Camera _cam;
    
    public Color _mainColor;

    [Header("set up")]
    public float _maxDis;
    [Range(0,300)]
    public int _MaxIter;
    [Range(0.0f,0.1f)]
    public float _Accuracy;


    [Header ("Dir Light")]
    public Transform _directionaLight;
    public Color _LightCol;
    public float _LightIntensity;

    [Header("Shadow")]
    [Range(0,4)]
    public float _ShadowIntensity;
    public Vector2 _ShadowDis;
    [Range(1, 128)]
    public float _ShadowPenumbra;

 
    [Header("Ambient Occlusion")]
    [Range(0.01f,10.0f)]
    public float _AoStepsize;
    [Range(0,1)]
    public float _AoIntesity;
    [Range(1,5)]
    public int _AoIter;



    [Header("signe dis field")]
    public float _box1round;
    public float _boxSphereSmooth;
    public float _sphereIntersectSmooth;
    public Vector4 _sphere1, _box1, _shpere2;
    //public Vector3 _modInterval;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!_raymarchMaterial)
        {
            Graphics.Blit(source, destination);
            return;
        }

        _raymarchMaterial.SetVector("_LightDir", _directionaLight ? _directionaLight.forward: Vector3.down);
        _raymarchMaterial.SetMatrix("_CamFrustum", camFrustm(_camera));
        _raymarchMaterial.SetMatrix("_CamToWorld", _camera.cameraToWorldMatrix);
        _raymarchMaterial.SetFloat("_MaxDis",_maxDis);
        _raymarchMaterial.SetFloat("_box1round", _box1round);
        _raymarchMaterial.SetFloat("_boxSphereSmooth", _boxSphereSmooth);
        _raymarchMaterial.SetFloat("_sphereIntersectSmooth", _sphereIntersectSmooth);

        _raymarchMaterial.SetVector("_shpere1", _sphere1);
        _raymarchMaterial.SetVector("_shpere2", _shpere2);
        _raymarchMaterial.SetVector("_box1", _box1);
        _raymarchMaterial.SetColor("_mainColor", _mainColor);

        _raymarchMaterial.SetColor("_LightCol", _LightCol);
        _raymarchMaterial.SetFloat("_LightIntensity", _LightIntensity);
        _raymarchMaterial.SetFloat("_ShadowIntensity", _ShadowIntensity);
        _raymarchMaterial.SetVector("_ShadowDis", _ShadowDis);
        _raymarchMaterial.SetFloat("_ShadowPenumbra", _ShadowPenumbra);

        _raymarchMaterial.SetInt("_MaxIter", _MaxIter);
        _raymarchMaterial.SetFloat("_Accuracy", _Accuracy);

        _raymarchMaterial.SetFloat("_AoStepsize", _AoStepsize);
        _raymarchMaterial.SetFloat("_AoIntesity", _AoIntesity);
        _raymarchMaterial.SetInt("_AoIter", _AoIter);


        // _raymarchMaterial.SetVector("_modInterval", _modInterval);
        RenderTexture.active = destination;
        _raymarchMaterial.SetTexture("_MainTex", source);

        //https://docs.unity3d.com/cn/2020.2/ScriptReference/GL.LoadOrtho.html
        GL.PushMatrix(); //将模型、视图和投影矩阵保存到矩阵堆栈顶部。
        GL.LoadOrtho();//将正交投影加载到投影矩阵中，将标识加载到 模型和视图矩阵中。
        _raymarchMaterial.SetPass(0);
        GL.Begin(GL.QUADS);

        //BL
        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.Vertex3(0, 0.0f, 3.0f);
        //BR
        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.Vertex3(1, 0.0f, 2.0f);
        //TR
        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 1.0f);
        //TL
        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();

    }

    //计算摄像机视锥底面 上下左右四个点
    private Matrix4x4 camFrustm(Camera cam)
    {
        Matrix4x4 frustm = Matrix4x4.identity;
        float fov = Mathf.Tan(cam.fieldOfView *0.5f * Mathf.Deg2Rad);//计算沿垂直方面的视野区域
        Vector3 goUp = Vector3.up* fov;
        Vector3 goRight = Vector3.right * fov * cam.aspect;//aspect:宽高比
        Vector3 TL = (-Vector3.forward - goRight + goUp);
        Vector3 TR = (-Vector3.forward + goRight + goUp);
        Vector3 BR = (-Vector3.forward + goRight - goUp);
        Vector3 BL = (-Vector3.forward-goRight-goUp);
        
        frustm.SetRow(0,TL);
        frustm.SetRow(1, TR);
        frustm.SetRow(2, BR);
        frustm.SetRow(3, BL);
        
        return frustm;
    }
}

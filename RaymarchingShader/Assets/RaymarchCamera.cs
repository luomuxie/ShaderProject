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

    [Header("Reflection")]
    [Range(0, 2)]
    public int _ReflectionCnt;
    [Range(0, 1)]
    public float _ReflactionIntensity;
    [Range(0, 1)]
    public float _EnvReflIntensity;
    public Cubemap _ReflactionCube;

    [Header("signe dis field")]
    public Color _mainColor;
    public Vector4 _sphere;
    public float _sphereSmooth;
    public float _degreeRotate;    

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
        _raymarchMaterial.SetFloat("_sphereSmooth", _sphereSmooth);
        _raymarchMaterial.SetFloat("_degreeRotate", _degreeRotate);
        _raymarchMaterial.SetVector("_sphere", _sphere);
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

        _raymarchMaterial.SetInt("_ReflectionCnt", _ReflectionCnt);
        _raymarchMaterial.SetFloat("_ReflactionIntensity", _ReflactionIntensity);
        _raymarchMaterial.SetFloat("_EnvReflIntensity", _EnvReflIntensity);
        _raymarchMaterial.SetTexture("_ReflactionCube", _ReflactionCube);




        // _raymarchMaterial.SetVector("_modInterval", _modInterval);
        RenderTexture.active = destination;
        _raymarchMaterial.SetTexture("_MainTex", source);

        //https://docs.unity3d.com/cn/2020.2/ScriptReference/GL.LoadOrtho.html
        GL.PushMatrix(); //��ģ�͡���ͼ��ͶӰ���󱣴浽�����ջ������
        GL.LoadOrtho();//������ͶӰ���ص�ͶӰ�����У�����ʶ���ص� ģ�ͺ���ͼ�����С�
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

    //�����������׶���� ���������ĸ���
    private Matrix4x4 camFrustm(Camera cam)
    {
        Matrix4x4 frustm = Matrix4x4.identity;
        float fov = Mathf.Tan(cam.fieldOfView *0.5f * Mathf.Deg2Rad);//�����ش�ֱ�������Ұ����
        Vector3 goUp = Vector3.up* fov;
        Vector3 goRight = Vector3.right * fov * cam.aspect;//aspect:��߱�
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

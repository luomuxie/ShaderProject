using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaymarchCamera : MonoBehaviour
{
    [SerializeField]
    private Shader shader;
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

    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!_raymarchMaterial)
        {
            Graphics.Blit(source, destination);
            return;
        }
        
    }
}

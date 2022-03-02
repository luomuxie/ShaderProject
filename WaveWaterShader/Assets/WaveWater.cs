using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WaveWater : MonoBehaviour
{
    public int _waveWidth;
    public int _waveHeight;
    [Range(0f, 0.1f)]
    public float _dampVal;
    public int _radius;
    private float[,] _waveA;
    private float[,] _waveB;
    private Texture2D _texUV;
    private bool _isRun = true;
    private int _sleepTime;
    private Color[] _colorBuffer;

    void Start()
    {
        _waveA =  new float[_waveWidth,_waveHeight];
        _waveB = new float[_waveWidth, _waveHeight];
        _texUV = new Texture2D(_waveWidth, _waveHeight);
        GetComponent<Renderer>().material.SetTexture("_WaterWave", _texUV);
        _colorBuffer = new Color[_waveHeight*_waveHeight];
        Thread thread = new Thread(new ThreadStart(ComPuteWave));
        thread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        _sleepTime = (int)(Time.deltaTime * 1000);
        _texUV.SetPixels(_colorBuffer);
        _texUV.Apply();

        if (Input.GetMouseButton(0))
        {
            RaycastHit hit; 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
            if(Physics.Raycast(ray, out hit))
            {
                Vector3 pos = (hit.point);
                pos = transform.worldToLocalMatrix.MultiplyPoint(pos);

                int w = (int)((pos.x + 0.5) * _waveWidth);
                int h = (int)((pos.y + 0.5) * _waveHeight);
                PutPop(w, h);
            }
        }
        
        //ComPuteWave();
        
    }

    //为水波添加周期性能量，使水波更真具真实感
    void PutPop(int x ,int y)
    {
        //加入能量
        float dist;
        for (int i = -_radius; i <=_radius; i++)
        {
            for (int j = -_radius; j < _radius; j++)
            {
                if(((x+i>=0)&&(x+i<_waveWidth-1)) && (y + j >= 0) && (y + j < _waveHeight - 1))
                {
                    dist = Mathf.Sqrt((j * j + i * i));
                    if (dist < _radius)
                    {
                        _waveA[x+i,y+j]  = Mathf.Cos(dist*Mathf.PI/_radius)*5;
                    }
                }
            }
        }
    }


    //计算能量衰减及UV偏移
    void ComPuteWave()
    {
        while (_isRun)
        {
            for (int w = 1; w < _waveHeight - 1; w++)
            {
                for (int h = 1; h < _waveWidth - 1; h++)
                {
                    _waveB[w, h] = (
                        _waveA[w - 1, h] +
                        _waveA[w + 1, h] +
                        _waveA[w, h - 1] +
                        _waveA[w, h + 1] +
                        _waveA[w - 1, h - 1] +
                        _waveA[w - 1, h + 1] +
                        _waveA[w + 1, h - 1] +
                        _waveA[w + 1, h + 1]) / 4 - _waveB[w, h];

                    float val = _waveB[w, h];
                    if (val > 1) _waveB[w, h] = 1;
                    if (val < -1) _waveB[w, h] = -1;

                    float offset_u = (_waveB[w - 1, h] - _waveB[w + 1, h]) / 2;
                    float offset_v = (_waveB[w, h - 1] - _waveB[w, h + 1]) / 2;

                    //把uv值转到颜色范围（0-1）
                    float r = offset_u / 2 + 0.5f;
                    float g = offset_v / 2 + 0.5f;
                    _colorBuffer[w+_waveWidth*h] = new Color(r, g, 0);
                    _waveB[w, h] -= _waveB[w, h] * _dampVal;
                }

            }

            float[,] temp = _waveA;
            _waveA = _waveB;
            _waveB = temp;
            Thread.Sleep(_sleepTime);
        }
       
    }

    private void OnDestroy()
    {
        _isRun = false;
    }
}

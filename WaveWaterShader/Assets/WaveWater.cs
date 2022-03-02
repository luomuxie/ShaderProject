using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveWater : MonoBehaviour
{
    public int _waveWidth;
    public int _waveHeight;
    [Range(0f, 0.1f)]
    public float _dampVal;
    private float[,] _waveA;
    private float[,] _waveB;
    private Texture2D _texUV;
    void Start()
    {
        _waveA =  new float[_waveWidth,_waveHeight];
        _waveB = new float[_waveWidth, _waveHeight];
        _texUV = new Texture2D(_waveWidth, _waveHeight);
        GetComponent<Renderer>().material.SetTexture("_WaterWave", _texUV);
        PutPop();
    }

    // Update is called once per frame
    void Update()
    {
        ComPuteWave();
        
    }


    void PutPop()
    {
        //��������
        _waveA[_waveWidth / 2, _waveHeight / 2] = 1;
        _waveA[_waveWidth / 2-1, _waveHeight / 2] = 1;
        _waveA[_waveWidth / 2 + 1, _waveHeight / 2] = 1;
        _waveA[_waveWidth / 2, _waveHeight / 2+1] = 1;
        _waveA[_waveWidth / 2, _waveHeight / 2 - 1] = 1;
        _waveA[_waveWidth / 2-1, _waveHeight / 2 - 1] = 1;
        _waveA[_waveWidth / 2 + 1, _waveHeight / 2 - 1] = 1;
        _waveA[_waveWidth / 2 - 1, _waveHeight / 2 +1] = 1;
        _waveA[_waveWidth / 2 + 1, _waveHeight / 2 + 1] = 1;
    }


    //��������˥����UVƫ��
    void ComPuteWave()
    {
        for (int w = 1; w < _waveHeight-1; w++)
        {
            for (int h = 1; h < _waveWidth-1; h++)
            {
                _waveB[w, h] = (
                    _waveA[w - 1, h] +//��
                    _waveA[w + 1, h] +//��
                    _waveA[w, h - 1] +
                    _waveA[w, h + 1] +
                    _waveA[w - 1, h - 1] +
                    _waveA[w - 1, h + 1] +
                    _waveA[w + 1, h - 1] +
                    _waveA[w + 1, h + 1])/4-_waveB[w,h];

                float val = _waveB[w, h];
                if (val > 1) _waveB[w, h] = 1;
                if(val < -1) _waveB[w,h] = -1;
                
                float offset_u = (_waveB[w - 1, h] - _waveB[w + 1, h]) / 2;
                float offset_v = (_waveB[w, h-1] - _waveB[w, h+1]) / 2;

                //��uvֵת����ɫ��Χ��0-1��
                float r = offset_u / 2 + 0.5f;
                float g = offset_v / 2 + 0.5f;
                _texUV.SetPixel(w, h, new Color(r, g, 0));
                _waveB[w, h] -= _waveB[w, h] * _dampVal;
            }

        }

        _texUV.Apply();
        float[,] temp = _waveA;
        _waveA = _waveB;
        _waveB = temp;
    }
}

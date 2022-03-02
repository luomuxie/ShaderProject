using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveWater : MonoBehaviour
{
    public int _waveWidth;
    public int _waveHeight;
    private float[,] _waveA;
    private float[,] _waveB;
    private Texture2D _texUV;
    void Start()
    {
        _waveA =  new float[_waveWidth,_waveHeight];
        _waveA = new float[_waveWidth, _waveHeight];
        _texUV = new Texture2D(_waveWidth, _waveHeight); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //计算能量衰减及UV偏移
    void ComPuteWave()
    {
        for (int w = 1; w < _waveHeight-1; w++)
        {
            for (int h = 1; h < _waveWidth-1; h++)
            {
                _waveB[w, h] = (
                    _waveA[w - 1, h] +
                    _waveA[w + 1, h] +
                    _waveA[w, h - 1] +
                    _waveA[w, h + 1] +
                    _waveA[w - 1, h - 1] +
                    _waveA[w + 1, h - 1] +
                    _waveA[w + 1, h + 1])/4-_waveB[w,h];

                float val = _waveB[w, h];
                if (val > 1) _waveB[w, h] = 1;
                if(val < -1) _waveB[w,h] = 1;
                float offset_u = (_waveB[w - 1, h] - _waveB[w + 1, h]) / 2;
                float offset_v = (_waveB[w, h-1] - _waveB[w, h+1]) / 2;
            }

        }
    }
}

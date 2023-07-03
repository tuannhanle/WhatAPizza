using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CircleRotator : MonoBehaviour
{
    [SerializeField] private float _speed;
    private float _ratio = 1f;
    private float _cacheSpeed;

    // Start is called before the first frame update
    async UniTaskVoid Start()
    {
        _cacheSpeed = _speed;
        while (true)
        {

            await UniTask.WaitForFixedUpdate();
            if (this == null)
                return;
            this.transform.Rotate(0,_speed * _ratio * Time.deltaTime,0);
        }
        
    }

    public void StopRotate(bool isStop)
    {
        _speed = isStop ? 0 : _cacheSpeed;
    }

    public void SpeedUp(float ratio)
    {
        _ratio = ratio;
    }
    
}

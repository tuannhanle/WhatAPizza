using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class CameraDebug : MonoBehaviour
    {
        [SerializeField] private Slider _povSliderY;
        [SerializeField] private Slider _povSliderZ;
        [SerializeField] private Slider _fovSlider;
        [SerializeField] private Transform _cameraHolder;
        float fovMin = 40;
        float fovMax = 100;
        float fovDefault = 66;

        private float povMinZ = -50;
        private float povMaxZ = 20;
        
        private float povMinY = -10;
        private float povMaxY = 20;
        
        private Vector3 _povDefault;
        private void Awake()
        {
            _povDefault = _cameraHolder.transform.position;
            _povSliderY.onValueChanged.AddListener(OnPovYChanged);
            _povSliderZ.onValueChanged.AddListener(OnPovZChanged);
            _fovSlider.onValueChanged.AddListener(OnFovChanged);
            Reset();
        }

        public void Reset()
        {
            Camera.main.fieldOfView = fovDefault;
            _cameraHolder.position = _povDefault;
            var fov = GetInterpolationResult(fovMin, fovMax, fovDefault);
            _fovSlider.value = fov;
            var povZ = GetInterpolationResult(povMinZ, povMaxZ, _povDefault.z);
            _povSliderZ.value = povZ;
            
            var povY = GetInterpolationResult(povMinY, povMaxY, _povDefault.y);
            _povSliderY.value = povY;


        }

        private void OnFovChanged(float arg0)
        {
            float fov = Mathf.Lerp(fovMin, fovMax, arg0 );
            Camera.main.fieldOfView = fov;

        }

        private void OnPovYChanged(float arg0)
        {
            var pos = Mathf.Lerp (povMinY, povMaxY, arg0 );
            _cameraHolder.position = new Vector3(_cameraHolder.position.x, pos, _cameraHolder.position.z);

        }
        
        private void OnPovZChanged(float arg0)
        {
            var pos = Mathf.Lerp (povMinZ, povMaxZ, arg0 );
            _cameraHolder.position = new Vector3(_cameraHolder.position.x, _cameraHolder.position.y, pos );

        }
        
        private float GetInterpolationResult(float min, float max, float x)
        {
            if (x == min)
            {
                return 0;
            }
            else if (x == max)
            {
                return 1;
            }
            else
            {
                float normalizedX = (float)(x - min) / (max - min);
                return normalizedX;
            }
        }
    }
}
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] public GameObject _flareFx;
        [SerializeField] public GameObject _glowFx;
        public float recoilForce = 10f;  // The force of the recoil
        public float recoilRotation = 10f;  // The rotation applied during recoil
        public float recoilRecoverySpeed = 5f;  // The speed at which the gun recovers from the recoil

        private Vector3 originalPosition;  // The original position of the gun
        private Quaternion originalRotation;  // The original rotation of the gun
        private Vector3 recoilVelocity;  // The current recoil velocity

        [SerializeField] Renderer _flareRendererrend;
        [SerializeField] Renderer _glowRendererrend;

        private Vector2[] flareFxs = { 
            new Vector2(0,0),
            new Vector2(0,0.5f),
            new Vector2(0.5f,0.5f),
            new Vector2(0.5f,0)
        };

        private void Start()
        {
            _glowFx.SetActive(false);
            originalRotation = transform.localRotation;
            originalPosition = transform.localPosition;
        }

        public void StopShoot()
        {
            _isRecoverying = false;
            if (_glowFx == null)
                return;
            _glowFx?.SetActive(false);
            
        }

        public void Recoil()
        {
            _glowFx.SetActive(true);
            CastFlareFx();
            originalRotation = transform.localRotation;
            ApplyRecoil();
            if (_isRecoverying == false)
            {
                Recovery().Forget();
            }
        }

        private bool _isRecoverying = false;

        private async UniTaskVoid Recovery()
        {
            _isRecoverying = true;
            while (_isRecoverying)
            {
                await UniTask.Yield();
                if (this ==null)
                    return;
                transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, recoilRecoverySpeed * Time.deltaTime);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, recoilRecoverySpeed * Time.deltaTime);
            }
        }

        private void ApplyRecoil()
        {
            // Apply recoil force
            // Apply recoil force
            Vector3 recoilDirection = -Vector3.up*0.01f;
            transform.localPosition -= recoilDirection * recoilForce;

            // Apply recoil rotation
            Quaternion recoilRotation = Quaternion.Euler(-this.recoilRotation, 0f, 0f);
            transform.localRotation *= recoilRotation;
        }

        private int _lastIndex = 0;
        private void CastFlareFx()
        {
            _glowFx.transform.Rotate(Vector3.up * 30);
            var index = Random.Range(0, flareFxs.Length);
            if (index == _lastIndex)
            {
                CastFlareFx();
                return;
            }
            _lastIndex = index;
            var randomFlareFx = flareFxs[index];
            _flareRendererrend.material.mainTextureOffset = randomFlareFx;

        }
    }
}
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DefaultNamespace
{
    public class SoundManager : MonoBehaviour
    {
        public AudioSource _bulletPrefab;

        public void CastBullet()
        {
            // CastBulletAsync().Forget();
            UniTask.Create(async () => 
            {
                var bulletSFXObject = _bulletPrefab.gameObject.GetFromPool();
                var audioSource = bulletSFXObject.GetComponent<AudioSource>();
                audioSource.Play();
                audioSource.volume = 1;
                await UniTask.Delay(TimeSpan.FromSeconds(audioSource.clip.length/3f));
                audioSource.volume = 0.6f;
                await UniTask.Delay(TimeSpan.FromSeconds(audioSource.clip.length/3f));
                if (bulletSFXObject.activeSelf)
                {
                    bulletSFXObject.ReturnToPool();
                }
            }).Forget();
        }

        public void TurnSoundOn(bool isOn)
        {
            AudioListener.pause = !isOn;
        }
        
    }
    
}
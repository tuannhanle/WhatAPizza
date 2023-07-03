using System;
using System.ComponentModel;
using Cysharp.Threading.Tasks;
using Unity.BossRoom.Infrastructure;
using UnityEngine;
using VContainer;

namespace DefaultNamespace
{
    public class BulletSpawner : MonoBehaviour
    {
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private Weapon _weapon;
        [SerializeField] private Aim _aim;
        [SerializeField] private float _shootSpeed;
        
        [Inject] private GameManager _gameManager;
        [Inject] private SoundManager _soundManager;
        protected UniTaskTokenHelper _cancelToken = new UniTaskTokenHelper();

        private const int OFFSET = 15;

        private void OnDestroy()
        {
            StopShootRoutine();
        }
        

        private void Awake()
        {

            _bulletPrefab.gameObject.AddToPool(5);
        }

        public async UniTaskVoid ShootRoutine()
        {
            _cancelToken = new UniTaskTokenHelper();
            while (_cancelToken != null)
            {
                if (_gameManager.IsGameDone)
                    return;

                var bullet = Spawn();
                Shoot(bullet);
                _weapon.Recoil();
                await UniTask.Delay(TimeSpan.FromSeconds(_shootSpeed)).AttachExternalCancellation(_cancelToken.Token);
            }
        }

        public void StopShootRoutine()
        {
            _weapon.StopShoot();
            _cancelToken.ClearRunningTasks();
        }

        public void Shoot(Bullet bullet)
        {
            var dir = _aim.ray.direction;
            bullet.Shoot(dir);
            _gameManager.CastBullet();
            _soundManager.CastBullet();
        }

        
        Bullet Spawn()
        {
            var currentIngredient = _gameManager.GetCurrentIngredient();
            var dir = _aim.ray.direction;
            var bulletObject = _bulletPrefab.gameObject.GetFromPool();
            bulletObject.transform.position = _aim.transform.position + dir * OFFSET;
            bulletObject.transform.Rotate(Vector3.up, 30);
            bulletObject.transform.Rotate(Vector3.right, 15);
            var bullet = bulletObject.GetComponent<Bullet>();
            bullet.Recovery(currentIngredient);
            bullet.OnTriggerMultipleObject -= Multiply;
            bullet.OnTriggerMultipleObject += Multiply;
            bullet.OnCollidePizza -= UpdatePoint;
            bullet.OnCollidePizza += UpdatePoint;
            return bullet;

        }

        private void UpdatePoint(string ingredientName)
        {
            _gameManager.UpdatePoint(ingredientName);
        }

        Bullet Spawn(Vector3 pos)
        {
            var bullet = Spawn();
            bullet.transform.position = pos;
            return bullet;
        }

        private void Multiply(Bullet bullet, MultipleObject multipleObject)
        {

            var pos = multipleObject.transform.position;
            var multipleRatio = multipleObject.GetMultipleRatio;
            if (multipleRatio == 0)
            {
                bullet.Return();
                return;
            }
            for (int i = 0; i < multipleRatio; i++)
            {
                var newBullet = Spawn(pos);
                newBullet.Shoot(Vector3.down);
                newBullet.CacheIds = bullet.CacheIds;
            }
        }
    }
}
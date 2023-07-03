using System;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace DefaultNamespace
{

    public class UserInput : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform aim;
        [SerializeField] private Transform _gun;
        [SerializeField] private BulletSpawner _bulletSpawner;

        [Inject] private GameManager _gameManager;
        
        private Vector2 lastTouchPos = Vector2.zero;
        private float height = Screen.height ;
        private float width = Screen.width ;

        private bool _isTouchable = false;
#if !UNITY_EDITOR
        
        void Update()
        {


            // Handle screen touches.
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 pos = touch.position;

                // down
                if (touch.phase == TouchPhase.Began)
                {
                    if (_gameManager.IsGameDone ||_isTouchable == false )
                        return;
                    lastTouchPos = new Vector2(touch.position.x, touch.position.y);
                    _bulletSpawner.ShootRoutine().Forget();
                }
                
                // up
                if (touch.phase == TouchPhase.Ended)
                {
                    _bulletSpawner.StopShootRoutine();
                }
                
                // Drag
                if (touch.phase == TouchPhase.Moved)
                {
                    if (_gameManager.IsGameDone ||_isTouchable == false )
                        return;
                    var curPos = new UnityEngine.Vector2(touch.position.x, touch.position.y);
                    var diff = curPos - lastTouchPos;
                    var copyAimPos = aim.anchoredPosition + diff;
                    if (copyAimPos.x > width || copyAimPos.x < 0)
                    {
                        diff = new UnityEngine.Vector2(0, diff.y);
                    }

                    if ( copyAimPos.y < 0 || copyAimPos.y > height)
                    {

                        diff = new UnityEngine.Vector2(diff.x, 0);
                    }
  
            
                    aim.anchoredPosition += diff;
                    lastTouchPos = curPos;
                    RotateGun();
                }
            }

        }
#endif

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_gameManager.IsGameDone)
                return;
            _isTouchable = true;
#if UNITY_EDITOR
            
            lastTouchPos = new Vector2(eventData.position.x, eventData.position.y);
            _bulletSpawner.ShootRoutine().Forget();   
#endif

        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            _isTouchable = false;

#if UNITY_EDITOR

            _bulletSpawner.StopShootRoutine();
#endif

        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (_gameManager.IsGameDone)
                return;
            _isTouchable = true;

#if UNITY_EDITOR


            var curPos = new UnityEngine.Vector2(eventData.position.x, eventData.position.y);
            var diff = curPos - lastTouchPos;
            var copyAimPos = aim.anchoredPosition + diff;
            if (copyAimPos.x > width || copyAimPos.x < 0)
            {
                diff = new UnityEngine.Vector2(0, diff.y);
            }
        
            if ( copyAimPos.y < 0 || copyAimPos.y > height)
            {
        
                diff = new UnityEngine.Vector2(diff.x, 0);
            }
        
            
            aim.anchoredPosition += diff;
            lastTouchPos = curPos;
            RotateGun();
#endif

        }

        private void RotateGun()
        {
            // Get the mouse position in screen coordinates
            float mouseX = aim.anchoredPosition.x / Screen.width;
            float mouseY =  aim.anchoredPosition.y / Screen.height;

            // Calculate the rotation value based on the X position
            float rotationY = Mathf.Lerp(200f, 320f, mouseX );
            float rotationZ = Mathf.Lerp(-30, 60, mouseY );

            // Apply the rotation to the object
            Quaternion targetRotation = Quaternion.Euler(0f, rotationY, rotationZ);
            _gun.rotation = Quaternion.Inverse(transform.rotation) * targetRotation;
        }
    }
}
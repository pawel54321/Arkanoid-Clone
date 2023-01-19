using UnityEngine;

namespace Cameras
{
    [RequireComponent(typeof(Camera))]
    public class MatchWidthCamera : MonoBehaviour
    {
        #region Fields
        [SerializeField] private float _aspectRatio = 1.8f;
        private Camera _camera;
        #endregion

        #region Methods
        private void Awake()
        {
            _camera = GetComponent<Camera>();
            SetViewport();
        }
        private void SetViewport()
        {
            float windowAspectRatio = Screen.width / (float)Screen.height;
            float scaleHeight = windowAspectRatio / _aspectRatio;

            Rect rect = _camera.rect;

            if (scaleHeight < 1)
            {
                rect.x = 0;
                rect.y = (1 - scaleHeight) / 2;
                rect.width = 1;
                rect.height = scaleHeight;
            }
            else
            {
                float scaleWidth = 1 / scaleHeight;

                rect.x = (1 - scaleWidth) / 2;
                rect.y = 0;
                rect.width = scaleWidth;
                rect.height = 1;
            }

            _camera.rect = rect;
        }
        #endregion
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Collections.Shaders.CircleTransition
{
    public class CircleTransition : MonoBehaviour
    {
        public Transform player;

        private Canvas _canvas;
        private Image _blackScreen;

        private Vector2 _playerCanvasPos;

        // Shader property IDs
        private static readonly int RADIUS = Shader.PropertyToID("_Radius");
        private static readonly int CENTER_X = Shader.PropertyToID("_CenterX");
        private static readonly int CENTER_Y = Shader.PropertyToID("_CenterY");

        // Parameters for transitions
        public float transitionDuration = 2f;
        public float defaultStartRadius = 1f;
        public float defaultEndRadius = 0f;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _blackScreen = GetComponentInChildren<Image>();
        }

        private void Start()
        {
            CloseBlackScreenDefault(); // ERASE THIS LINE AFTER TESTING
            Invoke(nameof(OpenBlackScreenDefault), 2f); // ERASE THIS LINE AFTER TESTING
        }

		public void OpenBlackScreenDefault()
        {
            OpenBlackScreen();
        }

        public void CloseBlackScreenDefault()
        {
            CloseBlackScreen();
        }

        public void OpenBlackScreen(float duration = -1, float startRadius = -1, float endRadius = -1)
        {
            DrawBlackScreen();
            StartCoroutine(Transition(
                duration > 0 ? duration : transitionDuration,
                startRadius > 0 ? startRadius : defaultStartRadius,
                endRadius >= 0 ? endRadius : defaultEndRadius));
        }

        public void CloseBlackScreen(float duration = -1, float startRadius = -1, float endRadius = -1)
        {
            DrawBlackScreen();
            StartCoroutine(Transition(
                duration > 0 ? duration : transitionDuration,
                startRadius > 0 ? startRadius : defaultEndRadius,
                endRadius >= 0 ? endRadius : defaultStartRadius));
        }

        private void DrawBlackScreen()
        {
            var screenWidth = Screen.width;
            var screenHeight = Screen.height;

            var playerScreenPos = Camera.main.WorldToScreenPoint(player.position);

            var canvasRect = _canvas.GetComponent<RectTransform>().rect;
            var canvasWidth = canvasRect.width;
            var canvasHeight = canvasRect.height;

            _playerCanvasPos = new Vector2
            {
                x = (playerScreenPos.x / screenWidth) * canvasWidth,
                y = (playerScreenPos.y / screenHeight) * canvasHeight,
            };

            var squareValue = 0f;
            if (canvasWidth > canvasHeight)
            {
                squareValue = canvasWidth;
                _playerCanvasPos.y += (canvasWidth - canvasHeight) * 0.5f;
            }
            else
            {
                squareValue = canvasHeight;
                _playerCanvasPos.x += (canvasHeight - canvasWidth) * 0.5f;
            }

            _playerCanvasPos /= squareValue;

            var mat = _blackScreen.material;
            mat.SetFloat(CENTER_X, _playerCanvasPos.x);
            mat.SetFloat(CENTER_Y, _playerCanvasPos.y);

            _blackScreen.rectTransform.sizeDelta = new Vector2(squareValue, squareValue);
        }

        private IEnumerator Transition(float duration, float beginRadius, float endRadius)
        {
            var mat = _blackScreen.material;
            var time = 0f;
            while (time <= duration)
            {
                time += Time.deltaTime;
                var t = time / duration;
                var radius = Mathf.Lerp(beginRadius, endRadius, t);

                mat.SetFloat(RADIUS, radius);

                yield return null;
            }
        }
    }
}
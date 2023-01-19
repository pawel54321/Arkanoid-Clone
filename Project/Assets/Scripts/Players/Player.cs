using Core;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Players
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour, IHealthable, IScorable
    {
        #region Fields
        [Header("Settings to Multiplayer"), Space]
        [SerializeField] private InputsUtility.Player _controlling = InputsUtility.Player.Horizontal;
        [SerializeField] private int _playerIndex = 0;
        private string _controllingName;
        [Header("Settings to Movement"), Space]
        [SerializeField] private float _speedToMove = 1;
        private Rigidbody2D _rigidbody2D;
        private float _horizontal = 0;
        [Header("Settings to Hearts"),Space]
        [SerializeField] private int _defaultHeartsValue = 5;
        private int _hearts;
        private int _life = 1;
        [Header("Settings to Score"), Space]
        [SerializeField] private int _scoreValueIncrement = 1;
        private int _defaultScoreValue = 0;
        private int _score;
        [Header("Settings to Ball"), Space]
        [SerializeField] private Transform _ballPosition;
        [SerializeField] private InputsUtility.Player _ballControlling = InputsUtility.Player.Jump;
        private string _ballControllingName;
        [Header("Setting to Bonus Resize")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private CapsuleCollider2D _capsuleCollider2D;
        private float _defaultWidthPaddle;
        private IEnumerator _iEnumerator;
        private GameMode _gameMode;
        private List<Tween> _tweenResize = new();
        #endregion

        #region Properties
        public int PlayerIndex => _playerIndex;
        public int Hearts
        {
            get { return _hearts; }
            set { _hearts = value; }
        }
        public int Life => _life;
        public int ScoreValueIncrement
        {
            get { return _scoreValueIncrement; }
            set { _scoreValueIncrement = value; }
        }
        public int Score {
            get { return _score; }
            set { _score = value; }
        }
        public Transform BallPosition => _ballPosition;
        public string BallControllingName => _ballControllingName;
        #endregion

        #region Methods
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _controllingName = InputsUtility.GetNameEnumerationPlayer(_controlling);
            _ballControllingName = InputsUtility.GetNameEnumerationPlayer(_ballControlling);
            _defaultWidthPaddle = _spriteRenderer.size.x;

            _hearts = _defaultHeartsValue;
            _score = _defaultScoreValue;
        }
        private void Start() => _gameMode = GameMode.GetInstance();
        private void Update()
        {
            #region Inputs
            if (_gameMode.GameplayModePropertie == GameMode.GameplayMode.Playing)
                _horizontal = Input.GetAxisRaw(_controllingName) * _speedToMove;
            #endregion
        }
        private void FixedUpdate()
        {
            if (_gameMode.GameplayModePropertie == GameMode.GameplayMode.Playing)
                Moving();
        }
        private void Moving() => _rigidbody2D.AddForce(new Vector2(_horizontal * Time.fixedDeltaTime, 0), ForceMode2D.Impulse);
        private IEnumerator RecoveryDefaultWidthPaddle(float resizeDuration, float delayRecoveryDefaultWidthPaddle)
        {
            yield return new WaitForSeconds(delayRecoveryDefaultWidthPaddle);

            if (_gameMode.GameplayModePropertie != GameMode.GameplayMode.GameOver)
            {
                _tweenResize.Add(DOTween.To(() => _spriteRenderer.size, x => _spriteRenderer.size = x, new Vector2(_defaultWidthPaddle, _spriteRenderer.size.y), resizeDuration));
                _tweenResize.Add(DOTween.To(() => _capsuleCollider2D.size, x => _capsuleCollider2D.size = x, new Vector2(_defaultWidthPaddle, _capsuleCollider2D.size.y), resizeDuration));
            }
            _iEnumerator = null;
            _tweenResize.Clear();
        }
        public int TakeLives() => _hearts -= _life;
        public int IncrementScore() => _score += _scoreValueIncrement;
        public void Resize(float widthPaddle, float resizeDuration, float delayRecoveryDefaultWidthPaddle)
        {
            if (_iEnumerator != null)
            {
                StopCoroutine(_iEnumerator);
                OffResize();
                _tweenResize.Clear();
            }

            _tweenResize.Add(DOTween.To(() => _spriteRenderer.size, x => _spriteRenderer.size = x, new Vector2(widthPaddle, _spriteRenderer.size.y), resizeDuration));
            _tweenResize.Add(DOTween.To(() => _capsuleCollider2D.size, x => _capsuleCollider2D.size = x, new Vector2(widthPaddle, _capsuleCollider2D.size.y), resizeDuration));

            _iEnumerator = RecoveryDefaultWidthPaddle(resizeDuration, delayRecoveryDefaultWidthPaddle);
            StartCoroutine(_iEnumerator);
        }
        public void SetDefaultParamaters(float defaultPalettePositionX)
        {
            _hearts = _defaultHeartsValue;
            _score = _defaultScoreValue;

            if (_iEnumerator != null)
                StopCoroutine(_iEnumerator);

            OffResize();
            _tweenResize.Clear();

            _spriteRenderer.size = new Vector2(_defaultWidthPaddle, _spriteRenderer.size.y);
            _capsuleCollider2D.size = new Vector2(_defaultWidthPaddle, _capsuleCollider2D.size.y);
            transform.position = new Vector3(defaultPalettePositionX, transform.position.y, transform.position.z);

        }
        public void OffResize()
        {
            for (int i = 0; i < _tweenResize.Count; i++)
                _tweenResize[i].Kill();
        }
        #endregion
    }

}
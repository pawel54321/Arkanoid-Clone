using Blocks;
using Core;
using Core.Managers;
using Players;
using System.Collections;
using UnityEngine;
using Utility;

namespace Balls
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class Ball : MonoBehaviour
    {
        #region Fields
        [Header("Settings to Ball"), Space]
        [SerializeField] private float _speedToBall = 1;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private AudioSource _audioSource;
        private Rigidbody2D _rigidbody2D;
        private bool _jump;
        private bool _isMoving = false;
        private GameMode _gameMode;
        private GameplayManager _gameplayManager;
        private Player _player;
        private IEnumerator _iEnumerator;
        private bool _isIgnoreHits = false;
        private Color _defaultColor;
        #endregion

        #region Properties
        public Color BallColor
        {
            get { return _spriteRenderer.color; }
            set { _spriteRenderer.color = value; }
        }
        public Player Player
        {
            get { return _player; }
            set { _player = value; }
        }
        public Color DefaultColor
        {
            get { return _defaultColor; }
            set { _defaultColor = value; }
        }
        public GameMode GameModePropertie => _gameMode;
        #endregion

        #region Methods
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _audioSource = GetComponent<AudioSource>();
        }
        private void Start()
        {
            _gameplayManager = GameplayManager.GetInstance();
            _gameMode = GameMode.GetInstance();
        }
        private void Update()
        {
            #region Inputs
            if (_gameMode.GameplayModePropertie == GameMode.GameplayMode.Playing)
                _jump = Input.GetButton(_player.BallControllingName);
            #endregion
        }
        private void FixedUpdate()
        {
            if (_gameMode.GameplayModePropertie == GameMode.GameplayMode.Playing)
                StartMoving();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            GameObject objectOfCollision = collision.gameObject;

            if (objectOfCollision.CompareTag(TagsUtility.KillZone))
            {
                _gameplayManager.TakeLives(_player.PlayerIndex);
                ResetBall();
            }
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            GameObject objectOfCollision = collision.contacts[0].collider.gameObject;

            if (objectOfCollision.CompareTag(TagsUtility.Player))
            {
                float x = (transform.position.x - collision.transform.position.x) / collision.collider.bounds.size.x;
                Vector2 direction = new Vector2(x, 1).normalized;
                _rigidbody2D.velocity = direction * _rigidbody2D.velocity.magnitude;
            }
                if (!objectOfCollision.CompareTag(TagsUtility.KillZone))
                _gameplayManager.PlayOneShotSoundEffect(_audioSource, GameplayManager.SoundEffect.Bounce);

            if (objectOfCollision.CompareTag(TagsUtility.Block))
            {
                if (objectOfCollision.TryGetComponent(out Block block))
                {
                    if (!_isIgnoreHits)
                    {
                        if (!block.CollisionWithBlock(this))
                        {
                            _gameplayManager.UpdateBlockHits(objectOfCollision.transform.GetSiblingIndex(), block.BlockHits);
                            return;
                        }

                    }
                    _gameplayManager.IncrementScore(_player.PlayerIndex);
                    _gameplayManager.ShowExplosion(objectOfCollision.transform.position);
                    objectOfCollision.SetActive(false);
                    _gameplayManager.CheckNextLevel(objectOfCollision.transform.GetSiblingIndex());
                }
            }
        }
        private void OnDisable() => SetDefaultParamaters();
        private void StartMoving()
        {
            if (_jump && !_isMoving)
            {
                _rigidbody2D.isKinematic = false;
                transform.SetParent(null);

                _rigidbody2D.AddForce(new Vector2(0, Time.fixedDeltaTime * _speedToBall), ForceMode2D.Impulse);
                _isMoving = true;
            }
        }
        private void SetDefaultParamaters()
        {
            if (_iEnumerator != null)
                StopCoroutine(_iEnumerator);

            DefaultSettingsBall();
        }
        private IEnumerator RecoveryDefaultSettingsBall(float duration)
        {
            yield return new WaitForSeconds(duration);
            DefaultSettingsBall();
        }
        private void DefaultSettingsBall()
        {
            BallColor = _defaultColor;
            _isIgnoreHits = false;
            _iEnumerator = null;
        }
        public void ResetBall()
        {
            _rigidbody2D.isKinematic = true;
            _rigidbody2D.velocity = Vector2.zero;
            transform.SetParent(_player.BallPosition);
            transform.localPosition = Vector2.zero;
            _isMoving = false;
        }
        public void ExtraHitsBall(Color color, float duration)
        {
            if (_iEnumerator != null)
                StopCoroutine(_iEnumerator);

            BallColor = color;
            _isIgnoreHits = true;

            _iEnumerator = RecoveryDefaultSettingsBall(duration);
            StartCoroutine(_iEnumerator);
        }
        #endregion
    }
}
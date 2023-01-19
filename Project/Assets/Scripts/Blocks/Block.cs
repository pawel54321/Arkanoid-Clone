using Balls;
using Levels;
using Levels.Powerups;
using System.Collections.Generic;
using UnityEngine;

namespace Blocks
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Block : MonoBehaviour, IBlock
    {
        #region Fields
        [SerializeField] private Powerup _powerupPrefab;
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        #endregion

        #region Properties
        public int IDTypeBlockData { get; private set; }
        public Color BlockColor
        {
            get { return _spriteRenderer.color; }
            private set { _spriteRenderer.color = value; }
        }
        public int BlockHits { get; private set; }
        public List<BasePowerups> BlockBasePowerups { get; private set; }
        #endregion

        #region Methods
        private void OnEnable()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
        }
        private void GeneratePowerup(Ball ball)
        {
            IsPowerupBallExtraHits(ball);

            if (BlockBasePowerups.Count == 0)
                return;

            Powerup powerup = Instantiate(_powerupPrefab, transform.position, Quaternion.identity);
            powerup.InitializationPowerups(BlockBasePowerups, ball.GameModePropertie);
        }
        private void IsPowerupBallExtraHits(Ball ball)
        {
            for (int i = 0; i < BlockBasePowerups.Count; i++)
            {
                BallExtraHitsPowerupData ballExtraHitsPowerupData = BlockBasePowerups[i] as BallExtraHitsPowerupData;

                if (ballExtraHitsPowerupData)
                {
                    ball.ExtraHitsBall(ballExtraHitsPowerupData.Color, ballExtraHitsPowerupData.Duration);
                    BlockBasePowerups = BlockBasePowerups.FindAll(IsNotBallExtraHitsPowerupData);
                    break;
                }
            }
        }
        private bool IsNotBallExtraHitsPowerupData(BasePowerups basePowerups) => !(basePowerups as BallExtraHitsPowerupData);
        public void InitializationBlock(int idTypeBlockData, Color blockColor, int blockHits, List<BasePowerups> blockBasePowerups)
        {
            IDTypeBlockData = idTypeBlockData;
            BlockColor = blockColor;
            BlockHits = blockHits;
            BlockBasePowerups = blockBasePowerups;

            if (!gameObject.activeInHierarchy)
                return;

            _animator.Rebind();
            _animator.Update(0f);

        }
        public bool CollisionWithBlock(Ball ball)
        {
            BlockHits -= 1;

            if (BlockHits == 0)
            {
                GeneratePowerup(ball);
                return true;
            }

            return false;
        }
        #endregion
    }
}

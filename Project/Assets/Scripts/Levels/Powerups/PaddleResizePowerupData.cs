using UnityEngine;

namespace Levels.Powerups
{
    /// <summary>
    /// Data of a single paddle resize powerup.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(PaddleResizePowerupData), menuName = nameof(Levels) + "/" + nameof(Powerups) + "/" + nameof(PaddleResizePowerupData))]
    public class PaddleResizePowerupData : BasePowerups
    {
        #region Properties
        [field: SerializeField, Range(0.48f, 5.04f)]
        public float WidthPaddle { get; private set; } = 1.04f;

        [field: SerializeField, Range(0.5f, 5)]
        public float ResizeDuration { get; private set; } = 2f;

        [field: SerializeField, Range(5, 100)]
        public float DelayRecoveryDefaultWidthPaddle { get; private set; } = 10;
        #endregion
    }
}
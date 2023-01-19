using UnityEngine;

namespace Levels.Powerups
{
    /// <summary>
    /// Data of a powerup - negates more than 1 hits.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(BallExtraHitsPowerupData), menuName = nameof(Levels) + "/" + nameof(Powerups) + "/" + nameof(BallExtraHitsPowerupData))]
    public class BallExtraHitsPowerupData : BasePowerups
    {
        #region Properties
        [field: SerializeField]
        public Color Color { get; private set; } = Color.black;

        [field: SerializeField, Range(0.5f, 30)]
        public float Duration { get; private set; } = 15f;
        #endregion
    }
}
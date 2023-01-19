using System;
using System.Collections.Generic;
using UnityEngine;

namespace Levels
{
    /// <summary>
    /// Data of a single block.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(BlockData), menuName = nameof(Levels) + "/" + nameof(BlockData))]
    public class BlockData : ScriptableObject, IBlock
    {
        #region Properties
        [field: SerializeField]
        public int IDTypeBlockData { get; private set; }

        [field: SerializeField]
        public Color BlockColor { get; private set; }

        [field: SerializeField, Range(1, 10)]
        public int BlockHits { get; private set; }

        [field: SerializeField]
        public List<BasePowerups> BlockBasePowerups { get; private set; }
        #endregion
    }
}
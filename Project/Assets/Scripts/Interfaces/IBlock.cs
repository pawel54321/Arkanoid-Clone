using System.Collections.Generic;
using UnityEngine;

namespace Levels
{
    /// <summary>
    /// All datas block should use this interface.
    /// </summary>
    public interface IBlock
    {
        int IDTypeBlockData { get; }
        Color BlockColor { get; }
        int BlockHits { get; }
        List<BasePowerups> BlockBasePowerups { get; }
    }
}
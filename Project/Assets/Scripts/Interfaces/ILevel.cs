using System.Collections.Generic;

namespace Levels
{
    public interface ILevel
    {
        List<int> IDTypeBlockData { get; }
        List<bool> ActiveBlockData { get; }
    }
}
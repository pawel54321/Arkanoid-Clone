using UnityEngine;

namespace Levels
{
    /// <summary>
    /// Contains all game elements.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(GameContents), menuName = nameof(Levels) + "/" + nameof(GameContents))]
    public class GameContents : ScriptableObject
    {
        #region Fields
        public BlockData[] blocks;
        #endregion

        #region Methods
        #if UNITY_EDITOR
        [ContextMenu(nameof(LoadAll))]
        public void LoadAll()
        {
            blocks = (BlockData[])Resources.FindObjectsOfTypeAll(typeof(BlockData));
        }
        #endif
        #endregion
    }
}
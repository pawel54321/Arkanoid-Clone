using System.Collections.Generic;

namespace Levels
{
    public class LevelData
    {
        #region Fields
        private List<int> _idTypeBlockData = new(96);
        private List<bool> _activeBlockData = new(96);
        private List<int> _hitsBlockData = new(96);
        #endregion

        #region Properties
        public List<int> IDTypeBlockData
        {
            get { return _idTypeBlockData; }
            set { _idTypeBlockData = value; }
        }
        public List<bool> ActiveBlockData
        {
            get { return _activeBlockData; }
            set { _activeBlockData = value; }
        }
        public List<int> HitsBlockData
        {
            get { return _hitsBlockData; }
            set { _hitsBlockData = value; }
        }
        #endregion
    }
}
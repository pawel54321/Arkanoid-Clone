using System;
using System.Collections.Generic;

namespace Core
{
    [Serializable]
    public class SaveLevelsData
    {
        #region Fields
        private List<int> _idTypeSaveBlockData = new(96);
        private List<bool> _activeSaveBlockData = new(96);
        private List<int> _hitsSaveBlockData = new(96);
        #endregion

        #region Properties
        public List<int> IDTypeSaveBlockData
        {
            get { return _idTypeSaveBlockData; }
            set { _idTypeSaveBlockData = value; }
        }
        public List<bool> ActiveSaveBlockData
        {
            get { return _activeSaveBlockData; }
            set { _activeSaveBlockData = value; }
        }
        public List<int> HitsSaveBlockData
        {
            get { return _hitsSaveBlockData; }
            set { _hitsSaveBlockData = value; }
        }
        #endregion
    }
}
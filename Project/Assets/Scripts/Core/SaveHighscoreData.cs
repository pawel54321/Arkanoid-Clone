using System;

namespace Core
{
    [Serializable]
    public class SaveHighscoreData
    {
        #region Fields
        private int _highscore = 0;
        #endregion

        #region Properties
        public int Highscore
        {
            get { return _highscore; }
            set { _highscore = value; }
        }
        #endregion
    }
}
using Players;
using System;
using System.Collections.Generic;
using static Core.GameMode;

namespace Core
{
    [Serializable]
    public class SaveGameData
    {
        #region Fields
        private List<int> _score = new(2);
        private List<int> _hearts = new(2);
        private GeneralMode _generalMode;
        //Levels
        private int _currentLevel;
        private List<SaveLevelsData> _saveLevelsData = new(3);
        #endregion

        #region Properties
        public List<int> Score
        {
            get { return _score; }
            private set { _score = value; }
        }
        public List<int> Hearts
        {
            get { return _hearts; }
            private set { _hearts = value; }
        }
        public GeneralMode GeneralMode
        {
            get { return _generalMode; }
            set { _generalMode = value; }
        }
        public int CurrentLevel
        {
            get { return _currentLevel; }
            set { _currentLevel = value; }
        }
        public List<SaveLevelsData> SaveLevelsDataPropertie
        {
            get { return _saveLevelsData; }
            set { _saveLevelsData = value; }
        }
        #endregion

        #region Methods
        public void SetScore(Player[] players)
        {
            _score.Clear();

            for (int i = 0; i < players.Length; i++)
                _score.Add(players[i].Score);
        }
        public void SetHearts(Player[] players)
        {
            _hearts.Clear();

            for (int i = 0; i < players.Length; i++)
                _hearts.Add(players[i].Hearts);
        }
        #endregion
    }
}
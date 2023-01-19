using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Utility;

namespace Core.Managers
{
    public class DataManager : Singleton<DataManager>
    {
        #region Fields
        private string[] _path = new string[2];
        private SaveGameData _saveGameData;
        private SaveHighscoreData _saveHighscoreData;
        #endregion

        #region Properties
        public SaveGameData SaveGameData
        {
            get { return _saveGameData; }
            set { _saveGameData = value; }
        }
        public SaveHighscoreData SaveHighscoreData
        {
            get { return _saveHighscoreData; }
            set { _saveHighscoreData = value; }
        }
        #endregion

        #region Enumerations
        public enum TypeOfSave
        {
            Game = 0,
            Highscore = 1
        }
        #endregion

        #region Methods
        protected override void Awake()
        {
            base.Awake();
            _path[0] = $"{Application.persistentDataPath}/Game.dat";
            _path[1] = $"{Application.persistentDataPath}/Highscore.dat";
            _saveGameData = new SaveGameData();
            _saveHighscoreData = new SaveHighscoreData();
            LoadAll(TypeOfSave.Highscore);
        }
        public void SaveAll(TypeOfSave typeOfSave)
        {
            int type = (int)typeOfSave;

            BinaryFormatter bf = new BinaryFormatter();
            using FileStream fs = new(_path[type], FileMode.Create);
            {
                if (type == 0)
                    bf.Serialize(fs, _saveGameData);
                else if (type == 1)
                    bf.Serialize(fs, _saveHighscoreData);
            }
        }
        public void LoadAll(TypeOfSave typeOfSave)
        {
            int type = (int)typeOfSave;

            if (IsFileExists(typeOfSave))
            {
                BinaryFormatter bf = new BinaryFormatter();
                using FileStream fs = new(_path[type], FileMode.Open);
                {
                    if (type == 0)
                        _saveGameData = (SaveGameData)bf.Deserialize(fs);
                    else if (type == 1)
                        _saveHighscoreData = (SaveHighscoreData)bf.Deserialize(fs);
                }
            }
        }
        public bool IsFileExists(TypeOfSave typeOfSave)
        {
            int type = (int)typeOfSave;

            return File.Exists(_path[type]);
        }
        public void FileDelete(TypeOfSave typeOfSave)
        {
            int type = (int)typeOfSave;

            File.Delete(_path[type]);
        }
        #endregion
    }
}
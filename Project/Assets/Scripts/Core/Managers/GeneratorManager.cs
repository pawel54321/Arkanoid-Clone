using Blocks;
using Levels;
using Levels.Powerups;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Utility;

namespace Core.Managers
{
    public class GeneratorManager : Singleton<GeneratorManager>
    {
        #region Fields
        [Header("Settings to Level"), Space]
        [SerializeField] private int _numberOfLevels = 3;
        [SerializeField] private float _delayToDisableStage = 2;
        private int _currentLevel = 0;
        [Header("Settings to Block"), Space]
        [SerializeField] private GameObject _blockPrefab;
        [SerializeField] private GameContents _gameContents;
        [SerializeField] private Transform _blocksSpawnContainer;
        [SerializeField, Range(4, 96)] private int _maxNumberOfBlocks = 96;
        [SerializeField, Range(2, 12)] private int _offsetOfBlocks = 12; //Lenght of lines
        [SerializeField] private float _initX = -7;
        [SerializeField] private float _initY = 3;
        [SerializeField] private int _minNumberOfRandomShowBlocks = 1;
        private float _offsetX;
        private float _offsetY;
        private List<Block> _blocks = new(96);
        private List<LevelData> sessionLevelsData = new(3);
        private List<LevelData> runtimeLevelsData = new(3);
        private int _destroyedBlocks = 0;
        private int _numberOfActiveBlocks;
        private DataManager _dataManager;
        [Header("UI"), Space]
        [SerializeField] private GameUI _gameUI;
        #endregion

        #region Properties
        public int MaxNumberOfBlocks
        {
            get { return _maxNumberOfBlocks; }
            set { _maxNumberOfBlocks = value; }
        }
        #endregion

        #region Methods
        protected override void Awake()
        {
            base.Awake();
            GenerateBlocks();

            for (int i = 0; i < _numberOfLevels; i++)
                InitializationLevels(i);
        }

        private void Start() => _dataManager = DataManager.GetInstance();
        private void GenerateBlocks()
        {
            float x = _initX;
            float y = _initY;

            if (_blockPrefab.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                _offsetX = spriteRenderer.sprite.bounds.size.x;
                _offsetY = spriteRenderer.sprite.bounds.size.y;
            }

            for (int i = 1; i <= _maxNumberOfBlocks; i++)
            {
                GameObject blockClone = Instantiate(_blockPrefab, _blocksSpawnContainer);
                blockClone.SetActive(false);
                blockClone.transform.position = new Vector3(x, y, 0);
                x += _offsetX;

                if (i % _offsetOfBlocks == 0)
                {
                    y -= _offsetY;
                    x = _initX;
                }

                Block block = blockClone.GetComponent<Block>();
                _blocks.Add(block);
            }
        }
        private void InitializationLevels(int level)
        {
            //Show Default Block With Random Symetrics

            int lenghtLineHorizontal = _maxNumberOfBlocks / _offsetOfBlocks;
            int lenghtLineVertical = _offsetOfBlocks;

            int lineHorizontal = lenghtLineHorizontal / 2;
            int lineVertical = lenghtLineVertical / 2;

            int maxNumberOfRandomShowBlocks = lineHorizontal * lineVertical;

            if (_minNumberOfRandomShowBlocks >= maxNumberOfRandomShowBlocks + 1)
            {
                _minNumberOfRandomShowBlocks = 1;
                Debug.LogError($"The minimum value cannot be greater than or equal to the maximum value! Default value set to 1!");
            }

            int numberOfRandomShowBlocks = Random.Range(_minNumberOfRandomShowBlocks, maxNumberOfRandomShowBlocks + 1);

            LevelData levelData = new();

            for (int i = 0; i < _maxNumberOfBlocks; i++)
            {
                levelData.IDTypeBlockData.Add(0);
                levelData.ActiveBlockData.Add(false);
                levelData.HitsBlockData.Add(1);
            }

            List<(int, int)> numbers = new();

            for (int i = 0; i < numberOfRandomShowBlocks; i++)
            {
                int randomLineY = Random.Range(0, lineHorizontal);
                int randomLineX = Random.Range(0, lineVertical);

                if (numbers.Contains((randomLineY, randomLineX)))
                {
                    i--;
                    continue;
                }

                numbers.Add((randomLineY, randomLineX));

                int firstIndexUpperLeft = randomLineY * lenghtLineVertical + randomLineX;
                int secondIndexUpperRight = randomLineY * lenghtLineVertical + (lenghtLineVertical - 1) - randomLineX;

                int thirdIndexBottomLeft = (lenghtLineHorizontal - randomLineY - 1) * lenghtLineVertical + randomLineX;
                int fourthIndexBottomRight = (lenghtLineHorizontal - randomLineY - 1) * lenghtLineVertical + (lenghtLineVertical - 1) - randomLineX;

                levelData.ActiveBlockData[firstIndexUpperLeft] = true;
                levelData.ActiveBlockData[secondIndexUpperRight] = true;
                levelData.ActiveBlockData[thirdIndexBottomLeft] = true;
                levelData.ActiveBlockData[fourthIndexBottomRight] = true;
            }

            sessionLevelsData.Add(levelData);

            //Show and Initialization Unique Blocks With Randoms

            int numberOfRandomUniqueBlocks = Random.Range(0, (maxNumberOfRandomShowBlocks + 1) - numberOfRandomShowBlocks);

            List<int> indexHiddenBlocks = new();

            for (int i = 0; i < sessionLevelsData[level].ActiveBlockData.Count; i++)
            {
                if (!sessionLevelsData[level].ActiveBlockData[i])
                    indexHiddenBlocks.Add(i);
            }

            for (int i = 0; i < numberOfRandomUniqueBlocks; i++)
            {
                int indexOfRandomUniqueBlock = Random.Range(1, _gameContents.blocks.Length);

                BlockData blockData = _gameContents.blocks[indexOfRandomUniqueBlock];
                int indexOfRandomHiddenBlock = Random.Range(0, indexHiddenBlocks.Count);

                int index = indexHiddenBlocks[indexOfRandomHiddenBlock];

                sessionLevelsData[level].IDTypeBlockData[index] = blockData.IDTypeBlockData;
                sessionLevelsData[level].ActiveBlockData[index] = true;
                sessionLevelsData[level].HitsBlockData[index] = blockData.BlockHits;
            }

        }
        private void InitializationBlocks()
        {
            BlockData[] blocks = _gameContents.blocks;

            for (int i = 0; i < _blocks.Count; i++)
            {
                int indexTypeOfBlock = runtimeLevelsData[_currentLevel].IDTypeBlockData[i];

                _blocks[i].gameObject.SetActive(runtimeLevelsData[_currentLevel].ActiveBlockData[i]);
                _blocks[i].InitializationBlock(blocks[indexTypeOfBlock].IDTypeBlockData, blocks[indexTypeOfBlock].BlockColor, runtimeLevelsData[_currentLevel].HitsBlockData[i], blocks[indexTypeOfBlock].BlockBasePowerups);

                if (_blocks[i].gameObject.activeInHierarchy)
                    _numberOfActiveBlocks++;
            }
        }
        private void ReadLoadedLevels()
        {
            runtimeLevelsData.Clear();
            _currentLevel = _dataManager.SaveGameData.CurrentLevel;

            for (int i = 0; i < _numberOfLevels; i++)
            {
                LevelData levelsData = new();

                for (int j = 0; j < _maxNumberOfBlocks; j++)
                {
                    levelsData.IDTypeBlockData.Add(_dataManager.SaveGameData.SaveLevelsDataPropertie[i].IDTypeSaveBlockData[j]);
                    levelsData.ActiveBlockData.Add(_dataManager.SaveGameData.SaveLevelsDataPropertie[i].ActiveSaveBlockData[j]);
                    levelsData.HitsBlockData.Add(_dataManager.SaveGameData.SaveLevelsDataPropertie[i].HitsSaveBlockData[j]);
                }

                runtimeLevelsData.Add(levelsData);
            }
        }
        public void PrepareSaveLevels()
        {
            _dataManager.SaveGameData.SaveLevelsDataPropertie.Clear();
            _dataManager.SaveGameData.CurrentLevel = _currentLevel;

            for (int i = 0; i < _numberOfLevels; i++)
            {
                SaveLevelsData saveLevelsData = new();
                saveLevelsData.IDTypeSaveBlockData.AddRange(runtimeLevelsData[i].IDTypeBlockData);
                saveLevelsData.ActiveSaveBlockData.AddRange(runtimeLevelsData[i].ActiveBlockData);
                saveLevelsData.HitsSaveBlockData.AddRange(runtimeLevelsData[i].HitsBlockData);
                _dataManager.SaveGameData.SaveLevelsDataPropertie.Add(saveLevelsData);
            }
        }
        public void StartNewGame() 
        {
            _currentLevel = 0;
            runtimeLevelsData.Clear();

            for (int i = 0; i < _numberOfLevels; i++)
            {
                LevelData levelData = new();
                levelData.IDTypeBlockData.AddRange(sessionLevelsData[i].IDTypeBlockData);
                levelData.ActiveBlockData.AddRange(sessionLevelsData[i].ActiveBlockData);
                levelData.HitsBlockData.AddRange(sessionLevelsData[i].HitsBlockData);
                runtimeLevelsData.Add(levelData);
            }

            _destroyedBlocks = 0;
            _numberOfActiveBlocks = 0;

            _gameUI.ShowStage(_delayToDisableStage, (_currentLevel + 1));
            InitializationBlocks();
        }
        public void ContinueGame()
        {
            ReadLoadedLevels();

            _destroyedBlocks = 0;
            _numberOfActiveBlocks = 0;
            InitializationBlocks();
            _gameUI.ShowStage(_delayToDisableStage, (_currentLevel + 1));
        }
        public bool AreDestroyedAllBlocks(int indexBlock)
        {
            runtimeLevelsData[_currentLevel].ActiveBlockData[indexBlock] = false;
            _destroyedBlocks++;

            if (_destroyedBlocks < _numberOfActiveBlocks)
                return false;

            _destroyedBlocks = 0;
            _numberOfActiveBlocks = 0;
            return true;
        }
        public bool IsLoadNextLevel() 
        {
            if ((_currentLevel + 1) >= _numberOfLevels) 
                return false;
    
            _currentLevel++;

            _gameUI.ShowStage(_delayToDisableStage, (_currentLevel + 1));
            InitializationBlocks();
            return true;
        }
        public void UpdateBlockHits(int indexBlock, int blockHits) => runtimeLevelsData[_currentLevel].HitsBlockData[indexBlock] = blockHits;
        #endregion

    }
}
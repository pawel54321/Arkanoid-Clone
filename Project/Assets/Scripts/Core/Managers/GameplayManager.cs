using Balls;
using Players;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Utility;
using static Core.GameMode;
using static Core.Managers.MusicManager;

namespace Core.Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class GameplayManager : Singleton<GameplayManager>
    {
        #region Fields
        [Header("Settings Mulitplayer"), Space]
        [SerializeField] private Ball _ballPrefab;
        [SerializeField] private Color[] _ballColor = new Color[2];
        [SerializeField] private Player[] _players = new Player[2];
        [SerializeField] private float[] _defaultPalettePositionX = new float[2];
        private List<Ball> _ballsList = new();
        [Header("UI"), Space]
        [SerializeField] private GameUI _gameUI;
        [SerializeField] private MainMenuUI _mainMenuUI;
        [Header("Block Explosion"), Space]
        [SerializeField] private ParticleSystem _particleSystemForBlockExplosion;
        [Header("Effects"), Space]
        [SerializeField] private AudioClip[] _effects;
        private AudioSource _audioSource;
        private GameMode _gameMode;
        private DataManager _dataManager;
        private GeneratorManager _generatorManager;
        private MusicManager _musicManager;
        #endregion

        #region Enumerations
        public enum SoundEffect
        {
            Bounce = 0,
            Deadth = 1,
            ExplosionBlock = 2
        }
        #endregion

        #region Methods
        private void OnEnable()
        {
            GameUI.backToMainMenu += DisableBalls;
            GameUI.backToMainMenu += SetDefaultMusic;
            GameUI.backToGame += PauseOrContinueMusic;
        }
        protected override void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();

            _gameUI.SaveButton.onClick.AddListener(() => OnClickSave());
            _mainMenuUI.NewGameButton.onClick.AddListener(() => {
                _gameMode.ChangeGeneralMode(GeneralMode.Singleplayer);
                InitializationPlayers();
                InitializationDefaultParameters();
                InitializationUI();
            });
            _mainMenuUI.ContinueButton.onClick.AddListener(() => {
                OnClickContinueRead();
                InitializationPlayers();
                InitializationUI();
            });
            _mainMenuUI.MultiplayerButton.onClick.AddListener(() => {
                _gameMode.ChangeGeneralMode(GeneralMode.Multiplayer);
                InitializationPlayers();
                InitializationDefaultParameters();
                InitializationUI();
            });
        }
        private void Start()
        {
            _gameMode = GameMode.GetInstance();
            _dataManager = DataManager.GetInstance();
            _generatorManager = GeneratorManager.GetInstance();
            _musicManager = MusicManager.GetInstance();
            InitializationBalls();
        }
        private void Update()
        {
            #region Inputs
            if (Input.GetButtonDown(InputsUtility.Cancel) && _gameMode.GameplayModePropertie == GameplayMode.Playing)
            {
                _gameUI.ShowPause();
                _musicManager.PauseOrContinueMusic();
            }
            #endregion
        }
        private void OnDisable()
        {
            GameUI.backToMainMenu -= DisableBalls;
            GameUI.backToMainMenu -= SetDefaultMusic;
            GameUI.backToGame -= PauseOrContinueMusic;
        }
        private void SetDefaultMusic()
        {
            if (!_musicManager.IsPlayingSameMusic(Music.General))
                _musicManager.PlayMusic(Music.General);
        }
        private void InitializationDefaultParameters()
        {
            if (_dataManager.IsFileExists(DataManager.TypeOfSave.Game))
                _dataManager.FileDelete(DataManager.TypeOfSave.Game);
  
            for (int i = 0; i < _players.Length; i++)
                _players[i].SetDefaultParamaters(_defaultPalettePositionX[i]);

            _generatorManager.StartNewGame();
        }
        private void InitializationPlayers()
        {
            if (_gameMode.GeneralModePropertie == GeneralMode.Singleplayer)
            {
                for (int i = 1; i < _players.Length; i++)
                    _players[i].gameObject.SetActive(false);
                for (int i = 0; i < 1; i++)
                    _ballsList[i].gameObject.SetActive(true);
                _gameUI.VisiblePlayersUI(false);
            }
            else if (_gameMode.GeneralModePropertie == GeneralMode.Multiplayer)
            {
                for (int i = 1; i < _players.Length; i++)
                    _players[i].gameObject.SetActive(true);
                for (int i = 0; i < _players.Length; i++)
                    _ballsList[i].gameObject.SetActive(true);
                _gameUI.VisiblePlayersUI(true);
            }
        }
        private void InitializationBalls()
        {
            for (int i = 0; i < _players.Length; i++)
            {
                Ball ball = Instantiate(_ballPrefab, _players[i].BallPosition);
                ball.name = $"{ball.name}{(i + 1)}";
                ball.DefaultColor = ball.BallColor = _ballColor[i];
                ball.Player = _players[i];
                ball.gameObject.SetActive(false);
                _ballsList.Add(ball);
            }
        }
        private void InitializationUI()
        {
            for (int i = 0; i < _players.Length; i++)
            {
                _gameUI.ShowScore(i, _players[i].Score);
                _gameUI.ShowHearts(i, _players[i].Hearts);
                _gameUI.ShowHighscore(_dataManager.SaveHighscoreData.Highscore);
            }
        }
        private void OnClickContinueRead()
        {
            _dataManager.LoadAll(DataManager.TypeOfSave.Game);

            for (int i = 0; i < _players.Length; i++)
            {
                _players[i].Score = _dataManager.SaveGameData.Score[i];
                _players[i].Hearts = _dataManager.SaveGameData.Hearts[i];
            }

            _gameMode.ChangeGeneralMode(_dataManager.SaveGameData.GeneralMode);
            _generatorManager.ContinueGame();
        }
        private void DisableBalls()
        {
            for (int i = 0; i < _players.Length; i++)
            {
                if (_ballsList[i].gameObject.activeInHierarchy)
                    _ballsList[i].ResetBall();
                _ballsList[i].gameObject.SetActive(false);
            }
        }
        private int CalculateSummaryScore()
        {
            int summaryScore = 0;

            for (int i = 0; i < _players.Length; i++)
                summaryScore += _players[i].Score;

            return summaryScore;
        }
        public void GameOver(bool isWin = false)
        {
            DisableBalls();
            _gameMode.ChangeGameplayMode(GameMode.GameplayMode.GameOver);

            for (int i = 0; i < _players.Length; i++)
                _players[i].OffResize();

            bool isHighest;
            int calculateSummaryScore = CalculateSummaryScore();
            int highscore = _dataManager.SaveHighscoreData.Highscore;

            if (calculateSummaryScore > highscore)
            {
                isHighest = true;
                _dataManager.SaveHighscoreData.Highscore = calculateSummaryScore;
                _dataManager.SaveAll(DataManager.TypeOfSave.Highscore);
                _gameUI.ShowHighscore(calculateSummaryScore);
            }
            else
                isHighest = false;

            if (isWin)
                _musicManager.PlayMusic(Music.Victory);

            _gameUI.ShowGameOver(calculateSummaryScore, isHighest, isWin);
        }
        public void OnClickSave()
        {
            _dataManager.SaveGameData.SetScore(_players);
            _dataManager.SaveGameData.SetHearts(_players);
            _dataManager.SaveGameData.GeneralMode = _gameMode.GeneralModePropertie;

            _generatorManager.PrepareSaveLevels();

            _dataManager.SaveAll(DataManager.TypeOfSave.Game);
        }
        public void IncrementScore(int playerIndex)
        {
            int score = _players[playerIndex].IncrementScore();
            _gameUI.ShowScore(playerIndex, score);
        }
        public void ShowExplosion(Vector3 blockPosition)
        {
             PlayOneShotSoundEffect(_audioSource, SoundEffect.ExplosionBlock);
            _particleSystemForBlockExplosion.transform.position = blockPosition;
            _particleSystemForBlockExplosion.Stop();
            _particleSystemForBlockExplosion.Play();
        }
        public void TakeLives(int playerIndex)
        {
            PlayOneShotSoundEffect(_audioSource, SoundEffect.Deadth);
            int hearts = _players[playerIndex].TakeLives();
            _gameUI.ShowHearts(playerIndex, hearts);

            if (hearts == 0)
                GameOver();
        }
        public void CheckNextLevel(int indexBlock)
        {
            if (_generatorManager.AreDestroyedAllBlocks(indexBlock))
            {
                for (int i = 0; i < _players.Length; i++)
                {
                    if (_ballsList[i].gameObject.activeInHierarchy)
                        _ballsList[i].ResetBall();
                }

                if (!_generatorManager.IsLoadNextLevel())
                    GameOver(true);
            }
        }
        public void UpdateBlockHits(int indexBlock, int blockHits) => _generatorManager.UpdateBlockHits(indexBlock, blockHits);
        public void PlayOneShotSoundEffect(AudioSource audioSource, SoundEffect soundEffect) => audioSource.PlayOneShot(_effects[(int)soundEffect]);
        public void PauseOrContinueMusic() => _musicManager.PauseOrContinueMusic();
        #endregion
    }
}
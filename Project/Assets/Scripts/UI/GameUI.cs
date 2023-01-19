using Core;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Core.GameMode;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        #region Actions
        public static Action backToMainMenu;
        public static Action backToGame;
        #endregion

        #region Fields
        [Header("Gameplay"), Space]
        [SerializeField] private GameObject _gameplay;
        [Header("Level"), Space]
        [SerializeField, TextArea] private string _stageHeader = "Stage\n{0}";
        [SerializeField] private TextMeshProUGUI _stageText;
        private IEnumerator _iEnumeratorStage;
        [Header("Multiplayer"), Space]
        [SerializeField] private GameObject[] _playersUI;
        [Header("Score"), Space]
        [SerializeField] private string _scoreHeader = "Score";
        [SerializeField] private TextMeshProUGUI[] _scoreText;
        [Header("Hearts"), Space]
        [SerializeField] private string _heartsHeader = "Hearts left";
        [SerializeField] private TextMeshProUGUI[] _heartsText;
        [Header("Highscore"), Space]
        [SerializeField, TextArea] private string _highscoreHeader = "Highscore\n{0}";
        [SerializeField] private TextMeshProUGUI _highscoreText;
        [Header("Pause"), Space]
        [SerializeField] private GameObject _pause;
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private MainMenuUI _mainMenuCanvas;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _quitButton;
        private GameMode _gameMode;
        [Header("Game Over"), Space] //Summary/Win
        [SerializeField] private GameObject _gameOver;
        [SerializeField, TextArea] private string _yourScoreHeader = "Your score\n{0}\n\nis\n{1} than highscore";
        [SerializeField] private TextMeshProUGUI _yourScoreText;
        [SerializeField] private Button _quitGameOverButton;
        [SerializeField] private string[] _yourScoreHigherHeader = new string[2] { "higher", "lower" };
        [SerializeField] private TextMeshProUGUI _summaryText;
        [SerializeField] private string[] _summaryWinHeader = new string[2] { "You Win", "Summary"};
        [Header("Alert"), Space]
        [SerializeField] private GameObject _alert;
        [SerializeField] private TextMeshProUGUI _informationText;
        [SerializeField] private string _information = "Saved";
        [SerializeField] private float _delayToDisableAlert = 0.5f;
        private IEnumerator _iEnumeratorAlert;
        #endregion

        #region Properties
        public Button SaveButton
        {
            get { return _saveButton; }
            set { _saveButton = value; }
        }
        #endregion

        #region Methods
        private void Awake()
        {
            _backButton.onClick.AddListener(() => OnClickClosePause());
            _saveButton.onClick.AddListener(() => OnClickSaveAlert());
            _quitButton.onClick.AddListener(() => OnClickQuit());
            _quitGameOverButton.onClick.AddListener(() => OnClickQuitGameOver());
        }
        private void Start() => _gameMode = GameMode.GetInstance();
        private void OnClickClosePause()
        {
            _pause.SetActive(false);
            _gameMode.ChangeGameplayMode(GameplayMode.Playing);
            Time.timeScale = 1; //Remove if we don't need it, because we also have a flag
            backToGame.Invoke();
        }
        private void OnClickSaveAlert()
        {
            _saveButton.interactable = false;
            ShowAlert(_delayToDisableAlert, _information);
        }
        private void OnClickQuit()
        {
            OnClickClosePause();
            BackToMainMenu();
        }
        private void OnClickQuitGameOver()
        {
            _gameOver.SetActive(false);
            BackToMainMenu();
        }
        private void BackToMainMenu()
        {
            _mainMenu.SetActive(true);
            _mainMenuCanvas.enabled = true;
            _gameMode.ChangeGameplayMode(GameplayMode.MainMenu);
            _gameplay.SetActive(false);
            CloseStage();
            backToMainMenu?.Invoke();
        }
        private void ShowAlert(float delayToDisable, string content)
        {
            if (_iEnumeratorAlert != null)
                StopCoroutine(_iEnumeratorAlert);

            _iEnumeratorAlert = ShowAlertIEnumerator(delayToDisable, content);
            StartCoroutine(_iEnumeratorAlert);
        }
        private IEnumerator ShowAlertIEnumerator(float delayToDisable, string content)
        {
            _alert.SetActive(true);
            _informationText.text = $"{content}";
            yield return new WaitForSecondsRealtime(delayToDisable);
            _alert.SetActive(false);
            _iEnumeratorAlert = null;
        }
        private IEnumerator ShowStageIEnumerator(float delayToDisable, int level)
        {
            _stageText.gameObject.SetActive(true);
            _stageText.text = string.Format(_stageHeader, level);
            yield return new WaitForSecondsRealtime(delayToDisable);
            _stageText.gameObject.SetActive(false);
            _iEnumeratorStage = null;
        }
        private void CloseStage()
        {
            if (_iEnumeratorStage != null)
            {
                StopCoroutine(_iEnumeratorStage);
                _stageText.gameObject.SetActive(false);
                _iEnumeratorStage = null;
            }
        }
        public void ShowPause()
        {
            _pause.SetActive(true);
            _gameMode.ChangeGameplayMode(GameplayMode.Pause);
            _saveButton.interactable = true;
            Time.timeScale = 0; //Remove if we don't need it, because we also have a flag
        }
        public void ShowScore(int playerIndex, int score) => _scoreText[playerIndex].text = $"{_scoreHeader}: {score}";
        public void ShowHearts(int playerIndex, int hearts) => _heartsText[playerIndex].text = $"{_heartsHeader}: {hearts}";
        public void ShowHighscore(int highscore) => _highscoreText.text = string.Format(_highscoreHeader, highscore);
        public void ShowGameOver(int summaryScore, bool isHighest, bool isWin)
        {
            _gameOver.SetActive(true);
            _yourScoreText.text = string.Format(_yourScoreHeader, summaryScore, isHighest ? _yourScoreHigherHeader[0] : _yourScoreHigherHeader[1]);
            _summaryText.text = isWin ? _summaryWinHeader[0] : _summaryWinHeader[1];
        }
        public void VisiblePlayersUI(bool isVisible)
        {
            for (int i = 1; i < _playersUI.Length; i++)
                _playersUI[i].SetActive(isVisible);
        }
        public void ShowStage(float delayToDisable, int level)
        {
            if (_iEnumeratorStage != null)
                StopCoroutine(_iEnumeratorStage);

            _iEnumeratorStage = ShowStageIEnumerator(delayToDisable, level);
            StartCoroutine(_iEnumeratorStage);
        }
        #endregion
    }
}
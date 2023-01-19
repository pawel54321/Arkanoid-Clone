using Core;
using Core.Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Core.GameMode;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        #region Fields
        [Header("Main Menu"), Space]
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _multiplayerButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameObject _gameplay;
        private GameMode _gameMode;
        private DataManager _dataManager;
        #endregion

        #region Properties
        public Button NewGameButton
        {
            get { return _newGameButton; }
            set { _newGameButton = value; }
        }
        public Button ContinueButton
        {
            get { return _continueButton; }
            set { _continueButton = value; }
        }
        public Button MultiplayerButton
        {
            get { return _multiplayerButton; }
            set { _multiplayerButton = value; }
        }
        #endregion

        #region Methods
        private void Start() => _gameMode = GameMode.GetInstance();
        private void OnEnable() => StartCoroutine(InitializationContinue());
        private void Awake()
        {
            _newGameButton.onClick.AddListener(() => OnClickNewGame());
            _multiplayerButton.onClick.AddListener(() => OnClickMultiplayer());
            _continueButton.onClick.AddListener(() => OnClickContinue());
            _exitButton.onClick.AddListener(() => OnClickExit());
        }
        private void InitializationUIGameplay()
        {
            _gameMode.ChangeGameplayMode(GameplayMode.Playing);
            _mainMenu.SetActive(false);
            _gameplay.SetActive(true);
            this.enabled = false;
        }
        private IEnumerator InitializationContinue()
        {
            while (_dataManager == null)
            {
                _dataManager = DataManager.GetInstance();
                yield return new WaitForSeconds(0.01f);
            }

            if (_dataManager.IsFileExists(DataManager.TypeOfSave.Game))
                _continueButton.interactable = true;
            else
                _continueButton.interactable = false;
        }
        private void OnClickNewGame() => InitializationUIGameplay();
        private void OnClickMultiplayer() => InitializationUIGameplay();
        private void OnClickContinue() => InitializationUIGameplay();
        private void OnClickExit() => Application.Quit();
        #endregion
    }
}
using Utility;

namespace Core
{
    public class GameMode : Singleton<GameMode>
    {
        #region Fields
        private GameplayMode _gameplayMode = GameplayMode.MainMenu;
        private GeneralMode _generalMode = GeneralMode.Singleplayer;
        #endregion

        #region Properties
        public GameplayMode GameplayModePropertie => _gameplayMode;
        public GeneralMode GeneralModePropertie => _generalMode;
        #endregion

        #region Enumerations
        public enum GameplayMode
        {
            MainMenu,
            Playing,
            Pause,
            GameOver
        }
        public enum GeneralMode
        {
            Singleplayer,
            Multiplayer
        }
        #endregion

        #region Methods
        protected override void Awake() => base.Awake();
        public void ChangeGameplayMode(GameplayMode gameplayMode) => _gameplayMode = gameplayMode;
        public void ChangeGeneralMode(GeneralMode generalMode) => _generalMode = generalMode;
        #endregion
    }
}
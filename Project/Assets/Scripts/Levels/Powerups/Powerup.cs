using Core;
using Players;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Levels.Powerups
{
    public class Powerup : MonoBehaviour
    {
        #region Fields
        private GameMode _gameMode;
        private List<BasePowerups> _basePowerups;
        #endregion

        #region Methods
        private void Update()
        {
            if(_gameMode.GameplayModePropertie != GameMode.GameplayMode.Playing && _gameMode.GameplayModePropertie != GameMode.GameplayMode.Pause)
                Destroy(gameObject);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            GameObject objectOfCollision = collision.gameObject;

            if (objectOfCollision.CompareTag(TagsUtility.KillZone))
            {
                Destroy(gameObject);
            }

        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            GameObject objectOfCollision = collision.contacts[0].collider.gameObject;

            if (objectOfCollision.CompareTag(TagsUtility.Player))
            {
                Player player = objectOfCollision.GetComponentInParent<Player>();

                if (player == null)
                    return;

                for (int i = 0; i < _basePowerups.Count; i++)
                {
                    PaddleResizePowerupData paddleResizePowerupData = _basePowerups[i] as PaddleResizePowerupData;

                    if (paddleResizePowerupData)
                    {
                        player.Resize(paddleResizePowerupData.WidthPaddle, paddleResizePowerupData.ResizeDuration, paddleResizePowerupData.DelayRecoveryDefaultWidthPaddle);
                        break;
                    }
                }

                gameObject.SetActive(false);

            }
        }
        public void InitializationPowerups(List<BasePowerups> basePowerups, GameMode gameMode)
        {
            _basePowerups = basePowerups;
            _gameMode = gameMode;
        }
        #endregion
    }
}
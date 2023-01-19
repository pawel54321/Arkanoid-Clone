using System;
using UnityEngine;

namespace Utility
{
    public class InputsUtility : MonoBehaviour
    {   
        //Easy to use without mistackes
        #region Field
        public const string Cancel = "Cancel";
        #endregion

        #region Enumeration
        public enum Player
        {
            Horizontal,
            Mouse_X,
            Jump,
            Fire2
        }
        #endregion

        #region Method
        public static string GetNameEnumerationPlayer(Player _controller) => Enum.GetName(typeof(Player), _controller).Replace('_', ' ');
        #endregion
    }
}
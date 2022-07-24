using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.UI.Windows
{
    public class SaveLoadWindow : UIWindow
    {
        [SerializeField] private TMPro.TMP_InputField saveNameInputField;

        public void Save()
        {
            SaveStates.SaveStateManager.Save( saveNameInputField.text );
        }

        public void Load()
        {
            SaveStates.SaveStateManager.Load( saveNameInputField.text );
        }
    }
}
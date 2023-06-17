using RPGGame.Progression.Dialogues;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityPlus.AssetManagement;

namespace RPGGame.UI
{
    public class DialogueUI : MonoBehaviour
    {
        public Dialogue Dialogue { get; set; }

        [SerializeField] RectTransform spokenTextContainer;
        [SerializeField] RectTransform selectTextContainer;

        [SerializeField] GameObject dialogueContentHeaderPrefab;
        [SerializeField] GameObject dialogueContentPrefab;
        [SerializeField] GameObject dialogueSelectOptionPrefab;

        List<GameObject> spokenList = new List<GameObject>();
        List<GameObject> selectList = new List<GameObject>();

        public void ClearSpoken()
        {
            foreach( var spoken in spokenList )
            {
                Destroy( spoken );
            }

            spokenList.Clear();
        }

        public void ClearSelect()
        {
            foreach( var select in selectList )
            {
                Destroy( select );
            }

            selectList.Clear();
        }

        public void AddSelect( List<DialogueOption> options )
        {
            foreach( var option in options )
            {
                GameObject go = SpawnOption( option );
                selectList.Add( go );
            }
        }

        public void AddSpoken( DialogueOption option )
        {
            (GameObject header, GameObject content) = SpawnSpoken( option );
            spokenList.Add( header );
            spokenList.Add( content );
        }

        private (GameObject header, GameObject content) SpawnSpoken( DialogueOption option )
        {
            GameObject headerGameObject = Instantiate( dialogueContentHeaderPrefab, spokenTextContainer );

            TMPro.TextMeshProUGUI headerText = headerGameObject.GetComponent<TMPro.TextMeshProUGUI>();

            headerText.text = option.Speaker.DisplayName;

            GameObject contentGameObject = Instantiate( dialogueContentPrefab, spokenTextContainer );

            TMPro.TextMeshProUGUI contentText = contentGameObject.GetComponent<TMPro.TextMeshProUGUI>();

            contentText.text = option.Text;

            return (headerGameObject, contentGameObject);
        }

        private GameObject SpawnOption( DialogueOption option )
        {
            GameObject optionGameObject = Instantiate( dialogueSelectOptionPrefab, selectTextContainer );

            DialogueOptionUI opt = optionGameObject.GetComponent<DialogueOptionUI>();

            opt.Init( option, this );

            return optionGameObject;
        }

        public static DialogueUI Create()
        {
            GameObject dialogueWindow = Instantiate( AssetRegistry.Get<GameObject>( "Prefabs/UI/dialogue_window" ), Main.UIWindowCanvas.transform );

            return dialogueWindow.GetComponent<DialogueUI>();
        }
    }
}
using RPGGame.Progression.Dialogues;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Editor
{
    /// <summary>
    /// This is gonna hold the dialogues serialized in the editor.
    /// Alternatively we could also use the entire game scene, as it is a burner scene and is loaded at runtime later.
    /// </summary>
    public class DialogueWrapper : MonoBehaviour
    {
        [SerializeField]
        public List<Dialogue> dialogue = new List<Dialogue>() { new Dialogue() };
    }
}
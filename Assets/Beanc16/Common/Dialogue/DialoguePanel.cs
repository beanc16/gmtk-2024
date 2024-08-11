using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using Beanc16.Common.UI;

namespace Beanc16.Common.Dialogue
{
    [System.Serializable]
    public class LineStartEvent : UnityEvent<string> { }

    [System.Serializable]
    public class LineCompleteEvent : UnityEvent<string> { }



    [RequireComponent(typeof (OnClick))]
    public class DialoguePanel : MonoBehaviour
    {
        [SerializeField]
        private DialogueScriptableObject dialogue;
        [SerializeField, Range(0f, 3f)]
        private float textSpeed = 0.75f;
        [SerializeField]
        private DialogueStartTimeEnum whenToStartDialogue = DialogueStartTimeEnum.ON_START;
        private TextMeshProUGUI textComponent;

        private int curLineIndex = 0;

        public UnityEvent OnDialogueStart;
        public UnityEvent OnDialogueComplete;
        public LineStartEvent OnLineStart;
        public LineCompleteEvent OnLineComplete;

        private float TextSpeed
        {
            // get { return (this.textSpeed * 10) * Time.deltaTime; }
            get { return this.textSpeed; }
        }

        private string CurrentDialogueLine
        {
            get { return this.dialogue.lines[this.curLineIndex]; }
        }

        private bool DoneTypingCurrentLine
        {
            get
            {
                return this.textComponent.text == this.CurrentDialogueLine;
            }
        }



        private void Awake()
        {
            this.textComponent = this.GetComponentInChildren<TextMeshProUGUI>();

            OnClick onClickComponent = this.GetComponent<OnClick>();
            onClickComponent.OnClickStart.AddListener(AdvanceDialogue);

            if (this.dialogue == null)
            {
                Debug.LogError("Dialogue is not set in " + this.gameObject.name);
            }
        }

        private void Start()
        {
            this.ClearDisplayText();

            if (this.whenToStartDialogue == DialogueStartTimeEnum.ON_START)
            {
                this.StartDialogue();
            }
        }

        private void AdvanceDialogue(PointerEventData eventData)
        {
            if (this.DoneTypingCurrentLine)
            {
                this.NextLine();
                this.TryCallLineStartEvent();
            }

            else
            {
                StopAllCoroutines();
                this.textComponent.text = this.CurrentDialogueLine;
                this.TryCallLineCompleteEvent();
            }
        }



        public void StartDialogue()
        {
            this.curLineIndex = 0;
            this.TryCallDialogueStartEvent();
            StartCoroutine(TypeLine());
        }

        private IEnumerator TypeLine()
        {
            // Type each character one by one
            foreach (char c in this.CurrentDialogueLine.ToCharArray())
            {
                this.textComponent.text += c;
                yield return new WaitForSeconds(this.TextSpeed);
            }
        }

        private void NextLine()
        {
            if (this.curLineIndex < this.dialogue.lines.Count - 1)
            {
                this.curLineIndex++;
                this.ClearDisplayText();
                StartCoroutine(TypeLine());
            }
            else
            {
                this.gameObject.SetActive(false);
                this.TryCallDialogueCompleteEvent();
            }
        }

        private void ClearDisplayText()
        {
            this.textComponent.text = string.Empty;
        }



        private void TryCallDialogueStartEvent()
        {
            if (OnDialogueStart != null)
            {
                OnDialogueStart.Invoke();
            }
        }

        private void TryCallDialogueCompleteEvent()
        {
            if (OnDialogueComplete != null)
            {
                OnDialogueComplete.Invoke();
            }
        }

        private void TryCallLineStartEvent()
        {
            if (OnLineStart != null)
            {
                OnLineStart.Invoke(this.CurrentDialogueLine);
            }
        }

        private void TryCallLineCompleteEvent()
        {
            if (OnLineComplete != null)
            {
                OnLineComplete.Invoke(this.CurrentDialogueLine);
            }
        }
    }

    public enum DialogueStartTimeEnum
    {
        MANUAL,
        ON_START,
    }
}

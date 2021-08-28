using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HurricaneVR.Framework.Core.Utils;

public class DialogManager : MonoBehaviour {
    private Animator animator;
    private int animHashIsOpen;

    private Coroutine typingCoroutine;
    private Dialog activeDialog;
    private int currentSentenceIndex;

    public DialogEventChannel dialogEventChannel;
    public TextMeshProUGUI dialogText, speakerNameText;


    [Tooltip("Necessary for waiting for the dialog window to animate open")]
    [Range(0f, 2f)]
    public float initialDialogDelay;
    [Range(0f, 0.2f)]
    public float defaultDialogSpeed;
    private float timeSinceLastCharTyped;

    public Vector2 charTypedPitchRange;

    private void Awake() {
        animator = GetComponent<Animator>();
        animHashIsOpen = Animator.StringToHash("isOpen");
    }

    private void OnEnable() {
        dialogEventChannel.onDialogNext += DisplayNextSentence;
        dialogEventChannel.onDialogSkip += SkipCurrentSentence;
    }
    private void OnDisable() {
        dialogEventChannel.onDialogNext -= DisplayNextSentence;
        dialogEventChannel.onDialogSkip -= SkipCurrentSentence;
    }

    // --------------------------------------------------------------------------------
    // -- Interface --
    // --------------------------------------------------------------------------------
    public void StartDialog(Dialog dialog) {
        // If we're already in dialog, do nothing
        if (activeDialog) return;
        if (dialog.isComplete) return;

        animator.SetBool(animHashIsOpen, true);
        activeDialog = dialog;
        activeDialog.onDialogStart.Invoke();
        currentSentenceIndex = 0;

        dialogEventChannel.RaiseOnDialogStarted();
        StartTypingSentence();
    }

    public void EndDialogEarly() {
        EndDialog(false);
    }
    private void EndDialog(bool wasDialogFullyCompleted) {
        if (!activeDialog) return;
        if (activeDialog.isComplete) {
            return;
        }

        dialogText.text = "";
        speakerNameText.text = "";
        animator.SetBool(animHashIsOpen, false);

        // TODO: Send the message downstream to event channels
        if (wasDialogFullyCompleted) {
            activeDialog.isComplete = true;
            activeDialog.onDialogCompleted.Invoke();
        } else {
            activeDialog.onDialogCanceled.Invoke();
        }

        activeDialog = null;
        dialogEventChannel.RaiseOnDialogFinished();
    }

    public void DisplayNextSentence() {
        // If we're in the middle of typing a sentence, this function will just skip to the end of the sentence.
        if (typingCoroutine != null) {
            SkipCurrentSentence();
            return;
        }

        currentSentenceIndex++;
        StartTypingSentence();
    }


    public void SkipCurrentSentence() {
        if (typingCoroutine != null) {
            StopCoroutine(typingCoroutine);

            // TODO: This doesn't account for the case where the player skips dialog halfway through, but there was already some un-erased text before it
            Sentence currentSentence = activeDialog.dialogContents.sentences[currentSentenceIndex];
            dialogText.text = currentSentence.text;
            SFXPlayer.Instance.PlaySFXRandomPitch(GetCharTypedAudioClip(currentSentence), 
                transform.position, 
                charTypedPitchRange.x, 
                charTypedPitchRange.y);

            typingCoroutine = null;
        }
    }

    // --------------------------------------------------------------------------------
    // -- Internal --
    // --------------------------------------------------------------------------------
    /// <summary>
    /// Complete the dialog if there's no more sentences, or start typing the new sentence
    /// </summary>
    private void StartTypingSentence() {
        if (!activeDialog) {
            return;
        }

        if (activeDialog.dialogContents.sentences.Count == 0) {
            EndDialog(true);
            return;
        }

        if (currentSentenceIndex >= activeDialog.dialogContents.sentences.Count) {
            EndDialog(true);
            return;
        }

        typingCoroutine = StartCoroutine(TypeSentence());
    }

    private IEnumerator TypeSentence() {
        Sentence currentSentence = activeDialog.dialogContents.sentences[currentSentenceIndex];
        ConfigureTextbox(currentSentence);

        if (currentSentenceIndex == 0) {
            yield return new WaitForSeconds(initialDialogDelay);
        }


        foreach (char letter in currentSentence.text.ToCharArray()) {
            timeSinceLastCharTyped = 0f;

            // Wait some delay before typing the character
            do {
                timeSinceLastCharTyped += Time.deltaTime;
                yield return null;
            } while (GetDialogSpeed(currentSentence)> timeSinceLastCharTyped);

            // Type a letter, and play some SFX
            dialogText.text += letter;
            if (letter != ' ') {
                SFXPlayer.Instance.PlaySFXRandomPitch(GetCharTypedAudioClip(currentSentence), 
                    transform.position, 
                    charTypedPitchRange.x, 
                    charTypedPitchRange.y);
            }

            yield return null;
        }

        typingCoroutine = null;
    }


    // --------------------------------------------------------------------------------
    // -- Config --
    // --------------------------------------------------------------------------------
    private void ConfigureTextbox(Sentence s) {
        speakerNameText.text = s.speaker.speakerName;

        if (s.clearExistingSentence) {
            dialogText.text = "";
        }
    }
    private float GetDialogSpeed(Sentence s) {
        if (s.dialogSpeedOverride == 0f) return defaultDialogSpeed;
        return s.dialogSpeedOverride;
    }

    private AudioClip GetCharTypedAudioClip(Sentence s) {
        if (s.charTypedSFXOverride == null) return s.speaker.letterTypedSFX;
        return s.charTypedSFXOverride;
    }
}

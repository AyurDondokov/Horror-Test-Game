using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Settings")]
    [SerializeField] private bool isFadeIn = true;
    [SerializeField] private bool isFadeOut = true;
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float afterLineDelay = 0.75f;
    [SerializeField] private bool isWaitPlayerClick = false;

    [Header("Typing Sound")]
    [SerializeField] private AudioClip typingSound;
    [SerializeField, Range(0f, 1f)] private float typingVolume = 0.4f;
    [SerializeField, Range(1, 5)] private int lettersPerSound = 2;

    private Dialogue currentDialogue;
    private int currentLineIndex;
    private Coroutine typingCoroutine;
    private bool isPlaying;
    public bool IsPlaying() => isPlaying;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    public void PlayDialogue(Dialogue dialogue)
    {
        if (isPlaying) return;

        currentDialogue = dialogue;
        currentLineIndex = 0;
        gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }
    public void SetFadeIn(bool fadein) {  isFadeIn = fadein; }
    public void SetFadeOut(bool fadeout) {  isFadeOut = fadeout; }
    public void Next()
    {
        if (!isPlaying) return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentDialogue.lines[currentLineIndex].text;
            typingCoroutine = null;
            return;
        }

        currentLineIndex++;

        if (currentLineIndex < currentDialogue.lines.Length)
            ShowLine(currentDialogue.lines[currentLineIndex]);
        else
            StartCoroutine(FadeOut());
    }

    private void ShowLine(DialogueLine line)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    private IEnumerator TypeLine(DialogueLine line)
    {
        dialogueText.text = "";
        string fullText = line.text;
        int letterCounter = 0;

        foreach (char c in fullText)
        {
            dialogueText.text += c;
            letterCounter++;

            if (typingSound && letterCounter % lettersPerSound == 0)
                AudioManager.Instance.PlaySoundGlobal(typingSound, typingVolume);

            yield return new WaitForSeconds(line.typingSpeed);
        }

        typingCoroutine = null;

        if (!isWaitPlayerClick)
        {
            yield return new WaitForSeconds(afterLineDelay);
            Next();
        }
    }

    private IEnumerator FadeIn()
    {
        isPlaying = true;
        dialogueText.text = "";
        float t = 0f;
        if (isFadeIn)
        {
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
                yield return null;
            }
        }
        canvasGroup.alpha = 1;
        ShowLine(currentDialogue.lines[currentLineIndex]);
    }

    private IEnumerator FadeOut()
    {
        float t = 0f;

        if (isFadeOut)
        {
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
                yield return null;
            }
        }
        canvasGroup.alpha = 0;
        isPlaying = false;
        gameObject.SetActive(false);
    }
}

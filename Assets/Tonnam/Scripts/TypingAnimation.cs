using System;
using TMPro;

[Serializable]
public class TypingAnimation
{
    public bool IsComplete { get; private set; }
    public event Action OnComplete;

    private TMP_Text dialogueText;

    private int counter;
    private int totalChars;
    private float secondsPerCharacter;
    private float typingTimer;

    public TypingAnimation(TMP_Text dialogueText, float charPerSecond)
    {
        if (dialogueText == null)
        {
            throw new ArgumentNullException(nameof(dialogueText));
        }

        dialogueText.ForceMeshUpdate();

        this.dialogueText = dialogueText;
        this.counter = 0;
        this.totalChars = dialogueText.textInfo.characterCount;
        this.secondsPerCharacter = 1f / charPerSecond;
    }

    public void Tick(float deltaTime)
    {
        if (IsComplete) return;

        if (typingTimer > 0f)
        {
            typingTimer -= deltaTime;
            return;
        }
        typingTimer = secondsPerCharacter;

        int visible = counter % totalChars;
        dialogueText.maxVisibleCharacters = visible;
        counter++;

        if (counter >= totalChars && !IsComplete)
        {
            OnTypingComplete();
        }
    }

    public void Skip()
    {
        if (IsComplete) return;

        OnTypingComplete();
    }

    private void OnTypingComplete()
    {
        dialogueText.maxVisibleCharacters = int.MaxValue;

        IsComplete = true;
        OnComplete?.Invoke();
    }
}

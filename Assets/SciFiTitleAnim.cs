using UnityEngine;
using TMPro;

public class SciFiTitleAnim : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public float revealSpeed = 0.05f;
    public AudioSource typingAudioSource;
    public AudioClip typingClip;

    private string fullText;

    void Awake()
    {
        fullText = titleText.text;
        titleText.text = ""; // Clear before Start runs
    }

    void Start()
    {
        StartCoroutine(RevealTitle());
    }

    System.Collections.IEnumerator RevealTitle()
    {
        foreach (char c in fullText)
        {
            titleText.text += c;

            if (typingAudioSource && typingClip)
            {
                
                typingAudioSource.PlayOneShot(typingClip);
            }

            yield return new WaitForSeconds(revealSpeed);
        }

        StartCoroutine(EndFlicker());
    }

    System.Collections.IEnumerator EndFlicker()
    {
        for (int i = 0; i < 3; i++)
        {
            titleText.alpha = 0.2f;
            yield return new WaitForSeconds(0.1f);
            titleText.alpha = 1f;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
using System.Collections;
using UnityEngine;

// Goes on a character that talks in the world. Holds their voice and spawns the text.
public class DialogueSpeaker : MonoBehaviour
{
    public WorldDialogueText textPrefab;
    [Tooltip("Where the text spawns, e.g. an empty above the head.")]
    public Transform textAnchor;

    [Header("Voice")]
    public AudioSource audioSource;
    [Tooltip("One is picked at random for each blip.")]
    public AudioClip[] voiceClips;
    [Tooltip("Play a blip every this many letters.")]
    public int lettersPerSound = 2;
    public float minPitch = 0.9f;
    public float maxPitch = 1.15f;

    public void PlayVoice()
    {
        if (voiceClips.Length == 0) return;
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(voiceClips[Random.Range(0, voiceClips.Length)]);
    }

    // Use inside a sequence: yield return speaker.Say("Hello!", "Lovely night.");
    // W skips the typing, then W again moves to the next line.
    public IEnumerator Say(params string[] lines)
    {
        Transform anchor = textAnchor != null ? textAnchor : transform;

        foreach (string line in lines)
        {
            WorldDialogueText bubble = Instantiate(textPrefab, anchor.position, Quaternion.identity, anchor);
            yield return bubble.Play(line, this);
            yield return PlayerInteractor.WaitForInteract();
            yield return bubble.Close();
        }
    }
}

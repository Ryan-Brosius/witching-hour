using System.Collections;
using TMPro;
using UnityEngine;

// The juicy in-world text. Lives on a prefab with a TextMeshPro (3D, not UI) component.
// Letters pop in one at a time, wobble, and the whole thing punches on each voice blip.
// Supports <link=shake>words</link> and <link=wave>words</link>, plus normal TMP tags like <color>.
public class WorldDialogueText : MonoBehaviour
{
    public TMP_Text text;
    [Tooltip("Optional little arrow that bobs once the line is done.")]
    public Transform continueIcon;
    public float iconBob = 0.1f;

    [Header("Typing")]
    public float charsPerSecond = 35f;
    [Tooltip("Extra pause after . , ! ? so it reads like speech.")]
    public float punctuationPause = 0.12f;

    [Header("Letter Pop (distances are in letter heights)")]
    public float popTime = 0.2f;
    public float popDrop = 0.6f;
    public float popSpin = 25f;

    [Header("Idle Wobble")]
    public float wobbleHeight = 0.04f;
    public float wobbleSpeed = 5f;

    [Header("Whole Text")]
    public float openTime = 0.25f;
    public float closeTime = 0.15f;
    [Tooltip("Small swell on each voice blip.")]
    public float blipPunch = 0.06f;

    [Header("Link Effects")]
    public float shakeAmount = 0.08f;
    public float waveHeight = 0.3f;

    Transform cam;
    Vector3 iconStart;
    TMP_MeshInfo[] baseMesh;
    float[] revealTimes; // when each letter appeared, -1 = not yet
    int[] effects;       // 0 none, 1 shake, 2 wave
    float openStart;
    float closeStart = -1f;
    float punch;
    bool done;

    void Awake()
    {
        cam = Camera.main.transform;
        if (continueIcon != null)
        {
            iconStart = continueIcon.localPosition;
            continueIcon.gameObject.SetActive(false);
        }
    }

    public IEnumerator Play(string line, DialogueSpeaker speaker)
    {
        text.text = line;
        text.ForceMeshUpdate();
        TMP_TextInfo info = text.textInfo;
        baseMesh = info.CopyMeshInfoVertexData();

        int count = info.characterCount;
        revealTimes = new float[count];
        effects = new int[count];
        for (int i = 0; i < count; i++) revealTimes[i] = -1f;

        // Mark which letters are inside <link=shake> / <link=wave>.
        for (int l = 0; l < info.linkCount; l++)
        {
            TMP_LinkInfo link = info.linkInfo[l];
            int fx = link.GetLinkID() == "shake" ? 1 : link.GetLinkID() == "wave" ? 2 : 0;
            for (int c = 0; c < link.linkTextLength; c++)
                effects[link.linkTextfirstCharacterIndex + c] = fx;
        }

        openStart = Time.time;

        // Type it out. W skips to the end.
        int index = 0;
        int letters = 0;
        float wait = 0f;
        while (index < count)
        {
            yield return null;
            if (PlayerInteractor.InteractDown()) break;

            wait -= Time.deltaTime;
            while (wait <= 0f && index < count)
            {
                TMP_CharacterInfo c = info.characterInfo[index];
                revealTimes[index] = Time.time;

                if (c.isVisible && letters++ % speaker.lettersPerSound == 0)
                {
                    speaker.PlayVoice();
                    punch = 1f;
                }

                wait += 1f / charsPerSecond;
                if (".,!?".IndexOf(c.character) >= 0) wait += punctuationPause;
                index++;
            }
        }

        // If skipped, drop the rest in as a super quick cascade.
        for (int i = index; i < count; i++) revealTimes[i] = Time.time + (i - index) * 0.008f;

        done = true;
        if (continueIcon != null) continueIcon.gameObject.SetActive(true);
    }

    public IEnumerator Close()
    {
        closeStart = Time.time;
        if (continueIcon != null) continueIcon.gameObject.SetActive(false);
        yield return new WaitForSeconds(closeTime);
        Destroy(gameObject);
    }

    void LateUpdate()
    {
        transform.rotation = cam.rotation; // always face the camera
        if (revealTimes == null) return;

        float now = Time.time;
        punch = Mathf.MoveTowards(punch, 0f, Time.deltaTime * 8f);
        float whole = WholeScale(now) * (1f + punch * blipPunch);

        TMP_TextInfo info = text.textInfo;
        for (int i = 0; i < revealTimes.Length; i++)
        {
            TMP_CharacterInfo c = info.characterInfo[i];
            if (!c.isVisible) continue;

            int m = c.materialReferenceIndex;
            int v = c.vertexIndex;
            Vector3[] src = baseMesh[m].vertices;
            Vector3[] dst = info.meshInfo[m].vertices;

            Vector3 mid = (src[v] + src[v + 2]) * 0.5f;
            float h = src[v + 1].y - src[v].y; // letter height, so effects scale with font size

            // 0 -> 1 as the letter pops in
            float t = revealTimes[i] < 0f ? 0f : Mathf.Clamp01((now - revealTimes[i]) / popTime);
            float scale = EaseOutBack(t);

            Vector3 offset = Vector3.up * ((1f - t) * popDrop * h);
            offset.y += Mathf.Sin(now * wobbleSpeed + i * 0.5f) * wobbleHeight * h;
            if (effects[i] == 1) offset += (Vector3)Random.insideUnitCircle * shakeAmount * h;
            if (effects[i] == 2) offset.y += Mathf.Sin(now * 10f + i * 0.7f) * waveHeight * h;

            Quaternion spin = Quaternion.Euler(0f, 0f, (1f - t) * popSpin);
            for (int j = 0; j < 4; j++)
                dst[v + j] = (mid + spin * (src[v + j] - mid) * scale + offset) * whole;
        }
        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);

        if (done && continueIcon != null)
            continueIcon.localPosition = iconStart + Vector3.up * Mathf.Abs(Mathf.Sin(now * 6f)) * iconBob;
    }

    // Pops open with a little overshoot, shrinks away on close.
    float WholeScale(float now)
    {
        if (closeStart >= 0f)
        {
            float c = Mathf.Clamp01((now - closeStart) / closeTime);
            return 1f - c * c;
        }
        return EaseOutBack(Mathf.Clamp01((now - openStart) / openTime));
    }

    static float EaseOutBack(float t)
    {
        const float s = 1.70158f;
        t -= 1f;
        return 1f + (s + 1f) * t * t * t + s * t * t;
    }
}

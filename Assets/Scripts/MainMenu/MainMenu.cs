using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public ClockfaceHandsUI clockface;
    public CameraFollow cam;
    public string gameScene = "GameScene";

    [Header("Intro")]
    [Tooltip("Empty high above the tower. The camera starts here.")]
    public Transform introPoint;
    [Tooltip("Empty where the camera rests on the menu.")]
    public Transform menuPoint;
    [Tooltip("Follow smoothing during the float down. Higher = slower.")]
    public float introSmoothTime = 2.5f;
    public float introDuration = 4f;
    public CanvasGroup buttons;
    public float buttonFadeTime = 1f;

    [Header("Minute Tick")]
    public AudioSource bellSound;
    [Header("Minute Tick")]
    public float shakeTime = 0.2f;
    public float shakeStrength = 0.2f;

    [Header("Play")]
    public float playSmoothTime = 1f;
    [Tooltip("Full screen black Image with a CanvasGroup.")]
    public CanvasGroup fade;
    public float spinTime = 2.5f;
    public float holdTime = 0.6f;
    public float fadeTime = 1f;

    int lastMinute = -1;
    bool leaving;
    float normalSmoothTime;

    IEnumerator Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        fade.alpha = 0f;
        buttons.alpha = 0f;
        buttons.interactable = false;

        normalSmoothTime = cam.smoothTime;
        cam.offset = Vector3.zero;
        cam.target = introPoint;
        cam.transform.position = introPoint.position;
        yield return null;

        cam.smoothTime = introSmoothTime;
        cam.target = menuPoint;
        yield return new WaitForSeconds(introDuration);
        if (leaving) yield break;

        cam.smoothTime = normalSmoothTime;
        yield return FadeGroup(buttons, 1f, buttonFadeTime);
        buttons.interactable = true;
    }

    void Update()
    {
        if (leaving) return;

        DateTime now = DateTime.Now;
        if (now.Minute == lastMinute) return;

        bool firstTime = lastMinute < 0;
        lastMinute = now.Minute;
        clockface.SetTime(now.Hour, now.Minute);
        if (firstTime) return;

        cam.Shake(shakeTime, shakeStrength);
        if (bellSound != null) bellSound.Play();
    }

    public void Play()
    {
        if (!leaving)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            StartCoroutine(StartGame());
        }
    }

    public void Quit() => Application.Quit();

    IEnumerator StartGame()
    {
        leaving = true;
        buttons.interactable = false;
        StartCoroutine(FadeGroup(buttons, 0f, 0.3f));

        cam.smoothTime = playSmoothTime;

        float start = clockface.hour * 60f + clockface.minute;
        float end = 2 * 60 + 50;
        while (end < start + 360f) end += 720f;

        for (float t = 0f; t < spinTime; t += Time.deltaTime)
        {
            SetTotalMinutes(Mathf.Lerp(start, end, Mathf.SmoothStep(0f, 1f, t / spinTime)));
            yield return null;
        }
        SetTotalMinutes(end);

        cam.Shake(shakeTime * 2f, shakeStrength * 2f);
        yield return new WaitForSeconds(holdTime);

        yield return FadeGroup(fade, 1f, fadeTime);
        SceneManager.LoadScene(gameScene);
    }

    void SetTotalMinutes(float total)
    {
        clockface.SetTime((int)(total / 60f) % 24, total % 60f);
    }

    IEnumerator FadeGroup(CanvasGroup group, float to, float time)
    {
        float from = group.alpha;
        for (float t = 0f; t < time; t += Time.deltaTime)
        {
            group.alpha = Mathf.Lerp(from, to, t / time);
            yield return null;
        }
        group.alpha = to;
    }
}
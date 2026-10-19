using TMPro;
using UnityEngine;

public class WinTimeText : MonoBehaviour
{
    public TMP_Text text;
    [Tooltip("{0} = minutes, {1} = seconds.")]
    public string format = "You beat the game in {0} minutes and {1} seconds";

    public void SetTime(float seconds)
    {
        int total = Mathf.FloorToInt(seconds);
        text.text = string.Format(format, total / 60, total % 60);
    }
}

using TMPro;
using UnityEngine;

public class GameClock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText = null;
    [SerializeField] private TextMeshProUGUI weekText = null;
    [SerializeField] private TextMeshProUGUI seasonText = null;
    [SerializeField] private TextMeshProUGUI yearText = null;

    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += UpdateGameTime;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= UpdateGameTime;
    }

    private void UpdateGameTime(int gameYear, Season gameSeason, int gameDay, Week gameDayOfWeek, int gameHour,
        int gameMinute, int gameSecond)
    {
        //保证在游戏中只显示10分钟的倍数，如果是17的话，显示为10
        gameMinute = gameMinute - (gameMinute % 10);

        string ampm = "";
        string minute;

        if (gameHour >= 12)
        {
            ampm = "下午";
        }
        else
        {
            ampm = "上午";
        }

        if (gameHour >= 13)
        {
            gameHour -= 12;
        }

        if (gameMinute < 10)
        {
            minute = "0" + gameMinute.ToString();
        }
        else
        {
            minute = gameMinute.ToString();
        }

        string time = gameHour.ToString() + ":" + minute + " " + ampm;
        
        
        timeText.SetText(time);
        weekText.SetText(gameDayOfWeek.ToString());
        seasonText.SetText(gameSeason.ToString());
        yearText.SetText("第" + gameYear + "年");
    }
    
}
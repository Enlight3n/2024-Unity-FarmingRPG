using UnityEngine;
using System;
using System.Collections.Generic;
using System.Data.Common;


public class TimeManager : SingletonMonoBehaviour<TimeManager>, ISavable
{
    private int gameYear = 1;
    private Season gameSeason = Season.春;
    private int gameDay = 1;
    private int gameHour = 6;
    private int gameMinute = 30;
    private int gameSecond = 0;
    private Week gameDayOfWeek = Week.周一;
    
    public bool gameClockPaused = false;
    private float gameTick = 0f;

    
    private void Start()
    {
        EventHandler.CallAdvanceGameMinuteEvent(
            gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute,
            gameSecond);
    }

    private void Update()
    {
        if (!gameClockPaused)
        {
            GameTick();
        } 
    }
    
    
    private void OnEnable()
    {
        IRegister();
        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent += AfterSceneLoadFadeIn;
    }

    private void OnDisable()
    {
        IDeregister();
        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoadFadeIn;
    }

    private void BeforeSceneUnloadFadeOut()
    {
        gameClockPaused = true;
    }

    private void AfterSceneLoadFadeIn()
    {
        gameClockPaused = false;
    }
    
    
    private void GameTick()
    {
        gameTick += Time.deltaTime;

        if (gameTick >= Settings.SecondsPerGameSecond)
        {
            gameTick -= Settings.SecondsPerGameSecond;

            UpdateGameSecond();
        }
    }

    private void UpdateGameSecond()
    {
        gameSecond++;

        if (gameSecond > 59)
        {
            gameSecond = 0;
            gameMinute++;

            if (gameMinute > 59)
            {
                gameMinute = 0;
                gameHour++;
                if (gameHour > 23)
                {
                    gameHour = 0;
                    gameDay++;
                    if (gameDay > 30)
                    {
                        gameDay = 1;

                        //无异于gameSeason++
                        int gs = (int)gameSeason;
                        gs++;
                        gameSeason = (Season)gs;

                        if (gs > 3)
                        {
                            gs = 0;
                            gameSeason = (Season)gs;
                            
                            gameYear++;

                            if (gameYear > 9999)
                                gameYear = 1;
                            

                            EventHandler.CallAdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek,
                                gameHour, gameMinute, gameSecond);
                        }

                        EventHandler.CallAdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour,
                            gameMinute, gameSecond);
                    }
                    
                    gameDayOfWeek = GetDayOfWeek(gameDayOfWeek);

                    EventHandler.CallAdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute,
                    gameSecond);
                }
                EventHandler.CallAdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute,
                    gameSecond);
            }
            EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute,
                gameSecond);
            
        }
        /*Debug.Log("GameYear:" + gameYear + "    GameSeason:" + gameSeason + "    GameDay:" + gameDay +
                  "    GameDayOfWeek:" + gameDayOfWeek + "    GameHour:" + gameHour + "    GameMinute:" +
                  gameMinute +
                  "    GameSecond" + gameSecond);*/
    }



    //识别下周
    private Week GetDayOfWeek(Week gamedayofweek)
    {
        return (Week)((int)gamedayofweek + 1 % (int)Week.Count);
        
    }
    
    #region 开发者函数

    public void TestAdvanceGameMinute()
    {
        for (int i = 0; i < 60; i++)
        {
            UpdateGameSecond();
        }
    }
    
    public void TestAdvanceGameDay()
    {
        for (int i = 0; i < 86400; i++)
        {
            UpdateGameSecond();
        }
    }
    
    #endregion

    #region ISavable

    public void IRegister()
    {
        SaveLoadManager.Instance.iSavableObjectList.Add(this);
    }

    public void IDeregister()
    {
        SaveLoadManager.Instance.iSavableObjectList.Remove(this);
    }

    public void ISave(ref GameSave gameSave)
    {
        gameSave.timeSave.gameYear = gameYear;
        gameSave.timeSave.gameSeason = gameSeason;
        gameSave.timeSave.gameDay = gameDay;
        gameSave.timeSave.gameHour = gameHour;
        gameSave.timeSave.gameMinute = gameMinute;
        gameSave.timeSave.gameSecond = gameSecond;
        gameSave.timeSave.gameDayOfWeek = gameDayOfWeek;
    }

    public void ILoad(GameSave gameSave)
    {
        gameYear = gameSave.timeSave.gameYear;
        gameSeason = gameSave.timeSave.gameSeason;
        gameDay = gameSave.timeSave.gameDay;
        gameHour = gameSave.timeSave.gameHour;
        gameMinute = gameSave.timeSave.gameMinute;
        gameSecond = gameSave.timeSave.gameSecond;
        gameDayOfWeek = gameSave.timeSave.gameDayOfWeek;
    }
    
    #endregion
}

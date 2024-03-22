using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    //持久化菜单的整体
    //[SerializeField] private GameObject persistentMenu = null;
    //暂停菜单的整体
    [SerializeField] private GameObject pauseMenu = null;
    //运行菜单的整体
    [SerializeField] private GameObject runningMenu = null;
    
    
    
    //暂停菜单的中的按钮
    [SerializeField] private Button[] menuButtons = null;
    //暂停菜单中，每个按钮打开的界面
    [SerializeField] private GameObject[] menuTabs = null;
    
    //获取游戏中的物品栏——为了取消打开暂停菜单时还能够拖拽物体
    [SerializeField] private UIInventoryBar uiInventoryBar = null;
    

    //暂停菜单开关的标志
    public bool pauseMenuOn;


    [SerializeField] private Button quitButton;
    [SerializeField] private Scrollbar quitScrollbar;
    private bool isHoldingQuitButton;
    private float quitTime = 0f;
    
    protected override void Awake()
    {
        base.Awake();

        pauseMenuOn = false;
        pauseMenu.SetActive(false);
        runningMenu.SetActive(true);
    }

    private void Update()
    {
        UISwitch();
    }

    private void FixedUpdate()
    {
        //CheckHoldingStatus();
    }


    #region 菜单切换
    //每帧调用，处理UI的切换
    private void UISwitch()
    {
        //检测是否按下按键
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
        {
            //如果菜单本来是开的，那么关闭菜单，否则启用菜单
            if (pauseMenuOn)
            {
                EnableRunningMenu();
            }
            else
            {
                EnablePauseMenu();
            }
            
            pauseMenuOn = !pauseMenuOn;
        }
    }
    
    
    //启用菜单界面
    private void EnablePauseMenu()
    {
        //摧毁每一个slot记录的拖拽的物体
        uiInventoryBar.DestroyCurrentlyDraggedItems();
        
        //清除 选中红框
        uiInventoryBar.ClearCurrentlySelectedItems();
        
        
        //禁用玩家输入和时间流逝
        Player.Instance.PlayerInputIsDisabled = true;
        Time.timeScale = 0;
        TimeManager.Instance.gameClockPaused = true;
        
        runningMenu.SetActive(false);
        pauseMenu.SetActive(true);


        //在暂停的时候调用gc，收回内存，玩家不会注意到卡顿
        GC.Collect();
        
        //高亮选中的按钮
        HighlightButtonForSelectedTab();
        
    }

    
    //开启运行菜单
    public void EnableRunningMenu()
    {
        //恢复玩家输入和时间流逝
        Player.Instance.PlayerInputIsDisabled = false;
        Time.timeScale = 1;
        TimeManager.Instance.gameClockPaused = false;
        

        pauseMenu.SetActive(false);
        runningMenu.SetActive(true);
    }
    
    #endregion
    
    
    
    
    #region 高亮按钮
    
    /*高亮的意义：因为button按下去后虽然会高亮，但是点击其他地方后会恢复原来的颜色。手动高亮有助于玩家意识到现在选择的哪个panel*/
    private void HighlightButtonForSelectedTab()
    {
        for (int i = 0; i < menuTabs.Length; i++)
        {
            if (menuTabs[i].activeSelf)
            {
                SetButtonColorToActive(menuButtons[i]);
            }

            else
            {
                SetButtonColorToInactive(menuButtons[i]);
            }
        }
    }
    
    private void SetButtonColorToActive(Button button)
    {
        ColorBlock colors = button.colors;

        colors.normalColor = colors.pressedColor;

        button.colors = colors;

    }

    private void SetButtonColorToInactive(Button button)
    {
        ColorBlock colors = button.colors;

        colors.normalColor = colors.disabledColor;

        button.colors = colors;

    }

    #endregion
    
    
    //按钮的点击事件，每个按钮传入对应序号的整数，当点击其中一个按钮时，遍历，打开其对应的界面，同时关掉其他界面
    public void SwitchPauseMenuTab(int tabNum)
    {
        for (int i = 0; i < menuTabs.Length; i++)
        {
            if (i != tabNum)
            {
                menuTabs[i].SetActive(false);
            }
            else
            {
                menuTabs[i].SetActive(true);

            }
        }
        HighlightButtonForSelectedTab();
    }

    #region 实现长按退出
    
    public void PrepareToQuite(BaseEventData baseEventData)
    {
        isHoldingQuitButton = true;
        quitScrollbar.gameObject.SetActive(true);
    }

    public void IsQuit(BaseEventData baseEventData)
    {
        isHoldingQuitButton = false;
        quitScrollbar.gameObject.SetActive(false);
        quitTime = 0f;
    }

    private void CheckHoldingStatus()
    {
        if (isHoldingQuitButton)
        {
            if (quitTime > 1f)
            {
                QuitGame();
            }
            else
            {
                quitTime += 0.02f;  //这里不能用Time.deltaTime，因为Time.timeScale=0
            }
        }
        SetQuitScrollBar();
    }

    private void SetQuitScrollBar()
    {
        quitScrollbar.size = quitTime;
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
    
}

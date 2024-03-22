using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToolEffect
{
    none,
    watering
}

public enum Season
{
    春,
    夏,
    秋,
    冬,
    none,
    count
}

public enum Week
{
    周一,
    周二,
    周三,
    周四,
    周五,
    周六,
    周日,
    Count,

}
public enum HarvestActionEffect
{

}

public enum Direction
{
    up,
    down,
    left,
    right,
    none
}

public enum SceneName
{
    主菜单,
    玩家场景,
    农场,
    室内,
}

public enum ItemType
{
    镐子,

    锄头,
    
    水壶,
    
    斧头,

    镰刀,
    
    种子,
    
    果实,
    
    材料,
    
    树,
    
    矿,
    
    植物
    
}


public enum SoundName
{
    none = 0,
    
    脚步声轻 = 10,
    
    脚步声重 = 20,
    
    使用斧头 = 30,
    
    使用镐子 = 40,
    
    使用镰刀 = 50,
    
    使用锄头 = 60,
    
    浇水声 = 70,
    
    收获声 = 80,
    
    拾取声 = 90,
    
    草摇晃声 = 100,
    
    树倾倒声 = 110,
    
    种植种子声 = 120,
    
    采摘声 = 130,
    
    石头碎裂声 = 140,
    
    木头落地声 = 150,
    
    野外噪声1 = 1000,
    
    野外噪声2 = 1010,
    
    室内噪声 = 1020,
    
    BGM1 = 2000,
    
    BGM2 = 2010
}

public enum 手臂动画
{
    none,
    举起,
    锄地,
    挖矿,
    斧头,
    镰刀,
    浇水,
    count
}

public enum 角色动画部位
{
    none,
    身体,
    头发,
    帽子,
    手臂,
    工具,
    工具特效,
}

public enum 角色动画类型
{
    none,
    锄头,
    斧头,
    镐子,
    镰刀,
    水壶,
    举起,
}




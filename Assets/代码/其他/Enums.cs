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
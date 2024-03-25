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
    None = 0,

    镐子 = 1,

    锄头 = 2,

    水壶 = 3,

    斧头 = 4,

    镰刀 = 5,
    
    菜篮 = 6,

    种子 = 11,

    果实 = 21,

    材料 = 31,

    树 = 41,

    矿 = 51,

    植物 = 61
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

    野外环境声1 = 1000,

    野外环境声2 = 1010,

    室内环境声 = 1020,

    BGM1 = 2000,

    BGM2 = 2010
}




#region 动画覆盖机

/*public enum 手臂动画
{
    none,
    举起,
    锄地,
    挖矿,
    斧头,
    镰刀,
    浇水,
    count
}*/

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

#endregion




public enum 地块性质
{
    可耕作,
    可放置物品,
}

public enum HarvestActionEffect
{
    树叶落下 = 0,
    树倒 = 10,
    伐木 = 20,
    挖矿 = 30,
    收获 = 40,
    None = 50
}
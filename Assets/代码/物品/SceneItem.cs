/// <summary>
/// 这个类用来保存场景中的物品，一个实例对应一个物品
/// </summary>
[System.Serializable]
public class SceneItem
{
    public int itemCode;
    public Vector3Serializable position;
    public string itemName;

    public SceneItem()
    {
        position = new Vector3Serializable();
    }
}

/// <summary>
/// 这个类用来保存物品的坐标
/// </summary>
[System.Serializable]
public class Vector3Serializable
{
    public float x, y, z;

    public Vector3Serializable(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3Serializable()
    {
        
    }
}
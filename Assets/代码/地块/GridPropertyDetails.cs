[System.Serializable]
public class GridPropertyDetails
{
    public int gridX;
    public int gridY;
    
    public bool bDiggable;
    public bool bCanDropItem;
    
    public int daysSinceDug = -1;
    public int daysSinceWatered = -1;
    
    public int seedItemCode = -1;
    public int growthDays = -1; //作物至今的生长天数
    
}

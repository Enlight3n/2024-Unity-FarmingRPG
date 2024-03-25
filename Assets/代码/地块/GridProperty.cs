[System.Serializable]
public class GridProperty
{
    public GridCoordinate gridCoordinate;
    public 地块性质 gridProperty;
    public GridProperty(GridCoordinate gridCoordinate, 地块性质 gridProperty)
    {
        this.gridCoordinate = gridCoordinate;
        this.gridProperty = gridProperty;
    }
}
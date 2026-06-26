using UnityEngine;

public class VHelper
{
    public static readonly Vector2[] directions = {Vector2.right, Vector2.down, Vector2.left, Vector2.up};

    public static Vector2Int Floor(Vector2 pos)
    {
        return new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
    }

    public static Vector2 TopFraction(Vector2 pos)
    {
        return pos - Floor(pos);
    }

    public static Vector2Int Round(Vector2 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }

    public static Vector2Int Rotate90(Vector2Int vector, int quartersCount)
    {
        switch (quartersCount % 4)
        {
            case 1: 
                return new Vector2Int(vector.y, -vector.x);
            case 2:
                return -vector;
            case 3:
                return new Vector2Int(-vector.y, vector.x);
            default:
                return vector;
        }
    }
}
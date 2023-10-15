using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AdjacentRooms {
    public static BlankTile border = new BlankTile(true);

    public DungeonTile Above;
    public DungeonTile Below;
    public DungeonTile Left;
    public DungeonTile Right;

    public DungeonTile AboveFurther;
    public DungeonTile BelowFurther;
    public DungeonTile LeftFurther;
    public DungeonTile RightFurther;

    public DungeonTile[] Diagonals;
    public Vector2Int Location;

    public AdjacentRooms(DungeonTile[,] roomGrid, int x, int y) {
        Location = new Vector2Int(x, y);

        Above = GetTile(roomGrid, x, y + 1);
        Below = GetTile(roomGrid, x, y - 1);
        Left = GetTile(roomGrid, x - 1, y);
        Right = GetTile(roomGrid, x + 1, y);

        AboveFurther = GetTile(roomGrid, x, y + 2);
        BelowFurther = GetTile(roomGrid, x, y - 2);
        LeftFurther = GetTile(roomGrid, x - 2, y);
        RightFurther = GetTile(roomGrid, x + 2, y);

        Diagonals = new DungeonTile[4] {
            GetTile(roomGrid, x + 1, y + 1),
            GetTile(roomGrid, x - 1, y + 1),
            GetTile(roomGrid, x + 1, y - 1),
            GetTile(roomGrid, x - 1, y - 1)
        };
    }

    private DungeonTile GetTile(DungeonTile[,] roomGrid, int x, int y) {
        if(x < 0 || y < 0 || x >= roomGrid.GetLength(1) || y >= roomGrid.GetLength(0)) {
            return border;
        }

        return roomGrid[x, y];
    }
}

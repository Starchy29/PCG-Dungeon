using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum RoomType {
    None,
    SolidWall,

    Start,
    Boss,
    Key,

    MonsterRoom,
    TreasureRoom,

    HoriHall,
    VertHall,
    PlusHall
    //LHall,
    //THall,
    // trap hall?
}

public struct AdjacentRooms {
    public RoomType Above;
    public RoomType Below;
    public RoomType Left;
    public RoomType Right;

    public RoomType AboveFurther;
    public RoomType BelowFurther;
    public RoomType LeftFurther;
    public RoomType RightFurther;

    public RoomType[] Diagonals;

    public AdjacentRooms(RoomType[,] roomGrid, int x, int y) {
        Above = DungeonGenerator.GetTile(roomGrid, x, y + 1);
        Below = DungeonGenerator.GetTile(roomGrid, x, y - 1);
        Left = DungeonGenerator.GetTile(roomGrid, x - 1, y);
        Right = DungeonGenerator.GetTile(roomGrid, x + 1, y);

        AboveFurther = DungeonGenerator.GetTile(roomGrid, x, y + 2);
        BelowFurther = DungeonGenerator.GetTile(roomGrid, x, y - 2);
        LeftFurther = DungeonGenerator.GetTile(roomGrid, x - 2, y);
        RightFurther = DungeonGenerator.GetTile(roomGrid, x + 2, y);

        Diagonals = new RoomType[4] {
            DungeonGenerator.GetTile(roomGrid, x + 1, y + 1),
            DungeonGenerator.GetTile(roomGrid, x - 1, y + 1),
            DungeonGenerator.GetTile(roomGrid, x + 1, y - 1),
            DungeonGenerator.GetTile(roomGrid, x - 1, y - 1)
        };
    }
    
    public bool IsAdjacent(RoomType type) {
        return Above == type || Below == type || Left == type || Right == type;
    }

    public bool TwoConsecutiveRooms() {
        return DungeonGenerator.IsRoom(Above) && DungeonGenerator.IsRoom(AboveFurther) ||
            DungeonGenerator.IsRoom(Below) && DungeonGenerator.IsRoom(BelowFurther) ||
            DungeonGenerator.IsRoom(Left) && DungeonGenerator.IsRoom(LeftFurther) ||
            DungeonGenerator.IsRoom(Right) && DungeonGenerator.IsRoom(RightFurther);
    }

    public bool TwoConsecutiveHalls() {
        return DungeonGenerator.IsHall(Above) && DungeonGenerator.IsHall(AboveFurther) ||
            DungeonGenerator.IsHall(Below) && DungeonGenerator.IsHall(BelowFurther) ||
            DungeonGenerator.IsHall(Left) && DungeonGenerator.IsHall(LeftFurther) ||
            DungeonGenerator.IsHall(Right) && DungeonGenerator.IsHall(RightFurther);
    }

    public bool BetweenRooms() {
        return DungeonGenerator.IsRoom(Above) && DungeonGenerator.IsRoom(Below) ||
            DungeonGenerator.IsRoom(Left) && DungeonGenerator.IsRoom(Right);
    }

    public bool BetweenHalls() {
        return DungeonGenerator.IsHall(Above) && DungeonGenerator.IsHall(Below) ||
            DungeonGenerator.IsHall(Left) && DungeonGenerator.IsHall(Right);
    }

    public bool HasAdjacentRoom() {
        return DungeonGenerator.IsRoom(Above) || DungeonGenerator.IsRoom(Below) || DungeonGenerator.IsRoom(Left) || DungeonGenerator.IsRoom(Right);
    }

    public bool HasAdjacentHall() {
        return DungeonGenerator.IsHall(Above) || DungeonGenerator.IsHall(Below) || DungeonGenerator.IsHall(Left) || DungeonGenerator.IsHall(Right);
    }
}

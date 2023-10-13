using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType {
    None,
    Start,
    Boss,
    Miniboss,
    Key,

    MonsterRoom,
    TreasureRoom,

    SolidWall,

    HoriHall,
    VertHall,
    //LHall,
    //THall,
    PlusHall
    // trap hall?
}

public struct AdjacentRooms {
    public RoomType Above;
    public RoomType Below;
    public RoomType Left;
    public RoomType Right;

    public AdjacentRooms(RoomType above, RoomType below, RoomType left, RoomType right) {
        Above = above;
        Below = below;
        Left = left;
        Right = right;
    }
    
    public bool Contains(RoomType type) {
        return Above == type || Below == type || Left == type || Right == type;
    }
}

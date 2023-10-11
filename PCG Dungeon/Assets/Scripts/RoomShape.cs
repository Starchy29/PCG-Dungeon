using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType {
    Start,
    Boss,
    Miniboss,
    Key,

    MonsterRoom,
    TreasureRoom,

    TrapHall,
    StraightHall,
    LHall,
    THall,
    PlusHall
}

public struct RoomShape
{
    public RoomType Type;
    public RoomType[] UpOptions;
    public RoomType[] DownOptions;
    public RoomType[] RightOptions;
    public RoomType[] LeftOptions;
}

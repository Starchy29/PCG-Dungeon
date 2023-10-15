using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DungeonTile
{
    public bool OpenUp { get; protected set; }
    public bool OpenDown { get; protected set; }
    public bool OpenLeft { get; protected set; }
    public bool OpenRight { get; protected set; }

    public GameObject Prefab { get; private set; }

    public bool NeedsConnect { get { return this is HallTile; } }

    public DungeonTile(GameObject prefab) {
        Prefab = prefab;
    }

    private bool CanConnect(AdjacentRooms adjacents) {
        // check if this blocks any neighbors
        if(adjacents.Above.NeedsConnect && adjacents.Above.OpenDown && !OpenUp ||
            adjacents.Below.NeedsConnect && adjacents.Below.OpenUp && !OpenDown ||
            adjacents.Left.NeedsConnect && adjacents.Left.OpenRight && !OpenLeft ||
            adjacents.Right.NeedsConnect && adjacents.Right.OpenLeft && !OpenRight
        ) {
            return false;
        }
        
        // check if any neighbors block this
        if(NeedsConnect &&
            (OpenUp && !adjacents.Above.OpenDown ||
            OpenDown && !adjacents.Below.OpenUp ||
            OpenLeft && !adjacents.Left.OpenRight ||
            OpenRight && !adjacents.Right.OpenLeft)
        ) {
                return false;
        }

        return true;
    }

    public abstract bool FollowsRules(AdjacentRooms adjacents);

    public bool IsValid(AdjacentRooms adjacents) {
        return CanConnect(adjacents) && FollowsRules(adjacents);
    }
}

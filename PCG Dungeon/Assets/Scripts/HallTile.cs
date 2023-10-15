using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallTile : DungeonTile
{
    public HallTile(GameObject prefab, bool openUp, bool openDown, bool openLeft, bool openRight) : base(prefab) {
        this.OpenUp = openUp;
        this.OpenDown = openDown;
        this.OpenLeft = openLeft;
        this.OpenRight = openRight;
    }

    // prevent a hallway path that goes straight for three tiles in a row
    public override bool FollowsRules(AdjacentRooms adjacents) {
        return true;
    }
}

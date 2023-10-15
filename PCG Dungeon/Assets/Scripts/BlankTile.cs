using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlankTile : DungeonTile
{
    public BlankTile(bool isBorder) : base(null) {
        OpenUp = !isBorder;
        OpenDown = !isBorder;
        OpenLeft = !isBorder;
        OpenRight = !isBorder;
    }

    public override bool FollowsRules(AdjacentRooms adjacents) {
        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTile : DungeonTile
{
    public WallTile(GameObject prefab) : base(prefab) {
        OpenUp = false;
        OpenDown = false;
        OpenLeft = false;
        OpenRight = false;
    }

    // no wall tiles next to each other
    public override bool FollowsRules(AdjacentRooms adjacents) {
        int numAdjacentsWalls = 0;
        if(adjacents.Above is WallTile) {
            numAdjacentsWalls++;
        }
        if(adjacents.Below is WallTile) {
            numAdjacentsWalls++;
        }
        if(adjacents.Left is WallTile) {
            numAdjacentsWalls++;
        }
        if(adjacents.Right is WallTile) {
            numAdjacentsWalls++;
        }

        return numAdjacentsWalls < 3;
    }
}

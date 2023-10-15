using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomTile : DungeonTile
{
    public enum Type {
        Start,
        Key,
        Boss,
        Monster,
        Treasure
    }

    public Type RoomType { get; private set; }

    public RoomTile(GameObject prefab, Type roomType) : base(prefab) {
        RoomType = roomType;

        OpenUp = true;
        OpenDown = true;
        OpenLeft = true;
        OpenRight = true;
    }

    public override bool FollowsRules(AdjacentRooms adjacents) {
        switch(RoomType) {
            case Type.Monster:
                //cannot be next to any non-monster rooms
                if(adjacents.Above is RoomTile && !IsMonsterRoom(adjacents.Above) ||
                    adjacents.Below is RoomTile && !IsMonsterRoom(adjacents.Below) ||
                    adjacents.Left is RoomTile && !IsMonsterRoom(adjacents.Left) ||
                    adjacents.Right is RoomTile && !IsMonsterRoom(adjacents.Right)
                ) {
                    return false;
                }

                // no 3 monster rooms in a row
                if(IsMonsterRoom(adjacents.Above) && IsMonsterRoom(adjacents.Below) ||
                    IsMonsterRoom(adjacents.Left) && IsMonsterRoom(adjacents.Right) ||
                    IsMonsterRoom(adjacents.Above) && IsMonsterRoom(adjacents.AboveFurther) ||
                    IsMonsterRoom(adjacents.Below) && IsMonsterRoom(adjacents.BelowFurther) ||
                    IsMonsterRoom(adjacents.Left) && IsMonsterRoom(adjacents.LeftFurther) ||
                    IsMonsterRoom(adjacents.Right) && IsMonsterRoom(adjacents.RightFurther)
                ) {
                    return false;
                }
                break;

            case Type.Treasure:
                // cannot be adjacent to any room, and cannot be near any other treasure room
                foreach(DungeonTile tile in adjacents.Diagonals) {
                    if(IsTreasureRoom(tile)) {
                        return false;
                    }
                }

                if(adjacents.Above is RoomTile || IsTreasureRoom(adjacents.AboveFurther) ||
                    adjacents.Below is RoomTile || IsTreasureRoom(adjacents.BelowFurther) ||
                    adjacents.Left is RoomTile || IsTreasureRoom(adjacents.LeftFurther) ||
                    adjacents.Right is RoomTile || IsTreasureRoom(adjacents.RightFurther)
                ) {
                    return false;
                }
                break;

            case Type.Start:
                return false;

            case Type.Key:
                // place after the boss room exists
                if(!DungeonGenerator.Instance.BossSpot.HasValue) {
                    return false;
                }

                // only one key spot
                if(DungeonGenerator.Instance.KeySpot.HasValue) {
                    return false;
                }

                // do not place next to other rooms
                if(adjacents.Above is RoomTile || adjacents.Below is RoomTile || adjacents.Left is RoomTile || adjacents.Right is RoomTile) {
                    return false;
                }

                // place away from the start and end
                if(CalcTileDistance(DungeonGenerator.Instance.BossSpot.Value, adjacents.Location) < DungeonGenerator.Instance.Dimensions / 2 || CalcTileDistance(DungeonGenerator.Instance.StartSpot.Value, adjacents.Location) < DungeonGenerator.Instance.Dimensions / 2) {
                    return false;
                }
                break; 

            case Type.Boss:
                // only one boss spot
                if(DungeonGenerator.Instance.BossSpot.HasValue) {
                    return false;
                }

                // do not place next to other rooms
                if(adjacents.Above is RoomTile || adjacents.Below is RoomTile || adjacents.Left is RoomTile || adjacents.Right is RoomTile) {
                    return false;
                }

                // place far away from the start tile
                Vector2Int? startTile = DungeonGenerator.Instance.StartSpot;
                if(startTile.HasValue && CalcTileDistance(startTile.Value, adjacents.Location) < DungeonGenerator.Instance.Dimensions) {
                    return false;
                }
                break;
        }

        return true;
    }

    private bool IsMonsterRoom(DungeonTile tile) {
        return tile is RoomTile && ((RoomTile)tile).RoomType == Type.Monster;
    }

    private bool IsTreasureRoom(DungeonTile tile) {
        return tile is RoomTile && ((RoomTile)tile).RoomType == Type.Treasure;
    }

    private int CalcTileDistance(Vector2Int tile1, Vector2Int tile2) {
        return Mathf.Abs(tile1.x - tile2.x) + Mathf.Abs(tile1.y - tile2.y);
    }
}

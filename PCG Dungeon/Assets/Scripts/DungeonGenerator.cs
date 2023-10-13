using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonGenerator : MonoBehaviour
{
    private RoomType[,] roomGrid;
    private RoomType[] roomTypes;
    [SerializeField] private int Dimensions;
    [SerializeField] private float RoomScale;

    [Header("Order of this list must match the order of Enum RoomType")]
    [SerializeField] private GameObject[] TilePrefabs;

    void Start() {
        roomTypes = (RoomType[])Enum.GetValues(typeof(RoomType));

        for(int i = 0; i < 5 && roomGrid == null; i++) {
            Debug.Log("attempt number " + i);
            roomGrid = GenerateLayout();
        }

        if(roomGrid == null) {
            Debug.Log("algorithm failed multiple attempts :(");
        } else {
            SpawnDungeon();
        }
    }

    // places game objects based on the populated room grid
    private void SpawnDungeon() {
        float startCoord = (-Dimensions + 1) * RoomScale / 2.0f;

        for(int x = 0; x < Dimensions; x++) {
            for(int y = 0; y < Dimensions; y++) {
                GameObject newTile = Instantiate(TilePrefabs[(int)roomGrid[x, y]]);
                newTile.transform.localScale = new Vector3(RoomScale, RoomScale, 1);
                newTile.transform.position = new Vector3(startCoord + x * RoomScale, startCoord + y * RoomScale, 0);
            }
        }
    }

    // returns null if the algorithm fails
    private RoomType[,] GenerateLayout() {
        RoomType[,] roomGrid = new RoomType[Dimensions, Dimensions];

        for(int i = 0; i < Dimensions * Dimensions; i++) {
            // find the most restrictive tiles
            List<Vector2Int> tileOptions = new List<Vector2Int>();
            int minOptions = int.MaxValue;
            for(int x = 0; x < Dimensions; x++) {
                for(int y = 0; y < Dimensions; y++) {
                    if(roomGrid[x, y] != RoomType.None) {
                        // do not place a tile in the same spot twice
                        continue;
                    }

                    int numOptions = GetTileOptions(roomGrid, x, y).Count;
                    if(numOptions == 0) {
                        // algorithm failed
                        return null;
                    }
                    else if(numOptions < minOptions) {
                        // found a more restrictive tile
                        minOptions = numOptions;
                        tileOptions.Clear();
                        tileOptions.Add(new Vector2Int(x, y));
                    }
                    else if(numOptions == minOptions) {
                        // found an equally restrictive tile
                        tileOptions.Add(new Vector2Int(x, y));
                    }
                }
            }

            // place one of the most restrictive tiles
            Vector2Int chosenTile = tileOptions[UnityEngine.Random.Range(0, tileOptions.Count)];
            List<RoomType> typeOptions = GetTileOptions(roomGrid, chosenTile.x, chosenTile.y);
            roomGrid[chosenTile.x, chosenTile.y] = typeOptions[UnityEngine.Random.Range(0, typeOptions.Count)];
        }

        return roomGrid;
    }

    public static RoomType GetTile(RoomType[,] roomGrid, int x, int y) {
        if(x < 0 || y < 0 || x >= roomGrid.GetLength(1) || y >= roomGrid.GetLength(0)) {
            return RoomType.None;
        }

        return roomGrid[x, y];
    }

    private List<RoomType> GetTileOptions(RoomType[,] roomGrid, int x, int y) {
        List<RoomType> options = new List<RoomType>();
        AdjacentRooms adjacents = new AdjacentRooms(roomGrid, x, y);

        foreach(RoomType roomType in roomTypes) {
            if(IsRoomValid(roomType, adjacents)) {
                options.Add(roomType);
            }
        }

        return options;
    }

    // determines if the input room type is a valid option considering its four neighbors
    private bool IsRoomValid(RoomType type, AdjacentRooms adjacents) {
        switch(type) {
            case RoomType.Start:
            case RoomType.Boss:
            case RoomType.Key:
                return false;

            case RoomType.SolidWall:
                if(adjacents.IsAdjacent(RoomType.SolidWall)) {
                    // solid walls must be 1x1
                    return false;
                }
                if(HasRightPath(adjacents.Left) || HasLeftPath(adjacents.Right) || HasDownPath(adjacents.Above) || HasUpPath(adjacents.Below)) {
                    // do not block a hall
                    return false;
                }
                return true;

            case RoomType.MonsterRoom:
                if(adjacents.BetweenRooms() || adjacents.TwoConsecutiveRooms()) {
                    // cannot have three rooms in a row
                    return false;
                }
                if(adjacents.IsAdjacent(RoomType.TreasureRoom)) {
                    // no treasure rooms with monster rooms
                    return false;
                }
                return true;

            case RoomType.TreasureRoom:
                if(adjacents.HasAdjacentRoom()) {
                    // treasure rooms must be alone
                    return false;
                }
                if (adjacents.AboveFurther == RoomType.TreasureRoom || adjacents.BelowFurther == RoomType.TreasureRoom
                    || adjacents.LeftFurther == RoomType.TreasureRoom || adjacents.RightFurther == RoomType.TreasureRoom
                    || adjacents.Diagonals.Contains(RoomType.TreasureRoom)
                ) {
                    // no treasure rooms nearby existing treasure rooms
                    return false;
                }
                return true;

            case RoomType.HoriHall:
                if(HasDownPath(adjacents.Above) || HasUpPath(adjacents.Below) || HasRightBlocked(adjacents.Left) || HasLeftBlocked(adjacents.Right)) {
                    // no dead ends
                    return false;
                }
                if(IsHall(adjacents.Left) && IsHall(adjacents.Right) ||
                    IsHall(adjacents.Left) && IsHall(adjacents.LeftFurther) ||
                    IsHall(adjacents.Right) && IsHall(adjacents.RightFurther)
                ) {
                    // cannot have three rooms in a row
                    return false;
                }
                return true;

            case RoomType.VertHall:
                if(HasRightPath(adjacents.Left) || HasLeftPath(adjacents.Right) || HasUpBlocked(adjacents.Below) || HasDownBlocked(adjacents.Above)) {
                    // no dead ends
                    return false;
                }
                if(IsHall(adjacents.Above) && IsHall(adjacents.Below) ||
                    IsHall(adjacents.Above) && IsHall(adjacents.AboveFurther) ||
                    IsHall(adjacents.Below) && IsHall(adjacents.BelowFurther)
                ) {
                    // cannot have three rooms in a row
                    return false;
                }
                return true;

            case RoomType.PlusHall:
                if(HasRightBlocked(adjacents.Left) || HasLeftBlocked(adjacents.Right) || HasUpBlocked(adjacents.Below) || HasDownBlocked(adjacents.Above)) {
                    // no dead ends
                    return false;
                }
                if(adjacents.TwoConsecutiveHalls()) {
                    // cannot have three rooms in a row
                    return false;
                }
                return true;

            case RoomType.None:
                return false;
        }

        return false;
    }

    #region tile type definitions
    public static bool IsRoom(RoomType type) {
        switch(type) {
            case RoomType.Start:
            case RoomType.Boss:
            case RoomType.Key:
            case RoomType.MonsterRoom:
            case RoomType.TreasureRoom:
                return true;
        }

        return false;
    }

    public static bool IsHall(RoomType type) {
        switch(type) {
            case RoomType.HoriHall:
            case RoomType.VertHall:
            case RoomType.PlusHall:
                return true;
        }

        return false;
    }

    // these four define which halls go in which directions, return false for all rooms
    public static bool HasUpPath(RoomType type) {
        switch(type) {
            case RoomType.VertHall:
            case RoomType.PlusHall:
                return true;
        }

        return false;
    }

    public static bool HasDownPath(RoomType type) {
        switch(type) {
            case RoomType.VertHall:
            case RoomType.PlusHall:
                return true;
        }

        return false;
    }

    public static bool HasLeftPath(RoomType type) {
        switch(type) {
            case RoomType.HoriHall:
            case RoomType.PlusHall:
                return true;
        }

        return false;
    }

    public static bool HasRightPath(RoomType type) {
        switch(type) {
            case RoomType.HoriHall:
            case RoomType.PlusHall:
                return true;
        }

        return false;
    }

    // these four define which tiles cannot have a path connect to each side
    public static bool HasUpBlocked(RoomType type) {
        switch(type) {
            case RoomType.SolidWall:
            case RoomType.HoriHall:
                return true;
        }

        return false;
    }

    public static bool HasDownBlocked(RoomType type) {
        switch(type) {
            case RoomType.SolidWall:
            case RoomType.HoriHall:
                return true;
        }

        return false;
    }

    public static bool HasLeftBlocked(RoomType type) {
        switch(type) {
            case RoomType.SolidWall:
            case RoomType.VertHall:
                return true;
        }

        return false;
    }

    public static bool HasRightBlocked(RoomType type) {
        switch(type) {
            case RoomType.SolidWall:
            case RoomType.VertHall:
                return true;
        }

        return false;
    }
    #endregion
}

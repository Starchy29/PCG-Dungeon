using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private RoomType[,] roomGrid;
    private RoomType[] roomTypes;
    [SerializeField] private int Dimensions;
    [SerializeField] private float RoomScale;

    void Start() {
        roomTypes = (RoomType[])Enum.GetValues(typeof(RoomType));

        for(int i = 0; i < 5 && roomGrid == null; i++) {
            Debug.Log("attempt number " + i);
            roomGrid = GenerateLayout();
        }

        if(roomGrid == null) {
            Debug.Log("algorithm failed multiple attempts :(");
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
                    int numOptions = GetTileOptions(x, y).Count;
                    if(numOptions < minOptions) {
                        // found a more restrictive tile
                        minOptions = numOptions;
                        tileOptions.Clear();
                        tileOptions.Add(new Vector2Int(x, y));
                    }
                    else if(numOptions == minOptions) {
                        // found an equally restrictive tile
                        tileOptions.Add(new Vector2Int(x, y));
                    }
                    else if(numOptions == 0) {
                        // algorithm failed
                        return null;
                    }
                }
            }

            // place one of the most restrictive tiles
            Vector2Int chosenTile = tileOptions[UnityEngine.Random.Range(0, tileOptions.Count)];
            List<RoomType> typeOptions = GetTileOptions(chosenTile.x, chosenTile.y);
            roomGrid[chosenTile.x, chosenTile.y] = typeOptions[UnityEngine.Random.Range(0, typeOptions.Count)];
        }

        return roomGrid;
    }

    private RoomType GetTile(int x, int y) {
        if(x < 0 || y < 0 || x >= Dimensions || y >= Dimensions) {
            return RoomType.None;
        }

        return roomGrid[x, y];
    }

    private List<RoomType> GetTileOptions(int x, int y) {
        List<RoomType> options = new List<RoomType>();
        AdjacentRooms adjacents = new AdjacentRooms(
            GetTile(x, y + 1),
            GetTile(x, y - 1),
            GetTile(x - 1, y),
            GetTile(x + 1, y)
        );

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
            case RoomType.Miniboss:
            case RoomType.Key:
                return false;

            case RoomType.MonsterRoom:
                if(IsRoom(adjacents.Above) && IsRoom(adjacents.Below) || IsRoom(adjacents.Left) && IsRoom(adjacents.Right)) {
                    // cannot be the middle of two other rooms
                    return false;
                }
                if(adjacents.Contains(RoomType.TreasureRoom)) {
                    // no treasure rooms with monster rooms
                    return false;
                }
                return true;

            case RoomType.None:
                return false;
        }

        return false;
    }

    private bool IsRoom(RoomType type) {
        switch(type) {
            case RoomType.Start:
            case RoomType.Boss:
            case RoomType.Miniboss:
            case RoomType.Key:
            case RoomType.MonsterRoom:
            case RoomType.TreasureRoom:
                return true;
        }

        return false;
    }

    private bool IsHall(RoomType type) {
        switch(type) {
            case RoomType.HoriHall:
            case RoomType.VertHall:
            case RoomType.PlusHall:
                return true;
        }

        return false;
    }
}

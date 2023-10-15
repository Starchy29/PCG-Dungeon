using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonGenerator : MonoBehaviour
{
    private DungeonTile[,] roomGrid;
    private DungeonTile[] roomTypes;
    [SerializeField] public int Dimensions;
    [SerializeField] private float RoomScale;

    [Header("Room Tile Prefabs")]
    [SerializeField] private GameObject StartRoomPrefab;
    [SerializeField] private GameObject KeyRoomPrefab;
    [SerializeField] private GameObject BossRoomPrefab;

    [SerializeField] private GameObject MonsterRoomPrefab;
    [SerializeField] private GameObject WallTilePrefab;
    [SerializeField] private GameObject TreasureRoomPrefab;
    [SerializeField] private GameObject PlusHallPrefab;
    [SerializeField] private GameObject VerticalHallPrefab;
    [SerializeField] private GameObject HorizontalHallPrefab;

    [SerializeField] private GameObject LHallURPrefab;
    [SerializeField] private GameObject LHallULPrefab;
    [SerializeField] private GameObject LHallDRPrefab;
    [SerializeField] private GameObject LHallDLPrefab;

    [SerializeField] private GameObject THallUpPrefab;
    [SerializeField] private GameObject THallDownPrefab;
    [SerializeField] private GameObject THallLeftPrefab;
    [SerializeField] private GameObject THallRightPrefab;

    private RoomTile startRoom;
    private RoomTile bossRoom;
    private RoomTile keyRoom;
    private BlankTile emptyTile;
    private WallTile wallTile;
    private RoomTile monsterRoom;
    private RoomTile treasureRoom;
    private HallTile plusHall;
    private HallTile vertHall;
    private HallTile horiHall;
    private HallTile lHallUR;
    private HallTile lHallUL;
    private HallTile lHallDR;
    private HallTile lHallDL;
    private HallTile tHallUp;
    private HallTile tHallDown;
    private HallTile tHallLeft;
    private HallTile tHallRight;

    public static DungeonGenerator Instance { get; private set; }
    public Vector2Int? StartSpot { get; private set; }
    public Vector2Int? KeySpot { get; private set; }
    public Vector2Int? BossSpot { get; private set; }

    void Start() {
        Instance = this;

        emptyTile = new BlankTile(false);
        wallTile = new WallTile(WallTilePrefab);

        startRoom = new RoomTile(StartRoomPrefab, RoomTile.Type.Start);
        keyRoom = new RoomTile(KeyRoomPrefab, RoomTile.Type.Key);
        bossRoom = new RoomTile(BossRoomPrefab, RoomTile.Type.Boss);

        monsterRoom = new RoomTile(MonsterRoomPrefab, RoomTile.Type.Monster);
        treasureRoom = new RoomTile(TreasureRoomPrefab, RoomTile.Type.Treasure);

        plusHall = new HallTile(PlusHallPrefab, true, true, true, true);
        vertHall = new HallTile(VerticalHallPrefab, true, true, false, false);
        horiHall = new HallTile(HorizontalHallPrefab, false, false, true, true);

        lHallUR = new HallTile(LHallURPrefab, true, false, false, true);
        lHallUL = new HallTile(LHallULPrefab, true, false, true, false);
        lHallDR = new HallTile(LHallDRPrefab, false, true, false, true);
        lHallDL = new HallTile(LHallDLPrefab, false, true, true, false);

        tHallUp = new HallTile(THallUpPrefab, false, true, true, true);
        tHallDown = new HallTile(THallDownPrefab, true, false, true, true);
        tHallLeft = new HallTile(THallLeftPrefab, true, true, false, true);
        tHallRight = new HallTile(THallRightPrefab, true, true, true, false);

        roomTypes = new DungeonTile[16] {
            wallTile,
            keyRoom, bossRoom,
            monsterRoom, treasureRoom,
            plusHall, vertHall, horiHall,
            lHallUR, lHallUL, lHallDR, lHallDL,
            tHallUp, tHallDown, tHallLeft, tHallRight
        };

        for(int i = 0; i < 100 && roomGrid == null; i++) {
            Debug.Log("attempt number " + i);
            roomGrid = GenerateLayout();
        }

        if(roomGrid == null) {
            Debug.Log("algorithm failed too many times :(");
        } else {
            SpawnDungeon();
        }
    }

    // places game objects based on the populated room grid
    private void SpawnDungeon() {
        float startCoord = (-Dimensions + 1) * RoomScale / 2.0f;

        for(int x = 0; x < Dimensions; x++) {
            for(int y = 0; y < Dimensions; y++) {
                GameObject newTile = Instantiate(roomGrid[x, y].Prefab);
                newTile.transform.localScale = new Vector3(RoomScale, RoomScale, 1);
                newTile.transform.position = new Vector3(startCoord + x * RoomScale, startCoord + y * RoomScale, 0);
            }
        }
    }

    // returns null if the algorithm fails
    private DungeonTile[,] GenerateLayout() {
        DungeonTile[,] roomGrid = new DungeonTile[Dimensions, Dimensions];
        for(int x = 0; x < Dimensions; x++) {
            for(int y = 0; y < Dimensions; y++) {
                roomGrid[x, y] = emptyTile;
            }
        }

        // begin by placing the start tile on a border spot
        bool onHoriSide = UnityEngine.Random.value < 0.5f;
        int side = UnityEngine.Random.value < 0.5f ? 0 : Dimensions - 1;
        int sideSpot = UnityEngine.Random.Range(0, Dimensions);
        StartSpot = new Vector2Int(onHoriSide ? side : sideSpot, onHoriSide ? sideSpot : side);
        roomGrid[StartSpot.Value.x, StartSpot.Value.y] = startRoom;

        for(int i = 1; i < Dimensions * Dimensions; i++) {
            // find the most restrictive tiles
            List<Vector2Int> tileOptions = new List<Vector2Int>();
            int minOptions = int.MaxValue;
            for(int x = 0; x < Dimensions; x++) {
                for(int y = 0; y < Dimensions; y++) {
                    if( ! (roomGrid[x, y] is BlankTile)) {
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
            List<DungeonTile> typeOptions = GetTileOptions(roomGrid, chosenTile.x, chosenTile.y);
            roomGrid[chosenTile.x, chosenTile.y] = typeOptions[UnityEngine.Random.Range(0, typeOptions.Count)];

            // track the single key and boss rooms
            if(roomGrid[chosenTile.x, chosenTile.y] == keyRoom) {
                KeySpot = chosenTile;
            }
            else if(roomGrid[chosenTile.x, chosenTile.y] == bossRoom) {
                BossSpot = chosenTile;
            }
        }

        return roomGrid;
    }

    private List<DungeonTile> GetTileOptions(DungeonTile[,] roomGrid, int x, int y) {
        List<DungeonTile> options = new List<DungeonTile>();
        AdjacentRooms adjacents = new AdjacentRooms(roomGrid, x, y);

        foreach(DungeonTile roomType in roomTypes) {
            if(roomType.IsValid(adjacents)) {
                options.Add(roomType);
            }
        }

        return options;
    }
}

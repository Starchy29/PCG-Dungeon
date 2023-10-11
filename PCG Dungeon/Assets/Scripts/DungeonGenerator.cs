using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    void Start() {
        GenerateDungeon(5);
    }

    private void GenerateDungeon(int dimensions) {
        RoomType[,] roomGrid = new RoomType[dimensions, dimensions];

    }
}

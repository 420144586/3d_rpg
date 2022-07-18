using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionDestination : MonoBehaviour
{
    public enum DestinationTag
    {
        ENTER, FOREST, FOREST_LAND, DUNGEON, DUNGEON_LAND_1, DUNGEON_LAND_2, BOSS, BOSS_LAND
    }

    public DestinationTag destinationTag;
}

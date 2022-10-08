/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using System.Collections.Generic;
using UnityEngine;

public class Room{

    public float atmosO2  = 0;
    public float atmosN   = 0;
    public float atmosCO2 = 0;

    List<Tile> tiles;

    public Room() {
        this.tiles = new List<Tile>();
    }

    public void AssignTile(Tile t) {
        if(this.tiles.Contains(t)) {
            // Tile already in this room.
            return;
        }

        if(t.room != null) {
            // Belongs to some other room, remove from that room.
            t.room.tiles.Remove(t);
        }

        t.room = this;
        this.tiles.Add(t);
    }

    public void UnAssignAllTiles() {
        for(int i = 0; i < this.tiles.Count; i++) {
            this.tiles[i].room = this.tiles[i].world.GetDefaultRoom();
        }
        this.tiles = new List<Tile>();
    }

    public static void DoRoomFloodFill(Furniture sourceFurniture) {
        // sourceFurniture is the changed furniture (probably placed) that could
        // Potentially make/destroy rooms.
        // Check NESW of furniture tile and floodfill from them.

        World world = sourceFurniture.tile.world;

        Room oldRoom = sourceFurniture.tile.room;

        // Try building room starting from the north.
        foreach(Tile t in sourceFurniture.tile.GetNeighbours()) {
            Room.ActualFloodFill(t, oldRoom);
        }

        // Walls doors etc aren't in a 'room' as such.
        sourceFurniture.tile.room = null;
        oldRoom.tiles.Remove(sourceFurniture.tile);

        if(oldRoom != world.GetDefaultRoom()) {
            // oldRoom shouldn't have any tiles left inside.

            if(oldRoom.tiles.Count > 0) {
                Debug.LogError("oldRoom still contains tiles !!!");
            }

            world.DeleteRoom(oldRoom);
        }
    }

    protected static void ActualFloodFill(Tile tile, Room oldRoom) {
        if(tile == null) {
            // Trying to floodfill off map, so 'space' but that room is default
            // So no action required.
            return;
        }

        if(tile.room != oldRoom) {
            // Tile was already assigned to another 'new' room, direction picked
            // Is not isolated (part of another direction's room)
            return;
        }

        if(tile.furniture != null && tile.furniture.roomEnclosure) {
            // Tile can't be checked if wall/door etc is on it.
            return;
        }

        if(tile.Type == TileType.Empty) {
            // Tile is 'outside' in space so definitely not in a room etc.
            return;
        }

        // Checks complete, new room needs to be made.

        Room newRoom = new Room();
        Queue<Tile> tilesToCheck = new Queue<Tile>();

        // Current tile is obviously going to be in new room.
        tilesToCheck.Enqueue(tile);

        while(tilesToCheck.Count > 0) {
            Tile t = tilesToCheck.Dequeue();

            if(t.room == oldRoom) {
                newRoom.AssignTile(t);

                // Check tiles neighbours.
                Tile[] ns = t.GetNeighbours();
                foreach(Tile t2 in ns) {
                    if(t2 == null || t2.Type == TileType.Empty) {
                        // We have hit open space/edge of map.
                        // Room is not enclosed but part of outside.
                        // Immediately end flood fill (Major performance impact)
                        // Delete new room and re-assing all tiles back to default.
                        newRoom.UnAssignAllTiles();
                        return;
                    }

                    if(t2.room == oldRoom && (t2.furniture == null || t2.furniture.roomEnclosure == false)) {
                        tilesToCheck.Enqueue(t2);
                    }
                }
            }
        }

        // Tell world new room has been created. 
        tile.world.AddRoom(newRoom);
    }
}

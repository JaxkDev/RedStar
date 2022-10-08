/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using System.Collections.Generic;

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
            return;
        }

        t.room = this;
        this.tiles.Add(t);
    }

    public void UnAssignAllTiles() {
        for(int i = 0; i < this.tiles.Count; i++) {
            this.tiles[i].room = this.tiles[i].world.GetSpaceRoom();
        }
        this.tiles = new List<Tile>();
    }
    public static void DoRoomFloodFill(Furniture sourceFurniture) {
        // sourceFurniture is the changed furniture (probably placed) that could
        // Potentially make/destroy rooms.
        // Check NESW of furniture tile and floodfill from them.

        World world = sourceFurniture.tile.world;

        if(sourceFurniture.tile.room != world.GetSpaceRoom()) {
            //Re-assigns all tiles inside changed room to space for now.
            world.DeleteRoom(sourceFurniture.tile.room);
        }

        sourceFurniture.tile.room.UnAssignAllTiles();
    }
}

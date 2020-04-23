/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using System.Collections.Generic;
 
public class Path_TileGraph {

    Dictionary<Tile, Path_Node<Tile>> nodes;
    
    public Path_TileGraph(World world) {
        //Create all nodes via tiles.

        this.nodes = new Dictionary<Tile, Path_Node<Tile>>();

        for(int x = 0; x < world.Width; x++) {
            for(int y = 0; y < world.Height; y++) {
                Tile t = world.GetTileAt(x, y);

                if(t.movementCost > 0) {
                    //Can actually move through the tile...
                    Path_Node<Tile> n = new Path_Node<Tile>();
                    n.data = t;
                    this.nodes.Add(t, n);
                }
            }
        }

        //TODO edges.
    }

}

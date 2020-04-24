/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using UnityEngine;
using System.Collections.Generic;
 
public class Path_TileGraph {

    public Dictionary<Tile, Path_Node<Tile>> nodes;
    
    public Path_TileGraph(World world) {
        // Create all nodes via tiles.

        this.nodes = new Dictionary<Tile, Path_Node<Tile>>();

        for(int x = 0; x < world.Width; x++) {
            for(int y = 0; y < world.Height; y++) {
                Tile t = world.GetTileAt(x, y);

                // Can actually move through the tile...
                Path_Node<Tile> n = new Path_Node<Tile>();
                n.data = t;
                this.nodes.Add(t, n);
            }
        }



        // Create edges.

        foreach(Tile t in this.nodes.Keys) {
            Path_Node<Tile> n = this.nodes[t];

            List<Path_Edge<Tile>> edges = new List<Path_Edge<Tile>>();

            Tile[] neighbours = t.GetNeighbours(true); // Can walk diagonal.

            for(int i = 0; i < neighbours.Length; i++) {
                if(neighbours[i] != null && neighbours[i].movementCost > 0) {

                    // Ensure no corners can be clipped.
                    if(this.IsClippingCorner(t, neighbours[i])) continue;

                    Path_Edge<Tile> e = new Path_Edge<Tile>();
                    e.cost = neighbours[i].movementCost;
                    e.node = this.nodes[neighbours[i]];
                    edges.Add(e);
                }
            }

            n.edges = edges.ToArray();
        }
    }

    bool IsClippingCorner(Tile curr, Tile dest) {
        int diffX = curr.X - dest.X;
        int diffY = curr.Y - dest.Y;
        if(Mathf.Abs(diffX) + Mathf.Abs(diffY) == 2) {
            if(curr.world.GetTileAt(curr.X - diffX, curr.Y).movementCost == 0) return true;
            if(curr.world.GetTileAt(curr.X, curr.Y - diffY).movementCost == 0) return true;
       
        }

        return false;
    }
}

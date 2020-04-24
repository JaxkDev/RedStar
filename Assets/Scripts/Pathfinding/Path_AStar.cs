/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using UnityEngine;
using System.Linq;
using Priority_Queue;
using System.Collections.Generic;

public class Path_AStar {

    Queue<Tile> path;

	public Path_AStar(World world, Tile tileStart, Tile tileEnd) {

        if(world.tileGraph == null) world.tileGraph = new Path_TileGraph(world);

        Dictionary<Tile, Path_Node<Tile>> nodes = world.tileGraph.nodes;

        if(nodes.ContainsKey(tileStart) == false){
            Debug.LogError("And the world comes crashing down... (Starting tile is not in the list - Path_A*)");
            return;
        }

        if(nodes.ContainsKey(tileEnd) == false) {
            Debug.LogError("And the world comes crashing down... (Ending tile is not in the list - Path_A*)");
            return;
        }

        Path_Node<Tile> startNode = nodes[tileStart];
        Path_Node<Tile> goalNode = nodes[tileEnd];

        // Based from the wikipedia A*_search_algorithm psuedocode:

        List<Path_Node<Tile>> closedSet = new List<Path_Node<Tile>>();
        SimplePriorityQueue<Path_Node<Tile>> openSet = new SimplePriorityQueue<Path_Node<Tile>>();
        openSet.Enqueue(startNode, 0);

        Dictionary<Path_Node<Tile>, Path_Node<Tile>> cameFrom = new Dictionary<Path_Node<Tile>, Path_Node<Tile>>();
        Dictionary<Path_Node<Tile>, float> g_score = new Dictionary<Path_Node<Tile>, float>();

        foreach(Path_Node<Tile> n in nodes.Values) {
            g_score[n] = Mathf.Infinity;
        }
        g_score[startNode] = 0;

        Dictionary<Path_Node<Tile>, float> f_score = new Dictionary<Path_Node<Tile>, float>();
        foreach(Path_Node<Tile> n in nodes.Values) {
            f_score[n] = Mathf.Infinity;
        }
        f_score[startNode] = heuristic_cost_estimate(startNode, goalNode);

        while(openSet.Count > 0) {
            Path_Node<Tile> currentNode = openSet.Dequeue();

            if(currentNode == goalNode) {
                //todo
                reconstruct_path(cameFrom, currentNode);
                return;
            }

            closedSet.Add(currentNode);
            foreach(Path_Edge<Tile> neighbour in currentNode.edges) {
                Path_Node<Tile> neighbourNode = neighbour.node;
                if(closedSet.Contains(neighbourNode) == true) continue; //Already checked this neighbour.

                float movementCost = dist_between(currentNode, neighbourNode) * neighbourNode.data.movementCost;

                float tentative_g_score = g_score[currentNode] + movementCost;

                if(openSet.Contains(neighbourNode) && tentative_g_score >= g_score[neighbourNode]) continue;

                cameFrom[neighbourNode] = currentNode;
                g_score[neighbourNode] = tentative_g_score;
                f_score[neighbourNode] = g_score[neighbourNode] + heuristic_cost_estimate(neighbourNode, goalNode);

                if(openSet.Contains(neighbourNode) == false) {
                    openSet.Enqueue(neighbourNode, f_score[neighbourNode]);
                }
            }


        }

        // Gone through entire set, couldn't find a path !
        // Todo Fail.
        // throw new System.Exception("Woah couldnt find a path to that point !");
    }

    float heuristic_cost_estimate(Path_Node<Tile> a, Path_Node<Tile> b) {
        return Mathf.Sqrt(Mathf.Pow(a.data.X - b.data.X, 2) + Mathf.Pow(a.data.Y - b.data.Y, 2));
    }

    float dist_between(Path_Node<Tile> a, Path_Node<Tile> b) {
        if(Mathf.Abs(a.data.X - b.data.X) + Mathf.Abs(a.data.Y - b.data.Y) == 1) {
            return 1f;
        }
        if(Mathf.Abs(a.data.X - b.data.X) == 1 && Mathf.Abs(a.data.Y - b.data.Y) == 1) {
            return Mathf.Sqrt(2);
        }

        return heuristic_cost_estimate(a, b);
    }

    void reconstruct_path(Dictionary<Path_Node<Tile>, Path_Node<Tile>> cameFrom, Path_Node<Tile> currentNode) {
        Queue<Tile> total_path = new Queue<Tile>();
        total_path.Enqueue(currentNode.data);

        while(cameFrom.ContainsKey(currentNode)) {
            currentNode = cameFrom[currentNode];
            total_path.Enqueue(currentNode.data);
        }

        this.path = new Queue<Tile>(total_path.Reverse());
    }

    public Tile GetNextTile() {
        return this.path.Dequeue();
    }


	public int Length(){
		if(this.path == null) return 0;
		return this.path.Count;
	}
}

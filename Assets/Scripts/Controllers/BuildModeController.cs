/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using UnityEngine;

public class BuildModeController : MonoBehaviour {

	bool buildModeIsFurniture = false;
    string buildModeFurniture;
	TileType buildModeTile = TileType.Floor;

	void Start () {}

	public void SetMode_BuildFurniture(string furnType){
		this.buildModeIsFurniture = true;
        this.buildModeFurniture = furnType;
	}

	public void SetMode_BuildFloor(){
		this.buildModeIsFurniture = false;
		this.buildModeTile = TileType.Floor;
	}

	public void SetMode_Bulldoze(){
		this.buildModeIsFurniture = false;
		this.buildModeTile = TileType.Empty;
	}

    public void DoPathfindingTest() {
        WorldController.Instance.world.SetupPathfinding();
    }

    public void DoBuild(Tile tile) {
        if(this.buildModeIsFurniture == true) {
            //Create object instantly:
            //WorldController.Instance.world.PlaceFurniture(this.buildModeFurniture, t);

            //Check valid position:
            if(WorldController.Instance.world.IsFurniturePlacementValid(this.buildModeFurniture, tile) == false || tile.pendingFurnitureJob != null) return;

            //Queue the job:
            string furnitureType = this.buildModeFurniture;
            Job j = new Job(tile, furnitureType, (job) => {
                WorldController.Instance.world.PlaceFurniture(furnitureType, job.tile);
                job.tile.pendingFurnitureJob = null;
            }, 0.1f);

            //TODO CHANGE PUBLIC.
            tile.pendingFurnitureJob = j;
            j.RegisterJobCancelledCallback((job) => {
                job.tile.pendingFurnitureJob = null;
            });

            WorldController.Instance.world.jobQueue.Enqueue(j);
        } else {
            tile.Type = this.buildModeTile;
        }
    }
}

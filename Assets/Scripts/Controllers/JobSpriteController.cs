/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using UnityEngine;
using System.Collections.Generic;

public class JobSpriteController : MonoBehaviour {

    FurnitureSpriteController furnitureSpriteController;

    Dictionary<Job, GameObject> jobGameObjectMap;
    
	void Start () {
        this.furnitureSpriteController = GameObject.FindObjectOfType<FurnitureSpriteController>();
        this.jobGameObjectMap = new Dictionary<Job, GameObject>();

        WorldController.Instance.world.jobQueue.RegisterJobCreatedCallback(this.OnJobCreated);
	}

    void OnJobCreated(Job job) {

        if(this.jobGameObjectMap.ContainsKey(job) == true) {
            //Job requeued.
            return;
        }
        
        GameObject job_go = new GameObject();

        this.jobGameObjectMap.Add(job,job_go);

        job_go.name = "Job_"+job.jobObjectType + "_" + job.tile.X + "_" + job.tile.Y;
        job_go.transform.position = new Vector3(job.tile.X, job.tile.Y, 0);
        job_go.transform.SetParent(this.transform, true);

        SpriteRenderer sr = job_go.AddComponent<SpriteRenderer>();
        sr.sprite = this.furnitureSpriteController.GetSpriteForFurniture(job.jobObjectType);
        sr.sortingLayerName = "Jobs";
        sr.color = new Color(0.6f, 0.6f, 1f, 0.5f);

        if(job.jobObjectType == "Door"){
            //Check door rotation due to neighbour connections
            //Default West-East so only need to check North-South links.
            //FIXME Hardcoded...
            World world = WorldController.Instance.world;
            Tile northTile = world.GetTileAt(job.tile.X, job.tile.Y+1);
            Tile southTile = world.GetTileAt(job.tile.X, job.tile.Y-1);
            if(northTile != null && southTile != null && northTile.furniture != null && southTile.furniture != null &&
                northTile.furniture.furnitureType == "Wall" && southTile.furniture.furnitureType == "Wall"){
                job_go.transform.rotation = Quaternion.Euler(0, 0, 90);
                job_go.transform.Translate(1f, 0, 0, Space.World); //Due to bottom-left anchor point
            }
        }

        job.RegisterJobCancelledCallback(this.OnJobEnded);
        job.RegisterJobCompletedCallback(this.OnJobEnded);
    }

    void OnJobEnded(Job job) {
        GameObject job_go = this.jobGameObjectMap[job];

        job.UnRegisterJobCancelledCallback(this.OnJobEnded);
        job.UnRegisterJobCompletedCallback(this.OnJobEnded);

        Destroy(job_go);
        this.jobGameObjectMap.Remove(job);
    }
}

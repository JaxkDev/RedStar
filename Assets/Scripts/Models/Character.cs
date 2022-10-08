/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using System;
using UnityEngine;

// Serializing:
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public class Character : IXmlSerializable {
    public float X {
        get {
            return Mathf.Lerp(this.currTile.X, this.nextTile.X, movementPercentage);
        }
    }

    public float Y {
        get {
            return Mathf.Lerp(this.currTile.Y, this.nextTile.Y, movementPercentage);
        }
    }

    public Tile currTile { get; protected set; }
    public Tile nextTile { get; protected set; }
    public Tile destTile { get; protected set; }

    Path_AStar pathAStar;

    float movementPercentage;
    float movementSpeed = 6f; //Tiles per second.

    Action<Character> cbCharacterChanged;

    Job currJob;

    public Character(Tile currentTile) {
        this.currTile = this.destTile = this.nextTile = currentTile;
    }

    public void AbandonJob(bool ReEnqueue = true) {
        this.nextTile = this.destTile = this.currTile;
        this.pathAStar = null;
        this.currTile.world.jobQueue.Enqueue(this.currJob);
        this.currJob = null;
    }

    void Update_DoJob(float deltaTime) {
        if(this.currJob == null) {
            //Fetch new Job, TODO PRIORITY SYS HERE.
            Job newJob = currTile.world.jobQueue.Dequeue();

            if(newJob != null) {
                this.currJob = newJob;
                this.destTile = newJob.tile;

                newJob.RegisterJobCancelledCallback(this.OnJobEnded);
                newJob.RegisterJobCompletedCallback(this.OnJobEnded);
            }
        }

        if(this.currTile == this.destTile) {
            if(this.currJob != null) {
                this.currJob.DoWork(deltaTime);
            }
        }
    }

    void Update_Move(float deltaTime) {
        if(this.currTile == this.destTile) return;

        // currTile,
        // nextTile,
        // destTile

        if(this.nextTile == null || this.nextTile == this.currTile) {
            if(this.pathAStar == null || this.pathAStar.Length() == 0) {
                this.pathAStar = new Path_AStar(WorldController.Instance.world, this.currTile, this.destTile);
                if(this.pathAStar.Length() == 0) {
                    //Debug.LogError("No path to destination !");

                    this.AbandonJob();
                    this.pathAStar = null;
                    return;
                }

                this.nextTile = this.pathAStar.GetNextTile(); //Ignore first tile (current)
            }

            this.nextTile = this.pathAStar.GetNextTile();
        }

        if(nextTile.IsEnterable() == Enterability.Never) {
            Debug.LogError("FIXME, Character tried to enter an unwalkable tile.");
            //Map updated after generating path, TODO Handle.
            this.nextTile = null;
            this.pathAStar = null;
            return;
        } else if (nextTile.IsEnterable() == Enterability.Soon) {
            // Cant enter right now but soon, probably door.
            // DONT ABORT PATH, WAIT.
            return;
        }

        float distToTravel = Mathf.Sqrt(Mathf.Pow(this.currTile.X - this.nextTile.X, 2) + Mathf.Pow(this.currTile.Y - this.nextTile.Y, 2));
        float distThisFrame = this.movementSpeed / nextTile.movementCost * deltaTime;
        float percThisFrame = distThisFrame / distToTravel; //MHM.


        this.movementPercentage += percThisFrame;

        if(this.movementPercentage >= 1) {

            //TODO Get next dest tile if not finished path.

            this.currTile = this.nextTile;
            this.movementPercentage = 0f;
        }

        if(this.cbCharacterChanged != null) this.cbCharacterChanged(this);
    }

    public void Update(float deltaTime) {
        this.Update_DoJob(deltaTime);
        this.Update_Move(deltaTime);
    }

    public void SetDestinationTile(Tile tile) {
        if(this.currTile.IsNeighbour(tile, true) != false) {
            Debug.LogError("Woah trying to jump tiles here !");
            return;
        }

        this.destTile = tile;
    }

    public void RegisterCharacterChangedCallback(Action<Character> cb) {
        this.cbCharacterChanged += cb;
    }

    public void UnRegisterCharacterChangedCallback(Action<Character> cb) {
        this.cbCharacterChanged -= cb;
    }

    public void OnJobEnded(Job j) {
        if(this.currJob != j) {
            Debug.LogError("Job does not match the current job this character is handling.");
            return;
        }

        this.nextTile = this.destTile = this.currTile;
        this.pathAStar = null;
        this.currJob = null;
    }



    ////////////////////////////////////////////////////////////////////////////////////
    ///
    ///                             SAVING & LOADING
    ///
    ////////////////////////////////////////////////////////////////////////////////////

    
    public Character() {
        // DO NOT USE, XML Serialize ONLY.
    }

    public XmlSchema GetSchema() {
        return null;
    }

    public void WriteXml(XmlWriter writer) {
        writer.WriteAttributeString("X", Mathf.Floor(this.X).ToString());
        writer.WriteAttributeString("Y", Mathf.Floor(this.Y).ToString());
    }

    public void ReadXml(XmlReader reader) {
        
    }
}

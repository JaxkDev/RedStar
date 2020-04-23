/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using System;
using UnityEngine;
using System.Collections;

public class Character {
    public float X {
        get {
            return Mathf.Lerp(currTile.X, destTile.X, movementPercentage);
        }
    }

    public float Y {
        get {
            return Mathf.Lerp(currTile.Y, destTile.Y, movementPercentage);
        }
    }

    public Tile currTile { get; protected set; }
    public Tile destTile { get; protected set; }

    float movementPercentage;
    float movementSpeed = 2f; //Tiles per second.

    Action<Character> cbCharacterChanged;

    Job currJob;

    public Character(Tile currentTile) {
        this.currTile = this.destTile = currentTile;
    }

    public void Update(float deltaTime) {

        if(this.currJob == null) {
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
            return;
        }

        float distToTravel = Mathf.Sqrt(Mathf.Pow(this.currTile.X-this.destTile.X, 2) + Mathf.Pow(this.currTile.Y - this.destTile.Y, 2));
        float distThisFrame = this.movementSpeed * deltaTime;
        float percThisFrame = distThisFrame / distToTravel; //MHM.


        this.movementPercentage += percThisFrame;

        if(this.movementPercentage >= 1) {
            this.currTile = this.destTile;
            this.movementPercentage = 0f;
        }

        if(this.cbCharacterChanged != null) this.cbCharacterChanged(this);
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

        this.currJob = null;
    }
}

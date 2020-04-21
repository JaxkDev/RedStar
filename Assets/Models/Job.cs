/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using System;
using UnityEngine;
using System.Collections;

public class Job {

    Tile tile; // Where's this job happening ?

    float jobTime = 1f; // How long does this job take ?

    Action<Job> cbJobCompleted;
    //callback for started?
    Action<Job> cbJobCancelled;

    public Job (Tile tile, Action<Job> cbJobCompleted, float jobTime = 1f){
        this.tile = tile;
        this.jobTime = jobTime;

        this.cbJobCompleted += cbJobCompleted;
    }

    public void RegisterJobCompletedCallback(Action<Job> cb) {
        this.cbJobCompleted += cb;
    }

    public void UnRegisterJobCompletedCallback(Action<Job> cb) {
        this.cbJobCompleted -= cb;
    }

    public void DoWork(float workTime) {
        this.jobTime -= workTime;

        if(this.jobTime <= 0) {
            if(cbJobCompleted != null) this.cbJobCompleted(this);
        }
    }

    public void CancelJob() {
        if(this.cbJobCancelled != null) this.cbJobCancelled(this);
    }
}

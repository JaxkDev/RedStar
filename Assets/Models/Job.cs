/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using System;

public class Job {

    public Tile tile { get; protected set; } // Where's this job happening ?

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

    public void RegisterJobCancelledCallback(Action<Job> cb) {
        this.cbJobCancelled += cb;
    }

    public void UnRegisterJobCancelledCallback(Action<Job> cb) {
        this.cbJobCancelled -= cb;
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

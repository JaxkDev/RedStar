/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using System;
using System.Collections.Generic;

public class JobQueue {

    Queue<Job> jobQueue;

    Action<Job> cbJobCreated;

    public JobQueue() {
        this.jobQueue = new Queue<Job>();
    }

    public void Enqueue(Job job) {
        this.jobQueue.Enqueue(job);

        //todo callbacks.
        if(this.cbJobCreated != null) {
            this.cbJobCreated(job);
        }
    }

    public Job Dequeue() {
        if(this.jobQueue.Count == 0) return null;

        return this.jobQueue.Dequeue();
    }

    public void RegisterJobCreatedCallback(Action<Job> cb) {
        this.cbJobCreated += cb;
    }

    public void UnRegisterJobCreatedCallback(Action<Job> cb) {
        this.cbJobCreated -= cb;
    }
}

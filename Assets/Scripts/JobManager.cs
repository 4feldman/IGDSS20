using System;
using System.Collections.Generic;
using UnityEngine;

public class JobManager : MonoBehaviour
{

    private List<Job> _availableJobs = new List<Job>();
    public List<Worker> _unoccupiedWorkers = new List<Worker>();
    private System.Random _rand = new System.Random();

    #region MonoBehaviour
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleUnoccupiedWorkers();
    }
    #endregion


    #region Methods

    private void HandleUnoccupiedWorkers()
    {
        if (_unoccupiedWorkers.Count > 0 && _availableJobs.Count > 0)
        {
            for (int i = 0; i < Math.Min(_unoccupiedWorkers.Count, _availableJobs.Count); i++)
            {
                Job j = _availableJobs[_rand.Next(_availableJobs.Count)];
                Worker w = _unoccupiedWorkers[i];
                j.AssignWorker(w);
                _availableJobs.Remove(j);
                _unoccupiedWorkers.Remove(w);
                w._happiness = 1f;
                w._job = j;
            }
        }
    }

    public void RegisterWorker(Worker w)
    {
        _unoccupiedWorkers.Add(w);
    }



    public void RemoveWorker(Worker w)
    {
        if(_unoccupiedWorkers.Contains(w))
        {
            _unoccupiedWorkers.Remove(w);
        }
        if (w._job != null)
        {
            _availableJobs.Add(w._job);
            w._job.RemoveWorker(w);
            HandleUnoccupiedWorkers();
        }
    }

    public void RegisterJob(Job j)
    {
        _availableJobs.Add(j);
        HandleUnoccupiedWorkers();
    }

    public void RemoveJob(Job j)
    {
        _availableJobs.Remove(j);
        if (j._worker != null)
        {
            _unoccupiedWorkers.Add(j._worker);
            j.RemoveWorker(j._worker);
            HandleUnoccupiedWorkers();
        }
    }

    #endregion
}

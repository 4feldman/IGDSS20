using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobManager : MonoBehaviour
{

    private List<Job> _availableJobs = new List<Job>();
    public List<Worker> _unoccupiedWorkers = new List<Worker>();
    private System.Random _rand = new System.Random();
    private List<Worker> _allWorkers = new List<Worker>();
    private Text _workerText;
    public GameManager _gameManager;

    #region MonoBehaviour
    // Start is called before the first frame update
    void Start()
    {
        _workerText = GameObject.Find("WorkerText").GetComponent<Text>();
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
                w.AssignJob(j);
            }
        }
    }

    public void RegisterWorker(Worker w)
    {
        _unoccupiedWorkers.Add(w);
        _allWorkers.Add(w);
        UpdateWorkerUI();
    }



    public void RemoveWorker(Worker w)
    {
        if(_unoccupiedWorkers.Contains(w))
        {
            _unoccupiedWorkers.Remove(w);
            _allWorkers.Remove(w);
            UpdateWorkerUI();
        }
        if (w._job != null)
        {
            _availableJobs.Add(w._job);
            w._job.RemoveWorker(w);
            _allWorkers.Remove(w);
            UpdateWorkerUI();
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

    private void UpdateWorkerUI()
    {
        if (!_gameManager.GameEnded)
        {
            _workerText.text = _allWorkers.Count.ToString();
            if (_allWorkers.Count >= 1000)
            {
                _gameManager.GameEnd(true);
            }
        }
    }

    #endregion
}

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HousingBuilding : Building
{
    public float _maxResidents;
    public GameObject _workerPrefab;
    
    private List<Worker> _residents = new List<Worker>();
    public float _efficiency;
    public float _birth;
    
    #region MonoBehaviour
    // Start is called before the first frame update, TODO remove if not needed
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame, TODO remove if not needed
    protected override void Update()
    {
        base.Update();
        _efficiency = _residents.Select(resident => resident._happiness).Average();
        if (_efficiency >= 1f)
        {
            _birth += Time.deltaTime;
        }

        if (_birth > 30)
        {
            Birth();
            _birth = 0;
        }

    }
    #endregion

    #region Initialization
    public override void Initialize(Tile tile, ResourceManager resourceManager, JobManager jobManager)
    {
        base.Initialize(tile, resourceManager, jobManager);
        Debug.Log("Housing");
        AddWorker(15f);
        AddWorker(15f);
    }
    #endregion

    public void AddWorker(float age)
    {
        Worker worker = Instantiate(_workerPrefab, transform.position, Quaternion.identity).GetComponent<Worker>();
        worker.Initialize(_resourceManager, _jobManager, age);
        _residents.Add(worker);
    }
    
    public void Birth()
    {
        if (_residents.Count >= _maxResidents) return;
        AddWorker(0f);
    }
}

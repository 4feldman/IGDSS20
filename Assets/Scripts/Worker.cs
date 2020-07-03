using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Worker : MonoBehaviour
{
    #region Manager References
    JobManager _jobManager; //Reference to the JobManager
    GameManager _gameManager;//Reference to the GameManager
    ResourceManager _resourceManager;
    #endregion

    public enum ageGroup
    {
        Child,
        Adult,
        Senior
    }

    public float _age = 0; // The age of this worker
    public float _happiness = 1f; // The happiness of this worker
    public float _tax = 0f;
    public float _speed = 10f;
    public ageGroup _group = ageGroup.Child;

    public Job _job = null;
    public HousingBuilding _home = null;
    
    public float _workingDuration = 15;
    public float _freeTime = 20;

    public float _remainingWait;

    private Tile _targetTile = null;
    private List<Tile> _commute = null;
    private int _currentTarget = 0;
    private Dictionary<ResourceManager.ResourceTypes, float> _consumptionRate = new Dictionary<ResourceManager.ResourceTypes, float>();
    private Dictionary<ResourceManager.ResourceTypes, float> _consumption = new Dictionary<ResourceManager.ResourceTypes, float>();
    
    private float _consumptionHappiness;
    
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(Tax), 10.0f, 10.0f);
    }

    // Update is called once per frame
    void Update()
    {
        updateAge();
        Consumption();
        Action();
    }

    public void Initialize(ResourceManager resourceManager, JobManager jobManager, HousingBuilding home)
    {
        Initialize(resourceManager, jobManager, 0f, home);
    }

    public void AssignJob(Job job)
    {
        _job = job;
        if (_job != null)
        {
            _commute = _home.generatePath(_targetTile);
        }
        else
        {
            _commute = _job._building.generatePath(_targetTile);
        }
        Walk();
        _remainingWait = Random.Range(0,1);
    }

    public void Initialize(ResourceManager resourceManager, JobManager jobManager, float age, HousingBuilding home)
    {
        _age = age;
        _jobManager = jobManager;
        _home = home;
        _targetTile = _home._tile;
        _consumptionRate.Add(ResourceManager.ResourceTypes.Fish, 1);
        _consumptionRate.Add(ResourceManager.ResourceTypes.Clothes, 1);
        _consumptionRate.Add(ResourceManager.ResourceTypes.Schnapps, 0);
        _resourceManager = resourceManager;
        
        foreach (KeyValuePair<ResourceManager.ResourceTypes,float> resource in _consumptionRate)
        {
            _consumption.Add(resource.Key, 1f);
        }
        
        Age();
    }

    private void Consumption()
    {
        bool consumptionFulfilled = true;
        foreach (KeyValuePair<ResourceManager.ResourceTypes,float> resource in _consumptionRate)
        {
            if (_consumption[resource.Key] <= 0)
            {
                if (!_resourceManager.removeResource(resource.Key, 1))
                {
                    _consumptionHappiness -= Time.deltaTime * 0.01f;
                    consumptionFulfilled = false;
                }
                else
                {
                    _consumption[resource.Key] = 1;
                }
            }
            _consumption[resource.Key] -= _consumptionRate[resource.Key] * Time.deltaTime / 10;
        }

        if (_consumptionHappiness < 0f) _consumptionHappiness = 0;
        if (consumptionFulfilled) _consumptionHappiness = 1f;

        if (_job == null && _group == ageGroup.Adult)
        {
            float happiness = _consumptionHappiness - 0.4f;
            if (happiness < 0)
            {
                _happiness = 0f;
                return;
            }

            _happiness = happiness;
            return;
        }
        _happiness = _consumptionHappiness;
    }

    private void updateAge()
    {
        _age += Time.deltaTime / 15;
        //A life cycle, where a Worker ages by 1 year every 15 real seconds.
        //When becoming of age, the worker enters the job market, and leaves it when retiring.
        //Eventually, the worker dies and leaves an empty space in his home. His Job occupation is also freed up.

        Age();
    }
    
    private void Age()
    {
        if (_age > 14 && _group != ageGroup.Adult)
        {
            BecomeOfAge();
        }

        if (_age > 64 && _group != ageGroup.Senior)
        {
            Retire();
        }

        if (_age > 100)
        {
            Die();
        }
    }


    public void BecomeOfAge()
    {
        _group = ageGroup.Adult;
        _jobManager.RegisterWorker(this);
        _consumptionRate[ResourceManager.ResourceTypes.Schnapps] = 1;
        _consumptionRate[ResourceManager.ResourceTypes.Fish] = 2;
        _consumptionRate[ResourceManager.ResourceTypes.Clothes] = 2;
        _tax = 5;
        transform.localScale = new Vector3(1, 1, 1);
    }

    private void Retire()
    {
        _group = ageGroup.Senior;
        _jobManager.RemoveWorker(this);
        _job = null;
        _commute = _home.generatePath(_targetTile);
        _currentTarget = 0;
        _consumptionRate[ResourceManager.ResourceTypes.Schnapps] = 2;
        _consumptionRate[ResourceManager.ResourceTypes.Fish] = 1;
        _consumptionRate[ResourceManager.ResourceTypes.Clothes] = 1;
        _tax = 3;
    }

    private void Die()
    {
        Destroy(this.gameObject, 1f);
    }

    private void Tax()
    {
        if (_job == null)
        {
            _resourceManager.removeMoney(_tax);
        }
        else
        {
            _resourceManager.addMoney(_tax);
        }
    }

    private void Action()
    {
        if (_remainingWait > 0)
        {
            _remainingWait -= Time.deltaTime;
            return;
        }
        Walk();
    }
    
    public void Walk()
    {
        if (_commute == null)
        {
            return;
        }
        
        if (Vector3.Distance(transform.position, _targetTile.transform.position) == 0f)
        {
            if (_job != null && _commute[_currentTarget] == _job._building._tile) // At Work
            {
                _remainingWait = _workingDuration + Random.Range(0, 4);
                _commute = _home.generatePath(_commute[_currentTarget]);
                _currentTarget = 0;
            }
            else if (_commute[_currentTarget] == _home._tile) // At Home
            {
                _remainingWait = _freeTime + Random.Range(0, 2);
                if (_job == null)
                {
                    _commute = null;
                    _currentTarget = 0;
                    return;
                }
                _commute = _job._building.generatePath(_commute[_currentTarget]);
                _currentTarget = 0;
            }
            
            _currentTarget += 1;
            _targetTile = _commute[_currentTarget];
            transform.LookAt(_targetTile.transform.position);
        }
        transform.position = Vector3.MoveTowards(transform.position, _targetTile.transform.position,
            _speed * Time.deltaTime);
    }
}

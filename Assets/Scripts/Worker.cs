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
    public ageGroup _group = ageGroup.Child;

    public Job _job = null;

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
        Age();
        Consumption();
    }

    public void Initialize(ResourceManager resourceManager, JobManager jobManager)
    {
        Initialize(resourceManager, jobManager, 0f);
    }

    public void Initialize(ResourceManager resourceManager, JobManager jobManager, float age)
    {
        _age = age;
        _jobManager = jobManager;
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
        // TODO: Implement JobManager
        _jobManager.RegisterWorker(this);
        _consumptionRate[ResourceManager.ResourceTypes.Schnapps] = 1;
        _consumptionRate[ResourceManager.ResourceTypes.Fish] = 2;
        _consumptionRate[ResourceManager.ResourceTypes.Clothes] = 2;
        _tax = 5;
    }

    private void Retire()
    {
        _group = ageGroup.Senior;
        _jobManager.RemoveWorker(this);
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
}

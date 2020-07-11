using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{    
    public enum ResourceTypes { None, Fish, Wood, Planks, Wool, Clothes, Potato, Schnapps };

    public Dictionary<ResourceTypes, float> _resourcesInWarehouse =
        new Dictionary<ResourceTypes, float>(); //Holds a number of stored resources for every ResourceType

    public float _bank = 1000f;
    public int _constantIncome = 100;

    private Text _moneyText;

    //A representation of _resourcesInWarehouse, broken into individual floats. Only for display in inspector, will be removed and replaced with UI later
    [SerializeField] private float _ResourcesInWarehouse_Fish;
    [SerializeField] private float _ResourcesInWarehouse_Wood;
    [SerializeField] private float _ResourcesInWarehouse_Planks;
    [SerializeField] private float _ResourcesInWarehouse_Wool;
    [SerializeField] private float _ResourcesInWarehouse_Clothes;
    [SerializeField] private float _ResourcesInWarehouse_Potato;
    [SerializeField] private float _ResourcesInWarehouse_Schnapps;

    void Start()
    {
        _moneyText = GameObject.Find("MoneyText").GetComponent<Text>();
        _resourcesInWarehouse.Add(ResourceTypes.None, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Fish, 100);
        _resourcesInWarehouse.Add(ResourceTypes.Wood, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Planks, 100);
        _resourcesInWarehouse.Add(ResourceTypes.Wool, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Clothes, 100);
        _resourcesInWarehouse.Add(ResourceTypes.Potato, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Schnapps, 100);
        InvokeRepeating(nameof(Income), 60.0f, 60.0f);
    }

    private void Update()
    {
        UpdateInspectorNumbersForResources();
    }

    #region Money

    private void Income()
    {
        _bank += _constantIncome;
        UpdateMoneyUI();
    }

    public float moneyCount()
    {
        return _bank;
    }

    public void addMoney(float amount)
    {
        _bank += amount;
        UpdateMoneyUI();
    }

    public void removeMoney(float amount)
    {
        _bank -= amount;
        UpdateMoneyUI();
    }

    private void UpdateMoneyUI()
    {
        _moneyText.text = ((int)_bank).ToString();
    }

    #endregion

    #region Resource

    //Updates the visual representation of the resourceManager dictionary in the inspector. Only for debugging
    void UpdateInspectorNumbersForResources()
    {
        _ResourcesInWarehouse_Fish = _resourcesInWarehouse[ResourceTypes.Fish];
        _ResourcesInWarehouse_Wood = _resourcesInWarehouse[ResourceTypes.Wood];
        _ResourcesInWarehouse_Planks = _resourcesInWarehouse[ResourceTypes.Planks];
        _ResourcesInWarehouse_Wool = _resourcesInWarehouse[ResourceTypes.Wool];
        _ResourcesInWarehouse_Clothes = _resourcesInWarehouse[ResourceTypes.Clothes];
        _ResourcesInWarehouse_Potato = _resourcesInWarehouse[ResourceTypes.Potato];
        _ResourcesInWarehouse_Schnapps = _resourcesInWarehouse[ResourceTypes.Schnapps];
    }

    public float resourceCount(ResourceTypes resource)
    {
        return _resourcesInWarehouse[resource];
    }

    public void addResource(ResourceTypes resource, float amount)
    {
        _resourcesInWarehouse[resource] += amount;
    }

    public bool removeResource(ResourceTypes resource, float amount)
    {
        return removeResource(resource, amount, false) == amount;
    }

    public bool removeResource(Dictionary<ResourceTypes, float> resources)
    {
        foreach (KeyValuePair<ResourceTypes, float> resource in resources)
        {
            if (_resourcesInWarehouse[resource.Key] < resource.Value)
            {
                return false;
            }
        }

        foreach (KeyValuePair<ResourceTypes, float> resource in resources)
        {
            _resourcesInWarehouse[resource.Key] -= resource.Value;
        }

        return true;
    }

    public float removeResource(ResourceTypes resource, float amount, bool takeLess)
    {
        if (_resourcesInWarehouse[resource] < amount && !takeLess)
        {
            return 0;
        }

        if (_resourcesInWarehouse[resource] < amount && !takeLess)
        {
            float returnValue = _resourcesInWarehouse[resource];
            _resourcesInWarehouse[resource] -= returnValue;
            return returnValue;
        }

        _resourcesInWarehouse[resource] -= amount;
        return amount;
    }

    #endregion

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public static OnItemChanged onChanged;
    public int maxNumberOfItems = 200;

    public List<Item> items = new List<Item>();
    public delegate void OnItemChanged();

    #region Singleton
    public static Inventory instance;
    void Awake()
    {
        if(instance != null)
        {
            if(instance != this)
                Destroy(gameObject);
        } else
        {
            instance = this;
        }
    }
    #endregion

    // Use this for initialization
    void Start () {
		if(onChanged != null)
        {
            onChanged();
        }
	}
    
    //TODO: Add stackable items
    public bool Add(Item item)
    {
        if(items.Count >= maxNumberOfItems)
        {
            return false;
        }

        items.Add(item);

        if (onChanged != null)
        {
            onChanged();
        }

        return true;
    }

    public void Remove(Item item)
    {
        items.Remove(item);
        if (onChanged != null)
        {
            onChanged();
        }
    }

    public bool HasItem(Item item)
    {
        return items.Contains(item);
    }

    public bool HasItem(string name)
    {
        bool result = false;
        items.ForEach(item => {
            Debug.Log(item.name + " " + name);
           if( item != null && item.name == name )
            {
                result = true;
            }
        });

        return result;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat {

    [SerializeField]
    private float baseValue;
    private List<float> modifiers;

    public Stat(float val)
    {
        baseValue = val;
        modifiers = new List<float>();
    }

    public Stat()
    {
        modifiers = new List<float>();
    }

    public float GetValue()
    {
        float finalValue = baseValue;

        modifiers.ForEach(modifier => finalValue += modifier);

        return finalValue;
    }

    public float GetBase()
    {
        return baseValue;
    }

    public void AddModifier(float modifier)
    {
        if(modifier != 0)
        {
            modifiers.Add(modifier);
        }
    }

    public void RemoveModifier(float modifier)
    {
        if (modifier != 0)
        {
            modifiers.Remove(modifier);
        }
    }

    public static float operator *(Stat stat, float f)
    {
        return stat.GetValue() * f;
    }

    public static float operator *(Stat stat, int f)
    {
        return stat.GetValue() * f;
    }

    public static float operator *(float f, Stat stat)
    {
        return stat.GetValue() * f;
    }

    public static float operator *(int f, Stat stat)
    {
        return stat.GetValue() * f;
    }

    public static implicit operator float(Stat stat)
    {
        return stat.GetValue();
    }

    public static implicit operator Stat(float f)
    {
        return new Stat(f);
    }

    public static implicit operator Stat(int i)
    {
        return new Stat((float) i);
    }

}

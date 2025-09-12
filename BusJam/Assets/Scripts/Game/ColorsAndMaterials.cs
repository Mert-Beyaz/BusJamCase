using System;
using System.Collections.Generic;
using UnityEngine;

public class ColorsAndMaterials : MonoBehaviour
{
    public static ColorsAndMaterials Instance;

    [SerializeField] private List<ColorAndMaterial> colorAndMaterials = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Material GetColorInfo(ColorEnums color)
    {
        foreach (var c in colorAndMaterials)
        {
            if (c.ColorEnum.Equals(color))
            {
                return c.Material;
            }
        }
        return null;
    }
}

[Serializable]
public class ColorAndMaterial
{
    public ColorEnums ColorEnum;
    public Material Material;
}
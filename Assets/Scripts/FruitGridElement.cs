// Author(s): Paul Calande
// An element of the Fruit Gunch grid.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FruitGridElement : MonoBehaviour
{
    [SerializeField]
    [Tooltip("")]
    Text textName;

    public void SetName(string newName)
    {
        textName.text = newName;
    }

    public void SetColor(Color newColor)
    {
        textName.color = newColor;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamerObjectPicker : MonoBehaviour
{
    public enum ObjectType
    {
        Gun,
        Wall
    }

    public static ObjectType jank = ObjectType.Gun;

    public void SetType(ObjectType newType)
    {
        jank = newType;
    }

    public void SetGun()
    {
        SetType(ObjectType.Gun);
    }

    public void SetWall()
    {
        SetType(ObjectType.Wall);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamerObjectPicker : MonoBehaviour
{
    public enum ObjectType
    {
        Gun,
        Wall,
        Clear
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

    public void SetClear()
    {
        SetType(ObjectType.Clear);
    }

    private void CheckKey(KeyCode kc, ObjectType newType)
    {
        if (Input.GetKeyDown(kc))
        {
            SetType(newType);
        }
    }

    private void Update()
    {
        CheckKey(KeyCode.W, ObjectType.Wall);
        CheckKey(KeyCode.E, ObjectType.Clear);
        CheckKey(KeyCode.R, ObjectType.Gun);
    }
}
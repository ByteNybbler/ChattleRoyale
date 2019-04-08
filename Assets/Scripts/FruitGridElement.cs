// Author(s): Paul Calande
// An element of the Fruit Gunch grid.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FruitGridElement : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the player name text in this cell.")]
    Text textName;
    [SerializeField]
    [Tooltip("Reference to the gun GameObject in this cell.")]
    GameObject gun;
    [SerializeField]
    [Tooltip("Reference to the player gun GameObject.")]
    GameObject playerGun;
    [SerializeField]
    [Tooltip("Reference to the player icon.")]
    Image playerIcon;
    [SerializeField]
    [Tooltip("")]
    GameObject wall;

    // Whether the grid element is occupied by a solid object.
    bool isOccupied = false;

    bool hasGun = false;

    private void Start()
    {
        SetGun(false);
        SetPlayerGun(false);
        SetColor(new Color(0, 0, 0, 0));
        SetName("");

        wall.SetActive(false);
    }

    public void SetName(string newName)
    {
        textName.text = newName;
    }

    public void SetColor(Color newColor)
    {
        //textName.color = newColor;
        playerIcon.color = newColor;
    }

    public bool IsOccupied()
    {
        return isOccupied;
    }

    public void SetOccupied(bool isOccupied)
    {
        this.isOccupied = isOccupied;
    }

    public void SetGun(bool hasGun)
    {
        if (hasGun && isOccupied)
        {
            //Debug.Log("Can't place gun there!");
            return;
        }

        this.hasGun = hasGun;
        gun.SetActive(hasGun);

        /*
        if (hasGun)
        {
            Debug.Log("Gun activated.");
        }
        */

        /*
        if (hasGun && this.hasGun)
        {
            this.hasGun = false;
            gun.SetActive(false);
            wall.SetActive(true);
            SetOccupied(true);
        }
        else
        {
            this.hasGun = hasGun;
            gun.SetActive(hasGun);
            wall.SetActive(false);
        }
        */
    }

    public bool HasGun()
    {
        return hasGun;
    }

    public void SetPlayerGun(bool hasPlayerGun)
    {
        playerGun.SetActive(hasPlayerGun);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
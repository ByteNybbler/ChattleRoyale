// Author(s): Paul Calande
// TODO: Refactor!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionToDestroy2D : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The group of acceptable tags.")]
    SOTagGroup tags;

    // TODO: Revise this comment.
    // Returns true if the given collision can be accepted by the trigger at this time.
    private bool IsValid(string otherTag)
    {
        return tags.IsValid(otherTag);
    }

    // TODO: Weirdness with OnCollisionEnter2D not working at all?
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("CollisionToDestroy2D collision detected!");
        if (IsValid(collision.gameObject.tag))
        {
            Destroy(gameObject);
        }
    }
}
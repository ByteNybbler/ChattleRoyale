// Author(s): Paul Calande
// Resets the scene using the service locator.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneResetter : MonoBehaviour
{
    public void Fire()
    {
        ServiceLocator.GetSceneTracker().RestartScene();
    }
}
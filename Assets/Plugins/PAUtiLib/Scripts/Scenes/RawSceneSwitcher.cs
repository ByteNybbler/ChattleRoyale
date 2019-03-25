// Author(s): Paul Calande
// Switches scenes synchronously using Unity's default functionality.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RawSceneSwitcher : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The name of the new scene to switch to.")]
    string newScene;

    public void Fire()
    {
        SceneManager.LoadScene(newScene);
    }
}
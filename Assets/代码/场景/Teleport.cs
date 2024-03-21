using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private SceneName sceneName;
    [SerializeField] private Vector3 playerSpawanPosition;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        
        if (player != null)
        {
            SceneControllerManager.Instance.FadeAndLoadScene(sceneName.ToString(), playerSpawanPosition);
        }
        
    }
}

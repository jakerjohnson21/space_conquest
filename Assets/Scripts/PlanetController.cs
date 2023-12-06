using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    public GameObject unitPrefab;
    public float spawnInterval = 2f;
    public float timeSinceLastUnitGeneration = 0f;
    public float orbitSpeed = 30f; 
    private List<GameObject> units = new List<GameObject>();
    public string planetOwner = "";

    private void Update() {
        if (planetOwner != "") {
            timeSinceLastUnitGeneration += Time.deltaTime;
            if (timeSinceLastUnitGeneration >= spawnInterval) {
                SpawnUnit();
                timeSinceLastUnitGeneration -= spawnInterval;
            }
        }
    }

    private void SpawnUnit() {
        GameObject unit = Instantiate<GameObject>(unitPrefab);
        unit.transform.parent = transform;
        units.Add(unit);
    }








}

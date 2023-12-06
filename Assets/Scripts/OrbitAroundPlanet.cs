using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitAroundPlanet : MonoBehaviour
{
    public float orbitSpeed = 1.0f;
    public float orbitDistance = 1.5f;
    public Transform planet;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(planet.position, Vector3.forward, orbitSpeed * Time.deltaTime);
        transform.localPosition = new Vector3(0.0f, orbitDistance, 0.0f);
    }
}

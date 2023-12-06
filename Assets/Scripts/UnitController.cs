using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{

    public float orbitSpeed = 50f;
    private Transform planet;

    // Start is called before the first frame update
    void Start()
    {
        planet = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(planet.position, new Vector3(0,0,1), -orbitSpeed * Time.deltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PUnitController : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sr;

    [SerializeField] private string unitState = "orbiting";
    [SerializeField] private float unitSpeed = 5f;
    [SerializeField] private string currentPlanet;
    [SerializeField] private float orbitSpeed = 75f;
    [SerializeField] private Vector3 currentDestination;

    private Vector3 parentPosition;
    public string parentTag;
    public string owner;

    private Animator anim;

    enum UnitState { orbiting, traveling }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        parentPosition = transform.parent.position;
        parentTag = transform.parent.gameObject.tag; 

        if (owner != "Player")
        {
            sr.color = Color.red;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (unitState == "traveling")
        {
            travel();

            //Stop traveling when destination is reached
            if (transform.position == currentDestination)
            {
                unitState = "orbiting";

                PPlanetController pc = transform.parent.GetComponent<PPlanetController>();
                pc.addUnit(transform.gameObject);
            }
        }
        else
        {
            transform.RotateAround(parentPosition, new Vector3(0, 0, 1), orbitSpeed * Time.deltaTime);
        }




        //2 States: Orbiting and Traveling
    }

    public void setOwner(string newOwner)
    {
        owner = newOwner;
    }

    public string getOwner()
    {
        return owner;
    }

    public void setDestination(Vector3 destination)
    {
        //Update location of parent planet center position
        parentPosition = destination;

        //Generate a destination with a random distance from parent planet
        float xDif;
        float yDif;
        while(true)
        {
            xDif = Random.Range(-1.8f, 1.8f);
            yDif = Random.Range(-1.8f, 1.8f);
            
            if (Mathf.Abs(xDif) > 0.7f && Mathf.Abs(yDif) > 0.7f)
            {
                break;
            } 
        }

        //Update currentDestination with randomized values around parent planet.
        currentDestination = new Vector3(destination.x + xDif, destination.y + yDif, 0);

        //Update unitState to begin traveling
        unitState = "traveling";
    }

    private void travel()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentDestination, unitSpeed * Time.deltaTime);
    }

    public void Explode()
    {
        anim.SetTrigger("explode");
    }


}




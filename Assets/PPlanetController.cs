using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Random;
using UnityEngine.EventSystems;
using TMPro;

public class PPlanetController : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private TextMeshProUGUI planetHealthText;

    public float captureInterval = 2f;
    public float timeSinceLastCaptureIncrement = 0f;

    public float spawnInterval = 4f;
    public float timeSinceLastUnitGeneration = 0f;

    public float fightInterval = 0.75f;
    public float timeSinceLastFight = 0f;

    private int maxUnits = 10;

    public int ownUnitCount = 0;
    public int enemyUnitCount = 0;

    public string enemyArmy1;
    public string enemyArmy2;

    List<string> temp = new List<string>();
    List<string> armies = new List<string>();


    [SerializeField] private bool selected = false;

    [SerializeField] private int planetHealth = 25;
    [SerializeField] private float unitGenRate = 1f;
    public bool fightEngaged = false;
    public bool capturingEngaged = false;
    private bool fightAlt = false;
    [SerializeField] private string owner = "Player";
    [SerializeField] private GameObject unitPrefab;

    [SerializeField] private List<GameObject> units = new List<GameObject>();

    public class PlanetInfo
    {
        public int planetHealth;
        public int ownUnitCount;
        public int enemyUnitCount;
        public bool isFighting;
        public bool isCapturing;
        public string owner;

        public PlanetInfo(int planetHealth, int ownUnitCount, int enemyUnitCount, bool isFighting, bool isCapturing, string owner)
        {
            this.planetHealth = planetHealth;
            this.ownUnitCount = ownUnitCount;
            this.enemyUnitCount = enemyUnitCount;
            this.isFighting = isFighting;
            this.isCapturing = isCapturing;
            this.owner = owner;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        updatePlanetColor();

        checkOrbitingUnits();

        if (fightEngaged)
        {
            timeSinceLastFight += Time.deltaTime;

            if (timeSinceLastFight >= fightInterval)
            {
                fight();
                timeSinceLastFight -= fightInterval;
            }
        } else if (capturingEngaged)
        {
            timeSinceLastCaptureIncrement += Time.deltaTime;

            if (timeSinceLastCaptureIncrement >= captureInterval)
            {
                capture();
                timeSinceLastCaptureIncrement -= captureInterval;
            }
        }
        else if (owner != "")
        {
            timeSinceLastUnitGeneration += Time.deltaTime;

            if (timeSinceLastUnitGeneration >= spawnInterval && owner != "" && !fightEngaged && units.Count <= maxUnits)
            {
                SpawnUnit();

                if (planetHealth != 10)
                {
                    planetHealth++;
                }

                timeSinceLastUnitGeneration -= spawnInterval;
            }

        }

        updateText();
    }

    //Spawns unit around planet
    private void SpawnUnit()
    {
        GameObject unit = Instantiate<GameObject>(unitPrefab, new Vector3(transform.position.x + UnityEngine.Random.Range(0.5f,1.5f), transform.position.y + UnityEngine.Random.Range(0.5f, 1.5f)), transform.rotation);
        unit.transform.parent = transform;

        PUnitController unitController = unit.GetComponent<PUnitController>();
        if (unitController != null)
        {
            unitController.setOwner(owner);
        }

        units.Add(unit);
    }

    //Returns name of planet owner
    public string getOwner()
    {
        return owner;
    }

    //Send units to destination. Update their parent to the destination planet. Remove unit from current planet unit list. Add unit to destination planet unit list.
    public void sendUnits(Transform newPlanet, Vector3 destination)
    { 
        int counter = 0;
        while (transform.childCount > counter)
        {
            Transform child = transform.GetChild(transform.childCount - 1);
            PUnitController childCtrl = child.GetComponent<PUnitController>();

            if (childCtrl.getOwner() == owner)
            {
                child.parent = newPlanet;
                PUnitController childController = child.gameObject.GetComponent<PUnitController>();
                childController.setDestination(destination);
                units.Remove(child.gameObject);
            } else
            {
                counter++;
            }
            
        }
    }

    //Method to flip selected status
    public void setSelected()
    {
        if (selected == false)
        {
            selected = true;
        } else
        {
            selected = false;
        }
    }

    //Update planet color based off owner
    private void updatePlanetColor()
    {
        if (owner == "")
        {
            sr.color = Color.gray;
        }
        else if (owner == "Player")
        {
            if (selected)
            {
                sr.color = Color.white;
            }
            else
            {
                sr.color = Color.blue;
            }
        }
        else
        {
            sr.color = Color.red;
        }
    }

    //Add's a unit object to list of units for this planet
    public void addUnit(GameObject newUnit)
    {
        units.Add(newUnit);
    }

    //Checks if there are enemy ships on the planet and updates planet state accordingly
    private void checkOrbitingUnits()
    {
        ownUnitCount = 0;
        enemyUnitCount = 0;
        foreach(GameObject unit in units)
        {
            if (unit.GetComponent<PUnitController>().getOwner() == owner)
            {
                ownUnitCount++;
            }
            else {
                enemyUnitCount++;
            }
        }

        if (enemyUnitCount == 0)
        {
            fightEngaged = false;
            capturingEngaged = false;
        }
        else if (enemyUnitCount > 0 && ownUnitCount == 0)
        {
            if (getPlanetArmies().Count > 1)
            {
                fightEngaged = true;
                capturingEngaged = false;
            } else
            {
                fightEngaged = false;
                capturingEngaged = true;
            }
        } else if (enemyUnitCount > 0 && ownUnitCount > 0)
        {
            fightEngaged = true;
            capturingEngaged = false;
        }
    }

    //Perform one 'fight' between the ships. Alternates back and forth between owner of ship
    private void fight()
    {
        if (owner == "")
        {
            armies = getPlanetArmies();
            enemyArmy1 = armies[0];

            foreach (GameObject unit in units)
            {
                if (unit.GetComponent<PUnitController>().getOwner() != enemyArmy1 && fightAlt == false)
                {
                    units.Remove(unit);
                    Destroy(unit);
                    fightAlt = !fightAlt;
                    return;

                }
                else if (unit.GetComponent<PUnitController>().getOwner() == enemyArmy1 && fightAlt == true)
                {
                    units.Remove(unit);
                    Destroy(unit);
                    fightAlt = !fightAlt;
                    return;
                }
            }

        } else
        {
            foreach (GameObject unit in units)
            {
                if (unit.GetComponent<PUnitController>().getOwner() != owner && fightAlt == false)
                {
                    units.Remove(unit);
                    Destroy(unit);
                    fightAlt = !fightAlt;
                    return;

                }
                else if (unit.GetComponent<PUnitController>().getOwner() == owner && fightAlt == true)
                {
                    units.Remove(unit);
                    Destroy(unit);
                    fightAlt = !fightAlt;
                    return;
                }
            }
        }
    }

    private void capture()
    {
        planetHealth -= 1;

        if (planetHealth == 0)
        {
            PUnitController capturingUnit = units[0].GetComponent<PUnitController>();
            string newOwner = capturingUnit.getOwner();
            owner = newOwner;
            planetHealth = 10;
        }
    }

    public PlanetInfo getPlanetInfo()
    {
        return new PlanetInfo(planetHealth, ownUnitCount, enemyUnitCount, fightEngaged, capturingEngaged, owner);
    }

    private void updateText()
    {
        if (capturingEngaged || planetHealth < 10)
        {
            planetHealthText.text = "" + planetHealth;
        } else
        {
            planetHealthText.text = "";
        }
        
    }

    public List<string> getPlanetArmies()
    {
        temp.Clear();
        foreach (GameObject unit in units)
        {
            PUnitController uCtrl = unit.GetComponent<PUnitController>();
            string ownerName = uCtrl.getOwner();

            if (!(temp.Contains(ownerName)))
            {
                temp.Add(ownerName);
            }


        }

        return temp;
    }
}

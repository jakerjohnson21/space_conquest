using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{

    public float moveInterval = 8f;
    public float timeSinceLastMove = 0f;

    [SerializeField] private TextMeshProUGUI levelCompleteText;

    private GameObject selectedPlanetObject = null;
    private PPlanetController selectedPlanetController = null;
    private GameObject[] planets;

    public bool gameRunning = true;

    private SpriteRenderer sr;


    GameObject aiSelectedPlanet = null;
    GameObject aiDestinationPlanet = null;


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

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        planets = GameObject.FindGameObjectsWithTag("Planet");

    }

    void Update()
    {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 origin = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
                RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero, 50f);

                if (hit && hit.transform.gameObject.CompareTag("Planet"))
                {
                    GameObject newPlanetObject = hit.transform.gameObject;
                    PPlanetController newSelection = hit.transform.gameObject.GetComponent<PPlanetController>();

                    if (selectedPlanetObject == null)
                    {
                        //If no planet is selected, then select planet if it belongs to Player
                     
                        if (newSelection.getOwner() == "Player")
                        {
                            selectedPlanetController = newSelection; //Update stored selected planet
                            selectedPlanetObject = hit.transform.gameObject;
                            
                            selectedPlanetController.setSelected(); //Use PlanetController script to update planet 'selected' status
                        }

                    }
                    else
                    {
                        //If planet is already selected, and another planet is clicked then send units
                        //If the same planet is clicked again then clear selection, if another planet is selected send units.
                        if (selectedPlanetObject.name != newPlanetObject.name) //If selected planet does NOT get clicked again
                        {
                            selectedPlanetController.sendUnits(hit.transform, hit.transform.position);
                            ClearSelectedPlanet();
                        }
                        else
                        {
                            ClearSelectedPlanet();
                        }
                    }
            }
            else //No hit on planet
            {
                ClearSelectedPlanet();
            }
            }

            else
            {

            timeSinceLastMove += Time.deltaTime;

            if (timeSinceLastMove >= moveInterval)
                {
                    aiMove();
                    timeSinceLastMove -= moveInterval;
                }
            }


            string tempName = checkGameStatus();

            if (gameRunning == false)
            {
                if (tempName == "Player")
                {
                    levelCompleteText.text = "Level Complete";
                    Invoke("loadNextLevel", 3f);
                } else
                {
                    levelCompleteText.text = "You Lose";
                    Invoke("loadMainMenu", 3f);
                }
                
            } else
            {
                levelCompleteText.text = "";
            }

    }

    private void aiMove()
    {
        

        List<GameObject> botPlanets = new List<GameObject>();
        int highestTroops = 0;

        
        //Make list of planets in AI control
        foreach (GameObject planet in planets)
        {
            PPlanetController pctrl = planet.GetComponent<PPlanetController>();
            if (pctrl.getOwner() == "Bot")
            {
                botPlanets.Add(planet);
            }
        }

        if (botPlanets.Count == 1)
        {
            aiSelectedPlanet = botPlanets[0];
        } else if (botPlanets.Count > 1)
        {
            //Find AI planet with most troops
            int ownHighestTroops = 0;
            int planetTroops;

            foreach (GameObject planet in botPlanets)
            {
                planetTroops = planet.GetComponent<PPlanetController>().getPlanetInfo().ownUnitCount;

                if (planetTroops > ownHighestTroops)
                {
                    ownHighestTroops = planetTroops;
                    aiSelectedPlanet = planet;
                }
            }
        } else
        {
            return;  //Return if no units owned
        }

        
        //Look at planets to see if one is fighting
        foreach (GameObject planet in planets)
        {
            if (planet.GetComponent<PPlanetController>().getPlanetInfo().isFighting == true)
            {
                aiDestinationPlanet = planet;
                aiSendTroops(aiSelectedPlanet, aiDestinationPlanet);
                return;
            }
        }

        
        //Look at all planets to see if one is empty
        foreach (GameObject planet in planets)
        {
            if (planet.GetComponent<PPlanetController>().getPlanetInfo().owner == "")
            {
                aiDestinationPlanet = planet;
                aiSendTroops(aiSelectedPlanet, aiDestinationPlanet);
                return;
            }
        }

        int lowestUnitCount = 1000;

        
        //Look at all Player planets and find one with lowest troops
        foreach (GameObject planet in planets)
        {
            string planetOwner = planet.GetComponent<PPlanetController>().getPlanetInfo().owner;
            int planetUnits = planet.GetComponent<PPlanetController>().getPlanetInfo().ownUnitCount;
            
            if (planetOwner == "Player")
            {
                if (planetUnits < lowestUnitCount)
                {
                    lowestUnitCount = planetUnits;
                    aiDestinationPlanet = planet;
                }
            }
        }

        if (aiDestinationPlanet != null)
        {
            aiSendTroops(aiSelectedPlanet, aiDestinationPlanet);
        }
        
    }

    private void aiSendTroops(GameObject selectedPlanet, GameObject destination)
    {
        PPlanetController ctrl = selectedPlanet.GetComponent<PPlanetController>();
        ctrl.sendUnits(destination.transform, destination.transform.position);
    }

    private string checkGameStatus()
    {
        List<string> owners = new List<string>();

        foreach(GameObject planet in planets)
        {
            PPlanetController pctrl = planet.GetComponent<PPlanetController>();
            owners.Add(pctrl.getOwner());
        }

        string temp = owners[0];
        for (int x = 1; x < owners.Count; x++)
        {
            if (temp != owners[x])
            {
                gameRunning = true;
                return "";
            }
        }

        if (owners[0] == "Player")
        {
            gameRunning = false;
            return owners[0];
        }
        else
        {
            gameRunning = false;
            return owners[0];
        }
    }

    private void ClearSelectedPlanet()
        {
            if (selectedPlanetObject != null)
            {
                selectedPlanetController.setSelected(); //If planet was selected, have it set to deselected

                selectedPlanetController = null;  //Remove stored selected planet
                selectedPlanetObject = null;
            }
        }

    public void loadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void loadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}


using UnityEngine;
using System.Collections;

public class BikeScript2 : MonoBehaviour
{
    #region instance variables
    //turn counter is used to animate the turn action. It is also used
    //as a cooldown to prevent turning too often
    int turnCounter;
    int turnDirection;
    //keeps track of how long our tail can be
    int tailLength;
    //if this is true, the object is ready to be destroyed
    private bool timeToDestroy;
    //lets us know if we've hit something and need to start destruction
    private bool collisionHappened;
    //an array to hold our rendering components for animations
    Component[] renderers;

	//holds the player's score; based on number of Pickup objects collected
	private int score;
	public GUIText scoreText;

    //a color to help set the bikes transparency
    Color color;
    //some constants to tell us what direction to turn
    private static int LEFT = -1;
    private static int RIGHT = 1;
    //how many frames it takes to perform a turn
    private static int TURNFRAMES = 18;
    private static int TURNCOOLDOWN = 26;
    //the speed of our bike
    private static float SPEED = .4f;
    
    //objects to help with the wall creation and handling
    //the prefab that each wall is a clone of
    public GameObject wallPrefab;
    //the queue we're holding all the wall chunks in
    Queue wallQueue;
    //the controller of our current tail
    wallScript currentWallController;
    //the current wall segment being made behind our bike
    static GameObject currentWall;
    //the wall that is currently shrinking, the oldest wall
    wallScript oldestWall;
    #endregion

    // Use this for initialization
		void Start ()
		{
			score = 0;
			scoreText.text = "Boxes Munched: " + score;
            turnCounter = 0;
            tailLength = 150;
            //the rendering components
            renderers = GetComponentsInChildren(typeof(MeshRenderer));

            wallQueue = new Queue();
            //instantiate our first wall and add it to the queue
            //set that wall to be our current tail (the one directly behind the player)
            currentWall = Instantiate(wallPrefab, transform.position, transform.rotation) as GameObject;
            currentWallController = currentWall.GetComponent<wallScript>();
            //throw the current wall in the queue
            wallQueue.Enqueue(currentWall.GetComponent<wallScript>());
            //remember to tell our current wall when to start dissapearing
            currentWallController.delay(tailLength);
            oldestWall = (wallScript)wallQueue.Dequeue();
		}
	
		
		// Update is called once per frame
		void FixedUpdate ()
        {
            #region turning and moving
            //only move stuff if we haven't crashed
            if (!collisionHappened)//(!collisionHappened)
            {
                #region turn controls & moving
                //make sure we can turn before we register the keys to do so
                if (turnCounter == 0)
                {
                    //set the direction and set the turn counter to manage turn animation and cooldown
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        turnCounter = TURNCOOLDOWN;
                        turnDirection = LEFT;
                    }
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        turnCounter = TURNCOOLDOWN;
                        turnDirection = RIGHT;
                    }
                }

                //allow the bike to move forward, but don't allow
                //for it to turn until turnCounter=0
                if (turnCounter <= 5)
                {
                    //move the bike forward
                    transform.Translate(0.0f, 0.0f, SPEED);
                }
                #endregion

                #region turn handler (animation and actions taken when turning)
                //if turnCounter is >0, we're in the process of a turn animation
                //rotate the bike and reduce the counter
                if (turnCounter > 0)
                {
                    //stop our walls from changing size while we turn
                    if (turnCounter == TURNCOOLDOWN) { haltWallGrowthForTurn(); }
                    //don't let the car turn too far, stop it from rotating after TURNFRAMES frames have elapsed
                    if (turnCounter > TURNCOOLDOWN-TURNFRAMES)
                    {
                        if (turnDirection == LEFT)
                        {
                            transform.Rotate(0.0f, -5f, 0.0f);
                        }
                        if (turnDirection == RIGHT)
                        {
                            transform.Rotate(0.0f, 5f, 0.0f);
                        }
                    }
                    //spawn a new wall to follow us after each turn
                    if (turnCounter == 5)
                    {
                        spawnNewTailSegment();
                    }
                    //set the time until the next segment starts to dissappear the frame after
                    //we create it
                    if (turnCounter == 4)
                    {
                        currentWallController.timeTillShrink = tailLength;
                    }
                    turnCounter--;

                }
                #endregion
            }
            #endregion

            #region if its crashed

            if (collisionHappened)//(collisionHappened)
            {
                foreach (Renderer curRenderer in renderers)
                {
                    Color color;
                    foreach (Material material in curRenderer.materials)
                    {
                        if (material.color.a > 0f)
                        {
                            color = material.color;
                            // change alfa for transparency
                            color.a -= 0.01f;
                            material.color = color;
                        }
                        else
                        {
                            
                        }
                    }
                }
            }
            #endregion

            //check to see if the oldest wall is ready to be removed
            if (oldestWall.readyToDelete)
            {
                oldestWall.deleteSegment();
                Destroy(oldestWall);
                oldestWall = (wallScript)wallQueue.Dequeue();
            }

        }

        //responding to collecting a pickup
        public void increaseTailLengthOnPickup()
        {
            tailLength += 50;
            //make sure we extend the length of all the other tail segments too
            foreach (wallScript wall in wallQueue.ToArray())
            {
                wall.delay(50);
            }
        }

        //this method stops our walls from growing or shrinking while we turn
        private void haltWallGrowthForTurn()
        {
            //end the growth of our current tail segment so we may begin to turn
            currentWallController.stopGrowing();
            //make sure that we stop each tail segment from shrinking because we are
            //not growing the tail while we are in the process of turning
            foreach (wallScript wall in wallQueue.ToArray())
            {
                wall.delay(TURNFRAMES);
            }
        }

        //creates a new segment, adds it to the queue, and sets it to our current wall
        private void spawnNewTailSegment()
        {
            currentWall = Instantiate(wallPrefab, transform.position, transform.rotation) as GameObject;
            currentWallController = currentWall.GetComponent<wallScript>();
            wallQueue.Enqueue(currentWall.GetComponent<wallScript>());
            currentWallController.timeTillShrink = tailLength;
        }

        
        void OnTriggerEnter(Collider other)
        {
			if (other.gameObject.tag != "Pickup")
			{
				collisionHappened = true;
                Application.LoadLevel("GameOverScene");
			}
			else
			{
				//other.gameObject.SetActive (false);
				other.gameObject.transform.position = new Vector3(Random.Range(-71.5f, 71.5f), 1.5f, Random.Range(-71.5f, 71.5f));
				score++;
				scoreText.text = "Boxes Munched: " + score;
				increaseTailLengthOnPickup();
			}

        }
}

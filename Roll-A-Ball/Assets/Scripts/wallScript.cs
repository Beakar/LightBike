using UnityEngine;
using System.Collections;

public class wallScript : MonoBehaviour {

//
//
//
//============================IMPORTANT NOTES=============================
//                  
//    
//                          IMPORTANT METHOD
//    delay(int delayAmount) - this method is used whenever a pickup is collected,
//      it will halt the trail from shrinking, effectively extending the length
//      of the total trail
//      NOTE: This will also need to be called after each turn since the bike stops moving during the turn
//      animation
//
//    stopGrowing() - this method needs called when the bike turns. It tell the wall that it is done growing
//
//      
//                          OTHER NOTES
//
// When the wall is first created, it is passed a number timeTillShrink. This number is equivelant to the current
// max length of our bikes tail (in arbitrary units). Each frame, timeTillShrink decreases by 1, and once it reaches 0 the
// wall begins to shrink. 
//
//
//

    private bool isGrowing;
    private bool isShrinking;
    //the rate at which we will both create and remove the wall
    private Vector3 growthVector;
    //this should be equal to the SPEED constant in the bike class
    private static float GROWTH_RATE = .4f;

    //current length of the wall, if this is 0 it tells us we need to delete the wall object
    public int length;
    //Time until the wall starts to shrink
    public int timeTillShrink;
    public bool readyToDelete;

	// Use this for initialization
	void Start () 
    {
        isGrowing = true;
        isShrinking = false;
        readyToDelete = false;
        growthVector = new Vector3(0, 0, GROWTH_RATE);
        length = 0;
        //this will be changed shortly after creation to the appropriate value, but
        //is here now to prevent it from shrinking right away
        timeTillShrink = 50; //50 is totally arbitrary
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        
        if (isGrowing)
        {
            grow();
        }
        if (timeTillShrink==0)
        {
            shrink();
        }
        if (timeTillShrink > 0)
        {
            timeTillShrink -= 1;
        }
	}

    public void stopGrowing()
    {
        isGrowing = false;
        //=======================================TODO==================================
        //if would be wise to have the walls not collidable up until this point. since there
        //is no way the player would be able to crash into the wall until the player
        //has turned and the wall has stopped growing
    }

    //this method is to be called when a powerup is picked up. essentially, by increasing
    //the time until we start shrinking, we increase the total length of the trail
    public void delay(int delayAmount)
    {
        timeTillShrink += delayAmount;
    }

    private void grow()
    {
        //move the center of the wall at half the rate at which is is growing
        //to give the illusion of it only growing in one direction
        transform.Translate(0f, 0f, GROWTH_RATE/2);
        transform.localScale += growthVector;
        //increment length
        length += 1;
    }

    private void shrink()
    {
        if (length > 0)
        {
            //move the center of the wall at half the rate at which is is shrinking
            //to give the illusion of it only disappearing from one direction
            transform.Translate(0f, 0f, GROWTH_RATE/2);
            transform.localScale -= growthVector;
            //decriment length
            length -= 1;

            if (length == 0)
            {
            //    readyToDelete = true;
                Destroy(gameObject);
            //    Destroy(this);
            }
        }
    }

    //the method to be called when the wall is ready to be destroyed
    public void deleteSegment()
    {
         Destroy(transform.parent.gameObject);       
    }
    
}

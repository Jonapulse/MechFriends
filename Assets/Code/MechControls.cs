using UnityEngine;
using System.Collections;

public class MechControls : MonoBehaviour {

    public GameObject mechBody;
    public GameObject rightArm;
    public GameObject leftArm;
    public GameObject rightLeg;
    public GameObject leftLeg;

    private HingeJoint2D rightArmTopHinge;
    private HingeJoint2D rightArmBotHinge;
    private HingeJoint2D leftArmTopHinge;
    private HingeJoint2D leftArmBotHinge;
    private HingeJoint2D rightLegTopHinge;
    private HingeJoint2D rightLegBotHinge;
    private HingeJoint2D leftLegTopHinge;
    private HingeJoint2D leftLegBotHinge;

    private HingeJoint2D[] hinges; //List of the above hinges for looping through.

    public GameObject[] laserGraphics; //right arm, left arm, right leg, left leg
    public GameObject[] finalLimbs; //right arm, left arm, right leg, left leg
    private Rigidbody2D[] limbsToBoost;

    public float TURN_SPEED = 40f;
    public float BOOST_FORCE = 75f;

    // Use this for initialization
    void Start () {

        HingeJoint2D[] bodyHinges = mechBody.GetComponents<HingeJoint2D>();

        //Adjust how bodyHinges are matched.  Right now it's a guess
        rightArmTopHinge = bodyHinges[2];
        rightArmBotHinge = rightArm.GetComponent<HingeJoint2D>();
        leftArmTopHinge = bodyHinges[3];
        leftArmBotHinge = leftArm.GetComponent<HingeJoint2D>();
        rightLegTopHinge = bodyHinges[0];
        rightLegBotHinge = rightLeg.GetComponent<HingeJoint2D>();
        leftLegTopHinge = bodyHinges[1];
        leftLegBotHinge = leftLeg.GetComponent<HingeJoint2D>();

        hinges = new HingeJoint2D[8];
        hinges[0] = rightArmTopHinge;
        hinges[1] = rightArmBotHinge;
        hinges[2] = leftArmTopHinge;
        hinges[3] = leftArmBotHinge;
        hinges[4] = rightLegTopHinge;
        hinges[5] = rightLegBotHinge;
        hinges[6] = leftLegTopHinge;
        hinges[7] = leftLegBotHinge;

        limbsToBoost = new Rigidbody2D[4];
        for (int i = 0; i < finalLimbs.Length; i++) limbsToBoost[i] = finalLimbs[i].GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
        //Eventually make this a for loop going over the different players
        for (int i = 0; i < 4; i++) //Change this to 4 when you add all player inputs
        {
            JointMotor2D top = hinges[i * 2].motor;
            top.motorSpeed = Input.GetAxisRaw("Horizontal_P" + (i + 1)) * TURN_SPEED;
            hinges[i * 2].motor = top;
            JointMotor2D bot = hinges[i * 2 + 1].motor;
            bot.motorSpeed = Input.GetAxisRaw("Vertical_P" + (i + 1)) * TURN_SPEED;
            hinges[i * 2 + 1].motor = bot;

            //Not sure if that input will work
            if (Input.GetAxisRaw("Fire_P" + (i + 1)) == 1)
            {
             //   laserGraphics[i].SetActive(true);
                limbsToBoost[i].AddForce((new Vector2( limbsToBoost[i].transform.position.x, limbsToBoost[i].transform.position.y) - new Vector2(laserGraphics[i].transform.position.x, laserGraphics[i].transform.position.y)).normalized * BOOST_FORCE);
            }
            else
            {
             //   laserGraphics[i].SetActive(false);
            }
            //if fire, show graphic and add force
            //else hide graphic
        }


    }
}

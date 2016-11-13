using UnityEngine;
using System.Collections;

public class MechTautLooseControl : MonoBehaviour {

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

    
    public GameObject[] finalLimbs; //right arm, left arm, right leg, left leg
    public GameObject[] topVecRefs; //rA, lA, rL, lL
    public GameObject[] botVecRefs; //rA, lA, rL, lL
    public GameObject[] boostGraphics; //right leg, left leg
    private Rigidbody2D[] limbsToBoost;
    private float[] limbCooldowns;
    private int[] limbControllers;


    public float ORIENT_SPEED = 150f;
    public float CONTRACT_SPEED = 80f;
    public float RELEASE_SPEED = 300f;
    public float SHOOT_COOLDOWN = 0.25f;
    public float SHOOT_FORCE = 200f;
    public float BOOST_COOLDOWN = 3f;
    public float BOOST_DURATION = 1.5f;
    public float BOOST_FORCE = 60f;
    public float TOGGLE_COOLDOWN_TIME = 0.5f;
    private float toggleTimer = 0;

    //Player toggle graphics
    public GameObject[] portraitHolders;
    public GameObject[] playerPortraits;

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

        for (int i = 0; i < boostGraphics.Length; i++) boostGraphics[i].SetActive(false);

        limbCooldowns = new float[4] { 0, 0, 0, 0 };
        limbControllers = new int[4] { 1, 2, 3, 4 };
    }

    // Update is called once per frame
    void Update () {
	    //If single player, then should be toggled to respond to 1 of 4 appendages.  Else we listen for 4 players.

        for(int i = 0; i < 4; i++)
        {
            int playerNum = limbControllers[i];
            if (Manager.Instance.numPlayers == 1 && i != 1) continue;

            Vector2 playerOrientation = new Vector2(Input.GetAxisRaw("Horizontal_P" + i), Input.GetAxisRaw("Vertical_P" + i));

            float topMotorSpeed = 0;
            float botMotorSpeed = 0;


            if(playerOrientation.magnitude != 0)
            {

                if (Vector3.Cross(new Vector3(botVecRefs[i].gameObject.transform.position.x, botVecRefs[i].gameObject.transform.position.y, 0)
                - new Vector3(topVecRefs[i].gameObject.transform.position.x, topVecRefs[i].gameObject.transform.position.y, 0),
                new Vector3(playerOrientation.x, playerOrientation.y, 0)).z > 0)
                {
                    topMotorSpeed = ORIENT_SPEED;
                }
                else
                {
                    topMotorSpeed = -ORIENT_SPEED;
                }
            }
            


            //Add empty reference vectors to the limbs.  Use the angle compare to how you're pointing
            //to direct in which direction we turn      

            //Choose a top motorspeed to get us to this angle (you'll add it to the below one)

            //if pressed release... started a release and not finished 
            //set bool to doing this and not compressing even if you press compress
            //go rapidly back to straight angle, whatever it be, then stop and reset the bool
            //else if compressing, do fixed compress motorspeed

            JointMotor2D top = hinges[(playerNum - 1) * 2].motor;
            top.motorSpeed = topMotorSpeed;
            hinges[(playerNum - 1) * 2].motor = top;
            JointMotor2D bot = hinges[(playerNum - 1) * 2 + 1].motor;
            bot.motorSpeed = botMotorSpeed;
            hinges[(playerNum - 1) * 2 + 1].motor = bot;

            //if firing do that
            //if i < 2, fire a bullet and set cooldown

        }

        //MANAGE COOLDOWN TIMERS
        toggleTimer -= Time.deltaTime;
        for (int i = 0; i < limbCooldowns.Length; i++) if (limbCooldowns[i] > 0) limbCooldowns[i] -= Time.deltaTime;

        //Check for player toggling after all moves recorded
        for (int i = 0; i < 4; i++) {
            if (toggleTimer <= 0 && Input.GetAxisRaw("Toggle_P" + limbControllers[i]) == 1)
            {
                toggleTimer = TOGGLE_COOLDOWN_TIME;
                ToggleControl();
                break;
            }
        }

    }

    void ToggleControl()
    {
       
        int leftOverController = limbControllers[limbControllers.Length - 1];
        for(int i = limbControllers.Length - 2; i >= 0; i--)
        {
            limbControllers[i + 1] = limbControllers[i];
        }
        limbControllers[0] = leftOverController;

        //Graphically
        for (int i = 0; i < 4; i++) playerPortraits[limbControllers[i] - 1].transform.position = portraitHolders[i].gameObject.transform.position;
    }
}

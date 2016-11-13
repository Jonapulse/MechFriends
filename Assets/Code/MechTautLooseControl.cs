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
    public float CONTRACT_SPEED = 100f;
    public float RELEASE_SPEED = 600f;
    public float RELEASE_TIME = 0.2f;
    private float releaseTimer = 0;
    public float SHOOT_COOLDOWN = 0.25f;
    public float SHOOT_FORCE = 250f;
    public float BOOST_COOLDOWN = 3f;
    public float BOOST_DURATION = 1.5f;
    public float BOOST_FORCE = 60f;
    public float TOGGLE_COOLDOWN_TIME = 0.5f;
    private float toggleTimer = 0;

    //Player toggle graphics
    public GameObject[] portraitHolders;
    public GameObject[] playerPortraits;

    public GameObject bulletPrefab;
    public float bulletSpeed = 12.5f;

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

        for (int i = 0; i < 4; i++)
        {
            int playerNum = i + 1;
            int controllerNum = limbControllers[i];
            if (Manager.Instance.numPlayers == 1 && playerNum != 1) continue;

            Vector2 playerOrientation = new Vector2(Input.GetAxisRaw("Horizontal_P" + playerNum), Input.GetAxisRaw("Vertical_P" + playerNum));

            float topMotorSpeed = 0;
            float botMotorSpeed = 0;


            if (playerOrientation.magnitude != 0)
            {

                if (Vector3.Cross(new Vector3(botVecRefs[hackTranslate(controllerNum - 1)].gameObject.transform.position.x, botVecRefs[hackTranslate(controllerNum - 1)].gameObject.transform.position.y, 0)
                - new Vector3(topVecRefs[hackTranslate(controllerNum - 1)].gameObject.transform.position.x, topVecRefs[hackTranslate(controllerNum - 1)].gameObject.transform.position.y, 0),
                new Vector3(playerOrientation.x, playerOrientation.y, 0)).z > 0)
                {
                    topMotorSpeed = ORIENT_SPEED;
                }
                else
                {
                    topMotorSpeed = -ORIENT_SPEED;
                }
            }

            if (releaseTimer <= 0 && Input.GetAxisRaw("Compress_P" + playerNum) == 1)
            {
                topMotorSpeed -= CONTRACT_SPEED * 0.66f;
                //WARNING: the blow will change depending on the order feet and hands are controlled (they have opposite joints)
                if(hackTranslate(controllerNum - 1) < 2) botMotorSpeed += CONTRACT_SPEED;
                else botMotorSpeed -= CONTRACT_SPEED;

            }
            else
            {
                if( releaseTimer <= 0 && Input.GetAxisRaw("Release_P" + playerNum) == 1)
                {
                    releaseTimer = RELEASE_TIME;
                }

                if(releaseTimer > 0)
                {

                    topMotorSpeed += RELEASE_SPEED * 0.66f;
                    if (hackTranslate(controllerNum - 1) < 2) botMotorSpeed -= RELEASE_SPEED;
                    else botMotorSpeed += RELEASE_SPEED;
                }
            }

            JointMotor2D top = hinges[hackTranslate((controllerNum - 1)) * 2].motor;
            top.motorSpeed = topMotorSpeed;
            hinges[hackTranslate((controllerNum - 1)) * 2].motor = top;
            JointMotor2D bot = hinges[hackTranslate((controllerNum - 1)) * 2 + 1].motor;
            bot.motorSpeed = botMotorSpeed;
            hinges[hackTranslate((controllerNum - 1)) * 2 + 1].motor = bot;

            if(limbCooldowns[hackTranslate(controllerNum - 1)] <= 0 && Input.GetAxisRaw("Fire_P" + playerNum) == 1)
            {
                if(hackTranslate(controllerNum - 1) < 2)
                {
                    //FIRE PROJECTILE
                    limbCooldowns[hackTranslate(controllerNum - 1)] = SHOOT_COOLDOWN;

                    //Figure out which limb you're referencing
                    GameObject sourceLimb = finalLimbs[0];
                    if (hackTranslate(controllerNum - 1) == 1) sourceLimb = finalLimbs[1];

                    GameObject bullet = (GameObject)Instantiate(bulletPrefab, sourceLimb.transform.position, sourceLimb.transform.rotation);
                    bullet.transform.eulerAngles = new Vector3(bullet.transform.eulerAngles.x, bullet.transform.eulerAngles.y, bullet.transform.eulerAngles.z + 185f);

                    Vector3 fireVelocity = (sourceLimb.transform.position - botVecRefs[hackTranslate(controllerNum - 1)].transform.position).normalized * bulletSpeed;

                    bullet.GetComponent<BulletScript>().velocity = fireVelocity;

                    limbsToBoost[hackTranslate(controllerNum - 1)].AddForce(new Vector2(fireVelocity.x, fireVelocity.y).normalized * -SHOOT_FORCE);
                }
                else
                {
                    //FIRE BOOSTERS
                    limbCooldowns[hackTranslate(controllerNum - 1)] = BOOST_COOLDOWN;
                }
            }
        }

        //REACT TO BOOSTERS
        for(int i = 2; i < 4; i++)
        {
            if(limbCooldowns[i] > BOOST_COOLDOWN - BOOST_DURATION)
            {
                boostGraphics[i - 2].SetActive(true);
                Vector3 boostVelocity = (botVecRefs[i].transform.position - finalLimbs[i].transform.position).normalized * BOOST_FORCE;
                limbsToBoost[i].AddForce(new Vector2(boostVelocity.x, boostVelocity.y));
            }
            else
            {
                boostGraphics[i - 2].SetActive(false);
            }
        }

        //MANAGE COOLDOWN TIMERS
        if(toggleTimer >= 0) toggleTimer -= Time.deltaTime;
        if (releaseTimer >= 0) releaseTimer -= Time.deltaTime;
        for (int i = 0; i < limbCooldowns.Length; i++) if (limbCooldowns[i] >= 0) limbCooldowns[i] -= Time.deltaTime;

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

    //fixes some sloppiness where my control UI didn't match which limbs were controlled
    int hackTranslate(int startNum)
    {
        switch (startNum) {
            case (0):
                return 0;
            case (1):
                return 2;
            case (2):
                return 3;
            case (3):
                return 1;
        }
        return -1;
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

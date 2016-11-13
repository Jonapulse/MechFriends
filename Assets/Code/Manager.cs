using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour {

    //SINGLETON STUFF
    protected Manager() { }
    private static Manager _instance = null;

    public static Manager Instance
    {
        get
        {
            return Manager._instance;
        }
    }

    public GameObject titleScreen;
    public GameObject gameOverScreen;
    public GameObject victoryScreen;
    
    public GameObject playerHolderPreStart;

    //When these objects die the game ends
    public GameObject kindergarten;
    public GameObject[] enemiesAttackingKindergarten;


    public int numPlayers = 1;
    private bool gameFinished = false;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        titleScreen.SetActive(true);
        gameOverScreen.SetActive(false);
        victoryScreen.SetActive(false);
    }

    void Update () {
        if (gameFinished) return;

	    if(kindergarten == null)
        {
            gameOverScreen.SetActive(true);
            gameFinished = true;
        }
        else
        {
            bool attackerSurvives = false;
            for (int i = 0; i < enemiesAttackingKindergarten.Length; i++)
            {
                if (enemiesAttackingKindergarten[i] != null) attackerSurvives = true;
            }
            if (!attackerSurvives)
            {
                victoryScreen.SetActive(true);
                gameFinished = true;
            }
        }
	}

    public void StartGame(int players)
    {
        titleScreen.SetActive(false);

        playerHolderPreStart.SetActive(false);

        numPlayers = players;
    }

    public void Restart()
    {
        Application.LoadLevel(0);
    }
}

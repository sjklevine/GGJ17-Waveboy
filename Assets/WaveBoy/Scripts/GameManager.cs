using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
   

    //Static instance of GameManager which allows it to be accessed by any other script.
    public static GameManager instance = null;

    public int score;
    private int highScore;
    private int life;    
    private int wave;

    public int HighScore
    {
        get
        {
            return highScore;
        }
    }

    public int Life
    {
        set
        {
            life = value;
            Reset();
        }
        get
        {
            return life;
        }

    }

    public int Wave
    {
        set
        {
            Wave = value;
            NextLevel();
        }
        get
        {
            return Wave;
        }
    }

    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        //Get a component reference to the attached BoardManager script
        
        //Call the InitGame function to initialize the first level 
        InitGame();
    }

	
	

    //first time setup
    void InitGame()
    {
        wave = 1;
        life = 3;
        score = 0;
    }

    //reset life
    void Reset()
    {
        life--;
        if (life == 0)
        {
           
            GameOver();
        }
        
    }
    //increase wave 
    void NextLevel()
    {
        wave++;
    }

    public void NewGame()
    {
        InitGame();
    }


    //go back to title screen
    public void GameOver()
    {

        if (score > highScore)
            highScore = score;
        SceneManager.LoadScene("Title");
    }

   
}

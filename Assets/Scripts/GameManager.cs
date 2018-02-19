using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    #region Fields / Variables
    private static GameManager instance;            // Creates a singleton.
    // Creates the singleton for this class.
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }
    // Getter/Setter for the total number of enemies on a level.
    public int TotalNumOfEnemies
    {
        get
        {
            return totalNumOfEnemies;
        }

        set
        {
            totalNumOfEnemies = value;
        }
    }

    private int sceneIndex;                 // Index of the scene in the unity build array.
    private int totalNumOfLevels;           // Total number of scenes in the unity build array.
    [SerializeField]
    private Text remainingEnemies;          // Text to display the number of eneies remaining.
    [SerializeField]
    private Text playerLivesRemaining;      // Text to display the number of lives the player has.

    [SerializeField]
    private GameObject[] enemies;           // An array of enemies on the level.
    [SerializeField]
    private int totalNumOfEnemies;          // total number of eneimes in the enemies array.

    private bool isPaused;
    [SerializeField]
    private Canvas pauseMenu;
    
    
    #endregion

    #region Functions {Start(), Update(), ChangeLevel(), RemainingEnemyCount(), PlayerLives()}
    // Use this for initialization
    void Start()
    {
        totalNumOfLevels = Application.levelCount;      // Sets the number of levels in the unity bukild array.
        sceneIndex = Application.loadedLevel;           // Sets the current scenes index.

        isPaused = false;

        if(enemies != null)                             // makes sures there are enemies in the enemy array.
        {
            TotalNumOfEnemies = enemies.Length;         // sets the totalnumber of enemies to the lenght of the enemy array.
            RemainingEnemyCount();                      // Draws the number of enemies to the screen.
        }
        else
        {
            totalNumOfEnemies = 0;                      // Sets the enemies to 0 if the array is null.
            RemainingEnemyCount();                      // Draws the number of enemies to the screen.
        }
        if(Player.Instance != null)                     // If we have a player we draw thier lives on the screen.
        {
            PlayerLives();                              // draws the number of lives the player has on the screen.
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.Instance.SceneTrigger)           // Checks if the player has hit the scene trigger.
        {
            if((sceneIndex + 1 ) < totalNumOfLevels)    // if the build array has more scenes then runs the change level function.
            {
                StartCoroutine(ChanegLevel(sceneIndex + 1));    // progress to the next level.
            }
            else
            {
                return;                                 
            }
        }
        if(Player.Instance.NumOfLives <= 0)             // If the player has no lives left takes them to the game over screen.
        {
            StartCoroutine(ChanegLevel(4));             // loads the game over screen.
        }
        if(totalNumOfEnemies == 0 && sceneIndex != 0)   // Checks that the total number of enemies is 0 and if they are on the main menu.
        {
            // Checks to makes sure there are more levels to load and that th eplayer is alive to load them.
            if (((sceneIndex + 1) < totalNumOfLevels) && (((sceneIndex + 1) != 4) && Player.Instance.NumOfLives > 0))
            {
                StartCoroutine(ChanegLevel(sceneIndex + 1));    // loads next scene.
            }
            else if(sceneIndex == 3 && Player.Instance.NumOfLives >= 1) // checks that the player has atlest one life left and they are on the final level.
            {
                StartCoroutine(ChanegLevel(5)); // if the enemies are gone on the final level and the player has a life then loads the Win screen.
            }
        }

        if (isPaused)
        {
            PauseGame();            // calls the pause function.
        }
        else
        {
            PauseGame();            // calls the pause function.
        }
        if (sceneIndex == 0 || sceneIndex == 4 || sceneIndex == 5)  // If on the main menu or one fo the special screnes we brreak out before the player can use the pause menu.
        {
            return;  
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SwitchPause();          // calls the switch between pause and unpause.
        }
        
    }
    // Does a fade between game levels.
    public IEnumerator ChanegLevel(int lvlNum)
    {
        float fadeTime = this.gameObject.GetComponent<Fading>().BeginFade(1);   // gets the fadetime to fade to black.
        yield return new WaitForSeconds(fadeTime);  // waits til the screen is faded to black.
        Player.Instance.SceneTrigger = false;   // turns off the screne trigger used in the main menu.
        Application.LoadLevel(lvlNum);      // Loads the chosen level.
    }
    // draws the remaining enemies total to the screen.
    public void RemainingEnemyCount()
    {
        if (sceneIndex != 4)
            remainingEnemies.text = totalNumOfEnemies.ToString();
        else
            remainingEnemies.text = " ";

    }

    // Draws the reamining player lives to the screen.
    public void PlayerLives()
    {
        if (sceneIndex != 4)
            playerLivesRemaining.text = Player.Instance.NumOfLives.ToString();
        else
            playerLivesRemaining.text = " ";
    }
    // Pauses the game when the pause menu is visible.
    public void PauseGame()
    {
        if (isPaused)
        {
            pauseMenu.enabled = true;       // renders the pause menu to the screen.
            Time.timeScale = 0.0f;          // Staops all time in the game effectively pausing the game.
        }
        else
        {
            Time.timeScale = 1.0f;          // Returns the game time to normal unpausing the game.
            pauseMenu.enabled = false;      // removes the pause memu from the screen.
        }
    }
    // Switchs between paused and unpaused.
    public void SwitchPause()
    {
        isPaused = !isPaused;
    }
    // Exit the game. (Not useable in the editor.)
    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
}

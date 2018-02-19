using UnityEngine;
using System.Collections;
using System;

public class Enemy : Character
{
    #region Fields & Varaibles
    [SerializeField]
    private float meleeRange;       // the distance the player needs to be to the enemy before it will use a melee weapon.

    [SerializeField]
    private float throwRange;       // the distance the player need to be to the enemy before it will throw a weapon.

    [SerializeField]
    private Transform spawnPoint;   // point used for enemy respawns.
    


    public BoxCollider2D sightBox;  // The sight box of the enemy.

    public bool InMeleeRange    // Returns if the player is in range for melee combat.
    {
        get
        {
            if(Target != null)
            {
                return Vector2.Distance(transform.position, Target.transform.position) <= meleeRange; // Checks if the player is within the melee range.
            }
            return false;
        }
    }

    public bool InThrowRange    // Returns if the player is in range for range combat.
    {
        get
        {
            if (Target != null)
            {
                return Vector2.Distance(transform.position, Target.transform.position)  <= throwRange; // Checks if the player is in range to throw a dagger.
            }
            return false;
        }
    }

    private IEnemyState currentState;   // Tracks the currenrt state of the character.

    public GameObject Target { get; set; }  // Tells the computer who the target is.

    public override bool IsDead // Returns if the character is dead or not.
    {
        get
        {
            return health.CurrentVal <= 0;
        }
    }

    public GameObject Heart // Getter for the health pickup object.
    {
        get
        {
            return heart;
        }
    }

    public Transform HeartSpawn // Getter for the Spawn point of the health pickup.
    {
        get
        {
            return heartSpawn;
        }
    }
    [SerializeField]
    private GameObject oneUp;   // ! up pickup.
    public GameObject OneUp     // Getter for the 1 up pickup.
    {
        get
        {
            return oneUp;
        }
    }
    [SerializeField]
    private Transform oneUpSpawn;   // Spawn point for the 1 up pickup.
    public Transform OneUpSpawn     // Getter for the spawn point of the 1 up pickup.
    {
        get
        {
            return oneUpSpawn;
        }
    }

    

    [SerializeField]
    private Transform leftEdge;     // Left furthest edge of the enemy patrol boundry.
    [SerializeField]
    private Transform rightEdge;    // Right furthest edge of the enemy patrol boundry.

    private Canvas healthCanvas;    // the canvas that displays the enemy's health after taking damage.

    private int randomHealthSpawnVal;   // A random value to determine if a enemy can have a health pickup drop.
    private int randomOneUpSpawnValue;  // A random value to determine if a enemy can have a 1 up pickup drop.

    [SerializeField]
    private GameObject heart;   // the health drop game object.
    [SerializeField]
    private Transform heartSpawn;   // the position for the health drop to spawn.

    [SerializeField]
    private bool spawnHeart;    // Ttracks if a health pickup has been spawned.
    [SerializeField]
    private bool spawnOneUp;    // Tracks if a 1 up pickup has been spawned.
    public bool SpawnHeart      // Getter for the health spawn tracker.
    {
        get
        {
            return spawnHeart;
        }
    }

    public bool SpawnOneUp  // Getter for the 1 up tracker. prevents an enemy from droping more than one of the item.
    {
        get
        {
            return spawnOneUp;
        }
    }


    private int chanceToSpawnItem;  // A random value to wheter the enemy can even spawn an item.

    #endregion

    #region Unity Methods {Awake(), Start(), Update(), OnTriggerEnter2D(Collider2D)}
    public override void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    public override void Start ()
    {
        base.Start();
        healthCanvas = transform.GetComponentInChildren<Canvas>();  // Gets a reference to the canvas where the enemy health is drawn.
        healthCanvas.enabled = false;                               // Whiel at full health the health bar will not be displayed.
        Player.Instance.Dead += new DeadEventHandler(RemoveTarget); // Event handler to remove the player as a target if they are dead.
        ChangeState(new IdleState());   // initial state of the eney when it is spawned.
        SpawnPickup();  // Decides fi and what the enemys drop upon deaht will be.
    }
    

	// Update is called once per frame
	public override void Update ()
    {
        base.Update();
        if (!IsDead)    // As long as the character is not dead it can do something.
        {
            if (!TakingDamage)  // if the enemy isn't taking damage it can do somehting.
            {
                currentState.Execute(); // Runs the currents states excution method.
            }
            LookAtTarget(); // If the target is within the sight box or damages the enemy form behind it will look at the player.
        }
    }
    
    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        currentState.OnTriggerEnter(other);
    }
    #endregion

    #region Methods {LookAtTarget(), RemoverTarget(), ChangeState(IEnemyState),Move(), ChangeDirection(),TakeDamage(), SpawnHealthPickup(), Death()}
    /// <summary>
    /// Looks makes sure the enemy is facing the its target.
    /// </summary>
    private void LookAtTarget()
    {
        if (Target != null) // As long as the enemy has a traget in sight it will face it.
        {
            float xDir = Target.transform.position.x - transform.position.x;    // Gets the direction the traget is so the nemey knows which way to face.
            if (xDir < 0 && facingRight || xDir > 0 && !facingRight)    // Checks the direction and calls the change direction method.
            {
                ChangeDirection();  // Changes the enemys facing direction either right or left.
            }
        }
    }
    /// <summary>
    /// Removes the player as target and sets the enemy back to patrol state.
    /// </summary>
    public void RemoveTarget()
    {
        Target = null;
        ChangeState(new PatrolState());
    }
    /// <summary>
    /// Changes the enemys state.
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(IEnemyState newState)
    {
        if(currentState != null)    // As long as the enemy is in a state it will call the exit function when the state changes.
        {
            currentState.Exit();    // Handles anything the enemy may need to do before changing states.
        }

        currentState = newState;    // sets the current state to the new state.

        currentState.Enter(this);   // Eneter the new state.
    }
    /// <summary>
    /// Get the direction the enemy is facing and makes sure he stays within his patrol boundries as he moves.
    /// </summary>
    public void Move()
    {
        if (!Attack)    // if hes not attacking than he can move.
        {
            if ((GetDirection().x > 0 && transform.position.x < rightEdge.position.x) || (GetDirection().x < 0 && transform.position.x > leftEdge.position.x))
            {
                MyAnimator.SetFloat("speed", 1);    // Set the aniamtor to play the running animation.
                transform.Translate(GetDirection() * (movementSpeed * Time.deltaTime)); // uses the Translate method to move the enemy acroos the screen in the correct direction.
            }
        }

        // Chamages the direction if the enemy reaches the furthest right boundry point.
        if ((GetDirection().x > 0 && transform.position.x >= rightEdge.position.x))
        {
            ChangeDirection();  // Changes the facing and movement dirction of the enemy.

        }
        // Chamages the direction if the enemy reaches the furthest left boundry point.
        if ((GetDirection().x < 0 && transform.position.x <= leftEdge.position.x))
        {
            ChangeDirection();  // Changes the facing and movement dirction of the enemy.
        }
    }
    // Returns the direction the enemy is moving in.
    public Vector2 GetDirection()
    {
        return facingRight ? Vector2.right : Vector2.left;  // if facing right moving right else moving left.
    }
    // Changes the enemy sprites direction. 
    public override void ChangeDirection()
    {
        Transform tmp = transform.FindChild("EnemyHBCanvas").transform; // holds the postion of the health bar above the enemy.
        Vector3 pos = tmp.position; // hold the haelths bars position in world space.
        tmp.SetParent(null);    // Removes the health bar as a child befpre the enemy flips direction else the bar flips too.

        base.ChangeDirection(); // Flips the enemy sprite in the opposite direction.

        tmp.SetParent(transform);   //Sets the health bar back to a child of the enemy.
        tmp.position = pos;         // Sets the health bars postion back to where it was before the flip above the enemy.

    }
    /// <summary>
    /// handles the damage the enmey character takes.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override IEnumerator TakeDamage(Collider2D other)
    {
        if (!healthCanvas.isActiveAndEnabled)   // upon taking damages if the health bar is invisiable make sit visiable.
        {
            healthCanvas.enabled = true;    // makes the health bar visiable.
        }
        if(other.tag == "Knife")    // chacks to see what did the damage.
        {
            health.CurrentVal -= knife.GetComponent<knife>().Damage;    // gets the knife damage ad subtracts it from enemys health.
        }
        if (other.tag == "Sword")
        {
            health.CurrentVal -= 10;    // subtracts the healths from the enemy upon taking damages from a sword.
        }
        if (!IsDead)        // checks that the enemy is not dead form the damage.
        {
            MyAnimator.SetTrigger("damage");    // plays the damage animation as long as it is stil alive.
        }
        else
        {
            MyAnimator.SetTrigger("die");   // Plays the death aniamtion if it has no health.
            yield return null;
        }
    }
    /// <summary>
    /// Decides if a enemy will spawn a item upon death and what it will be.
    /// </summary>
    private void SpawnPickup()
    {

        chanceToSpawnItem = UnityEngine.Random.Range(0, 11);    // Random cahance to spawn an item.

        randomHealthSpawnVal = UnityEngine.Random.Range(0, 11); // Random chance to spawn a health pick up.
        randomOneUpSpawnValue = UnityEngine.Random.Range(0, 22);    // Random chance to spawn a 1 up pick up.

        if(chanceToSpawnItem > 5 && chanceToSpawnItem <= 10)    // 0 to 4 no pick up - 5 to 10 it will have a pick up. 
        {
            hasPickup = true;   // Sets has a pickup to true so the game knows to look and see what it has to drop.
        }
        if (hasPickup)
        {
            if(randomHealthSpawnVal > 5 && randomHealthSpawnVal <= 10)  // 0 to 4 no health pickup - 5 to 10 will spawn a health pickup.
            {
                spawnHeart = true;      // lets the game knwo to spawn a heart.
            }
            if(randomOneUpSpawnValue > 18 && randomOneUpSpawnValue <= 21)   // 0 to 17 no 1 up - 18 to 21 will have a 1 up pick up.
            {
                spawnOneUp = true;      // lets the game know it has a 1 up pick up to drop.
            }
            if(hasPickup && !spawnHeart && !spawnOneUp) // fail safe to guaretee a drop if it has a chance to drop.
            {
                spawnHeart = true;      // the default drop will be a health pick up.
            }
            if (SpawnHeart && SpawnOneUp)   // checks to make sure that only one pickup can be dropped.
            {
                spawnHeart = false;     // between the two types of drops the 1 up is the primary drop. 
            }
        }

        //Debug.Log(gameObject.name + " Has a pick up: "+hasPickup+ " Spawn heart: " + spawnHeart + "," + " Spawn One Up: " + spawnOneUp);
    }
    /// <summary>
    /// Handles the events of the enemy death.
    /// </summary>
    public override void Death()
    {
        if (spawnPoint != null) // if there is a spawn point for the enemey it has things to do before it respawns. 
        {
            MyAnimator.SetTrigger("idle");      // Resets the enemy idle animation trigger.
            MyAnimator.SetFloat("speed", 0);    // Sets the aninmation back to idle.
            if (healthCanvas.isActiveAndEnabled)    // Turns off the health bar.
            {
                healthCanvas.enabled = false;
            }
            if (Target != null)         // Removes the target from the enemy.
            {
                Target = null;
            }

           ChangeState(new IdleState());    // Puts the enemy back into an idle state.

            if (health.CurrentVal != health.MaxVal)     // Checks the health and sets it back to max.
            {
                health.CurrentVal = health.MaxVal;
            }
            transform.position = spawnPoint.position;   // spawns the enemy at the loaction.
            SpawnPickup();  // Spawns a pickup.
            if (GameManager.Instance.TotalNumOfEnemies < 0) // Checks with the Game manger to see if that was the last enemy
                GameManager.Instance.TotalNumOfEnemies = 0; // if it was makes sure the value can not go below 0.
            GameManager.Instance.RemainingEnemyCount(); // updates the reamining enemy hud.
        }
        else
        {
            Destroy(gameObject);    // Destorys the enemy object.
            GameManager.Instance.TotalNumOfEnemies--;   // decreases the number of enemies.
            if (GameManager.Instance.TotalNumOfEnemies < 0) //makes sure the value can not go below 0
                GameManager.Instance.TotalNumOfEnemies = 0;
            GameManager.Instance.RemainingEnemyCount(); // updates the raemaining enemy hud.
        }
    }
    #endregion
}

using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Movement;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PriorityManager : MonoBehaviour
{
    public const float X_WORLD_SIZE = 55;
    public const float Z_WORLD_SIZE = 32.5f;
    public const float AVOID_MARGIN = 4.0f;

    public const float MAX_LOOK_AHEAD = 5.0f;
    public const float MAX_SPEED = 40.0f;
    public const float MAX_ACCELERATION = 40.0f;
    public const float DRAG = 0.1f;

    public const float CLICK_ARRIVE_WEIGHT= 60.0f;

    public const float COHESION_WEIGHT = 30.0f;
    public const float FAN_ANGLE = 1.65f; //wannabe radian
    public const float COHESION_RADIUS = 15f;

    public const float SEPARATION_WEIGHT = 90.0f;
    public const float SEPARATION_FACTOR = 150.0f;
    public const float SEPARATION_RADIUS = 15f;

    public const float OBSTACLE_AVOIDANCE_WEIGHT = 70.0f;

    public const float MATCH_SPEED_WEIGHT = 30.0f;

    public const int MAX_NUMBER_OF_CLONES = 50;

    public static bool debugMode = false;
    public static Vector3 click = new Vector3(-1, -1, -1);

	private DynamicCharacter RedCharacter { get; set; }
	private DynamicCharacter BlueCharacter { get; set; }
	private DynamicCharacter GreenCharacter { get; set; }

    private Text RedMovementText { get; set; }

    private BlendedMovement Blended { get; set; }

    private PriorityMovement Priority { get; set; }

    private List<DynamicCharacter> Characters { get; set; }

    private List<KinematicData> CharactersKinData;

    private GameObject[] obstacles;

	// Use this for initialization
	void Start () 
	{
		var textObj = GameObject.Find ("InstructionsText");
		if (textObj != null) 
		{
			textObj.GetComponent<Text>().text = 
				"Instructions\n\n" +
				"B - Blended\n" +
				"P - Priority\n"+
                "Q - stop"; 
		}

	    //this.RedMovementText = GameObject.Find("RedMovement").GetComponent<Text>();
		var redObj = GameObject.Find ("Red");

	    this.RedCharacter = new DynamicCharacter(redObj)
	    {
	        Drag = DRAG,
	        MaxSpeed = MAX_SPEED
	    };


	    obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        int numberOfClones = Random.Range(1, MAX_NUMBER_OF_CLONES);
        this.Characters = this.CloneSecondaryCharacters(redObj, numberOfClones, obstacles);

	    this.Characters.Add(this.RedCharacter);

        CharactersKinData = new List<KinematicData>();
        foreach (DynamicCharacter ch in Characters)
        {
            CharactersKinData.Add(ch.KinematicData);
        }
        //this.InitializeMainCharacter(obstacles);

        //initialize all but the last character (because it was already initialized as the main character)
	    foreach (var character in this.Characters)
	    {
	        this.InitializeSecondaryCharacter(character, false);
	    }
	}

    private void InitializeSecondaryCharacter(DynamicCharacter character, bool isPriority)
    {
        var priority = new PriorityMovement
        {
            Character = character.KinematicData
        };

        var blended = new BlendedMovement
        {
            Character = character.KinematicData,
        };

        foreach (var obstacle in obstacles)
        {
            DynamicAvoidObstacle avoidObstacleMovement = new DynamicAvoidObstacle(obstacle)
            {
                MaxAcceleration = MAX_ACCELERATION,
                AvoidMargin = AVOID_MARGIN,
                MaxLookAhead = MAX_LOOK_AHEAD,
                Character = character.KinematicData,
                MovementDebugColor = Color.magenta
            };
            blended.Movements.Add(new MovementWithWeight(avoidObstacleMovement, OBSTACLE_AVOIDANCE_WEIGHT));
            priority.Movements.Add(avoidObstacleMovement);
        }


        var clickArrive = new DynamicClickArrive()
        {
            Character = character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MovingTarget = new KinematicData(),
            SlowRadius = 5f,
            StopRadius = 3.5f,
            TimeToTargetSpeed = 1.0f,
            Target = new KinematicData(),
        };
        blended.Movements.Add(new MovementWithWeight(clickArrive, CLICK_ARRIVE_WEIGHT));
        priority.Movements.Add(clickArrive);

        var cohesionCharacter = new DynamicCohesion()
        {
            Character = character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MovementDebugColor = Color.yellow,
            MovingTarget = new KinematicData(),
            SlowRadius = 5f,
            StopRadius = 3.5f,
            TimeToTargetSpeed = 1.0f,
            Target = new KinematicData(),
            FanAngle = FAN_ANGLE,
            Radius = COHESION_RADIUS,
            Flock = CharactersKinData
        };
        blended.Movements.Add(new MovementWithWeight(cohesionCharacter, COHESION_WEIGHT));
        priority.Movements.Add(cohesionCharacter);

        var separationCharacter = new DynamicSeparation()
        {
            Character = character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MovementDebugColor = Color.blue,
            Target = new KinematicData(),
            Flock = CharactersKinData,
            Radius = SEPARATION_RADIUS,
            SeparationFactor = SEPARATION_FACTOR
        };
        blended.Movements.Add(new MovementWithWeight(separationCharacter, SEPARATION_WEIGHT));
        priority.Movements.Add(separationCharacter);

        var flockVelocityMatch = new DynamicFlockVelocityMatching()
        {
            Character = character.KinematicData,
            FanAngle = FAN_ANGLE,
            Flock = CharactersKinData,
            MaxAcceleration = MAX_ACCELERATION,
            MovementDebugColor = Color.black,
            MovingTarget = new KinematicData(),
            Radius = COHESION_RADIUS,
            Target = new KinematicData(),
            TimeToTargetSpeed = 1.5f
        };
        blended.Movements.Add(new MovementWithWeight(flockVelocityMatch, MATCH_SPEED_WEIGHT));
        priority.Movements.Add(flockVelocityMatch);
        if (isPriority)
            character.Movement = priority;
        else character.Movement = blended;
    }

    private List<DynamicCharacter> CloneSecondaryCharacters(GameObject objectToClone,int numberOfCharacters, GameObject[] obstacles)
    {
        var characters = new List<DynamicCharacter>();
        for (int i = 0; i < numberOfCharacters; i++)
        {
            var clone = GameObject.Instantiate(objectToClone);
            //clone.transform.position = new Vector3(30,0,i*20);
            clone.transform.position = this.GenerateRandomClearPosition(obstacles);
            float randomizedMaxSpeed = Mathf.Max((Random.value * MAX_SPEED), 1);    //to avoid speed 0 dunno if its good
            //Debug.Log(randomizedMaxSpeed);
            var character = new DynamicCharacter(clone)
            {
                MaxSpeed = randomizedMaxSpeed,
                Drag = DRAG
            };
            //character.KinematicData.orientation = (float)Math.PI*i;
            characters.Add(character);
        }

        return characters;
    }


    private Vector3 GenerateRandomClearPosition(GameObject[] obstacles)
    {
        Vector3 position = new Vector3();
        var ok = false;
        while (!ok)
        {
            ok = true;

            position = new Vector3(Random.Range(-X_WORLD_SIZE,X_WORLD_SIZE), 0, Random.Range(-Z_WORLD_SIZE,Z_WORLD_SIZE));

            foreach (var obstacle in obstacles)
            {
                var distance = (position - obstacle.transform.position).magnitude;

                //assuming obstacle is a sphere just to simplify the point selection
                if (distance < obstacle.transform.localScale.x + AVOID_MARGIN)
                {
                    ok = false;
                    break;
                }
            }
        }

        return position;
    }

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Q)) 
		{
			this.RedCharacter.Movement = null;
		} 
		else if (Input.GetKeyDown (KeyCode.B))
		{
            foreach (var character in this.Characters)
            {
                this.InitializeSecondaryCharacter(character, false);
            }
		}
		else if (Input.GetKeyDown (KeyCode.P))
		{
		    foreach (var character in this.Characters)
	        {
	            this.InitializeSecondaryCharacter(character, true);
	        }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            debugMode = !debugMode;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float angle = Vector3.Angle(ray.direction, Vector3.down) * Mathf.Deg2Rad;
            float distance = Camera.main.transform.position.y / Mathf.Cos(angle);
            click = ray.origin + (ray.direction * distance);
            click.y = 0;
            //Debug.Log("Click point: " + click);
        }

	    foreach (var character in this.Characters)
	    {
	        this.UpdateMovingGameObject(character);
	    }

        //this.UpdateMovementText();
	}

    private void UpdateMovingGameObject(DynamicCharacter movingCharacter)
    {
        if (movingCharacter.Movement != null)
        {
            movingCharacter.Update();
            movingCharacter.KinematicData.ApplyWorldLimit(X_WORLD_SIZE,Z_WORLD_SIZE);
            movingCharacter.GameObject.transform.position = movingCharacter.Movement.Character.position;
        }
    }
}

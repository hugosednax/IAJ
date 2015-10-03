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
    public const float MAX_SPEED = 20.0f;
    public const float MAX_ACCELERATION = 40.0f;
    public const float DRAG = 0.1f;
    public const float COHESION_WEIGHT = 60.0f;
    public const float SEPARATION_WEIGHT = 30.0f;
    public const float MATCH_SPEED_WEIGHT = 30.0f;
    public const float COHESION_RADIUS = 60f;
    public const float SEPARATION_FACTOR = 60.0f;
    public const float FAN_ANGLE = 30f;

    public static Vector3 click = new Vector3(-1, -1, -1);

	private DynamicCharacter RedCharacter { get; set; }
	private DynamicCharacter BlueCharacter { get; set; }
	private DynamicCharacter GreenCharacter { get; set; }

    private Text RedMovementText { get; set; }

    private BlendedMovement Blended { get; set; }

    private PriorityMovement Priority { get; set; }

    private List<DynamicCharacter> Characters { get; set; }

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

	    this.RedMovementText = GameObject.Find("RedMovement").GetComponent<Text>();
		var redObj = GameObject.Find ("Red");

	    this.RedCharacter = new DynamicCharacter(redObj)
	    {
	        Drag = DRAG,
	        MaxSpeed = MAX_SPEED
	    };


	    var obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

	    this.Characters = this.CloneSecondaryCharacters(redObj, 10, obstacles);

	    this.Characters.Add(this.RedCharacter);

        this.InitializeMainCharacter(obstacles);

        //initialize all but the last character (because it was already initialized as the main character)
	    foreach (var character in this.Characters.Take(this.Characters.Count-1))
	    {
	        this.InitializeSecondaryCharacter(character, obstacles);
	    }
	}

    private void InitializeMainCharacter(GameObject[] obstacles)
    {
        this.Priority = new PriorityMovement
        {
            Character = this.RedCharacter.KinematicData
        };

        this.Blended = new BlendedMovement
        {
            Character = this.RedCharacter.KinematicData
        };
        
	    foreach (var obstacle in obstacles)
	    {
            //TODO: add your AvoidObstacle movement here
            
            DynamicAvoidObstacle avoidObstacleMovement = new DynamicAvoidObstacle(obstacle)
            {
                MaxAcceleration = MAX_ACCELERATION,
                AvoidMargin = AVOID_MARGIN,
                MaxLookAhead = MAX_LOOK_AHEAD,
                Character = this.RedCharacter.KinematicData,
                MovementDebugColor = Color.magenta
            };
            this.Blended.Movements.Add(new MovementWithWeight(avoidObstacleMovement,70.0f));
            this.Priority.Movements.Add(avoidObstacleMovement);
            
	    }

        foreach (var otherCharacter in this.Characters)
        {
            if (otherCharacter != this.RedCharacter)
            {
                //TODO: add your AvoidCharacter movement here
                var avoidCharacter = new DynamicAvoidCharacter()
				{
                    Character = this.RedCharacter.KinematicData,
                    MaxAcceleration = MAX_ACCELERATION,
                    AvoidMargin = AVOID_MARGIN,
					CollisionRadius = 2f,
					MaxTimeLookAhead = 1f,
					Target = otherCharacter.KinematicData,
                    MovementDebugColor = Color.cyan 
                };

                this.Priority.Movements.Add(avoidCharacter);
            }
        }

        
        // TODO: add your wander behaviour here!
        var wander = new DynamicWander
        {
            Character = this.RedCharacter.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MovementDebugColor = Color.yellow
        };

        this.Priority.Movements.Add(wander);
        this.Blended.Movements.Add(new MovementWithWeight(wander,obstacles.Length+this.Characters.Count));

        this.RedCharacter.Movement = this.Blended;

    }

    private void InitializeSecondaryCharacter(DynamicCharacter character, GameObject[] obstacles)
    {
        var priority = new PriorityMovement
        {
            Character = character.KinematicData
        };

        var blended = new BlendedMovement
        {
            Character = character.KinematicData,
        };

        List<KinematicData> otherCharacters = new List<KinematicData>();

        foreach (DynamicCharacter otherChar in this.Characters)
        {
            if (otherChar != character)
                otherCharacters.Add(otherChar.KinematicData);
        }
        otherCharacters.Remove(RedCharacter.KinematicData); // REMOVE THIS LINE AFTER

	    foreach (var obstacle in obstacles)
	    {

            //TODO: add your AvoidObstacle movement here
            DynamicAvoidObstacle avoidObstacleMovement = new DynamicAvoidObstacle(obstacle)
            {
                MaxAcceleration = MAX_ACCELERATION,
                AvoidMargin = AVOID_MARGIN,
                MaxLookAhead = MAX_LOOK_AHEAD,
                Character = character.KinematicData,
                MovementDebugColor = Color.magenta
            };
            blended.Movements.Add(new MovementWithWeight(avoidObstacleMovement, 5f));
            priority.Movements.Add(avoidObstacleMovement);
	    }

        /*foreach (var otherCharacter in this.Characters)
        {
            if (otherCharacter != character)
            {*/
                //TODO: add your avoidCharacter movement here
                /*var avoidCharacter = new DynamicAvoidCharacter()
                {
                    Character = character.KinematicData,
                    MaxAcceleration = MAX_ACCELERATION,
                    AvoidMargin = AVOID_MARGIN,
                    CollisionRadius = 2f,
                    MaxTimeLookAhead = 1f,
                    Target = otherCharacter.KinematicData,
                    MovementDebugColor = Color.cyan 
                };*/

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
                    Flock = otherCharacters
                };
                blended.Movements.Add(new MovementWithWeight(cohesionCharacter, COHESION_WEIGHT));

                var separationCharacter = new DynamicSeparation()
                {
                    Character = character.KinematicData,
                    MaxAcceleration = MAX_ACCELERATION,
                    MovementDebugColor = Color.blue,
                    Target = new KinematicData(),
                    Flock = otherCharacters,
                    Radius = COHESION_RADIUS,
                    SeparationFactor = SEPARATION_FACTOR
                };
                blended.Movements.Add(new MovementWithWeight(separationCharacter, SEPARATION_WEIGHT));

                var flockVelocityMatch = new DynamicFlockVelocityMatching()
                {
                    Character = character.KinematicData,
                    FanAngle = FAN_ANGLE,
                    Flock = otherCharacters,
                    MaxAcceleration = MAX_ACCELERATION,
                    MovementDebugColor = Color.black,
                    MovingTarget = new KinematicData(),
                    Radius = COHESION_RADIUS,
                    Target = new KinematicData(),
                    TimeToTargetSpeed = 1.5f
                };
                blended.Movements.Add(new MovementWithWeight(flockVelocityMatch, MATCH_SPEED_WEIGHT));


                //priority.Movements.Add(avoidCharacter);
           /* }
        }*/

        var straightAhead = new DynamicStraightAhead
        {
            Character = character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MovementDebugColor = Color.yellow
        };

        //priority.Movements.Add(straightAhead);
        //blended.Movements.Add(new MovementWithWeight(, 1.0f));
        blended.Movements.Add(new MovementWithWeight(separationCharacter, 5.0f));
        blended.Movements.Add(new MovementWithWeight(flockVelocityMatch, 6.0f));
        character.Movement = blended;
    }

    private List<DynamicCharacter> CloneSecondaryCharacters(GameObject objectToClone,int numberOfCharacters, GameObject[] obstacles)
    {
        var characters = new List<DynamicCharacter>();
        for (int i = 0; i < numberOfCharacters; i++)
        {
            var clone = GameObject.Instantiate(objectToClone);
            //clone.transform.position = new Vector3(30,0,i*20);
            clone.transform.position = this.GenerateRandomClearPosition(obstacles);
            var character = new DynamicCharacter(clone)
            {
                MaxSpeed = MAX_SPEED,
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
		    this.RedCharacter.Movement = this.Blended;
		}
		else if (Input.GetKeyDown (KeyCode.P))
		{
		    this.RedCharacter.Movement = this.Priority;
		}

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
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

        this.UpdateMovementText();
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

    private void UpdateMovementText()
    {
        if (this.RedCharacter.Movement == null)
        {
            this.RedMovementText.text = "Red Movement: Stationary";
        }
        else
        {
            this.RedMovementText.text = "Red Movement: " + this.RedCharacter.Movement.Name;
        }
    }
}

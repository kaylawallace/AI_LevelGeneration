using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class RollerAgent : Agent
{
    public GameObject[] floorTiles = new GameObject[16];
    public int[] tileStates = new int[16];
    public Material eSprite, hSprite, defaultSprite;
    public TargetVars Target;

    public override void OnEpisodeBegin()
    {
        // Start the agent at the position of the first floor tile (slightly up as to not get stuck / fall through)
        this.transform.localPosition = new Vector3(
            floorTiles[0].transform.localPosition.x,
            floorTiles[0].transform.localPosition.y + 2,
            floorTiles[0].transform.localPosition.z);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Iterate through all tiles in the grid, extracting their current state and converting it to an integer
        for (int i = 0; i < tileStates.Length; i++)
        {
            tileStates[i] = ConvertStateToInt(floorTiles[i]);
            sensor.AddObservation(tileStates[i]);           
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 4
        // Change to empty, change to enemy, change to health pickup, skip 

        // Define the branch for the action buffer - size set in editor 
        int changeState = actionBuffers.DiscreteActions[0];

        // Initialise agent's current and next positions 
        Vector3 currPos = this.transform.localPosition;
        Vector3 nextPos = Vector3.zero;

        // Iterate through all the tiles in the training area 
        for (int i = 0; i < floorTiles.Length; i++)
        {
            // Break out of this loop at the last tile
            if (i == 15)
            {
                break;
            }

            // Get the current tile the agent is at 
            if (currPos.x == floorTiles[i].transform.localPosition.x && currPos.z == floorTiles[i].transform.localPosition.z)
            {
                // Change tile state to empty 
                if (changeState == 1)
                {
                    // Change state 
                    tileStates[i] = 1;
                    floorTiles[i].GetComponent<TileState>().currState = TileState.State.Empty;
                    // Render state change 
                    floorTiles[i].GetComponent<MeshRenderer>().material = defaultSprite;
                    // Get position of next tile and move agent on
                    nextPos = floorTiles[i + 1].transform.localPosition;                       
                    nextPos.y += 1;
                    this.transform.localPosition = nextPos;
                }
                // Change tile state to enemy 
                else if (changeState == 2)
                {
                    // Change state 
                    tileStates[i] = 2;
                    floorTiles[i].GetComponent<TileState>().currState = TileState.State.Enemy;
                    // Render state change 
                    floorTiles[i].GetComponent<MeshRenderer>().material = eSprite;
                    // Get position of next tile and move agent on 
                    nextPos = floorTiles[i + 1].transform.localPosition;
                    nextPos.y += 1;
                    this.transform.localPosition = nextPos;
                }
                // Change tile state to health pickup
                else if (changeState == 3)
                {
                    // Change state
                    tileStates[i] = 3;
                    floorTiles[i].GetComponent<TileState>().currState = TileState.State.HealthPickup;
                    // Render state change 
                    floorTiles[i].GetComponent<MeshRenderer>().material = hSprite;
                    // Get position of next tile and move agent on 
                    nextPos = floorTiles[i + 1].transform.localPosition;
                    nextPos.y += 1;
                    this.transform.localPosition = nextPos;
                }
                // Leave tile state as is and move onto next tile 
                else if (changeState == 4)
                {
                    // Get position of next tile and move agent on
                    nextPos = floorTiles[i + 1].transform.localPosition;
                    nextPos.y += 1;
                    this.transform.localPosition = nextPos;
                }
            }
        }

        int numEnemies = 0, numHealthPickups = 0, numEmpty = 0;
        float rewardVal = 0;
        // If the agent is at the last tile position
        if (currPos.x == floorTiles[15].transform.localPosition.x && currPos.z == floorTiles[15].transform.localPosition.z)
        {
            // Iterate through the tile states and count how many of each state (enemy, health, empty) are in the grid 
            for (int i = 0; i < tileStates.Length; i++)
            {
                if (floorTiles[i].GetComponent<TileState>().currState == TileState.State.Enemy)
                {
                    numEnemies++;
                }
                else if (floorTiles[i].GetComponent<TileState>().currState == TileState.State.HealthPickup)
                {
                    numHealthPickups++;
                }
                else if (floorTiles[i].GetComponent<TileState>().currState == TileState.State.Empty)
                {
                    numEmpty++;
                }
            }

            // Calculate agent reward based on how close to the optimal values the counted values are
            // Reward +1 if the number of enemies falls between the target minimum and maximum number of enemies 
            if (numEnemies >= Target.minEnemies && numEnemies <= Target.maxEnemies)
            {
                rewardVal += 0.3f;
            }
            // Reward +0.5 if the number of enemies is one away from being within the target range 
            else if (numEnemies == Target.maxEnemies + 1)
            {
                rewardVal += 0.1f;
            }
            // Negative reward for no enemies or too many enemies 
            else
            {
                rewardVal -= 0.5f;
            }

            // Reward +1 if the number of health pickups in the tile grid is within the target range 
            if (numHealthPickups >= Target.minHealthPickups && numHealthPickups <= Target.maxHealthPickups)
            {
                rewardVal += 0.3f;
            }
            // Reward +0.5 if the number of health pickups is one away from being within the target range
            else if (numHealthPickups == Target.maxHealthPickups + 1 || numHealthPickups == Target.minHealthPickups - 1)
            {
                rewardVal += 0.1f;
            }
            //Negative reward for too many health pickups
            else
            {
                rewardVal -= 0.5f;
            }

            // Reward +0.5 if the number of empty tiles is greater than 5
            if (numEmpty > 5)
            {
                rewardVal += 0.1f;
            }
            // Reward +1 if the number of tiles is greater than 8 and less than 13 
            else  if (numEmpty > 8 && numEmpty < 13)
            {
                rewardVal += 0.3f;
            }

            if (rewardVal == 0.9f)
            {
                rewardVal = 1;
            }

            // Set the reward value and end the episode 
            Debug.Log(rewardVal);
            SetReward(rewardVal);
            EndEpisode();
        }
    }

    /*
     * Method to convert the tile states into integers for the collection of observations 
     */
    public int ConvertStateToInt(GameObject tile)
    {
        TileState.State state = tile.GetComponent<TileState>().currState;
        if (state == TileState.State.Empty)
        {
            return 1;
        }
        else if (state == TileState.State.Enemy)
        {
            return 2;
        }
        else if (state == TileState.State.HealthPickup)
        {
            return 3;
        }
        else
        {
            Debug.LogError("Tile state error");
            return 0; 
        }
    }
}

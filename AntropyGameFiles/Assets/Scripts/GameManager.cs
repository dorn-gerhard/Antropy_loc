using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{

    //general player properties
    public string playerName;
    public int totalAnts = 100;
    public int freeAnts = 100;
    //map specific properties
    public int rows = 10;
    public int columns = 10;
    public int[,] tileType;
    public int[,] distance;
    public string[,] tileName;
    public int[,] assignedMapAnts;
    public int[,] maxAssignedMapAnts;
    public int[,] resourceAmount;
    public int[,] resouceMaxAmount;
    public bool[,] isAnthill;
    public bool[,] isExplored;
    public bool[,] isVisible;
    //anthill specific properties (assuming a fixed list of chambers)
    public int[] assignedHillAnts;


    public int resources;
    public int currentSeason;
    public int currentWeather;

    public int hatcheryLevel;
    public int storageLevel;
    public int hatcheryMaxLevel;
    public int storageMaxLevel;

    public int[] hatcheryCost;
    public int[] storageCost;
    
    public int maxAntsResourceTile = 10;
    public int maxAntsAnthillTile = 10;

    /// <summary>
    /// Current Turn Number
    /// </summary>
    public int MaxTurnCount;

    /// <summary>
    /// Max allowed turn number
    /// </summary>
    public int currentTurnCount;

    /// <summary>
    /// Weight for the grass creating closer to less = more grass closer
    /// </summary>
    public float grassWeight = 1.5f;
    /// <summary>
    /// Weight for the max resources and current resources on tile, more = more resources further away
    /// </summary>
    public float resourceWeight = 0.05f;

    /// <summary>
    /// Weight for the max resources and current resources on tile, less = less resources on soil tiles
    /// </summary>
    public float soilWeight = 0.3f;


    public MapScript mapInstance;
    public MapCameraScript cameraInstance;




    // Creates an instance that is present in all other classes
    public static GameManager Instance;
        //takes care that there is only one instance of GameManager
    private void Awake()
    {
        // start of new code
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // end of new code

        Instance = this;
        DontDestroyOnLoad(gameObject);
        // optional: load last gameplay (if saved)
        //LoadLastGame();        


        //mapInstance = GameObject.Find("MapTiles").GetComponent<MapScript>();
        //cameraInstance = GameObject.Find("MapControls").GetComponent<MapCameraScript>();
    }

    [System.Serializable]
    class SaveData
    {
        // data to be saved (copy from above)
        // general player data
        public string playerName;
        public int totalAnts;
        public int freeAnts;
        //map specific properties
        public int[,] type;
        public int[,] assignedMapAnts;
        public int[,] maxAssignedMapAnts;
        public float[,] resourceAmount;
        public float[,] resouceMaxAmount;
        public bool[,] partOfAnthill;
        //anthill specific properties (assuming a fixed list of chambers)
        public int[] assignedHillAnts;

    }

    string TileName(int type) 
  {
    string type_name;

    /// TilePrefabs: [0]stone, [1]grass, [2]soil, [3]water, [4] anthill
    switch (type)
    {
      case 0:
        type_name = "Stone";
        break;
      case 1:
        type_name = "Grass";
        break;
      case 2:
        type_name = "Soil";
        break;
      case 3:
        type_name = "Water";
        break;
      case 4:
        type_name = "Anthill";
          break;
      case 5:
        type_name = "StoneBlack";
        break;
      case 6:
        type_name = "GrassBlack";
        break;
      case 7:
        type_name = "SoilBlack";
        break;
      case 8:
        type_name = "WaterBlack";
        break;
      default:
        type_name = "notSet";
        break;
    }
    return type_name;
  }

    // optional: add filename from textInput
    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.playerName = playerName;
        data.totalAnts = totalAnts;
        //....
        string json = JsonUtility.ToJson(data);
    
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    // optional: add filename
    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            playerName = data.playerName;
            totalAnts = data.totalAnts;
            //....
        }   
    }
    // Start is called before the first frame update
    void Start()
    {
        // initialize map values
        tileType = new int[rows, columns];
        distance = new int[rows, columns];
        tileName = new string[rows, columns];
        isAnthill = new bool[rows, columns];
        isExplored = new bool[rows, columns];
        isVisible = new bool[rows, columns];
        assignedMapAnts = new int[rows, columns];
        maxAssignedMapAnts = new int[rows, columns];
        resourceAmount = new int[rows, columns];
        resouceMaxAmount = new int[rows, columns];

      //Anthill values
      hatcheryLevel = 0;
      storageLevel = 0;
      hatcheryMaxLevel = 3;
      storageMaxLevel = 3;
      hatcheryCost = new int[] { 200, 400, 600, 800 };
      storageCost = new int[] { 100, 200, 400, 600 };

      //TODO DELETE THIS, after we have an actual game
      //Current Default in case someone forgets
      MaxTurnCount = 1000;
     // mapInstance = GameObject.Find("MapTiles").GetComponent<MapScript>();


        SpawnRandomMap();
      //mapInstance.SpawnRandomMap();
      //mapInstance.SpawnTerrainMap();
    }

    // Optional: Spawn Random Map here if not loaded

    public void SpawnRandomMap()
    {
    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
        int distance_anthill = 0;
        for (int j = 0; j < GameManager.Instance.columns; j++)
        {
            Debug.Log("index: " + i + " " + j);
            if(i == 0 && j == 0) 
            {
            CreateAnthillTile(i, j);
            }
            else 
            {
            WeightResourceTile(i, j, distance_anthill, GameManager.Instance.rows + GameManager.Instance.columns);
            //RandomResourceTile(i, j, distance_anthill);
            distance_anthill++;
            }
        }
    }
    
    }

    void CreateAnthillTile(int i, int j) 
    {
        tileType[i,j] = 4;
        distance[i,j] = -1;
        tileName[i,j] = (TileName(tileType[i,j]) + ": [" + i + "," + j + "]");
        isAnthill[i,j] = true;
        isExplored[i,j] = true;
        isVisible[i,j] = true;
        assignedMapAnts[i,j] = 0;
        maxAssignedMapAnts[i,j] = 0;
        resourceAmount[i,j] = 0;
        resouceMaxAmount[i,j] = 0;
    }

    void WeightResourceTile(int i, int j, int distance_anthill, int maxdist)
    {
        //int tileTypeRand = Random.Range(5, 9); //For normal map : 
        int tileTypeRand = Random.Range(0, 4);
        if (Random.Range(0, maxdist) > i+j + (maxdist) * GameManager.Instance.grassWeight)
        {
        // tileTypeRand = 6;
        tileTypeRand = 1; //Also needs to be in for normal map
        }
        tileType[i,j] = tileTypeRand;
        distance[i,j] = distance_anthill;
        tileName[i,j] = (TileName(tileType[i,j]) + ": [" + i + "," + j + "]");
        isAnthill[i,j] = false;
        isExplored[i,j] = false;
        isVisible[i,j] = true;

        assignedMapAnts[i,j] = 0;
        maxAssignedMapAnts[i,j] = maxAntsResourceTile;

        if (tileTypeRand == 6 || tileTypeRand == 7)
        {
            resouceMaxAmount[i,j] = 500;
            for (int k = 0; k < i+j; k++)
            {
               resouceMaxAmount[i,j] = (int)(resouceMaxAmount[i,j] + (resouceMaxAmount[i,j] * resourceWeight));
            }
            resourceAmount[i,j] = Random.Range(250, resouceMaxAmount[i,j]);
            if (tileTypeRand == 7)
            {
                resouceMaxAmount[i,j] = (int)(soilWeight * resouceMaxAmount[i,j]);
            }
        }
        //Random Amount of resources on the tile
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    float DistanceToHill(int pos_x, int pos_y)
    {
        
        List<float> distances = new List<float>();
        for (int i = 0; i < rows; i++){
            for (int j = 0; j < columns; j++){
                if (isAnthill[i,j])
                {
                    distances.Add( Mathf.Sqrt( (pos_x - i)^2 + (pos_y - j)^2 ));
                }
            }
        }
        return distances.Min();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextTurnScript : MonoBehaviour
{
  //private GameManager gameManager;

  private void Awake()
  {
    //gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
  }

  /// <summary>
  /// Turn Sequence, bind this to a button
  /// </summary>
  public void NextTurn() 
  {
    if(GameManager.Instance.currentTurnCount < GameManager.Instance.MaxTurnCount) 
    {
      Debug.Log("Turn: " + GameManager.Instance.currentTurnCount);
      AntTurn();
      MapTurn();
      WeatherTurn();
      EventTurn();
      SeasonTurn();
      MessageTurn();
      ExploreTurn();
      GameManager.Instance.currentTurnCount++;
    }
    else 
    {
      Debug.Log("Hit Max Turn Count, turn denied");
    }
  }

  void AntTurn() 
  {
    //Insert Ant Turn
    TileScript[,] gameMap = GameManager.Instance.mapInstance.GameMap;//game_resources.map_instance.GameMap;
    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
        //gameMap[i, j].CalculateNewResourceAmountFlat(50);
        if(gameMap[i, j].OwnedByPlayer) 
        {
          //calculate gathering rate (basic idea)
          int gathering_rate = 20;
          GameManager.Instance.resources += gathering_rate;
          gameMap[i, j].CalculateNewResourceAmount(-gathering_rate);
        }
      }
    }
  }

  void MapTurn() 
  {
    //Insert Map Turn
    //change the tile object
    TileScript[,] gameMap = GameManager.Instance.mapInstance.GameMap;//game_resources.map_instance.GameMap;

    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
        //constant growth +
        gameMap[i, j].CalculateNewResourceAmountFlat(500);

        //check if the growth if we reached a threshhold to update the tile mesh
        GameManager.Instance.mapInstance.TileErosionCheck(gameMap[i, j]);
      }
    }
  }

  void ExploreTurn()
  {
    TileScript[,] gameMap = GameManager.Instance.mapInstance.GameMap;
    int[,] adder = new int[,] { { -1, 0 }, { -1, -1 }, { -1, 1 }, { 1, 0 }, { 1, -1 }, { 1, 1 }, { 0, -1 }, { 0, 1 } };
    for (int i = 0; i < GameManager.Instance.rows; i++)
    {
      for (int j = 0; j < GameManager.Instance.columns; j++)
      {
        //constant growth +
        if(gameMap[i, j].AssignedAnts > 0)
        {
          for (int k = 0; k < adder.Length / 2; k++)
          {
            if (i + adder[k, 0] < GameManager.Instance.rows && i + adder[k, 0] >= 0 && j + adder[k, 1] < GameManager.Instance.columns && j + adder[k, 1] >= 0)
              if (gameMap[i + adder[k, 0], j + adder[k, 1]].Explored == false)
              {
                GameManager.Instance.mapInstance.SetExplored(gameMap[i + adder[k, 0], j + adder[k, 1]], true);
              }
          }
        }
        //check if the growth if we reached a threshhold to update the tile mesh
        GameManager.Instance.mapInstance.TileErosionCheck(gameMap[i, j]);
      }
    }
  }

  void WeatherTurn()
  {
    //Insert Weather Turn
  }

  void EventTurn() 
  {
    //Insert Event Turn
  }

  void MessageTurn() 
  {
    //Insert Message Turn
  }

  void SeasonTurn() 
  {
    //Insert Season Turn
  }
}

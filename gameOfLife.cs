using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;




public enum GameOfLifeBufferType
{
  buffer1, buffer2
}





public class GameOfLife
{

  // Data
  // ----
  public bool[,] cellsBuffer1;
  public bool[,] cellsBuffer2;

  public int simulationWidth;
  public int simulationHeight;

  private int terrainBorderAroundSimulation; 

  public int cellSize = 1;

  GameOfLifeBufferType writeBufferEnum = GameOfLifeBufferType.buffer1;




  // Methods
  // -------


  public GameOfLife(int width, int height, int border)
  {
    simulationWidth = width;
    simulationHeight = height;
    terrainBorderAroundSimulation = border;

    cellsBuffer1 = new bool[simulationWidth, simulationHeight];
    cellsBuffer2 = new bool[simulationWidth, simulationHeight];

  }


  public void Update()
  {
    ProcessSimulation();
    
    SwapBuffers();
    
  }



  public void ProcessSimulation()
  {
    for(int x = 0; x < simulationWidth; x++)
    {
      for (int y = 0; y < simulationHeight; y++)
      {


        ref bool readCell = ref GetCell(GetReadBufferID(), x, y);
        ref bool writeCell = ref GetCell(GetWriteBufferID(), x, y);

        // Si la cellule est vivante
        if (readCell)
        {

          writeCell = CanItStillLive(x, y);

        }

        // Si la cellule est morte
        else
        {
          writeCell = CanItLive(x, y);
        }


      }
    }
  }





  public bool CanItLive(int columnNum, int rowNum)
  {
    int livingNeighbours = 0;

    ref bool[,] plateau = ref GetReadBufferRef(); 

    // We exclude the edges of the map.
    if (columnNum == 0 || columnNum == (plateau.GetLength(0) - 1) || rowNum == 0 || rowNum == (plateau.GetLength(1) - 1))
      return false;

    // We check the neighbors
    if (plateau[columnNum - 1, rowNum])
      livingNeighbours++;
    if (plateau[columnNum - 1, rowNum + 1])
      livingNeighbours++;
    if (plateau[columnNum, rowNum + 1])
      livingNeighbours++;
    if (plateau[columnNum + 1, rowNum + 1])
      if (++livingNeighbours > 3)
        return false;
    if (plateau[columnNum + 1, rowNum])
      if (++livingNeighbours > 3)
        return false;
    if (plateau[columnNum + 1, rowNum - 1])
      if (++livingNeighbours > 3)
        return false;
    if (plateau[columnNum, rowNum - 1])
      if (++livingNeighbours > 3)
        return false;
    if (plateau[columnNum - 1, rowNum - 1])
      if (++livingNeighbours > 3)
        return false;

    // If exactly 3 neighbors, it can live.
    if (livingNeighbours == 3)
      return true;

    // Otherwise, it cannot live.
    return false;
  }



  public bool CanItStillLive(int columnNum, int rowNum)
  {
    int livingNeighbours = 0;

    ref bool[,] plateau = ref GetReadBufferRef();

    // We exclude the edges of the map.
    if (columnNum == 0 || columnNum == (plateau.GetLength(0) - 1) || rowNum == 0 || rowNum == (plateau.GetLength(1) - 1))
      return false;

    // We check the neighbors.
    if (plateau[columnNum - 1, rowNum])
      livingNeighbours += 1;
    if (plateau[columnNum - 1, rowNum + 1])
      livingNeighbours += 1;
    if (plateau[columnNum, rowNum + 1])
      livingNeighbours += 1;
    if (plateau[columnNum + 1, rowNum + 1])
      if ((livingNeighbours += 1) > 3)
        return false;
    if (plateau[columnNum + 1, rowNum])
      if ((livingNeighbours += 1) > 3)
        return false;
    if (plateau[columnNum + 1, rowNum - 1])
      if ((livingNeighbours += 1) > 3)
        return false;
    if (plateau[columnNum, rowNum - 1])
      if ((livingNeighbours += 1) > 3)
        return false;
    if (plateau[columnNum - 1, rowNum - 1])
      if ((livingNeighbours += 1) > 3)
        return false;

    // If 2 or 3 neighbors, it can continue to live.
    if (livingNeighbours >= 2) // We know it's not greater than 3 anyway.
      return true;

    // Otherwise, it cannot continue to live.
    return false;
  }


  public void RandomFeed()
  {

    ref bool[,] readBuffer = ref GetReadBufferRef();

    float percentage = 10.0f; 

    Random random = new Random();
    for (int column = 0; column < simulationWidth; ++column)
    {
      for (int row = 0; row < simulationHeight; ++row)
      {
        readBuffer[column,row] = (random.Next(100) > (100 - percentage));
      }
    }
  }




  // Returns true if the given position is in terrain.
  private bool IsInTerrain(Point position)
  {
    return (
      position.X >= 0 &&
      position.X < simulationWidth &&
      position.Y >= 0 &&
      position.Y < simulationHeight);
  }


  public void SetCellValue(bool value, int gridX, int gridY)
  {
    GetCell(GetReadBufferID(), gridX, gridY) = value; 
  }


  private GameOfLifeBufferType GetWriteBufferID()
  {
    return writeBufferEnum;
  }



  private GameOfLifeBufferType GetReadBufferID()
  {
    if (writeBufferEnum == GameOfLifeBufferType.buffer1)
    {
      return GameOfLifeBufferType.buffer2;
    }

    else
    {
      return GameOfLifeBufferType.buffer1;
    }
  }

  private ref bool[,] GetWriteBufferRef()
  {
    if(writeBufferEnum == GameOfLifeBufferType.buffer1)
    {
      return ref cellsBuffer1; 
    }

    else
    {
      return ref cellsBuffer2; 

    }

  }

  private ref bool[,] GetReadBufferRef()
  {
    if (writeBufferEnum == GameOfLifeBufferType.buffer1)
    {
      return ref cellsBuffer2;
    }

    else
    {
      return ref cellsBuffer1;

    }

  }




  private void SwapBuffers()
  {
    if (writeBufferEnum == GameOfLifeBufferType.buffer1)
    {
      writeBufferEnum = GameOfLifeBufferType.buffer2;
    }

    else
    {
      writeBufferEnum = GameOfLifeBufferType.buffer1;
    }
  }


  private ref bool GetCell(GameOfLifeBufferType bufferType, int cellX, int cellY)
  {
    if (bufferType == GameOfLifeBufferType.buffer1)
    {
      return ref cellsBuffer1[cellX, cellY];
    }

    else
    {
      return ref cellsBuffer2[cellX, cellY];
    }
  }



  public void Draw(SpriteBatch spriteBatch, Texture2D texture)
  {
    for (int x = 0; x < simulationWidth; x++)
    {
      for (int y = 0; y < simulationHeight; y++)
      {
        bool isAlive = GetCell(GetWriteBufferID(), x, y);



        // Si la cellule n'est pas vide
        if (isAlive)
        {
     
          // Création du rectangle correspondant aux coodonnées de la cellule examinée
          Rectangle cellRect = new Rectangle(
            terrainBorderAroundSimulation + x * cellSize,
            terrainBorderAroundSimulation + y * cellSize, 
            cellSize, 
            cellSize);


          // Dessine la texture en utilisant les coordonnées calculées.
          spriteBatch.Draw(texture, cellRect, Color.White);
        }
      }
    }
  }
}











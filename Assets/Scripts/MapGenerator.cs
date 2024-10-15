using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class MapGenerator : MonoBehaviour
{
    [Header("Generation Variables")]

    public int width; //width of the generation grid
    public int height; //height of the generation grid
    [Range(0, 100)] public int randomFillChance; //chance a point on the grid will be filled
    public string seed; //seed to give the map a desired generation

    [Header("Smoothing varaibles")]

    [Range(1, 10)] public int smoothingIterations; //number of iterations through smoothing loop
    [Range(1,8)]public int smoothingRequirement; //number of surrounding similar tiles to change tile type

    private int[,] map;

    //function called on first frame
    void Start()
    {
        GenerateMap();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Generating new map");
            GenerateMap();
        }
    }

    //function useed to generate a map layout
    private void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();

        
        for (int i=0; i<smoothingIterations; i++)
        {
            SmoothMap();
        }
        
    }

    //function that randomly fills the map grid
    void RandomFillMap()
    {
        System.Random prng;

        //check for seed for creation
        if (string.IsNullOrEmpty(seed))
        {
            prng = new System.Random();
        }
        else
        {
            //seeding the pseudo-random number generator
            prng = new System.Random(seed.GetHashCode());
        }

        

        //loop populating the map
        for (int x=0; x<width; x++)
        {
            for (int y=0; y<height; y++)
            {
                //determine if on edge
                if (x == 0 || x == width-1 || y == 0 || y == height-1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    //Ternary operator to determine if space if filled or empty
                    map[x, y] = (prng.Next(0, 100) < randomFillChance) ? 1 : 0;
                }
            }
        }
    }

    // Function for smoothing map output 
    private void SmoothMap()
    {
        //getting copy to work on
        int[,] map = this.map;

        //loop through the map
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int surroundingWalls = GetSurroundingWallCount(x,y);

                //determine own wall state
                if (surroundingWalls > smoothingRequirement)
                {
                    map[x, y] = 1;
                }
                else if (surroundingWalls < smoothingRequirement)
                {
                    map[x, y] = 0;
                }
            }
        }

        //updating original
        this.map = map;
    }

    // Function for getting surrounding conditions
    private int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;

        //looping through neighbors
        for (int neighbourX = gridX -1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                //boundry check
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    //check that this is not the base tile
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    //increase wall count around edge of map
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    //Function that allows for 2D map visualization through Unity's Gizmos system
    void OnDrawGizmos()
    {
        //check if map is instantiated
        if (map != null)
        {
            //Debug.Log("Draw Gizmos being called");

            //loop drawing the map
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //Ternary operator to detemine color
                    Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-width/2 + x + 0.5f, 0, -height/2 + y + 0.5f);
                    Gizmos.DrawCube(pos,Vector3.one);
                }
            }
        }   
    }
}

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public SquareGrid squareGrid;
    
    public void GenerateMesh(int[,] map, float squareSize)
    {
        squareGrid = new SquareGrid(map, squareSize);
    }

    private void OnDrawGizmos()
    {
        if (squareGrid != null)
        {
            for (int x =0; x <squareGrid.squares.GetLength(0); x++)
            {
                for (int y =0; y< squareGrid.squares.GetLength(1); y++)
                {
                    Gizmos.color = (squareGrid.squares[x, y].topLeft.active) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x,y].topLeft.pos, Vector3.one * 0.4f);

                    Gizmos.color = (squareGrid.squares[x, y].topRight.active) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].topRight.pos, Vector3.one * 0.4f);

                    Gizmos.color = (squareGrid.squares[x, y].bottomLeft.active) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].bottomLeft.pos, Vector3.one * 0.4f);

                    Gizmos.color = (squareGrid.squares[x, y].bottomRight.active) ? Color.black : Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x, y].bottomRight.pos, Vector3.one * 0.4f);

                    Gizmos.color = Color.gray;
                    Gizmos.DrawCube(squareGrid.squares[x,y].centerTop.pos, Vector3.one * 0.15f);
                    Gizmos.DrawCube(squareGrid.squares[x, y].centerLeft.pos, Vector3.one * 0.15f);
                    Gizmos.DrawCube(squareGrid.squares[x, y].centerRight.pos, Vector3.one * 0.15f);
                    Gizmos.DrawCube(squareGrid.squares[x, y].centerBottom.pos, Vector3.one * 0.15f);
                }
            }
        }
    }

    //Class referencing a "node" which is the middle points in the sides of a square
    public class Node
    {
        public Vector3 pos;
        public int vertexIndex = -1;

        public Node(Vector3 pos)
        {
            this.pos = pos;
        }
    }

    //Class referencing a "control node" which is the corner point of a square
    public class ControlNode : Node
    {
        public bool active;
        public Node above, right;

        public ControlNode(Vector3 pos, bool active, float squareSize) : base(pos)
        {
            //determines if the node is active (on / off)
            this.active = active;

            //assigning sub nodes to the control node
            above = new Node(pos + Vector3.forward * squareSize / 2f);
            right = new Node(pos + Vector3.right * squareSize / 2f);
        }
    }

    //Class referencing a "square" object made up of nodes
    public class Square
    {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centerTop, centerRight, centerBottom, centerLeft;

        public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft)
        {
            //Assigning control nodes
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomRight = bottomRight;
            this.bottomLeft = bottomLeft;

            //Setting sub nodes
            centerTop = topLeft.right;
            centerBottom = bottomLeft.right;
            centerLeft = bottomLeft.above;
            centerRight = bottomRight.above;
        }
    }

    //Class defining the grid of "squares" that a map is made of
    public class SquareGrid
    {
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize)
        {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            //looping through control node creation
            for (int x=0; x< nodeCountX; x++)
            {
                for (int y=0; y< nodeCountY; y++)
                {
                    Vector3 pos = new Vector3(-mapWidth/2 + x * squareSize + squareSize/2, 0, -mapHeight/2 + y * squareSize + squareSize/2);
                    controlNodes[x, y] = new ControlNode(pos, map[x,y] == 1, squareSize);
                }
            }

            squares = new Square[nodeCountX - 1, nodeCountY - 1];

            //looping through control square creation
            for (int x = 0; x < nodeCountX-1; x++)
            {
                for (int y = 0; y < nodeCountY-1; y++)
                {
                    squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x+1,y+1], controlNodes[x+1, y], controlNodes[x, y]);
                }
            }
        }
    }
}

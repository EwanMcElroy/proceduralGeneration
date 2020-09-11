using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class proceduralGenerationScript : MonoBehaviour
{
    #region Variables
    #region componenets
    private Grid grid;
    #endregion
    #region private variables
    private int posX, posY;
    private int move;
    [SerializeField] private float sizeMult = 8;
    [SerializeField] private float waitTime = 0;
    private bool firstPoint = true;
    private bool downOne = false;
    private bool finished = false;
    private List<Vector3Int> worldPositions = new List<Vector3Int>()
    { new Vector3Int(-4, 1, 0), new Vector3Int(-3, 1, 0), new Vector3Int(-2, 1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 0), new Vector3Int(2, 1, 0), new Vector3Int(3, 1, 0),
      new Vector3Int(-4, 0, 0), new Vector3Int(-3, 0, 0), new Vector3Int(-2, 0, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 0), new Vector3Int(2, 0, 0), new Vector3Int(3, 0, 0),
      new Vector3Int(-4, -1, 0), new Vector3Int(-3, -1, 0), new Vector3Int(-2, -1, 0), new Vector3Int(-1, -1, 0), new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 0), new Vector3Int(2, -1, 0), new Vector3Int(3, -1, 0),
      new Vector3Int(-4, -2, 0), new Vector3Int(-3, -2, 0), new Vector3Int(-2, -2, 0), new Vector3Int(-1, -2, 0), new Vector3Int(0, -2, 0), new Vector3Int(1, -2, 0), new Vector3Int(2, -2, 0), new Vector3Int(3, -2, 0)};
    #endregion
    #region public variables
    public GameObject oGrid;
    public List<Vector3Int> positions;
    public GameObject[] environments, path;
    public GameObject steringAgent;
    [HideInInspector] public Vector3Int pos;
    [HideInInspector] public int startX;
    [HideInInspector] public int startY = 1;
    [HideInInspector] public List<int> pastMovement;
    #endregion
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        grid = oGrid.GetComponent<Grid>();
        StartCoroutine(proceduralGeneration(waitTime));
    }
    private void Update()
    {
        transform.position = grid.GetCellCenterWorld(pos);
    }
    private IEnumerator proceduralGeneration(float _waitTime)
    {
        yield return new WaitForSeconds(_waitTime);
        #region First Point
        if (firstPoint)
        {
            startX = Random.Range(-3, 3);
            posY = 1;
            posX = startX;
            startY = posY;
            var startPoint = Instantiate(path[3], grid.GetCellCenterWorld(new Vector3Int(startX, startY, 0)), Quaternion.identity);
            startPoint.transform.localScale *= sizeMult;
            move = 2;
            firstPoint = false;
        }
        #endregion
        else
        {
            #region Pick Direction
            if (pastMovement[pastMovement.Count - 1] == 2 && pos.x != -4 || pastMovement[pastMovement.Count - 1] == 2 && pos.x != 3)
            {
                move = Random.Range(1, 4);
            }
            else if (pastMovement[pastMovement.Count - 1] == 1 && pos.x > -3)
            {
                move = Random.Range(1, 3);
            }
            else if (pastMovement[pastMovement.Count - 1] == 3 && pos.x < 2)
            {
                move = Random.Range(2, 4);
            }
            #region fail safes
            else
            {
                if (pos.x < -3 && !downOne && pastMovement[pastMovement.Count - 1] != 1)
                {
                    move = 3;
                    downOne = true;
                }
                else if (pos.x > 2 && !downOne && pastMovement[pastMovement.Count - 1] != 3)
                {
                    move = 1;
                    downOne = true;
                }
                else
                {
                    move = 2;
                    downOne = false;
                }
            }
            #endregion
            #endregion
            #region Path Movement
            switch (move)
            {
                case (1):
                    {
                        if (pos.x < -3)
                        {
                            if (posY < -1)
                            {
                                finished = true;
                                break;
                            }
                            move = 2;
                            posY--;
                        }
                        else
                        {
                            posX--;
                        }
                        break;
                    }
                case (2):
                    {
                        if (pos.y < -1)
                        {
                            finished = true;
                            break;
                        }
                        else
                        {
                            posY--;
                        }
                        break;
                    }
                case (3):
                    {
                        if (pos.x > 2)
                        {
                            if (posY < -1)
                            {
                                finished = true;
                                break;
                            }
                            move = 2;
                            posY--;
                        }
                        else
                        {
                            posX++;
                        }
                        break;
                    }
            }
            #endregion
            #region path placement
            if (pastMovement.Count > 1)
            {
                    if (pastMovement[pastMovement.Count - 1] == 2 && move == 2)
                    {
                        Debug.Log("1");
                        var placed = Instantiate(path[1], grid.GetCellCenterWorld(pos), Quaternion.identity);
                        placed.transform.localScale *= sizeMult;
                        placed.transform.tag = "Path";
                    }
                    else if (pastMovement[pastMovement.Count - 1] == 1 && move == 2)
                    {
                    Debug.Log("2");
                        var placed = Instantiate(path[0], grid.GetCellCenterWorld(pos), Quaternion.Euler(0, 0, 270));
                        placed.transform.localScale *= sizeMult;
                        placed.transform.tag = "Path";
                    }
                    else if (pastMovement[pastMovement.Count - 1] == 1 && move == 1 || pastMovement[pastMovement.Count - 1] == 3 && move == 3)
                    {
                    Debug.Log("3");
                        var placed = Instantiate(path[1], grid.GetCellCenterWorld(pos), Quaternion.Euler(0, 0, 90));
                        placed.transform.localScale *= sizeMult;
                        placed.transform.tag = "Path";
                    }
                    else if (pastMovement[pastMovement.Count - 1] == 3 && move == 2)
                    {
                    Debug.Log("4");
                        var placed = Instantiate(path[2], grid.GetCellCenterWorld(pos), Quaternion.Euler(0, 0, 90));
                        placed.transform.localScale *= sizeMult;
                        placed.transform.tag = "Path";
                    }
                    else if (pastMovement[pastMovement.Count - 1] == 2 && move == 1)
                    {
                    Debug.Log("5");
                        var placed = Instantiate(path[2], grid.GetCellCenterWorld(pos), Quaternion.identity);
                        placed.transform.localScale *= sizeMult;
                        placed.transform.tag = "Path";
                    }
                    else if (pastMovement[pastMovement.Count - 1] == 2 && move == 3)
                    {
                    Debug.Log("6");
                        var placed = Instantiate(path[0], grid.GetCellCenterWorld(pos), Quaternion.identity);
                        placed.transform.localScale *= sizeMult;
                        placed.transform.tag = "Path";
                    }
            }
            #endregion  
        }
        #region lists
        pastMovement.Add(move);
        positions.Add(pos);
        pos = new Vector3Int(posX, posY, 0);
        transform.position = grid.GetCellCenterWorld(pos);
        #endregion
        #region recursion/starting environment
        if (!finished)
        {
            StartCoroutine(proceduralGeneration(waitTime));
        }
        else
        {
            StartCoroutine(makeEnvironment(waitTime));
        }
        #endregion
    }

    #region make environment
    private IEnumerator makeEnvironment(float _waitTIme)
    {
        for (int i = 0; i< worldPositions.Count; i++)
        {
            for (int j = 0; j < positions.Count; j++)
            {
                if (worldPositions[i] == positions[j])
                {
                    worldPositions[i] = new Vector3Int(-10, -10, 0);
                }
            }
        }
        
        for (int i = 0; i < worldPositions.Count; i++)
        {
            yield return new WaitForSeconds(_waitTIme);
            var placed = Instantiate(environments[Random.Range(0, environments.Length)],grid.GetCellCenterWorld(worldPositions[i]), Quaternion.identity);
            placed.transform.localScale *= sizeMult;
        }
        Instantiate(steringAgent);
    }
    #endregion
}

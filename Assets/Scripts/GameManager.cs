using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TextAsset m_levelText;
    public int m_levelNumber;

    public GameObject m_tileWall, m_tilePlayer, m_tileBox, m_tileGoal;

    private List<List<string>> m_levels = new List<List<string>>();

    private GameObject[,] m_wallObjects, m_boxObjects, m_goalObjects;
    private GameObject m_playerObject;

    public GameState m_currentState;

    public bool m_enablePlayerControls = false;

    // Start is called before the first frame update
    void Start()
    {
        LoadLevels();

        m_currentState = new GameState(m_levels[m_levelNumber]);

        CreateTilesFromState();
        AdjustCameraToLevel();
    }

    private void LoadLevels()
    {
        var lines = m_levelText.text.Split('\n');
        List<string> currentLevel = new List<string>();

        foreach (string line in lines)
        {
            string trimmedLine = line.TrimEnd('\r');
            if (trimmedLine.StartsWith("Level") || trimmedLine.StartsWith("'"))
            {
                // Do nothing
            }
            else if (trimmedLine == "")
            {
                if (currentLevel.Count > 0)
                {
                    m_levels.Add(currentLevel);
                    currentLevel = new List<string>();
                }
            }
            else
            {
                currentLevel.Add(trimmedLine);
            }
        }
    }

    private void CreateTilesFromState()
    {
        m_wallObjects = new GameObject[m_currentState.m_width, m_currentState.m_height];
        m_boxObjects = new GameObject[m_currentState.m_width, m_currentState.m_height];
        m_goalObjects = new GameObject[m_currentState.m_width, m_currentState.m_height];

        for (int y=0; y<m_currentState.m_height; y++)
        {
            for (int x=0; x<m_currentState.m_width; x++)
            {
                GameState.TileState tile = m_currentState.m_tiles[x, y];
                if (tile.hasWall)
                    m_wallObjects[x, y] = GameObject.Instantiate(m_tileWall, new Vector3(x, -y, 0), Quaternion.identity, transform);
                if (tile.hasBox)
                    m_boxObjects[x, y] = GameObject.Instantiate(m_tileBox, new Vector3(x, -y, 0), Quaternion.identity, transform);
                if (tile.hasGoal)
                    m_goalObjects[x, y] = GameObject.Instantiate(m_tileGoal, new Vector3(x, -y, -1), Quaternion.identity, transform);
            }
        }

        m_playerObject = GameObject.Instantiate(m_tilePlayer, new Vector3(m_currentState.m_playerPos.x, -m_currentState.m_playerPos.y, -1), Quaternion.identity, transform);
    }

    private void AdjustCameraToLevel()
    {
        Camera.main.transform.position = new Vector3(0.5f * m_currentState.m_width - 0.5f, -0.5f * m_currentState.m_height + 0.5f, Camera.main.transform.position.z);
        Camera.main.orthographicSize = 0.5f * Mathf.Max(m_currentState.m_height + 1, (m_currentState.m_width + 1) / Camera.main.aspect);
    }

    // Update is called once per frame
    void Update()
    {
        AdjustCameraToLevel();

        if (m_enablePlayerControls)
        {
            GameState.Action action = GameState.Action.None;

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                action = GameState.Action.Up;
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                action = GameState.Action.Down;
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                action = GameState.Action.Left;
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                action = GameState.Action.Right;
            else if (Input.GetKeyDown(KeyCode.R))
                StartCoroutine(RestartLevel());

            if (action != GameState.Action.None)
            {
                if (m_currentState.DoAction(action))
                {
                    AnimateAction(action);

                    if (m_currentState.IsSolved)
                    {
                        m_levelNumber++;
                        StartCoroutine(RestartLevel());
                    }
                }
            }
        }
    }

    private void AnimateAction(GameState.Action action)
    {
        // Note that this is called *after* the action has been applied to m_currentState

        m_playerObject.GetComponent<LerpMovement>().MoveTo(new Vector3(m_currentState.m_playerPos.x, -m_currentState.m_playerPos.y, m_playerObject.transform.position.z));

        var box = m_boxObjects[m_currentState.m_playerPos.x, m_currentState.m_playerPos.y];
        if (box != null)
        {
            Vector2Int delta = GameState.GetDelta(action);
            Vector2Int boxNewPos = m_currentState.m_playerPos + delta;
            m_boxObjects[m_currentState.m_playerPos.x, m_currentState.m_playerPos.y] = null;
            m_boxObjects[boxNewPos.x, boxNewPos.y] = box;
            box.GetComponent<LerpMovement>().MoveTo(new Vector3(boxNewPos.x, -boxNewPos.y, box.transform.position.z));
        }
    }

    private IEnumerator RestartLevel()
    {
        float animLength = 1.0f;

        for (float t = 0.0f; t < animLength; t += Time.deltaTime)
        {
            float p = t / animLength;

            foreach (Transform child in transform)
            {
                child.rotation = Quaternion.AngleAxis(p * 360.0f, Vector3.forward);
                child.localScale = Vector3.one * (1.0f - p);
            }

            yield return null;
        }

        foreach (Transform child in transform)
        {
            Destroy(child.transform.gameObject);
        }

        m_currentState = new GameState(m_levels[m_levelNumber]);

        CreateTilesFromState();
        AdjustCameraToLevel();
    }
}

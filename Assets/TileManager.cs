using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ConnectionDirection
{
    None,
    Up,
    Down,
    Left,
    Right
}

public class TileManager : MonoBehaviour
{
    public static TileManager instance;

    public List<Tile> currentPath = new List<Tile>();
    public Color currentPathColor = new Color(0, 0, 0, 0);

    public bool isMakingPath = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Tile[] tiles;

    private void Start()
    {
        GetTiles();
    }

    private void Update()
    {
        if (isMakingPath && Input.GetMouseButtonUp(0))
        {
            isMakingPath = false;
            // TODO: RESET PATH OR CONNECT PATH
            ClearPath();
        }
    }

    public void GetTiles()
    {
        tiles = FindObjectsOfType<Tile>();
    }

    public Tile[] GetAdjacentTiles(Tile tile)
    {
        if (currentPath.Count > 0 && currentPath[currentPath.Count - 1] != tile)
        {
            currentPath.Add(tile);
        }

        if (currentPath.Where(t => t == tile).Count() > 1)
        {
            int index = currentPath.IndexOf(tile);
            ClearPathFrom(index + 1);
        }

        List<Tile> adjacentTiles = new List<Tile>();
        foreach (Tile t in tiles)
        {
            if (t.transform.position == tile.transform.position + Vector3.up ||
                t.transform.position == tile.transform.position + Vector3.down ||
                t.transform.position == tile.transform.position + Vector3.left ||
                t.transform.position == tile.transform.position + Vector3.right)
            {
                adjacentTiles.Add(t);
            }
        }
        return adjacentTiles.ToArray();
    }

    public void StartPath(Tile tile)
    {
        if (!tile.isSource) return;
        currentPath.Clear();
        currentPath.Add(tile);
        currentPathColor = tile.sourceColor;
        isMakingPath = true;
    }

    public void ClearPathFrom(int index)
    {
        for (int i = index; i < currentPath.Count; i++)
        {
            currentPath[i].ResetNode();
        }
        currentPath.RemoveRange(index, currentPath.Count - index);
    }

    public void ClearPath()
    {
        foreach (Tile t in currentPath)
        {
            t.ResetNode();
        }

        currentPath.Clear();
        currentPathColor = new Color(0, 0, 0, 0);
    }

    public void CompletePath()
    {

        EventManager.ConnectNodes();
        CameraShaker.instance.ShakeCamera(0.8f, 0.8f);

        foreach (Tile t in currentPath)
        {
            t.MakePermanent();
        }

        currentPath.Clear();
        currentPathColor = new Color(0, 0, 0, 0);

        // Count how many source tiles are not connected
        int unconnectedSources = tiles.Where(t => t.isSource && !t.isConnected).Count();

        if (unconnectedSources == 0)
        {
            EventManager.Victory();
            ScreenFader.instance.FadeToNextScene();
            CameraShaker.instance.ShakeCamera(1f, 1f);
            Debug.Log("YOU WIN!");
        }
        else
        {
            int sourcesToDisplay = Mathf.CeilToInt(unconnectedSources / 2f);

            Debug.Log("GOOD JOB! " + sourcesToDisplay + " SOURCE(S) LEFT");

        }
    }

    public void HighLightTilesOfColor(Color color, bool enable)
    {
        foreach (Tile t in tiles)
        {
            if (t.currentColor == color)
            {
                t.Highlight(enable);
            }
        }
    }

    public void RemovePath(Color color)
    {
        foreach (Tile t in tiles)
        {
            if (t.currentColor == color)
            {
                t.ResetNode();
                t.isConnected = false;
            }
        }

        EventManager.CancelConnection();
    }
}

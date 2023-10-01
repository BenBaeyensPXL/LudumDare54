using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class Tile : MonoBehaviour
{
    public bool isSource;
    public bool isBeginning;
    public ConnectionDirection secondaryConnection;
    public ConnectionDirection primaryConnection;
    public Color sourceColor = Color.red;
    public Color currentColor = new Color(0, 0, 0, 0);

    public bool debug = false;

    public Sprite straightSprite;
    public Sprite cornerSprite;

    private SpriteRenderer spriteRenderer;
    public bool isConnected = false;


    private void Awake()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

        if (isSource)
        {
            spriteRenderer.color = sourceColor;
            currentColor = sourceColor;
        }
    }

    private void OnEnable()
    {
        EventManager.OnCancelConnection += ResetNode;
    }

    private void OnDisable()
    {
        EventManager.OnCancelConnection -= ResetNode;
    }

    private void OnMouseExit()
    {
        if (isConnected)
        {
            TileManager.instance.HighLightTilesOfColor(currentColor, false);
        }

        if (isSource) Highlight(false, false);
    }

    private void OnMouseOver()
    {
        if (isSource && Input.GetMouseButtonDown(0))
        {
            // Start the path with this color
            TileManager.instance.StartPath(this);
            isBeginning = true;
        }

        if (isConnected && Input.GetMouseButtonDown(0))
        {
            // Delete the entire path
            TileManager.instance.RemovePath(currentColor);
        }
    }

    private void OnMouseEnter()
    {
        if (!isConnected && isSource && !TileManager.instance.isMakingPath)
        {
            Highlight(true, false);
        }

        if (isSource)
        {
            return;
        }
        if (isConnected)
        {
            TileManager.instance.HighLightTilesOfColor(currentColor, true);
            return;
        }
        if (!TileManager.instance.isMakingPath) return;



        Tile[] adjacentTiles = TileManager.instance.GetAdjacentTiles(this);

        Tile validTile = null;

        foreach (Tile t in adjacentTiles)
        {
            if (CheckValidTile(t))
            {
                validTile = t;
                break;
            }
        }

        if (validTile != null)
        {
            // Reset the tile to its initial state to handle color and sprite issues.
            ResetToInitialState();

            // Now, connect to the tile
            currentColor = TileManager.instance.currentPathColor;
            spriteRenderer.color = currentColor;
            spriteRenderer.sprite = straightSprite;

            EventManager.PipePlace();
            CameraShaker.instance.ShakeCamera(0.05f, 0.05f);

            // Set the connected connection based on the valid tile's direction
            switch (GetDirectionFrom(validTile))
            {
                case ConnectionDirection.Up:
                    primaryConnection = ConnectionDirection.Down;
                    break;
                case ConnectionDirection.Down:
                    primaryConnection = ConnectionDirection.Up;
                    break;
                case ConnectionDirection.Left:
                    primaryConnection = ConnectionDirection.Right;
                    break;
                case ConnectionDirection.Right:
                    primaryConnection = ConnectionDirection.Left;
                    break;
            }

            ConnectToPreviousTile(validTile);


            // Call ConnectToNextTile for setting up sprites and rotations
            if (!validTile.isSource)
            {
                validTile.ConnectToNextTile(this);
            }

            // Check for neighboring source
            CheckForNeighboringSource(adjacentTiles);
        }
    }

    public void ConnectToPreviousTile(Tile tile)
    {
        // Calculate relative direction from tile to connectionDirection
        ConnectionDirection dir = GetDirectionTo(tile);

        if (dir == ConnectionDirection.Left || dir == ConnectionDirection.Right)
        {
            spriteRenderer.sprite = straightSprite;
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            spriteRenderer.sprite = straightSprite;
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }


    public void ResetNode()
    {
        if (!isSource)
        {
            spriteRenderer.sprite = null;
            spriteRenderer.color = currentColor = new Color(0, 0, 0, 0); // Changed from Color.white
            primaryConnection = ConnectionDirection.None;
            secondaryConnection = ConnectionDirection.None;
        }
        isBeginning = false;
    }


    // Called by next tile
    public void ConnectToNextTile(Tile tile)
    {
        ConnectionDirection tileDirection = GetDirectionTo(tile);
        secondaryConnection = tileDirection;

        if (primaryConnection == ConnectionDirection.Up && tileDirection == ConnectionDirection.Right ||
            primaryConnection == ConnectionDirection.Right && tileDirection == ConnectionDirection.Up)
        {
            spriteRenderer.sprite = cornerSprite;
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 270);
        }
        else if (primaryConnection == ConnectionDirection.Up && tileDirection == ConnectionDirection.Left ||
                primaryConnection == ConnectionDirection.Left && tileDirection == ConnectionDirection.Up)
        {
            spriteRenderer.sprite = cornerSprite;
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (primaryConnection == ConnectionDirection.Down && tileDirection == ConnectionDirection.Right ||
                primaryConnection == ConnectionDirection.Right && tileDirection == ConnectionDirection.Down)
        {
            spriteRenderer.sprite = cornerSprite;
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (primaryConnection == ConnectionDirection.Down && tileDirection == ConnectionDirection.Left ||
                primaryConnection == ConnectionDirection.Left && tileDirection == ConnectionDirection.Down)
        {
            spriteRenderer.sprite = cornerSprite;
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            spriteRenderer.sprite = straightSprite;
            if (tileDirection == ConnectionDirection.Up || tileDirection == ConnectionDirection.Down)
            {
                spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            else
            {
                spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }



    public bool CheckValidTile(Tile tile)
    {
        if (tile.isSource && TileManager.instance.currentPath.Count > 2)
        {
            return false;
        }
        if (CheckValidDirection(tile) && tile.currentColor == TileManager.instance.currentPathColor && !tile.isConnected)
        {
            Debug.Log("Valid tile!", tile.gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }

    public ConnectionDirection GetDirectionFrom(Tile tile)
    {
        // Calculate relative direction from tile to connectionDirection
        ConnectionDirection canConnectWith = ConnectionDirection.Up;

        if (tile.transform.position.y < transform.position.y)
        {
            canConnectWith = ConnectionDirection.Up;
        }
        else if (tile.transform.position.y > transform.position.y)
        {
            canConnectWith = ConnectionDirection.Down;
        }
        else if (tile.transform.position.x < transform.position.x)
        {
            canConnectWith = ConnectionDirection.Right;
        }
        else if (tile.transform.position.x > transform.position.x)
        {
            canConnectWith = ConnectionDirection.Left;
        }

        return canConnectWith;
    }

    public ConnectionDirection GetDirectionTo(Tile tile)
    {
        // Calculate relative direction from tile to connectionDirection
        ConnectionDirection canConnectWith = ConnectionDirection.Up;

        if (tile.transform.position.y < transform.position.y)
        {
            canConnectWith = ConnectionDirection.Down;
        }
        else if (tile.transform.position.y > transform.position.y)
        {
            canConnectWith = ConnectionDirection.Up;
        }
        else if (tile.transform.position.x < transform.position.x)
        {
            canConnectWith = ConnectionDirection.Left;
        }
        else if (tile.transform.position.x > transform.position.x)
        {
            canConnectWith = ConnectionDirection.Right;
        }

        return canConnectWith;
    }

    public bool CheckValidDirection(Tile tile)
    {
        // Can connect with a tile if it is a source with free tile x
        ConnectionDirection canConnectWith = GetDirectionFrom(tile);

        if (tile.isSource && canConnectWith != tile.secondaryConnection)
        {
            return false;
        }
        else
        {
            return true;
        }

    }

    public void ResetToInitialState()
    {
        if (isSource)
        {
            spriteRenderer.color = sourceColor;
            currentColor = sourceColor;
        }
        else
        {
            spriteRenderer.color = new Color(0, 0, 0, 0);
            currentColor = new Color(0, 0, 0, 0);
            spriteRenderer.sprite = null;
        }
    }

    public void MakePermanent()
    {
        isConnected = true;
        spriteRenderer.color = currentColor;
    }

    public void CheckForNeighboringSource(Tile[] adjacentTiles)
    {
        int totalNeighboringTilesOfColor = adjacentTiles.Where(t => t.currentColor == currentColor).Count();
        foreach (Tile t in adjacentTiles)
        {
            if (t.isSource && !t.isBeginning && t.currentColor == currentColor && CheckValidDirection(t) && totalNeighboringTilesOfColor > 1)
            {
                TileManager.instance.currentPath.Add(t);
                ConnectToNextTile(t);
                TileManager.instance.CompletePath();
                Debug.Log("Found neighboring source! PATH CONNECTED");
            }
        }
    }

    public void Highlight(bool enable, bool fullPath = true)
    {
        if (fullPath && isSource) return;

        // Make the color slightly lighter
        if (enable)
            spriteRenderer.color = currentColor - new Color(0f, 0f, 0f, 0.4f);
        else
        {
            spriteRenderer.color = currentColor;
        }
    }
}

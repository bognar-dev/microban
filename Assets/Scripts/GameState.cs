using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameState : System.IEquatable<GameState>
{
    public struct TileState
    {
        public TileState(char ch)
        {
            hasWall = (ch == '#');
            hasBox = (ch == '$' || ch == '*');
            hasGoal = (ch == '.' || ch == '*' || ch == '+');
        }

        public char ToChar(bool hasPlayer)
        {
            if (hasWall)
                return '#';
            else if (hasBox)
                return hasGoal ? '*' : '$';
            else if (hasPlayer)
                return hasGoal ? '+' : '@';
            else if (hasGoal)
                return '.';
            else
                return ' ';
        }

        public bool hasWall, hasBox, hasGoal;
    }

    public enum Action
    {
        None, Up, Down, Left, Right
    }

    public TileState[,] m_tiles;

    public int m_width, m_height;
    public Vector2Int m_playerPos;

    public GameState(List<string> level)
    {
        m_width = level.Max(line => line.Length);
        m_height = level.Count;

        m_tiles = new TileState[m_width, m_height];

        for (int y = 0; y < m_height; y++)
        {
            int x;
            for (x = 0; x < level[y].Length; x++)
            {
                char ch = level[y][x];
                m_tiles[x, y] = new TileState(ch);
                if (ch == '@' || ch == '+')
                    m_playerPos = new Vector2Int(x, y);
            }

            for (; x < m_width; x++)
                m_tiles[x, y] = new TileState(' ');
        }
    }

    public GameState(GameState other)
    {
        m_width = other.m_width;
        m_height = other.m_height;
        m_playerPos = other.m_playerPos;

        m_tiles = new TileState[m_width, m_height];
        for (int y=0; y<m_height; y++)
            for (int x=0; x<m_width; x++)
                m_tiles[x, y] = other.m_tiles[x, y];
    }

    public GameState Copy()
    {
        return new GameState(this);
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int y=0; y<m_height; y++)
        {
            for (int x=0; x<m_width; x++)
            {
                sb.Append(m_tiles[x, y].ToChar(x == m_playerPos.x && y == m_playerPos.y));
            }
            sb.Append("\n");
        }

        return sb.ToString();
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as GameState);
    }

    public bool Equals(GameState other)
    {
        return other != null && this.ToString() == other.ToString();
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    static public Vector2Int GetDelta(Action action)
    {
        switch (action)
        {
            case Action.Up: return new Vector2Int(0, -1);
            case Action.Down: return new Vector2Int(0, 1);
            case Action.Left: return new Vector2Int(-1, 0);
            case Action.Right: return new Vector2Int(1, 0);
            default: return Vector2Int.zero;
        }
    }

    public bool IsActionAvailable(Action action)
    {
        Vector2Int delta = GetDelta(action);
        Vector2Int targetPos = m_playerPos + delta;
        var targetTile = m_tiles[targetPos.x, targetPos.y];

        if (targetTile.hasWall)
        {
            return false;
        }
        else if (targetTile.hasBox)
        {
            Vector2Int boxTargetPos = targetPos + delta;
            var boxTargetTile = m_tiles[boxTargetPos.x, boxTargetPos.y];
            if (!boxTargetTile.hasWall && !boxTargetTile.hasBox)
                return true;
            else
                return false;
        }
        else
        {
            return true;
        }
    }

    public bool DoAction(Action action)
    {
        if (IsActionAvailable(action))
        {
            Vector2Int delta = GetDelta(action);
            Vector2Int targetPos = m_playerPos + delta;
            var targetTile = m_tiles[targetPos.x, targetPos.y];

            if (targetTile.hasBox)
            {
                Vector2Int boxTargetPos = targetPos + delta;
                m_tiles[boxTargetPos.x, boxTargetPos.y].hasBox = true;
                m_tiles[targetPos.x, targetPos.y].hasBox = false;
            }

            m_playerPos = targetPos;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsSolved
    { 
        get 
        {
            for (int y=0; y<m_height; y++)
            {
                for (int x=0; x<m_width; x++)
                {
                    var tile = m_tiles[x, y];

                    // If there is a box without a goal, or a goal without a box, then the level isn't solved
                    if (tile.hasGoal != tile.hasBox)
                        return false;
                }
            }

            return true;
        }
    }
}

// Author(s): Paul Calande
// The grid script for Fruit Gunch.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Client.Events;
using System.Linq;

public class Player
{
    int x;
    int y;
    string name;
    Color color;
    List2<FruitGridElement> grid;

    bool hasGun = false;
    
    public Player(string name, List2<FruitGridElement> grid)
    {
        this.name = name;
        this.grid = grid;

        x = 0;
        y = 0;
        while (grid.At(x, y).IsOccupied())
        {
            x = UnityEngine.Random.Range(0, 8);
            y = UnityEngine.Random.Range(0, 8);
        }
        //color = Random.ColorHSV();
        color = Color.red;

        OccupyCurrentCell();
    }

    public void GetGun()
    {
        hasGun = true;
    }

    public void LoseGun()
    {
        hasGun = false;
        GetCurrentCell().SetPlayerGun(false);
    }

    public FruitGridElement GetCurrentCell()
    {
        return grid.At(x, y);
    }

    public void ClearPlayerFromCell()
    {
        FruitGridElement element = grid.At(x, y);
        element.SetName("");
        element.SetOccupied(false);
        element.SetPlayerGun(false);
    }

    void OccupyCurrentCell()
    {
        FruitGridElement element = grid.At(x, y);
        element.SetName(name);
        element.SetColor(color);
        element.SetOccupied(true);
        if (hasGun)
        {
            element.SetPlayerGun(true);
        }
    }

    public void MoveHorizontal(int xdelta)
    {
        ClearPlayerFromCell();

        int sign = UtilMath.SignWithZero(xdelta);
        int result = x;
        for (int i = sign; i != xdelta + sign; i += sign)
        {
            if (!grid.IsWithinMatrix(x + i, y))
            {
                break;
            }
            if (grid.At(x + i, y).IsOccupied())
            {
                break;
            }
            result = x + i;
            FruitGridElement element = grid.At(result, y);
            if (element.HasGun())
            {
                GetGun();
                element.SetGun(false);
            }
        }
        x = result;

        OccupyCurrentCell();
    }

    public void MoveVertical(int ydelta)
    {
        ClearPlayerFromCell();

        int sign = UtilMath.SignWithZero(ydelta);
        int result = y;
        for (int i = sign; i != ydelta + sign; i += sign)
        {
            if (!grid.IsWithinMatrix(x, y + i))
            {
                break;
            }
            if (grid.At(x, y + i).IsOccupied())
            {
                break;
            }
            result = y + i;
            FruitGridElement element = grid.At(x, result);
            if (element.HasGun())
            {
                GetGun();
                element.SetGun(false);
            }
        }
        y = result;

        OccupyCurrentCell();
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }

    public Color GetColor()
    {
        return color;
    }

    public string GetName()
    {
        return name;
    }

    public bool HasGun()
    {
        return hasGun;
    }
}

// The collection of all players who have joined the game.
public class Playerbase
{
    public enum JoinResult
    {
        Success,
        AlreadyJoined,
        GameIsFull
    }

    // String mapped to player.
    Dictionary<string, Player> players = new Dictionary<string, Player>();
    // Reference to the grid.
    List2<FruitGridElement> grid;

    public Playerbase(List2<FruitGridElement> grid)
    {
        this.grid = grid;
    }

    public bool PlayerIsInGame(string playerName)
    {
        return players.ContainsKey(playerName);
    }

    // TODO: Check if game is full.
    public JoinResult TryJoin(string playerName)
    {
        if (PlayerIsInGame(playerName))
        {
            return JoinResult.AlreadyJoined;
        }
        else
        {
            players[playerName] = new Player(playerName, grid);
            return JoinResult.Success;
        }
    }

    public bool TryGetPlayer(string playerName, out Player player)
    {
        return players.TryGetValue(playerName, out player);
    }

    // Kills the given player.
    public void Kill(Player player)
    {
        player.ClearPlayerFromCell();
        players.Remove(player.GetName());
    }

    // Kills a random player.
    // callerName refers to the name of the player calling this method.
    // The player calling this method cannot be chosen.
    public void KillRandom(string callerName)
    {
        Player player;
        if (TryGetPlayer(callerName, out player))
        {
            List<Player> possiblePlayers = players.Values.ToList();
            possiblePlayers.Remove(player);
            if (possiblePlayers.Count != 0)
            {
                Kill(possiblePlayers.GetRandomElement());
            }
        }
    }

    public bool HasGun(string playerName)
    {
        Player player;
        if (players.TryGetValue(playerName, out player))
        {
            return player.HasGun();
        }
        return false;
    }

    public void RemoveGun(string playerName)
    {
        Player player;
        if (players.TryGetValue(playerName, out player))
        {
            player.LoseGun();
        }
    }
}

public class FruitGrid : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The prefab to use for a grid element.")]
    GameObject prefabCell;
    [SerializeField]
    [Tooltip("Grid container.")]
    RectTransform gridContainer;
    [SerializeField]
    [Tooltip("The prefab for a marking on the grid axis.")]
    GameObject prefabAxisMarking;

    // Could probably be segmented into a different class.
    [SerializeField]
    [Tooltip("Reference to the Twitch client.")]
    TwitchClient client;

    List2<FruitGridElement> elements;

    [SerializeField]
    [Tooltip("The width of the grid.")]
    int width = 8;
    [SerializeField]
    [Tooltip("The height of the grid.")]
    int height = 8;

    // The collection of players on the grid.
    Playerbase players;

    private void Start()
    {
        /*
        GridSpace2<FruitGridElement> grid = new GridSpace2<FruitGridElement>(
            width, height, true, gridContainer, CoordinateOrder2D.RightThenUp,
            prefabCell);
        grid.CreateAxisMarkings(prefabAxisMarking);
        elements = grid.GetContents();
        */

        elements = RectTransformGridCreator.MakeGrid<FruitGridElement>(
            width, height, prefabCell, true, gridContainer,
            prefabAxisMarking: prefabAxisMarking);

        players = new Playerbase(elements);

        client.ClientCommandReceived += ClientCommandReceived;
    }

    private FruitGridElement GetElement(int x, int y)
    {
        return elements.At(x, y);
    }

    // This method runs when a player successfully enters a Twitch chat command.
    private void PlayerDidCommand()
    {
        //Debug.Log("FruitGrid.PlayerDidCommand()");
    }

    private bool CommandGetCoordinates(List<string> commandArgs, out int x, out int y)
    {
        int.TryParse(commandArgs[0], out x);
        int.TryParse(commandArgs[1], out y);
        // Compensate for the lack of zero-indexing.
        x -= 1;
        y -= 1;
        return elements.IsWithinMatrix(x, y);
    }

    private void CommandPlayerMove(List<string> commandArgs, string sourceName,
        System.Action<Player, int> callback)
    {
        Player player;
        if (players.TryGetPlayer(sourceName, out player))
        {
            if (commandArgs.Count >= 1)
            {
                int x;
                if (int.TryParse(commandArgs[0], out x))
                {
                    //player.MoveHorizontal(x);
                    callback(player, x);
                }
            }
        }
    }

    private void ClientCommandReceived(object sender, OnChatCommandReceivedArgs e)
    {
        string command = e.Command.CommandText;
        string sourceName = e.Command.ChatMessage.Username;

        switch (command)
        {
            case "help":
                client.SendWhisper(sourceName, "no");
                break;

            case "join":
                players.TryJoin(sourceName);
                break;

            case "r":
                CommandPlayerMove(e.Command.ArgumentsAsList, sourceName,
                    (p, x) => p.MoveHorizontal(x));
                break;
            case "l":
                CommandPlayerMove(e.Command.ArgumentsAsList, sourceName,
                    (p, x) => p.MoveHorizontal(-x));
                break;
            case "u":
                CommandPlayerMove(e.Command.ArgumentsAsList, sourceName,
                    (p, x) => p.MoveVertical(x));
                break;
            case "d":
                CommandPlayerMove(e.Command.ArgumentsAsList, sourceName,
                    (p, x) => p.MoveVertical(-x));
                break;

            case "random":
                if (players.HasGun(sourceName))
                {
                    players.KillRandom(sourceName);
                    players.RemoveGun(sourceName);
                }
                break;

            default:
                if (players.HasGun(sourceName))
                {
                    Player player;
                    if (players.TryGetPlayer(command, out player))
                    {
                        players.Kill(player);
                        players.RemoveGun(sourceName);
                    }
                }
                break;

                /*
            default:
                // Spawn an object at the given coordinates.
                string objectName = command;
                if (e.Command.ArgumentsAsList.Count >= 2)
                {
                    if (CommandGetCoordinates(e.Command.ArgumentsAsList, out int x, out int y))
                    {
                        //Spawn(objectName, x, y, sourceName);
                        PlayerDidCommand();
                        return;
                    }
                }

                // If control reaches this point, the spawn command didn't work.
                client.SendWhisper(sourceName,
                    "I'm an idiot robot, so I don't know that command. Use !help for a list of commands.");
                break;
                */
        }
    }
}
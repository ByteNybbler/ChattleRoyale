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
    
    public Player(string name, List2<FruitGridElement> grid)
    {
        x = UnityEngine.Random.Range(0, 8);
        y = UnityEngine.Random.Range(0, 8);
        color = Random.ColorHSV();
        this.grid = grid;
    }

    public void MoveHorizontal(int xd)
    {
        grid.At(x, y).SetName("");
        x = Mathf.Clamp(x + xd, 0, 7);
        grid.At(x, y).SetName(name);
        grid.At(x, y).SetColor(color);
    }

    public void MoveVertical(int yd)
    {
        grid.At(x, y).SetName("");
        y = Mathf.Clamp(y + yd, 0, 7);
        grid.At(x, y).SetName(name);
        grid.At(x, y).SetColor(color);
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
    Dictionary<string, Player> players;
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

    // The name that the SVAI uses when running commands.
    const string botName = "Bot";

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
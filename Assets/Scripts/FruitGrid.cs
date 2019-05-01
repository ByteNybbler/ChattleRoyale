// Author(s): Paul Calande
// The grid script for Fruit Gunch.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Client.Events;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public enum GameState
{
    Lobby,
    Gameplay
}

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

        // TODO: Rewrite this so it doesn't have to check for grid occupation.
        x = UnityEngine.Random.Range(0, 8);
        y = UnityEngine.Random.Range(0, 8);
        while (grid.At(x, y).IsOccupied())
        {
            x = UnityEngine.Random.Range(0, 8);
            y = UnityEngine.Random.Range(0, 8);
        }
        color = Random.ColorHSV(0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);

        OccupyCurrentCell();
    }

    public void GetGun()
    {
        hasGun = true;
    }

    public Color GetColor()
    {
        return color;
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }

    public string GetName()
    {
        return name;
    }

    public bool HasGun()
    {
        return hasGun;
    }

    public string GetColoredName()
    {
        return UtilColor.EncloseInStyleTags(color, name);
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
        element.SetColor(new Color(0, 0, 0, 0));
        element.SetOccupied(false);
        element.SetPlayerGun(false);
    }

    void OccupyCurrentCell()
    {
        FruitGridElement element = grid.At(x, y);
        element.SetName(name);
        element.SetColor(color);
        element.SetOccupied(true);
        if (element.HasGun())
        {
            GetGun();
            element.SetGun(false);
        }
        if (hasGun)
        {
            element.SetPlayerGun(true);
        }
    }

    // Returns false if the player collides with something.
    public bool MoveHorizontal(int xdelta)
    {
        ClearPlayerFromCell();

        int sign = UtilMath.SignWithZero(xdelta);
        int result = x;
        bool collision = false;
        for (int i = sign; i != xdelta + sign; i += sign)
        {
            if (!grid.IsWithinMatrix(x + i, y))
            {
                break;
            }
            if (grid.At(x + i, y).IsOccupied())
            {
                collision = true;
                break;
            }
            result = x + i;
            /*
            FruitGridElement element = grid.At(result, y);
            if (element.HasGun())
            {
                GetGun();
                element.SetGun(false);
            }
            */
        }
        x = result;

        OccupyCurrentCell();
        return !collision;
    }

    public bool MoveVertical(int ydelta)
    {
        ClearPlayerFromCell();

        int sign = UtilMath.SignWithZero(ydelta);
        int result = y;
        bool collision = false;
        for (int i = sign; i != ydelta + sign; i += sign)
        {
            if (!grid.IsWithinMatrix(x, y + i))
            {
                break;
            }
            if (grid.At(x, y + i).IsOccupied())
            {
                collision = true;
                break;
            }
            result = y + i;
            /*
            FruitGridElement element = grid.At(x, result);
            if (element.HasGun())
            {
                GetGun();
                element.SetGun(false);
            }
            */
        }
        y = result;

        OccupyCurrentCell();
        return !collision;
    }
}

// The collection of all players who have joined the game.
public class Playerbase
{
    public enum JoinResult
    {
        Success,
        AlreadyJoined,
        NameAlreadyTaken,
        GameIsFull
    }

    // Dictionary mapping chat usernames to player instances.
    Dictionary<string, Player> players = new Dictionary<string, Player>();
    // Dictionary mapping in-game usernames to chat usernames.
    Dictionary<string, string> gameNamesToChatNames = new Dictionary<string, string>();
    // Reference to the text log.
    Text textLog;

    // Reference to the grid.
    List2<FruitGridElement> grid;
    // Reference to the audio controller.
    AudioController ac;

    // How many players are present at the start of the game.
    int playerCountStart = 0;
    // How many players are currently alive.
    int playerCountAlive = 0;

    public Playerbase(Text textLog, List2<FruitGridElement> grid)
    {
        this.textLog = textLog;
        this.grid = grid;

        ac = ServiceLocator.GetAudioController();
    }

    public bool PlayerIsInGame(string playerName)
    {
        return players.ContainsKey(playerName);
    }

    public bool GameNameIsTaken(string gameName)
    {
        //return players.Values.Any((p) => p.GetName() == gameName);
        return gameNamesToChatNames.ContainsKey(gameName);
    }

    public bool TryConvertToChatName(ref string name)
    {
        string newName = name;
        newName = newName.ToUpper();
        if (gameNamesToChatNames.TryGetValue(newName, out newName))
        {
            name = newName;
            return true;
        }
        return false;
    }

    // TODO: Check if game is full.
    public JoinResult TryJoin(string playerName, string gameName)
    {
        if (PlayerIsInGame(playerName))
        {
            return JoinResult.AlreadyJoined;
        }
        else if (GameNameIsTaken(gameName))
        {
            return JoinResult.NameAlreadyTaken;
        }
        else
        {
            players[playerName] = new Player(gameName, grid);
            gameNamesToChatNames[gameName] = playerName;
            ++playerCountStart;
            ++playerCountAlive;
            return JoinResult.Success;
        }
    }

    public bool TryGetPlayer(string playerName, out Player player)
    {
        TryConvertToChatName(ref playerName);
        return players.TryGetValue(playerName, out player);
    }

    // Removes the given player from the game.
    private void RemovePlayerFromGame(Player player)
    {
        player.ClearPlayerFromCell();
        string name = player.GetName();
        players.Remove(gameNamesToChatNames[name]);

        --playerCountAlive;
        float livingPlayerRatio = (float)playerCountAlive / playerCountStart;
        if (livingPlayerRatio < 0.666f)
        {
            ac.SetMusicChannelVolume(1, 1.0f, 1.0f);
        }
        if (livingPlayerRatio < 0.333f)
        {
            ac.SetMusicChannelVolume(2, 1.0f, 1.0f);
        }
    }

    // Logs the given kill.
    private void LogKill(Player playerKilled, Player playerKiller)
    {
        string killedName = playerKilled.GetColoredName();
        string killerName = playerKiller.GetColoredName();
        string logString = killerName
            + " has defeated " + killedName + "!";
        textLog.text = logString + "\n" + textLog.text;
    }

    // Kills the given player.
    public void Kill(Player playerKilled, Player playerKiller)
    {
        LogKill(playerKilled, playerKiller);
        RemovePlayerFromGame(playerKilled);
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
                Player target = possiblePlayers.GetRandomElement();
                Kill(target, player);
            }
        }
    }

    public bool HasGun(string playerName)
    {
        TryConvertToChatName(ref playerName);
        Player player;
        if (players.TryGetValue(playerName, out player))
        {
            return player.HasGun();
        }
        return false;
    }

    public void RemoveGun(string playerName)
    {
        TryConvertToChatName(ref playerName);
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

    [SerializeField]
    [Tooltip("The start game object to disable when the game starts.")]
    GameObject startGame;

    // The collection of players on the grid.
    Playerbase players;

    // The current state of the game.
    GameState gameState = GameState.Lobby;

    [SerializeField]
    [Tooltip("The kill log text.")]
    Text textLog;

    [SerializeField]
    [Tooltip("The possible sounds to play when firing a gun.")]
    SOAAudioClip sndGunFire;
    [SerializeField]
    [Tooltip("The music to play during gameplay.")]
    SOAMusicChannelData musicGameplay;
    [SerializeField]
    [Tooltip("The sound to play when dropping a weapon.")]
    AudioClip sndDropWeapon;
    [SerializeField]
    [Tooltip("The sound to play when breaking a block.")]
    AudioClip sndBreakBrock;

    // Reference to the audio controller.
    AudioController ac;

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
            width, height, prefabCell, true, gridContainer
            //, prefabAxisMarking: prefabAxisMarking
            );

        ac = ServiceLocator.GetAudioController();

        players = new Playerbase(textLog, elements);

        client.ClientMessageReceived += ClientMessageReceived;
    }

    public void PlayGame()
    {
        gameState = GameState.Gameplay;
        startGame.SetActive(false);
        ac.PlayMusicGroup(musicGameplay, 1.0f, 0.0f, 1.0f);
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
                    callback(player, x);
                }
            }
            else
            {
                callback(player, 1);
            }
        }
    }

    private void ClientMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        List<string> arguments = e.ChatMessage.Message.Split(' ').ToList();
        string command = arguments[0];
        if (command[0] == '!')
        {
            command = command.Remove(0, 1);
        }
        arguments.RemoveAt(0);
        string sourceName = e.ChatMessage.Username;
        EnterCommand(command, arguments, sourceName);
    }

    private void EnterCommand(string command, List<string> arguments, string sourceName)
    {
        // Convert the command and arguments to lower case.
        command = command.ToLower();
        for (int i = 0; i < arguments.Count; ++i)
        {
            arguments[i] = arguments[i].ToLower();
        }

        switch (command)
        {
            case "help":
                //client.SendWhisper(sourceName, "Hello! Welcome to Chattle Royale! To join the game, wait until the Lobby screen then type, “!join” followed by a space then your three letter nickname! To move during the game, use ! then any wasd key followed by a space and a number to move that many squares, like “!w 3”. Once you have a weapon, to eliminate another player, type their username after !, !random for a random available player, or !shoot wasd to shoot in a specific direction!");
                client.SendWhisper(sourceName, "Hello! Welcome to Chattle Royale! To join the game, wait until the Lobby screen then type, “!join” followed by a space then your three letter nickname! To move during the game, use ! then any wasd key followed by a space and a number to move that many squares, like “!w 3”. Once you have a weapon, to eliminate another player, type their username after !, or !random for a random available player!");
                break;
        }

        switch (gameState)
        {
            case GameState.Lobby:
                switch (command)
                {
                    case "join":
                        // Determine the player's in-game name.
                        // By default, use the first three letters of their Twitch name.
                        string gameName = sourceName;
                        if (arguments.Count >= 1)
                        {
                            gameName = arguments[0];
                            if (gameName.Length < 3)
                            {
                                client.SendWhisper(sourceName,
                                    "Your name must be at least 3 characters long!");
                                break;
                            }
                            if (!Regex.IsMatch(gameName, @"^[a-zA-Z]+$"))
                            {
                                client.SendWhisper(sourceName,
                                    "Your name must contain only letters!");
                                break;
                            }
                        }
                        else
                        {
                            // If the first characters of the Twitch account's name can't be
                            // used, go with a randomized name instead.
                            if (!Regex.IsMatch(gameName.Substring(0, 3), @"^[a-zA-Z]+$"))
                            {
                                gameName = UtilRandom.Spam(3);
                            }
                            /*
                            // Remove numbers from name.
                            gameName = new string(gameName.Where(c => char.IsLetter(c)).ToArray());
                            if (gameName.Length < 3)
                            {

                            }
                            */
                        }
                        gameName = gameName.Substring(0, 3).ToUpper();
                        Playerbase.JoinResult result = players.TryJoin(sourceName, gameName);
                        switch (result)
                        {
                            case Playerbase.JoinResult.Success:
                                client.SendWhisper(sourceName,
                                    "You have successfully joined the game as " +
                                    gameName + ".");
                                break;

                            case Playerbase.JoinResult.NameAlreadyTaken:
                                client.SendWhisper(sourceName,
                                    "The name " + gameName + " is already taken.");
                                break;

                            case Playerbase.JoinResult.AlreadyJoined:
                                client.SendWhisper(sourceName,
                                    "You can't join the same game twice!");
                                break;

                            case Playerbase.JoinResult.GameIsFull:
                                client.SendWhisper(sourceName,
                                    "The game is full. No more players can join.");
                                break;
                        }
                        break;

                    default:
                        TryMovePlayer(sourceName, command);
                        break;
                }
                break;

            case GameState.Gameplay:
                switch (command)
                {
                    case "join":
                        client.SendWhisper(sourceName, "You cannot join during gameplay!");
                        break;

                    case "w":
                        CommandPlayerMove(arguments, sourceName,
                            (p, x) => p.MoveVertical(x));
                        break;
                    case "a":
                        CommandPlayerMove(arguments, sourceName,
                            (p, x) => p.MoveHorizontal(-x));
                        break;
                    case "s":
                        CommandPlayerMove(arguments, sourceName,
                            (p, x) => p.MoveVertical(-x));
                        break;
                    case "d":
                        CommandPlayerMove(arguments, sourceName,
                            (p, x) => p.MoveHorizontal(x));
                        break;

                    case "random":
                        if (players.HasGun(sourceName))
                        {
                            players.KillRandom(sourceName);
                            players.RemoveGun(sourceName);
                        }
                        break;

                    case "shoot":
                        if (arguments.Count >= 1)
                        {
                            TryKillPlayer(sourceName, arguments[0]);
                        }
                        break;

                    default:
                        TryKillPlayer(sourceName, command);
                        TryMovePlayer(sourceName, command);
                        break;
                }
                break;
        }
    }

    // Tries to move the given player.
    private void TryMovePlayer(string sourceName, string movement)
    {
        Player player;
        if (players.TryGetPlayer(sourceName, out player))
        {
            for (int i = 0; i < movement.Length; ++i)
            {
                char ch = movement[i];

                // The movement command to run.
                System.Func<bool> moveCommand = () => false;

                if (ch == 'w')
                {
                    moveCommand = () => player.MoveVertical(1);
                }
                else if (ch == 'a')
                {
                    moveCommand = () => player.MoveHorizontal(-1);
                }
                else if (ch == 's')
                {
                    moveCommand = () => player.MoveVertical(-1);
                }
                else if (ch == 'd')
                {
                    moveCommand = () => player.MoveHorizontal(1);
                }
                if (!moveCommand())
                {
                    break;
                }
            }
        }
    }

    // Tries to kill the given player.
    private void TryKillPlayer(string sourceName, string targetName)
    {
        if (players.HasGun(sourceName))
        {
            Player playerKilled;
            if (players.TryGetPlayer(targetName, out playerKilled))
            {
                Player playerKiller;
                players.TryGetPlayer(sourceName, out playerKiller);
                players.Kill(playerKilled, playerKiller);
            }
            players.RemoveGun(sourceName);
        }
    }
}
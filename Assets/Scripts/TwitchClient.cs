// Author(s): Paul Calande
// A script for managing the Twitch client.
// Twitch-related scripts should subscribe to the events of this class.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Unity;
using TwitchLib.Client.Models;
using System;
using TwitchLib.Client.Events;

public class TwitchClient : MonoBehaviour
{
    public delegate void ClientConnectedHandler(object sender, OnConnectedArgs e);
    public event ClientConnectedHandler ClientConnected;

    public delegate void ClientMessageReceivedHandler(object sender, OnMessageReceivedArgs e);
    public event ClientMessageReceivedHandler ClientMessageReceived;

    public delegate void ClientCommandReceivedHandler(object sender, OnChatCommandReceivedArgs e);
    public event ClientCommandReceivedHandler ClientCommandReceived;

    [SerializeField]
    [Tooltip("The default channel name to use for the Twitch client.")]
    string defaultChannelName = "toomuchfanservice";

    Client client;
    static string channelName = "";

    private void Start()
    {
        SetChannelName(defaultChannelName);

        // Make sure the game is always running.
        Application.runInBackground = true;

        // Set up the bot.
        ConnectionCredentials credentials = new ConnectionCredentials(
            "theonetruebeetbot", Secrets.accessToken);
        client = new Client();
        client.Initialize(credentials, channelName);

        // Subscribe to the relevant events.
        client.OnConnected += Client_OnConnected;
        client.OnMessageReceived += Client_MessageReceived;
        client.OnChatCommandReceived += Client_CommandReceived;

        // Connect to the channel.
        client.Connect();
    }

    public static void SetChannelName(string channelName)
    {
        TwitchClient.channelName = channelName;
    }

    private void Client_OnConnected(object sender, OnConnectedArgs e)
    {
        if (ClientConnected != null)
        {
            ClientConnected(sender, e);
        }
    }

    // Callback for receiving a message.
    private void Client_MessageReceived(object sender, OnMessageReceivedArgs e)
    {
        if (ClientMessageReceived != null)
        {
            ClientMessageReceived(sender, e);
        }
    }

    // Callback for receiving a command.
    private void Client_CommandReceived(object sender, OnChatCommandReceivedArgs e)
    {
        if (ClientCommandReceived != null)
        {
            ClientCommandReceived(sender, e);
        }
    }

    // Send a chat message.
    public void SendChat(string message)
    {
        client.SendMessage(client.JoinedChannels[0], message);
    }

    public void SendWhisper(string receiver, string message)
    {
        client.SendWhisper(receiver, message);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class ComdakLobbyHook : LobbyHook 
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        if (lobbyPlayer == null)
            return;

        LobbyPlayer lp = lobbyPlayer.GetComponent<LobbyPlayer>();
        PlayerControlScript player = gamePlayer.GetComponent<PlayerControlScript>();

        player.playerName = lp.playerName;
        player.playerColor = lp.playerColor;
        //if(lp !=null)
        //    GameManager.
        
    }


}

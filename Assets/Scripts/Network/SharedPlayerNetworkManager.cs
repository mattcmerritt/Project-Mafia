using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/components/network-manager
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

public class SharedPlayerNetworkManager : NetworkManager
{
    [SerializeField] private GameObject playerCharacter;

    // Overrides the base singleton so we don't
    // have to cast to this type everywhere.
    public static new SharedPlayerNetworkManager singleton => (SharedPlayerNetworkManager) NetworkManager.singleton;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(playerPrefab, playerCharacter.transform);
        // instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);

        // connect controls to player manager
        PlayerControls playerControls = player.GetComponent<PlayerControls>(); 
        PlayerManager.Instance.AddPlayerControls(playerControls);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Debug.Log($"Connection {conn.connectionId} has disconnected.");
        base.OnServerDisconnect(conn);
    }
}

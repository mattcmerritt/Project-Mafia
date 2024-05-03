using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

// Automatic connector used for the showcase.
// Assumes that the scene was built with IP addresses loaded in a file called "network-setup.txt".
// This file must be located in "C:\Users\<USER>\AppData\Roaming\Project-Mafia"

// Line one of the file is S for server or C for client
// Line two is the other machine's IP address

public class ShowcaseConnector : MonoBehaviour
{
    private void Start()
    {
        string configFilepath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Project-Mafia";
        Directory.CreateDirectory(configFilepath);

        string[] fileContents = File.ReadAllLines(configFilepath + "\\network-setup.txt");

        if (fileContents[0].Trim() == "S")
        {
            SharedPlayerNetworkManager.singleton.StartHost();
        }

        else if (fileContents[0].Trim() == "C")
        {
            SharedPlayerNetworkManager.singleton.networkAddress = fileContents[1].Trim();
            SharedPlayerNetworkManager.singleton.StartClient();
        }
    }
}

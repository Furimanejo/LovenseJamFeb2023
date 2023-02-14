using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGUI : MonoBehaviour
{
    private void OnGUI()
    {
        var style = GUIStyle.none;
        style.normal.textColor = Color.white;
        style.fontSize = 20;
        int y = 0;
        if(BoardPlayer.Local != null)
        {
            GUI.Label(new Rect(10, y, 200, 50), $"Room Name: { BoardPlayer.Local.Runner.SessionInfo.Name} | Region: {BoardPlayer.Local.Runner.SessionInfo.Region}", style);
            y += 20;
            GUI.Label(new Rect(10, y, 200, 50), $"Networking Ping: {NetworkManager.ping} ms", style);
            y += 20;
        }
        GUI.Label(new Rect(10, y, 200, 50), $"Lovense: {ToyManager.lovenseStatus}", style);
        y += 20;
        if (ToyManager.toysEnabled)
        {
            GUI.Label(new Rect(10, y, 200, 50), $"Vibration Score: {(int)ToyManager.vibrationScore} %", style);
        }
    }
}
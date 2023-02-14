using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct PlayerInputData : INetworkInput
{
    public Vector2 movement;
    public Vector2 rotation;
    public  Vector3 mousePosition;
    public bool Mouse0;
    public bool Mouse1;
}

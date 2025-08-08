
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class OnTriggerManager : UdonSharpBehaviour
{
    public bool[] isFriction = new bool[(int)HumanBodyBones.LeftToes];
    void Start()
    {
        
    }
}

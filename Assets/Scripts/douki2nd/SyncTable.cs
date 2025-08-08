
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SyncTable : UdonSharpBehaviour
{
    [UdonSynced] public int[] pidStatuses = new int[11];
    /*
     * index = pid
     * pidStatus[pid] = capsuleをつけるかどうか,どのアバターを使うかどうか
     * 0:つけない,1:rurune
     */

    public bool[][] isFriction = new bool[(int)HumanBodyBones.LeftToes*10][];

    private void Start()
    {
        for(int i=0; i<isFriction.Length; i++)
        {
            isFriction[i] = new bool[i];
        }
    }

    public void ReqSerialization()
    {
        RequestSerialization();
    }

    private void Update()
    {

    }

    
}

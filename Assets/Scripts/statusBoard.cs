
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class statusBoard : UdonSharpBehaviour
{
    [UdonSynced] public int[] pidStatuses = new int[10];
    /*
     * index = pid
     * pidStatus[pid] = capsuleをつけるかどうか,どのアバターを使うかどうか
     * 0:つけない,1:rurune
     */

    private void Start()
    {
        for(int i=0; i<pidStatuses.Length; i++)
        {
            pidStatuses[i] = 0;
        }
    }

    private void Update()
    {
        RequestSerialization();
    }
}

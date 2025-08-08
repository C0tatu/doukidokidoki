
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class OnTriggerDetector : UdonSharpBehaviour
{
    //こすれてるかどうかを管理するやつと自分がどのidかをとっとくやつ
    public SyncTable stb;
    public int playerNumber = -1;
    private int boneNumber = -1;
    void Start()
    {
        
        for(int i=0; i<(int)HumanBodyBones.LeftToes; i++)
        {
            if(this.name == ((HumanBodyBones)i).ToString())
            {
                boneNumber = i;
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("111111111111111111");
        //相手も判定対象なら
        OnTriggerDetector tar = other.GetComponent<OnTriggerDetector>();
        if (tar != null && playerNumber != -1 && boneNumber != -1 && tar.playerNumber != -1 && tar.boneNumber != -1 && (stb.pidStatuses[playerNumber] != 0) && (stb.pidStatuses[tar.playerNumber] != 0))
        {
            Debug.Log("aaaaaaaaaaaaaaaaaa");
            int i, j;
            if (playerNumber < tar.playerNumber)
            {
                i = (playerNumber - 1) * (int)HumanBodyBones.LeftToes + boneNumber;
                j = (tar.playerNumber - 1) * (int)HumanBodyBones.LeftToes + tar.boneNumber;
            }
            else
            {
                j = (playerNumber - 1) * (int)HumanBodyBones.LeftToes + boneNumber;
                i = (tar.playerNumber - 1) * (int)HumanBodyBones.LeftToes + tar.boneNumber;
            }

            stb.isFriction[i][j] = true;

        }

    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerDetector tar = other.GetComponent<OnTriggerDetector>();
        if (tar != null && playerNumber != -1 && boneNumber != -1 && tar.playerNumber != -1 && tar.boneNumber != -1 && (stb.pidStatuses[playerNumber] != 0) && (stb.pidStatuses[tar.playerNumber] != 0))
        {
            int i, j;
            if (playerNumber < tar.playerNumber)
            {
                i = (playerNumber - 1) * (int)HumanBodyBones.LeftToes + boneNumber;
                j = (tar.playerNumber - 1) * (int)HumanBodyBones.LeftToes + tar.boneNumber;
            }
            else
            {
                j = (playerNumber - 1) * (int)HumanBodyBones.LeftToes + boneNumber;
                i = (tar.playerNumber - 1) * (int)HumanBodyBones.LeftToes + tar.boneNumber;
            }

            stb.isFriction[i][j] = false;
        }
    }
}

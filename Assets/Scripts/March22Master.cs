using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  This is the master script for making VN/IntNovel games.

    If you are just using M22 for narrative/dialogue,  
    do not put this script in your scene, as it 
    is responsible for initializing the game script.
*/

namespace M22
{

    public class March22Master : MonoBehaviour
    {
        public bool DebugMode = false;
        ScriptMaster SMPtr;

        // Use this for initialization
        void Start()
        {
            SMPtr = Camera.main.GetComponent<ScriptMaster>();
            SMPtr.LoadScript("START_SCRIPT");

            if(DebugMode == true)
            {
                GameObject debugPrefab = Resources.Load<GameObject>("Prefabs/DebugDisplay") as GameObject;
                if(debugPrefab != null)
                {
                    GameObject.Instantiate<GameObject>(debugPrefab, Camera.main.transform);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

    }

}
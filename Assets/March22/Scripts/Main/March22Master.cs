using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  
 *  This is the master script for making VN/IntNovel games.
 *
 *  If you are just using M22 for narrative/dialogue,  
 *  do not put this script in your scene, as it 
 *  is responsible for initializing the game script.
*/

namespace M22
{

    public class March22Master : MonoBehaviour
    {
        public GameObject DebugDisplayPrefab;
        ScriptMaster SMPtr;

        // Use this for initialization
        void Start()
        {
            SMPtr = Camera.main.GetComponent<SceneManager>().ScriptMaster;
            SMPtr.RegisterCustomFunction(new M22.CustomFunctions.DrawSprite());
            SMPtr.RegisterCustomFunction(new M22.CustomFunctions.HeartThrob());
            SMPtr.RegisterCustomFunction(new M22.CustomFunctions.SakuraEffect());
            SMPtr.RegisterCustomFunction(new M22.CustomFunctions.SnowEffect());
            SMPtr.RegisterCustomFunction(new M22.CustomFunctions.WrittenNote());
            SMPtr.RegisterCustomFunction(new M22.CustomFunctions.HospitalMask());
            M22.ScriptCompiler.InitializeCustomFunctions();
            SMPtr.LoadScript("START_SCRIPT");

            if(DebugDisplayPrefab == null)
            {
                DebugDisplayPrefab = Resources.Load<GameObject>("Prefabs/DebugDisplay") as GameObject;
                if(DebugDisplayPrefab != null)
                {
                    GameObject.Instantiate<GameObject>(DebugDisplayPrefab, Camera.main.transform);
                }
            }
            else
            {
                GameObject.Instantiate<GameObject>(DebugDisplayPrefab, Camera.main.transform);
            }
        }

        public ref ScriptMaster getScriptMaster()
        {
            return ref SMPtr;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        }

    }

}
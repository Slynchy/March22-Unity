using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace M22
{
    
    enum WAIT_STATE
    {
        NOT_WAITING,
        CHARACTER_FADEIN,
        CHARACTER_FADEOUT,
        TRANSITION
    }

    public class ScriptMaster : MonoBehaviour
    {
        [HideInInspector]
        public line_c CURRENT_LINE;

        public Image background;
        public Image backgroundTrans;

        private M22.Script.Script currentScript_c = new M22.Script.Script();
        private int lineIndex = 0;

        private VNHandler VNHandlerScript;
        private AudioMaster AudioMasterScript;

        private Image TextboxIMG;

        public TypeWriterScript TEXT;

        private WAIT_STATE WaitState = 0;

        public GameObject TransitionPrefab;
        private Dictionary<string, Sprite> TransitionEffects;

        void Awake()
        {
            M22.ScriptCompiler.Initialize();
        }

        void Start()
        {
            VNHandlerScript = this.gameObject.GetComponent<M22.VNHandler>();
            AudioMasterScript = this.gameObject.GetComponent<AudioMaster>();
            currentScript_c = M22.ScriptCompiler.CompileScript("START_SCRIPT");
            TextboxIMG = GameObject.Find("Textbox").GetComponent<Image>();
            if (TEXT == null)
            {
                Debug.Log("TEXT not found in ScriptMaster; falling back to searching...");
                TEXT = GameObject.Find("Text").GetComponent<TypeWriterScript>();
                if (TEXT == null)
                    Debug.Log("This also failed! :(");
            }
            if (background == null)
            {
                Debug.Log("background not found in ScriptMaster; falling back to searching...");
                background = GameObject.Find("Background").GetComponent<Image>();
                if (background == null)
                    Debug.Log("This also failed! :(");
            }
            if (backgroundTrans == null)
            {
                Debug.Log("backgroundTrans not found in ScriptMaster; falling back to searching...");
                backgroundTrans = GameObject.Find("BackgroundTransition").GetComponent<Image>();
                if (backgroundTrans == null)
                    Debug.Log("This also failed! :(");
            }

            if (TransitionPrefab == null)
                Debug.LogError("TransitionPrefab not attached to ScriptMaster! Check this under Main Camera!");
            TransitionEffects = new Dictionary<string, Sprite>();
            TransitionEffects.Add("tr_eyes", Resources.Load<Sprite>("Transitions/tr_eyes") as Sprite);

            CURRENT_LINE = currentScript_c.GetLine(lineIndex);
            TEXT.SetNewCurrentLine(CURRENT_LINE.m_lineContents);
            ExecuteFunction(CURRENT_LINE);
        }

        public delegate void VoidDelegate();
        public void NextLine()
        {
            ++lineIndex;
            if(VNHandlerScript.VNMode == true)
                TEXT.Reset(true);
            CURRENT_LINE = currentScript_c.GetLine(lineIndex);
            TEXT.SetNewCurrentLine("");
            ExecuteFunction(CURRENT_LINE);
        }
        public void GotoLine(int lineNum)
        {
            lineIndex = lineNum;
            if (VNHandlerScript.VNMode == true)
                TEXT.Reset(true);
            CURRENT_LINE = currentScript_c.GetLine(lineIndex);
            TEXT.SetNewCurrentLine("");
            ExecuteFunction(CURRENT_LINE);
        }

        public void ExecuteFunction(line_c _line)
        {
            switch (_line.m_lineType)
            {
                case LINETYPE.NARRATIVE:
                    TEXT.SetNewCurrentLine(CURRENT_LINE.m_lineContents);
                    VNHandlerScript.ClearCharacterName();
                    break;
                case LINETYPE.DIALOGUE:
                    TEXT.SetNewCurrentLine(CURRENT_LINE.m_lineContents);
                    VNHandlerScript.UpdateCharacterName(CURRENT_LINE.m_speaker);
                    break;
                case LINETYPE.NEW_PAGE:
                    if (VNHandlerScript == null || VNHandlerScript.VNMode == false)
                        TEXT.Reset(true, NextLine);
                    else
                        NextLine();
                    break;
                case LINETYPE.DRAW_BACKGROUND:
                    backgroundTrans.sprite = M22.BackgroundMaster.GetBackground(_line.m_parameters_txt[0]);
                    backgroundTrans.color = new Color(255, 255, 255, 0.001f);
                    break;
                case LINETYPE.GOTO:
                    bool success = false;
                    foreach (var item in M22.ScriptCompiler.currentScript_checkpoints)
                    {
                        if (item.m_name == _line.m_parameters_txt[0])
                        {
                            success = true;
                            GotoLine(item.m_position);
                            break;
                        }
                    }
                    if(!success)
                        Debug.LogError("Failed to find checkpoint: " + _line.m_parameters_txt[0]);
                    break;
                case LINETYPE.DRAW_CHARACTER:
                    VNHandlerScript.CreateCharacter(_line.m_parameters_txt[0], _line.m_parameters_txt[1], _line.m_parameters[0]);
                    //NextLine();
                    HideText();
                    WaitState = WAIT_STATE.CHARACTER_FADEIN;
                    break;
                case LINETYPE.PLAY_MUSIC:
                    M22.AudioMaster.ChangeTrack(_line.m_parameters_txt[0]);
                    NextLine();
                    break;
                case LINETYPE.STOP_MUSIC:
                    if(_line.m_parameters_txt != null)
                        AudioMasterScript.StopMusic(_line.m_parameters_txt[0]);
                    else
                        AudioMasterScript.StopMusic("1.0");
                    NextLine();
                    break;
                case LINETYPE.PLAY_STING:
                    M22.AudioMaster.PlaySting(_line.m_parameters_txt[0]);
                    NextLine();
                    break;
                case LINETYPE.HIDE_WINDOW:
                    HideText();
                    NextLine();
                    break;
                case LINETYPE.SHOW_WINDOW:
                    ShowText();
                    NextLine();
                    break;
                case LINETYPE.CLEAR_CHARACTERS:
                    VNHandlerScript.ClearCharacters();
                    HideText();
                    WaitState = WAIT_STATE.CHARACTER_FADEOUT;
                    break;
                case LINETYPE.EXECUTE_FUNCTION:
                    CustomFunctions.CustomFunctionDelegate temp = CustomFunctions.GetFunction(_line.m_parameters_txt[0]);
                    if (temp != null)
                        temp(_line.m_parameters_txt[1], _line.m_parameters_txt[2], _line.m_parameters_txt[3]);
                    else
                        Debug.LogError("Failed to find custom function: " + _line.m_parameters_txt[0]);
                    break;
                case LINETYPE.TRANSITION:
                    GameObject tempGO = GameObject.Instantiate<GameObject>(TransitionPrefab, GameObject.Find("Canvas").transform);
                    Transition TransitionObj = tempGO.GetComponent<Transition>();
                    TransitionObj.callback = FadeToBlackCallback;
                    TransitionObj.srcSprite = background.sprite;
                    TransitionEffects.TryGetValue(_line.m_parameters_txt[1], out TransitionObj.effect);
                    TransitionObj.destSprite = M22.BackgroundMaster.GetBackground(_line.m_parameters_txt[0]);
                    WaitState = WAIT_STATE.TRANSITION;
                    break;
                case LINETYPE.NUM_OF_LINETYPES:
                    // do nuzing.
                    Debug.LogError("End of script!");
                    break;
                default:
                    NextLine();
                    break;
            }
        }
        
        public void FadeToBlackCallback()
        {
            Transition temp = GameObject.FindGameObjectWithTag("Transition").GetComponent<Transition>();
            background.sprite = temp.destSprite;
            WaitState = WAIT_STATE.NOT_WAITING;
            NextLine();
        }

        public void FireInput()
        {
            if (TEXT.IsLineComplete())
            {
                NextLine();
            }
            else
            {
                TEXT.FireInput();
            }
        }

        void HideText()
        {
            for (int i = 0; i < TextboxIMG.gameObject.transform.childCount; i++)
            {
                var obj = TextboxIMG.gameObject.transform.GetChild(i).gameObject.GetComponent<Text>();
                obj.color = new Color(255, 255, 255, 0);
            }
            TextboxIMG.color = new Color(255, 255, 255, 0);
        }

        void ShowText()
        {
            for (int i = 0; i < TextboxIMG.gameObject.transform.childCount; i++)
            {
                var obj = TextboxIMG.gameObject.transform.GetChild(i).gameObject.GetComponent<Text>();
                obj.color = new Color(255, 255, 255, 255);
            }
            TextboxIMG.color = new Color(255, 255, 255, 255);
        }

        void Update()
        {
            if (WaitState == WAIT_STATE.NOT_WAITING && backgroundTrans.color.a == 0 && Input.GetKeyDown(KeyCode.Return))
            {
                FireInput();
            }

            if (backgroundTrans.color.a != 0)
            {
                backgroundTrans.color = new Color(
                    backgroundTrans.color.r,
                    backgroundTrans.color.g,
                    backgroundTrans.color.b,
                    Mathf.Lerp(backgroundTrans.color.a, 1, Time.deltaTime)
                );

                if (backgroundTrans.color.a >= 0.97f)
                {
                    backgroundTrans.color = new Color(
                        backgroundTrans.color.r,
                        backgroundTrans.color.g,
                        backgroundTrans.color.b,
                        0
                    );
                    background.sprite = backgroundTrans.sprite;
                    NextLine();
                }
            }

            if(WaitState == WAIT_STATE.CHARACTER_FADEIN)
            {
                int size = GameObject.FindGameObjectsWithTag("Character").Length;
                int count = 0;
                foreach (var item in GameObject.FindGameObjectsWithTag("Character"))
                {
                    if(item.GetComponent<FadeInImage>().complete == true)
                    {
                        count++;
                    }
                }

                if(size == count)
                {
                    WaitState = 0;
                    ShowText();
                    NextLine();
                }
            }
            else if(WaitState == WAIT_STATE.CHARACTER_FADEOUT)
            {
                int size = GameObject.FindGameObjectsWithTag("Character").Length;
                if (size == 0)
                {
                    WaitState = WAIT_STATE.NOT_WAITING;
                    ShowText();
                    NextLine();
                }
            }
        }

        // from http://stackoverflow.com/questions/9545619/a-fast-hash-function-for-string-in-c-sharp
        static UInt64 CalculateHash(string read)
        {
            UInt64 hashedValue = 3074457345618258791ul;
            for (int i = 0; i < read.Length; i++)
            {
                hashedValue += read[i];
                hashedValue *= 3074457345618258799ul;
            }
            return hashedValue;
        }

        static public M22.LINETYPE CheckLineType(string _input)
        {
            string temp = _input.TrimEnd('\r', '\n');
            LINETYPE TYPE;
            if (!M22.ScriptCompiler.FunctionHashes.TryGetValue(CalculateHash(temp), out TYPE))
            {
                // could be narrative, need to check if comment
                if (temp.Length > 1)
                {
                    if (temp[0] == '-' && temp[1] == '-')
                    {
                        return LINETYPE.CHECKPOINT;
                    }
                    else if (temp[0] == '/' && temp[1] == '/')
                    {
                        return LINETYPE.COMMENT;
                    }
                    else
                        return LINETYPE.NARRATIVE;
                }
                else
                    return LINETYPE.NARRATIVE;
            }
            else
            {
                return TYPE;
            }

        }
    }
}
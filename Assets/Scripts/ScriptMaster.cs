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
        TRANSITION,
        WAIT_COMMAND
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

        public GameObject DecisionsPrefab;

        private Canvas CANVAS;

        public List<string> SCRIPT_FLAGS;

        private float waitCommandTimer = -1.0f;

        void Awake()
        {
            SCRIPT_FLAGS = new List<string>();
            M22.ScriptCompiler.Initialize();

            if (DecisionsPrefab == null)
            {
                Debug.Log("Decisions prefab not specified under ScriptMaster! Attempting to load from Resources/Prefabs...");
                DecisionsPrefab = Resources.Load<GameObject>("Prefabs/Decisions");
            }
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

            CANVAS = Camera.main.GetComponentInChildren<Canvas>();

            if (TransitionPrefab == null)
                Debug.LogError("TransitionPrefab not attached to ScriptMaster! Check this under Main Camera!");
            TransitionEffects = new Dictionary<string, Sprite>();
            TransitionEffects.Add("tr_eyes", Resources.Load<Sprite>("Transitions/tr_eyes") as Sprite);
            TransitionEffects.Add("default", Resources.Load<Sprite>("white") as Sprite);
            TransitionEffects.Add("tr-pronoise", Resources.Load<Sprite>("Transitions/tr-pronoise") as Sprite);

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
            lineIndex = lineNum-1;
            NextLine();
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
                case LINETYPE.WAIT:
                    WaitState = WAIT_STATE.WAIT_COMMAND;
                    waitCommandTimer = 0;
                    break;
                case LINETYPE.DRAW_CHARACTER:
                    VNHandlerScript.CreateCharacter(_line.m_parameters_txt[0], _line.m_parameters_txt[1], _line.m_parameters[0]);
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
                case LINETYPE.SET_FLAG:
                    if (SCRIPT_FLAGS.Contains(_line.m_parameters_txt[0]))
                    {
                        // Flag already set, ignore
                    }
                    else
                    {
                        SCRIPT_FLAGS.Add(_line.m_parameters_txt[0]);
                    }
                    NextLine();
                    break;
                case LINETYPE.IF_STATEMENT:
                    // m22IF _flag_to_check_if_true Command [params]
                    if (SCRIPT_FLAGS.Contains(_line.m_parameters_txt[0]))
                    {
                        //Debug.Log("True!");
                        line_c tempCompiledLine = new line_c();
                        tempCompiledLine.m_lineType = _line.m_lineTypeSecondary;
                        tempCompiledLine.m_parameters_txt = new List<string>();
                        if(_line.m_parameters_txt != null && _line.m_parameters_txt.Count > 1)
                        {
                            for (int i = 1; i < _line.m_parameters_txt.Count; i++)
                            {
                                tempCompiledLine.m_parameters_txt.Add(_line.m_parameters_txt[i]);
                            }
                        }
                        else
                            tempCompiledLine.m_parameters_txt = _line.m_parameters_txt;
                        tempCompiledLine.m_parameters = _line.m_parameters;
                        tempCompiledLine.m_lineContents = _line.m_lineContents;
                        ExecuteFunction(tempCompiledLine);
                    }
                    else
                    {
                        //Debug.Log("False!");
                        NextLine();
                    }
                    break;
                case LINETYPE.MAKE_DECISION:
                    GameObject tempObj = GameObject.Instantiate<GameObject>(DecisionsPrefab, CANVAS.transform);
                    if(_line.m_parameters_txt.Count == 6)
                        tempObj.GetComponent<Decision>().Initialize(_line.m_parameters_txt[0], _line.m_parameters_txt[1], _line.m_parameters_txt[2], _line.m_parameters_txt[3], _line.m_parameters_txt[4], _line.m_parameters_txt[5]);
                    else
                        tempObj.GetComponent<Decision>().Initialize(_line.m_parameters_txt[0], _line.m_parameters_txt[1], _line.m_parameters_txt[2], _line.m_parameters_txt[3]);
                    break;
                case LINETYPE.ENABLE_NOVEL_MODE:
                    if (VNHandlerScript.VNMode == true)
                        VNHandlerScript.ToggleVNMode();
                    NextLine();
                    break;
                case LINETYPE.DISABLE_NOVEL_MODE:
                    if (VNHandlerScript.VNMode == false)
                        VNHandlerScript.ToggleVNMode();
                    NextLine();
                    break;
                case LINETYPE.CLEAR_CHARACTERS:
                    VNHandlerScript.ClearCharacters();
                    HideText();
                    WaitState = WAIT_STATE.CHARACTER_FADEOUT;
                    break;
                case LINETYPE.EXECUTE_FUNCTION:
                    CustomFunctionHandler.CustomFunctionDelegate temp = CustomFunctionHandler.GetFunction(_line.m_parameters_txt[0]);
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
                    Debug.LogError("End of script! This shouldn't happen; make sure your script ends properly!");
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
                    backgroundTrans.color.a + (Time.deltaTime * 0.5f)
                );

                if (backgroundTrans.color.a >= 1.33f)
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

            switch (WaitState)
            {
                case WAIT_STATE.CHARACTER_FADEIN:
                    int size = GameObject.FindGameObjectsWithTag("Character").Length;
                    int count = 0;
                    foreach (var item in GameObject.FindGameObjectsWithTag("Character"))
                    {
                        if (item.GetComponent<FadeInImage>().complete == true)
                        {
                            count++;
                        }
                    }

                    if (size == count)
                    {
                        WaitState = 0;
                        ShowText();
                        NextLine();
                    }
                    break;
                case WAIT_STATE.CHARACTER_FADEOUT:
                    size = GameObject.FindGameObjectsWithTag("Character").Length;
                    if (size == 0)
                    {
                        WaitState = WAIT_STATE.NOT_WAITING;
                        ShowText();
                        NextLine();
                    }
                    break;
                case WAIT_STATE.WAIT_COMMAND:
                    waitCommandTimer += Time.deltaTime;
                    if(waitCommandTimer >= (CURRENT_LINE.m_parameters[0] * 0.001f))
                    {
                        WaitState = WAIT_STATE.NOT_WAITING;
                        NextLine();
                        waitCommandTimer = -1;
                    }
                    break;
            }
            
        }

        static public M22.LINETYPE CheckLineType(string _input)
        {
            string temp = _input.TrimEnd('\r', '\n');
            LINETYPE TYPE;
            if (!M22.ScriptCompiler.FunctionHashes.TryGetValue(M22.ScriptCompiler.CalculateHash(temp), out TYPE))
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

namespace M22
{

    public class ScriptMaster : MonoBehaviour
    {
        [HideInInspector]
        public line_c CURRENT_LINE;

        public Image background;

        private M22.Script.Script currentScript_c = new M22.Script.Script();
        private int lineIndex = 0;

        private VNHandler VNHandlerScript;

        public TypeWriterScript TEXT;
        void Awake()
        {
            M22.ScriptCompiler.Initialize();
        }

        void Start()
        {
            VNHandlerScript = this.gameObject.GetComponent<M22.VNHandler>();
            currentScript_c = M22.ScriptCompiler.CompileScript("START_SCRIPT");
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

            CURRENT_LINE = currentScript_c.GetLine(lineIndex);
            TEXT.SetNewCurrentLine(CURRENT_LINE.m_lineContents);
            ExecuteFunction(CURRENT_LINE);
        }

        public delegate void VoidDelegate();
        void NextLine()
        {
            ++lineIndex;
            if(VNHandlerScript.VNMode == true)
                TEXT.Reset(true);
            CURRENT_LINE = currentScript_c.GetLine(lineIndex);
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
                    TEXT.Reset(true, NextLine);
                    break;
                case LINETYPE.DRAW_BACKGROUND:
                    background.sprite = M22.BackgroundMaster.GetBackground(_line.m_parameters_txt[0]);
                    NextLine();
                    break;
                case LINETYPE.PLAY_MUSIC:
                    M22.AudioMaster.ChangeTrack(_line.m_parameters_txt[0]);
                    NextLine();
                    break;
                case LINETYPE.PLAY_STING:
                    M22.AudioMaster.PlaySting(_line.m_parameters_txt[0]);
                    NextLine();
                    break;
                case LINETYPE.HIDE_WINDOW:
                    NextLine();
                    break;
                case LINETYPE.SHOW_WINDOW:
                    NextLine();
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

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                FireInput();
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
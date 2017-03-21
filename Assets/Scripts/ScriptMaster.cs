using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

namespace M22
{

    namespace Script
    {

        public class ScriptMaster : MonoBehaviour
        {
            [HideInInspector]
            public line_c CURRENT_LINE;

            private static readonly string[] FunctionNames = {
                "NewPage",
                "thIsIssNarraTIVE", // <- This should never be returned!
                "DrawBackground",
                "PlayMusic",
                "PlaySting",
                "--",
                "//", // Nor this, but is set up to handle it
                "SetActiveTransition"
            };

            [HideInInspector]
            public static Dictionary<UInt64, LINETYPE> FunctionHashes;

            private List<line_c> currentScript_c = new List<line_c>();
            [HideInInspector]
            public List<script_checkpoint> currentScript_checkpoints = new List<script_checkpoint>();
            private int lineIndex = 0;

            public TypeWriterScript TEXT;
            void Awake()
            {
                FunctionHashes = new Dictionary<UInt64, LINETYPE>();

                if (FunctionNames.Length != (int)LINETYPE.NUM_OF_LINETYPES)
                    Debug.LogError("Number of LINETYPE entries do not match number of FunctionNames");

                for (int i = 0; i < FunctionNames.Length; i++)
                {
                    FunctionHashes.Add(CalculateHash(FunctionNames[i]), (LINETYPE)i);
                }
            }

            void Start()
            {
                currentScript_c.Clear();
                currentScript_c = ScriptCompiler.CompileScript("START_SCRIPT", ref currentScript_checkpoints);
                if (TEXT == null)
                {
                    Debug.Log("TEXT not found in ScriptMaster; falling back to searching...");
                    TEXT = GameObject.Find("Text").GetComponent<TypeWriterScript>();
                    if (TEXT == null)
                        Debug.Log("This also failed! :(");
                }
                
                Debug.Log(currentScript_c.Count);
                CURRENT_LINE = currentScript_c[lineIndex];
                TEXT.SetNewCurrentLine(CURRENT_LINE.m_lineContents);
                ExecuteFunction(CURRENT_LINE);
            }

            public delegate void VoidDelegate();
            void NextLine()
            {
                CURRENT_LINE = currentScript_c[++lineIndex];
                ExecuteFunction(CURRENT_LINE);
            }

            public void ExecuteFunction(line_c _line)
            {
                switch(_line.m_lineType)
                {
                    case LINETYPE.NARRATIVE:
                        TEXT.SetNewCurrentLine(CURRENT_LINE.m_lineContents);
                        break;
                    case LINETYPE.NEW_PAGE:
                        TEXT.Reset(true, NextLine);
                        break;
                    case LINETYPE.PLAY_MUSIC:
                    case LINETYPE.PLAY_STING:
                    case LINETYPE.CHECKPOINT:
                    case LINETYPE.SET_ACTIVE_TRANSITION:
                    case LINETYPE.DRAW_BACKGROUND:
                        NextLine();
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
                    try
                    {
                        CURRENT_LINE = currentScript_c[++lineIndex];
                        ExecuteFunction(CURRENT_LINE);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e, this);
                    }
                }
                else
                {
                    TEXT.FireInput();
                }
            }

            void Update()
            {
                if(Input.GetKeyDown(KeyCode.Return))
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

            static public LINETYPE CheckLineType(string _input)
            {
                string temp = _input.TrimEnd('\r', '\n');
                LINETYPE TYPE;
                if (!FunctionHashes.TryGetValue(CalculateHash(temp), out TYPE))
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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

namespace M22
{

    namespace Script
    {

        public enum LINETYPE
        {
            NEW_PAGE,
            NARRATIVE
        }

        public struct line_c
        {
            public M22.Script.LINETYPE m_lineType;
            public List<int> m_parameters;
            public List<string> m_parameters_txt;
            public string m_lineContents;
            public int m_speaker; // deprecated?
            public int m_ID;
        }

        public struct script_checkpoint
        {
            int m_position;
            string m_name;
        }

        public class ScriptMaster : MonoBehaviour
        {
            [HideInInspector]
            public line_c CURRENT_LINE;

            private List<line_c> currentScript_c = new List<line_c>();
            private List<script_checkpoint> currentScript_checkpoints = new List<script_checkpoint>();
            private int lineIndex = 0;


            public TypeWriterScript TEXT;

            private static readonly string[] FunctionNames = { "thIsIssNarraTIVE", "NewPage" };
            public static Dictionary<UInt64, LINETYPE> FunctionHashes;
            void Awake()
            {
                for (int i = 0; i < FunctionNames.Length; i++)
                {
                    FunctionHashes.Add(CalculateHash(FunctionNames[i]), (LINETYPE)i);
                }
            }
            
            void DebugScript()
            {
                line_c temp;
                for (int i = 0; i < 5; i++)
                {
                    temp = new Script.line_c();
                    temp.m_lineType = LINETYPE.NARRATIVE;
                    temp.m_lineContents = "Well...";
                    currentScript_c.Add(temp);
                }
                temp = new Script.line_c();
                temp.m_lineType = LINETYPE.NEW_PAGE;
                temp.m_lineContents = "";
                currentScript_c.Add(temp);
                for (int i = 0; i < 5; i++)
                {
                    temp = new Script.line_c();
                    temp.m_lineType = LINETYPE.NARRATIVE;
                    temp.m_lineContents = "Well...";
                    currentScript_c.Add(temp);
                }
            }

            void Start()
            {
                var test = ScriptCompiler.CompileScript("START_SCRIPT");
                if (TEXT == null)
                {
                    Debug.Log("TEXT not found in ScriptMaster; falling back to searching...");
                    TEXT = GameObject.Find("Text").GetComponent<TypeWriterScript>();
                    if (TEXT == null)
                        Debug.Log("This also failed! :(");
                }

                // ---------==============
                DebugScript();
                // ---------==============

                CURRENT_LINE = currentScript_c[lineIndex];
                TEXT.SetNewCurrentLine(CURRENT_LINE.m_lineContents);
            }

            public void ExecuteFunction(line_c _line)
            {
                switch(_line.m_lineType)
                {
                    case LINETYPE.NARRATIVE:
                        TEXT.SetNewCurrentLine(CURRENT_LINE.m_lineContents);
                        break;
                    case LINETYPE.NEW_PAGE:
                        TEXT.Reset(true);
                        CURRENT_LINE = currentScript_c[++lineIndex];
                        ExecuteFunction(CURRENT_LINE);
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
                Regex.Replace(_input, @"\s+", "");
                LINETYPE TYPE;
                if (!FunctionHashes.TryGetValue(CalculateHash(_input), out TYPE))
                {
                    // narrative
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace M22
{

    public enum LINETYPE
    {
        NEW_PAGE,
        NARRATIVE,
        DRAW_BACKGROUND,
        PLAY_MUSIC,
        PLAY_STING,
        CHECKPOINT,
        COMMENT,
        SET_ACTIVE_TRANSITION,
        NUM_OF_LINETYPES
    }

    public struct line_c
    {
        public M22.LINETYPE m_lineType;
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
        public script_checkpoint(int _a, string _b)
        {
            m_position = _a;
            m_name = _b;
        }
    }

    public class ScriptCompiler
    {
        public static Dictionary<ulong, M22.LINETYPE> FunctionHashes;
        public static List<M22.script_checkpoint> currentScript_checkpoints = new List<M22.script_checkpoint>();
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

        public static void Initialize()
        {
            FunctionHashes = new Dictionary<ulong, M22.LINETYPE>();

            if (FunctionNames.Length != (int)M22.LINETYPE.NUM_OF_LINETYPES)
                Console.WriteLine("Number of LINETYPE entries do not match number of FunctionNames");

            for (int i = 0; i < FunctionNames.Length; i++)
            {
                FunctionHashes.Add(CalculateHash(FunctionNames[i]), (M22.LINETYPE)i);
            }
        }

        private static void ThisIsABreakpoint()
        {
            bool lol;
            return;
        }

        private static bool IsNewLine(string s)
        {
            if (s == "\r\n" || s == "\n") return true;
            else return false;
        }
        private static bool IsComment(string s)
        {
            if (s.Length == 0) return true;
            if (s.Length == 1) return false;

            if (s[0] == '/' && s[1] == '/')
                return true;
            else
                return false;
        }

        static private int SplitString(string txt, List<string> result, char ch)
        {
            result.Clear();
            string[] temp = txt.Split(ch);
            for (int i = 0; i < temp.Length; i++)
            {
                result.Add(temp[i]);
            }
            return result.Count;
        }

        static public List<M22.line_c> CompileScript(string filename)
        {
            var result = new List<M22.line_c>();

            //var file = File.ReadAllText(filename, Encoding.UTF8);
            string file = M22.UnityWrapper.LoadTextFileToString(filename);

            if (file.Length == 0)
            {
                Console.WriteLine("Failed to load script file!");
                return result;
            }
            var scriptLines = new List<string>();
            string currentLine = "";
            for (int i = 0; i < file.Length; i++)
            {
                currentLine += file[i];
                if (file[i] == '\n')
                {
                    // new line
                    scriptLines.Add(currentLine);
                    currentLine = "";
                }
            }

            scriptLines.RemoveAll(IsNewLine);
            scriptLines.RemoveAll(IsComment);

            List<string> CURRENT_LINE_SPLIT = new List<string>();
            int scriptPos = 0;
            for (int i = 0; i < scriptLines.Count; i++)
            {
                SplitString(scriptLines[i], CURRENT_LINE_SPLIT, ' ');
                if (CURRENT_LINE_SPLIT.Count == 0) continue;
                M22.line_c tempLine_c = new M22.line_c();
                tempLine_c.m_lineType = CheckLineType(CURRENT_LINE_SPLIT[0]);

                if (tempLine_c.m_lineType == M22.LINETYPE.NARRATIVE)
                {
                    tempLine_c.m_lineContents = scriptLines[i];
                }
                else
                {
                    CompileLine(ref tempLine_c, CURRENT_LINE_SPLIT, ref currentScript_checkpoints, scriptPos);
                }

                result.Add(tempLine_c);
                scriptPos++;
            }

            return result;
        }

        // from http://stackoverflow.com/questions/9545619/a-fast-hash-function-for-string-in-c-sharp
        static ulong CalculateHash(string read)
        {
            ulong hashedValue = 3074457345618258791ul;
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
            temp = temp.TrimEnd('\n');
            M22.LINETYPE TYPE;
            if (!FunctionHashes.TryGetValue(CalculateHash(temp), out TYPE))
            {
                // could be narrative, need to check if comment
                if (temp.Length > 1)
                {
                    if (temp[0] == '-' && temp[1] == '-')
                    {
                        return M22.LINETYPE.CHECKPOINT;
                    }
                    else if (temp[0] == '/' && temp[1] == '/')
                    {
                        return M22.LINETYPE.COMMENT;
                    }
                    else
                        return M22.LINETYPE.NARRATIVE;
                }
                else
                    return M22.LINETYPE.NARRATIVE;
            }
            else
            {
                return TYPE;
            }

        }

        static void CompileLine(ref M22.line_c _lineC, List<string> _splitStr, ref List<M22.script_checkpoint> _chkpnt, int _scriptPos)
        {
            switch (_lineC.m_lineType)
            {
                case M22.LINETYPE.CHECKPOINT:
                    _chkpnt.Add(new M22.script_checkpoint(_scriptPos, _splitStr[0]));
                    break;
                case M22.LINETYPE.SET_ACTIVE_TRANSITION:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        _splitStr[1] = _splitStr[1].TrimEnd('\r', '\n');
                        _lineC.m_parameters_txt.Add(_splitStr[1]);
                    }
                    break;
                case M22.LINETYPE.NEW_PAGE:
                    break;
                case M22.LINETYPE.PLAY_MUSIC:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        _splitStr[1] = _splitStr[1].Substring(0, _splitStr[1].Length - 2);
                        _lineC.m_parameters_txt.Add(_splitStr[1]);

                        if (!M22.AudioMaster.LoadAudio(_lineC.m_parameters_txt[0]))
                        {
                            Console.WriteLine("Failed to load music! - " + _lineC.m_parameters_txt[0]);
                        };
                    }
                    break;
                case M22.LINETYPE.DRAW_BACKGROUND:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        _splitStr[1] = _splitStr[1].Substring(0, _splitStr[1].Length - 2);
                        _lineC.m_parameters_txt.Add(_splitStr[1]);

                        if (!M22.BackgroundMaster.LoadBackground(_lineC.m_parameters_txt[0]))
                        {
                            Console.WriteLine("Failed to load background! - " + _lineC.m_parameters_txt[0]);
                        };
                    }
                    break;
            }
            return;
        }
    }

}
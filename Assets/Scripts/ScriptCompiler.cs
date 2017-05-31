using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace M22
{

    public enum LINETYPE
    {
        NEW_PAGE,
        NARRATIVE,
        DRAW_BACKGROUND,
        PLAY_MUSIC,
        STOP_MUSIC,
        PLAY_STING,
        CHECKPOINT,
        COMMENT,
        SET_ACTIVE_TRANSITION,
        HIDE_WINDOW,
        SHOW_WINDOW,
        DIALOGUE,
        DRAW_CHARACTER,
        TRANSITION,
        CLEAR_CHARACTERS,
        EXECUTE_FUNCTION,
        GOTO,
        WAIT,
        ENABLE_NOVEL_MODE,
        DISABLE_NOVEL_MODE,
        MAKE_DECISION,
        IF_STATEMENT,
        SET_FLAG,
        LOAD_SCRIPT,
        NUM_OF_LINETYPES
    }

    public struct line_c
    {
        public M22.LINETYPE m_lineType;
        public M22.LINETYPE m_lineTypeSecondary; // used for IF statements
        public List<int> m_parameters;
        public List<string> m_parameters_txt;
        public string m_lineContents;
        public script_character m_speaker; 
        public int m_ID;
    }

    public struct script_checkpoint
    {
        public int m_position;
        public string m_name;
        public script_checkpoint(int _a, string _b)
        {
            m_position = _a;
            m_name = _b;
        }
    }
    public struct script_character
    {
        public string name;
        public Color color;
    }

    public class ScriptCompiler
    {
        public static Dictionary<ulong, script_character> CharacterNames;
        public static Dictionary<ulong, M22.LINETYPE> FunctionHashes;
        public static List<M22.script_checkpoint> currentScript_checkpoints = new List<M22.script_checkpoint>();
        public static readonly string[] FunctionNames = {
                "NewPage",
                "thIsIssNarraTIVE", // <- This should never be returned!
                "DrawBackground",
                "PlayMusic",
                "StopMusic",
                "PlaySting",
                "--",
                "//", // Nor this, but is set up to handle it
                "SetActiveTransition",
                "HideWindow",
                "ShowWindow",
                "DiAlOGUeHeRe", // NOR THIS
                "DrawCharacter",
                "Transition",
                "ClearCharacters",
                "ExecuteFunction",
                "Goto",
                "Wait",
                "EnableNovelMode",
                "DisableNovelMode",
                "MakeDecision",
                "m22IF",
                "SetFlag",
                "LoadScript"
        };

        private static void InitializeCharNames()
        {
            CharacterNames = new Dictionary<ulong, script_character>();
            string tempStr = (Resources.Load("CHARACTER_NAMES") as TextAsset).text;

            string[] lines = tempStr.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                string[] lineSplit = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                string shortName = lineSplit[0];
                string longName = Regex.Match(line, "\"([^\"]*)\"").ToString();

                script_character temp = new script_character();
                temp.name = longName.Substring(1,longName.Length-2);
                temp.color = new Color(Int32.Parse(lineSplit[lineSplit.Length-3]), Int32.Parse(lineSplit[lineSplit.Length - 2]), Int32.Parse(lineSplit[lineSplit.Length - 1]));

                CharacterNames.Add(CalculateHash(shortName), temp);
            }
        }

        public static void Initialize()
        {
            FunctionHashes = new Dictionary<ulong, M22.LINETYPE>();

            InitializeCharNames();

            if (FunctionNames.Length != (int)M22.LINETYPE.NUM_OF_LINETYPES)
                Debug.LogError("Number of LINETYPE entries do not match number of FunctionNames");

            for (int i = 0; i < FunctionNames.Length; i++)
            {
                FunctionHashes.Add(CalculateHash(FunctionNames[i]), (M22.LINETYPE)i);
            }
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

        static private int SplitString(ref string txt, ref List<string> result, char ch)
        {
            result.Clear();
            string[] temp = txt.Split(ch);
            for (int i = 0; i < temp.Length; i++)
            {
                result.Add(temp[i]);
            }
            return result.Count;
        }

        static public M22.Script.Script CompileScript(string filename)
        {
            var result = new M22.Script.Script();

            //var file = File.ReadAllText(filename, Encoding.UTF8);
            var txtAsset = Resources.Load(filename) as TextAsset;
            string file;
            if (txtAsset != null)
            {
                file = txtAsset.text;
            }
            else
            {
                file = "";
            }

            if (file.Length == 0)
            {
                Debug.LogError("Failed to load script file: " + filename);
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
                    scriptLines[i] = scriptLines[i].Replace("\\n", "\n");
                    tempLine_c.m_lineContents = scriptLines[i];
                }
                else if(tempLine_c.m_lineType == M22.LINETYPE.DIALOGUE)
                {
                    tempLine_c.m_lineContents = scriptLines[i];
                    tempLine_c.m_lineContents = tempLine_c.m_lineContents.Substring(CURRENT_LINE_SPLIT[0].Length+1);
                    CharacterNames.TryGetValue(CalculateHash(CURRENT_LINE_SPLIT[0]), out tempLine_c.m_speaker);
                }
                else
                {
                    CompileLine(ref tempLine_c, CURRENT_LINE_SPLIT, ref currentScript_checkpoints, scriptPos);
                }

                result.AddLine(tempLine_c);
                scriptPos++;
            }

            return result;
        }

        // from http://stackoverflow.com/questions/9545619/a-fast-hash-function-for-string-in-c-sharp
        static public ulong CalculateHash(string read)
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
                    {
                        if (!CharacterNames.ContainsKey(CalculateHash(temp)))
                            return M22.LINETYPE.NARRATIVE;
                        else
                            return M22.LINETYPE.DIALOGUE;
                    }
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
            for (int i = 0; i < _splitStr.Count; i++)
            {
                _splitStr[i] = _splitStr[i].TrimEnd('\r', '\n');
            }
            switch (_lineC.m_lineType)
            {
                case M22.LINETYPE.CHECKPOINT:
                    _splitStr[0] = _splitStr[0].Substring(2);
                    _splitStr[0] = _splitStr[0].TrimEnd('\r', '\n');
                    _chkpnt.Add(new M22.script_checkpoint(_scriptPos, _splitStr[0]));
                    break;
                case M22.LINETYPE.TRANSITION:
                    if (_splitStr.Count > 1)
                    {   //Transition other_iwanako tr_eyes 0
                        _lineC.m_parameters_txt = new List<string>();
                        _lineC.m_parameters_txt.Add(_splitStr[1]);
                        _lineC.m_parameters_txt.Add(_splitStr[2]);
                        _lineC.m_parameters_txt.Add(_splitStr[3]);

                        if (!M22.BackgroundMaster.LoadBackground(_lineC.m_parameters_txt[0]))
                        {
                            Debug.LogError("Failed to load background! - " + _lineC.m_parameters_txt[0]);
                        };
                    }
                    break;
                case M22.LINETYPE.SET_ACTIVE_TRANSITION:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        _splitStr[1] = _splitStr[1].TrimEnd('\r', '\n');
                        _lineC.m_parameters_txt.Add(_splitStr[1]);
                    }
                    break;
                case M22.LINETYPE.GOTO:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        _splitStr[1] = _splitStr[1].TrimEnd('\r', '\n');
                        _lineC.m_parameters_txt.Add(_splitStr[1]);
                    }
                    break;
                case M22.LINETYPE.NEW_PAGE:
                    break;
                case M22.LINETYPE.DRAW_CHARACTER:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        _lineC.m_parameters_txt.Add(_splitStr[1]);
                        _lineC.m_parameters_txt.Add(_splitStr[2]);
                        _lineC.m_parameters = new List<int>();
                        _lineC.m_parameters.Add(Int32.Parse(_splitStr[3]));

                        if (!M22.VNHandler.LoadCharacter(_lineC.m_parameters_txt[0], _lineC.m_parameters_txt[1]))
                        {
                            Debug.LogError("Failed to load character! - " + _lineC.m_parameters_txt[0] + " - " + _lineC.m_parameters_txt[1]);
                        };
                    }
                    break;
                case M22.LINETYPE.WAIT:
                    _lineC.m_parameters = new List<int>();
                    if (_splitStr.Count > 1)
                        _lineC.m_parameters.Add(Int32.Parse(_splitStr[1]));
                    else
                        _lineC.m_parameters.Add(1000);
                    break;
                case M22.LINETYPE.PLAY_STING:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        _splitStr[1] = _splitStr[1].TrimEnd('\r', '\n');
                        _lineC.m_parameters_txt.Add(_splitStr[1]);

                        if (!M22.AudioMaster.LoadSting(_lineC.m_parameters_txt[0]))
                        {
                            Debug.LogError("Failed to load sting! - " + _lineC.m_parameters_txt[0]);
                        };
                    }
                    break;
                case M22.LINETYPE.PLAY_MUSIC:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        _splitStr[1] = _splitStr[1].TrimEnd('\r', '\n');
                        _lineC.m_parameters_txt.Add(_splitStr[1]);

                        if (!M22.AudioMaster.LoadMusic(_lineC.m_parameters_txt[0]))
                        {
                            Debug.LogError("Failed to load music! - " + _lineC.m_parameters_txt[0]);
                        };
                    }
                    break;
                case M22.LINETYPE.EXECUTE_FUNCTION:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        for (int i = 1; i < _splitStr.Count-1; i++)
                        {
                            _lineC.m_parameters_txt.Add(_splitStr[i]);
                        }
                        _splitStr[_splitStr.Count - 1] = _splitStr[_splitStr.Count - 1].TrimEnd('\r', '\n');
                        _lineC.m_parameters_txt.Add(_splitStr[_splitStr.Count - 1]);

                        // should be 4
                        while(_lineC.m_parameters_txt.Count < 4)
                            _lineC.m_parameters_txt.Add("");
                    }
                    break;
                case M22.LINETYPE.STOP_MUSIC:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        _splitStr[1] = _splitStr[1].TrimEnd('\r', '\n');
                        _lineC.m_parameters_txt.Add(_splitStr[1]);
                    }
                    // we store the float value as a string for later use, if provided.
                    // otherwise, just continue
                    break;
                case M22.LINETYPE.DRAW_BACKGROUND:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        _splitStr[1] = _splitStr[1].TrimEnd('\r', '\n');
                        _lineC.m_parameters_txt.Add(_splitStr[1]);

                        if (!M22.BackgroundMaster.LoadBackground(_lineC.m_parameters_txt[0]))
                        {
                            Debug.LogError("Failed to load background! - " + _lineC.m_parameters_txt[0]);
                        };
                    }
                    break;
                case M22.LINETYPE.ENABLE_NOVEL_MODE:
                    break;
                case M22.LINETYPE.DISABLE_NOVEL_MODE:
                    break;
                case M22.LINETYPE.LOAD_SCRIPT:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        _lineC.m_parameters_txt.Add(_splitStr[1]);
                    }
                    break;
                case M22.LINETYPE.SET_FLAG:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        _splitStr[1] = _splitStr[1].TrimEnd('\r', '\n');
                        _lineC.m_parameters_txt.Add(_splitStr[1]);
                    }
                    break;
                case M22.LINETYPE.IF_STATEMENT:
                    // m22IF _flag_to_check_if_true Command [params]
                    // 
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        _lineC.m_parameters_txt.Add(_splitStr[1]);

                        line_c tempCompiledLine = new line_c();
                        List<string> functionSplit = new List<string>();
                        for (int i = 2; i < _splitStr.Count; i++)
                        {
                            functionSplit.Add(_splitStr[i]);
                        }
                        tempCompiledLine.m_lineType = CheckLineType(_splitStr[2]);
                        CompileLine(ref tempCompiledLine, functionSplit, ref _chkpnt, _scriptPos);
                        if(tempCompiledLine.m_lineContents == null)
                        {
                            tempCompiledLine.m_lineContents = "";
                            for (int i = 2; i < _splitStr.Count; i++)
                            {
                                tempCompiledLine.m_lineContents += _splitStr[i] + " ";
                            }
                        }
                        tempCompiledLine.m_lineContents = tempCompiledLine.m_lineContents.Replace("\\n", "\n");
                        _lineC.m_lineContents = tempCompiledLine.m_lineContents;
                        _lineC.m_lineTypeSecondary = tempCompiledLine.m_lineType;

                        if(tempCompiledLine.m_parameters_txt != null)
                        {
                            for (int i = 0; i < tempCompiledLine.m_parameters_txt.Count; i++)
                            {
                                _lineC.m_parameters_txt.Add(tempCompiledLine.m_parameters_txt[i]);
                            }
                        }

                        if(tempCompiledLine.m_parameters != null)
                        {
                            _lineC.m_parameters = new List<int>();
                            for (int i = 0; i < tempCompiledLine.m_parameters.Count; i++)
                            {
                                _lineC.m_parameters.Add(tempCompiledLine.m_parameters[i]);
                            }
                        }
                    }
                    break;
                case M22.LINETYPE.MAKE_DECISION:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        string reconstructed = "";
                        for (int i = 0; i < _splitStr.Count; i++)
                        {
                            reconstructed += _splitStr[i] + " ";
                        }
                        List<string> splitByQuote = new List<string>();
                        SplitString(ref reconstructed, ref splitByQuote, '\"');

                        // Should be 5 or 7
                        if(splitByQuote.Count != 5 && splitByQuote.Count != 7)
                        {
                            Debug.LogError("MakeDecision error; mismatched number of quotemarks!");
                        }

                        for (int i = 1; i < splitByQuote.Count; i++)
                        {
                            //splitByQuote[i] = _splitStr[i].TrimEnd(' ');
                            //splitByQuote[i] = _splitStr[i].TrimStart(' ');
                            splitByQuote[i] = splitByQuote[i].Trim(' ', '\"');
                            _lineC.m_parameters_txt.Add(splitByQuote[i]);
                        }

                        // up to 6 parameters
                        // flags do not use "" but the text string does
                        // i.e. MakeDecision "Choice 1" choice_1 "Choice 2" choice_2 "Choice 3" choice_3
                        // if(num of quotemarks != 6) mismatch error
                        // 
                        // This means splitStr is useless cus of spaces, and will need to be re-split in terms of " marks
                        // i.e. splitStr[0] == "MakeDecision ";
                        // splitStr[1] == "Choice 1";
                        // splitStr[2] == " choice_1 ";
                    }
                    break;
            }
            return;
        }
    }

}
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace M22
{

    public enum LINETYPE
    {
        NULL_OPERATOR,
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
        CLEAR_CHARACTER,
        EXECUTE_FUNCTION,
        GOTO,
        WAIT,
        ENABLE_NOVEL_MODE,
        DISABLE_NOVEL_MODE,
        MAKE_DECISION,
        IF_STATEMENT,
        SET_FLAG,
        LOAD_SCRIPT,
        PLAY_VIDEO,
        MOVEMENT_SPEED,
        TEXT_SPEED,
        ANIMATION_TYPE,
        PLAY_SFX_LOOPED,
        STOP_SFX_LOOPED,
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
        public int m_origScriptPos; // used to tell where it is in the original script
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
        public UnityWrapper.Color32 color;
    }

    public class ScriptCompiler
    {
        public static Dictionary<ulong, script_character> CharacterNames;
        public static Dictionary<ulong, M22.LINETYPE> FunctionHashes;
        public static List<M22.script_checkpoint> currentScript_checkpoints = new List<M22.script_checkpoint>();
        public static readonly string[] FunctionNames = {
                "nopnopnop",
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
                "ClearCharacter",
                "ExecuteFunction",
                "Goto",
                "Wait",
                "EnableNovelMode",
                "DisableNovelMode",
                "MakeDecision",
                "m22IF",
                "SetFlag",
                "LoadScript",
                "PlayVideo",
                "SetMovementSpeed",
                "SetTextSpeed",
                "SetAnimationType",
                "PlayLoopedSting",
                "StopLoopedSting"
        };
        public enum ANIMATION_TYPES
        {
            SMOOTH,
            LERP,
            NUM_OF_ANIMATION_TYPES
        }

        private static void InitializeCharNames()
        {
            CharacterNames = new Dictionary<ulong, script_character>();
            string tempStr = UnityWrapper.LoadTextFileAsString("March22/CHARACTER_NAMES");
            tempStr += "\n\n"; // <- hack to fix last line being cut off

            string[] lines = tempStr.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                string[] lineSplit = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                string shortName = lineSplit[0];
                string longName = Regex.Match(line, "\"([^\"]*)\"").ToString();

                script_character temp = new script_character();
                temp.name = longName.Substring(1,longName.Length-2);
                temp.color = new UnityWrapper.Color32(
                    (byte)Int32.Parse(lineSplit[lineSplit.Length-3]),
                    (byte)Int32.Parse(lineSplit[lineSplit.Length - 2]),
                    (byte)Int32.Parse(lineSplit[lineSplit.Length - 1]),
                    (byte)255
                );

                CharacterNames.Add(CalculateHash(shortName), temp);
            }
        }

        public static void Initialize()
        {
            FunctionHashes = new Dictionary<ulong, M22.LINETYPE>();

            InitializeCharNames();

            if (FunctionNames.Length != (int)M22.LINETYPE.NUM_OF_LINETYPES)
                UnityWrapper.LogError("Number of LINETYPE entries do not match number of FunctionNames");

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
            s = s.Trim('\t');

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

        static private int SplitString(ref string txt, ref List<string> result, string ch)
        {
            result.Clear();
            string[] temp = txt.Split(new string[] { ch }, StringSplitOptions.None);
            for (int i = 0; i < temp.Length; i++)
            {
                result.Add(temp[i]);
            }
            return result.Count;
        }

        public struct LoadedVariables
        {
            public string[] variables;
            public string[] variableData;
        }

        static public LoadedVariables LoadVariablesFile()
        {
            var result = new LoadedVariables();
            var file = UnityWrapper.LoadTextFileAsString("March22/VARIABLES");
            if (file == null || file == "")
            {
                UnityWrapper.LogError("Failed to load \"Resources/March22/VARIABLES.txt\"! You should have one even if you aren't using it!");
                return result;
            }

            if (file[file.Length - 1] != '\n')
                file += '\n';
            var varLines = SplitByChar('\n', ref file);
            file = "";
            result.variables = new string[varLines.Count];
            result.variableData = new string[varLines.Count];

            for (int i = 0; i < varLines.Count; i++)
            {
                var currentLine = varLines[i];
                currentLine = currentLine.Trim(' ', '\n', '\r');
                if (currentLine.Length < 4)
                    continue;

                List<string> splitStr = new List<string>();
                SplitString(ref currentLine, ref splitStr, "===");
                for (int n = 0; n < splitStr.Count; n++)
                {
                    splitStr[n] = splitStr[n].Trim(' ', '\n' , '\r');
                }
                if (splitStr.Count < 2)
                    continue;

                result.variables[i] = splitStr[0];
                result.variableData[i] = splitStr[1];
            }

            return result;
        }

        static public List<string> SplitByChar(char _char, ref string _input)
        {
            var result = new List<string>();
            string currentLine = "";
            for (int i = 0; i < _input.Length; i++)
            {
                currentLine += _input[i];
                if (_input[i] == _char)
                {
                    // new line
                    result.Add(currentLine);
                    currentLine = "";
                }
            }
            return result;
        }

        public struct ScriptCompileProgress
        {
            float progress;
        }

        static public void CompileScriptAsync(string filename, ref M22.Script.Script _out)
        {
            M22.Script.Script output = new M22.Script.Script();
            _out = output;
            var aThread = new System.Threading.Thread(() => CompileScript(filename, ref output));
            aThread.Start();
            return;
        }

        static void Run(){ }

        static public void CompileScript(string filename, ref M22.Script.Script _out)
        {
            _out = CompileScript(filename);
        }

        static public M22.Script.Script CompileScript(string filename)
        {
            var result = new M22.Script.Script();
            
            LoadedVariables loadedVars = LoadVariablesFile();

            //var file = File.ReadAllText(filename, Encoding.UTF8);
            var file = UnityWrapper.LoadTextFileAsString(filename);

            if (file.Length == 0)
            {
                UnityWrapper.LogError("Failed to load script file: " + filename);
                return result;
            }

            var scriptLines = SplitByChar('\n', ref file);
            file = "";

            //List<string> CURRENT_LINE_SPLIT = new List<string>();
            int scriptPos = 0;
            for (int i = 0; i < scriptLines.Count; i++)
            {
                string currScriptLine = scriptLines[i];

                for (int n = 0; n < loadedVars.variables.Length; n++)
                {
                    if (loadedVars.variables[n] != null)
                    {
                        string tempStr = "[[" + loadedVars.variables[n] + "]]";
                        currScriptLine = currScriptLine.Replace(tempStr, loadedVars.variableData[n]);
                    }
                }

                M22.line_c tempLine_c = CompileLine(ref currScriptLine, i); // ref can't be an index so have to copy then copy back
                scriptLines[i] = currScriptLine;
                //if(tempLine_c.m_lineType == LINETYPE.NULL_OPERATOR)
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
            string temp = _input.Trim('\r', '\n', '\t');
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
                        {
                            //Debug.Log("rbeakboot!");
                            return M22.LINETYPE.DIALOGUE;
                        }
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

        static public M22.line_c CompileLine(ref string func, int _scriptPos)
        {
            List<string> CURRENT_LINE_SPLIT = new List<string>();
            M22.line_c tempLine_c = new M22.line_c();
            func = func.Trim('\t');
            if (func == "\r\n" || func == "\n")
            {
                return tempLine_c;
            }
            else if (func.Length == 0 || (func[0] == '/' && func[1] == '/'))
            {
                return tempLine_c;
            }
            SplitString(func, CURRENT_LINE_SPLIT, ' ');
            if (CURRENT_LINE_SPLIT.Count == 0) return tempLine_c;
            tempLine_c.m_origScriptPos = _scriptPos + 1;
            tempLine_c.m_lineType = CheckLineType(CURRENT_LINE_SPLIT[0]);

            if (tempLine_c.m_lineType == M22.LINETYPE.NARRATIVE)
            {
                func = func.Replace("\\n", "\n");
                tempLine_c.m_lineContents = func;
            }
            else if (tempLine_c.m_lineType == M22.LINETYPE.DIALOGUE)
            {
                tempLine_c.m_lineContents = func;
                tempLine_c.m_lineContents = tempLine_c.m_lineContents.Substring(CURRENT_LINE_SPLIT[0].Length + 1);
                CharacterNames.TryGetValue(CalculateHash(CURRENT_LINE_SPLIT[0]), out tempLine_c.m_speaker);
            }
            else
            {
                CompileLine(ref tempLine_c, CURRENT_LINE_SPLIT, ref currentScript_checkpoints, _scriptPos);
            }
            return tempLine_c;
        }
        
        static public void UnloadCheckpoints()
        {
            currentScript_checkpoints.Clear();
        }

        static public void CompileLine(ref M22.line_c _lineC, List<string> _splitStr, ref List<M22.script_checkpoint> _chkpnt, int _scriptPos)
        {
            for (int i = 0; i < _splitStr.Count; i++)
            {
                _splitStr[i] = _splitStr[i].Trim('\r', '\n', '\t');
            }
            switch (_lineC.m_lineType)
            {
                case M22.LINETYPE.CHECKPOINT:
                    _splitStr[0] = _splitStr[0].Substring(2);
                    _splitStr[0] = _splitStr[0].TrimEnd('\r', '\n');
                    _chkpnt.Add(new M22.script_checkpoint(_scriptPos, _splitStr[0]));
                    break;
                case M22.LINETYPE.ANIMATION_TYPE:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters = new List<int>();
                        _splitStr[1] = _splitStr[1].ToLower();
                        if (_splitStr[1].Equals("lerp"))
                        {
                            _lineC.m_parameters.Add((int)ANIMATION_TYPES.LERP);
                        }
                        else if (_splitStr[1].Equals("smooth"))
                        {
                            _lineC.m_parameters.Add((int)ANIMATION_TYPES.SMOOTH);
                        }
                        else
                        {
                            UnityWrapper.LogErrorFormat("Invalid animation type at line {0}!", _lineC.m_origScriptPos);
                            _lineC.m_parameters.Add(0);
                        }
                    }
                    break;
                case M22.LINETYPE.TEXT_SPEED:
                case M22.LINETYPE.MOVEMENT_SPEED:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        _lineC.m_parameters_txt.Add(_splitStr[1]);
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
                        if (_splitStr.Count < 4) UnityWrapper.LogError("Not enough parameters for DrawCharacter @ Line " + _lineC.m_origScriptPos.ToString());
                        _lineC.m_parameters_txt = new List<string>();
                        _lineC.m_parameters_txt.Add(_splitStr[1]);
                        _lineC.m_parameters_txt.Add(_splitStr[2]);
                        _lineC.m_parameters = new List<int>();
                        _lineC.m_parameters.Add(Int32.Parse(_splitStr[3]));
                        if(_splitStr.Count >= 5)
                        {
                            if(_splitStr[4].Equals("true"))
                                _lineC.m_parameters.Add(1);
                            else
                                _lineC.m_parameters.Add(0);
                        }
                        else
                            _lineC.m_parameters.Add(0);

                        if (!M22.VNHandler.LoadCharacter(_lineC.m_parameters_txt[0], _lineC.m_parameters_txt[1]))
                        {
                            UnityWrapper.LogErrorFormat("Failed to load character \"{0}\" at line {1}!", (_lineC.m_parameters_txt[0] + " - " + _lineC.m_parameters_txt[1]), _lineC.m_origScriptPos);
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
                case M22.LINETYPE.CLEAR_CHARACTERS:
                    _lineC.m_parameters = new List<int>();
                    if (_splitStr.Count > 1 && _splitStr[1].Equals("true"))
                    {
                        _lineC.m_parameters.Add(1);
                    }
                    else
                        _lineC.m_parameters.Add(0);
                    break;
                case M22.LINETYPE.PLAY_STING:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        _splitStr[1] = _splitStr[1].TrimEnd('\r', '\n');
                        _lineC.m_parameters_txt.Add(_splitStr[1]);

                        if (!M22.AudioMaster.LoadSting(_lineC.m_parameters_txt[0]))
                        {
                            UnityWrapper.LogError("Failed to load sting! - " + _lineC.m_parameters_txt[0]);
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
                            UnityWrapper.LogErrorFormat("Failed to load music file \"{0}\" at line {1}!", _lineC.m_parameters_txt[0], _lineC.m_origScriptPos);
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

                        if(CustomFunctionHandler.CheckFunctionExists(_lineC.m_parameters_txt[0]) == false)
                        {
                            UnityWrapper.LogErrorFormat("Custom function \"{0}\" does not exist at line {1}!", _lineC.m_parameters_txt[0], _lineC.m_origScriptPos);
                        }
                    }
                    break;
                case M22.LINETYPE.STOP_MUSIC:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        //_splitStr[1] = _splitStr[1].TrimEnd('\r', '\n');
                        _lineC.m_parameters_txt.Add(_splitStr[1]);
                    }
                    // we store the float value as a string for later use, if provided.
                    // otherwise, just continue
                    break;
                case M22.LINETYPE.TRANSITION:
                    if (_splitStr.Count > 1)
                    {   //Transition other_iwanako tr_eyes 0
                        _lineC.m_parameters_txt = new List<string>();
                        for (int i = 1; i < _splitStr.Count; i++)
                        {
                            _lineC.m_parameters_txt.Add(_splitStr[i]);
                        }

                        if (!M22.BackgroundMaster.LoadBackground(_lineC.m_parameters_txt[0]))
                        {
                            UnityWrapper.LogErrorFormat("Failed to load background - \"{0}\"", _lineC.m_parameters_txt[0]);
                            // failed to load bg!
                        };
                    }
                    break;
                case M22.LINETYPE.DRAW_BACKGROUND:
                    if (_splitStr.Count >= 2)
                    {
                        _lineC.m_parameters = new List<int>();
                        _lineC.m_parameters_txt = new List<string>();
                        _lineC.m_parameters_txt.Add(_splitStr[1]);
                        if (_splitStr.Count >= 3)
                        {
                            _lineC.m_parameters.Add(Int32.Parse(_splitStr[2]));
                            if (_splitStr.Count >= 4)
                                _lineC.m_parameters.Add(Int32.Parse(_splitStr[3]));
                            if (_splitStr.Count >= 5)
                                _lineC.m_parameters_txt.Add(_splitStr[4]);
                            if (_splitStr.Count >= 6)
                                _lineC.m_parameters_txt.Add(_splitStr[5]);
                        }
                        else
                        {
                            _lineC.m_parameters.Add(0);
                            _lineC.m_parameters.Add(0);
                            _lineC.m_parameters_txt.Add("1.0");
                            _lineC.m_parameters_txt.Add("1.0");
                        }

                        if (_splitStr[_splitStr.Count-1].Equals("true"))
                            _lineC.m_parameters.Add(1);
                        else
                            _lineC.m_parameters.Add(0);

                        if (!M22.BackgroundMaster.LoadBackground(_lineC.m_parameters_txt[0]))
                        {
                            UnityWrapper.LogErrorFormat("Failed to load background \"{0}\" at line {1}", _lineC.m_parameters_txt[0], _lineC.m_origScriptPos);
                        };
                    }
                    else
                        UnityWrapper.LogErrorFormat("Not enough parameters on DrawBackground at line {0}", _lineC.m_origScriptPos);
                    break;
                case M22.LINETYPE.ENABLE_NOVEL_MODE:
                    break;
                case M22.LINETYPE.DISABLE_NOVEL_MODE:
                    break;
                case M22.LINETYPE.PLAY_VIDEO:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        _lineC.m_parameters_txt.Add(_splitStr[1]);

                        if(M22.ScriptMaster.LoadVideoFile(_splitStr[1]) == false)
                        {
                            UnityWrapper.LogError("Failed to load video file: " + _splitStr[1]);
                        }
                    }
                    break;
                case M22.LINETYPE.CLEAR_CHARACTER:
                    if (_splitStr.Count >= 2)
                    {
                        _lineC.m_parameters = new List<int>();
                        _lineC.m_parameters_txt = new List<string>();
                        _lineC.m_parameters_txt.Add(_splitStr[1]);
                        if (_splitStr.Count >= 3)
                        {
                            if (_splitStr[2].Equals("true"))
                                _lineC.m_parameters.Add(1);
                            else
                                _lineC.m_parameters.Add(0);
                        }
                        else
                            _lineC.m_parameters.Add(0);
                    }
                    break;
                case M22.LINETYPE.LOAD_SCRIPT:
                case M22.LINETYPE.HIDE_WINDOW:
                case M22.LINETYPE.SHOW_WINDOW:
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
                case M22.LINETYPE.PLAY_SFX_LOOPED:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        _lineC.m_parameters_txt.Add(_splitStr[1]);

                        if(_splitStr.Count > 3)
                        {
                            _lineC.m_parameters_txt.Add(_splitStr[2]);
                            _lineC.m_parameters_txt.Add(_splitStr[3]);
                        }
                        else
                        {
                            _lineC.m_parameters_txt.Add("1.0");
                            _lineC.m_parameters_txt.Add("1.0");
                        }

                        if (!M22.AudioMaster.LoadSting(_lineC.m_parameters_txt[0]))
                        {
                            UnityWrapper.LogError("Failed to load sting! - " + _lineC.m_parameters_txt[0]);
                        };
                    }
                    break;
                case M22.LINETYPE.STOP_SFX_LOOPED:
                    if (_splitStr.Count > 1)
                    {
                        _lineC.m_parameters_txt = new List<string>();
                        _lineC.m_parameters_txt.Add(_splitStr[1]);

                        if(AudioMaster.IsAudioLoaded(_splitStr[1]) == false)
                        {
                            UnityWrapper.LogWarningFormat("Stopping a looped SFX that isn't played/loaded yet at line {0}; this shouldn't happen!", _lineC.m_origScriptPos);
                        }

                        if(_splitStr.Count > 2)
                        {
                            _lineC.m_parameters_txt.Add(_splitStr[2]);
                        }
                        else
                        {
                            _lineC.m_parameters_txt.Add("1.0");
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
                            UnityWrapper.LogError("MakeDecision error; mismatched number of quotemarks!");
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
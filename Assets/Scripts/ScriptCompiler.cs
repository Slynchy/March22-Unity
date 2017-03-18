using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptCompiler
{
    private static void ThisIsABreakpoint()
    {
        bool lol;
        return;
    }

    private static bool IsNewLine(string s)
    {
        return s == "\r\n";
    }
    private static bool IsComment(string s)
    {
        if (s.Length < 2) return true;

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

    static public List<M22.Script.line_c> CompileScript(string filename)
    {
        var result = new List<M22.Script.line_c>();
        var file = Resources.Load(filename) as TextAsset;
        if (!file)
        {
            Debug.Log("failed to load file");
            return result;
        }

        var scriptLines = new List<string>();
        string currentLine = "";
        for (int i = 0; i < file.text.Length; i++)
        {
            currentLine += file.text[i];
            if (file.text[i] == '\n')
            {
                // new line
                scriptLines.Add(currentLine);
                currentLine = "";
            }
        }

        scriptLines.RemoveAll(IsNewLine);
        scriptLines.RemoveAll(IsComment);

        List<string> CURRENT_LINE_SPLIT = new List<string>();
        for (int i = 0; i < scriptLines.Count; i++)
        {
            SplitString(scriptLines[i], CURRENT_LINE_SPLIT, ' ');// scriptLines[i]
            if (CURRENT_LINE_SPLIT.Count == 0) continue;
            M22.Script.line_c tempLine_c = new M22.Script.line_c();
            tempLine_c.m_lineType = M22.Script.ScriptMaster.CheckLineType(CURRENT_LINE_SPLIT[0]);

            if(tempLine_c.m_lineType == M22.Script.LINETYPE.NARRATIVE)
            {
                tempLine_c.m_lineContents = scriptLines[i];
            }
            else
            {
                CompileLine(tempLine_c, CURRENT_LINE_SPLIT);
            }
        }

        foreach (var item in scriptLines)
        {
            Debug.Log(item);
        }

        return new List<M22.Script.line_c>();
    }

    static void CompileLine(M22.Script.line_c _lineC, List<string> _splitStr)
    {
        return;
    }
}

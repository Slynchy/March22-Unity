using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace M22
{


    public static class FadeEffectClass
    {

        public static IEnumerator FadeOutIMG(Image img, float FadeTime)
        {
            while (img.color.a > 0)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a - (Time.deltaTime * FadeTime));

                yield return null;
            }
        }

        public static IEnumerator FadeInIMG(Image img, float FadeTime)
        {
            while (img.color.a < 1)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a + (Time.deltaTime * FadeTime));

                yield return null;
            }
        }

        public static IEnumerator FadeInTXT(Text img, float FadeTime)
        {
            while (img.color.a < 1)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a + (Time.deltaTime * FadeTime));

                yield return null;
            }
        }

        public static IEnumerator FadeOutTXT(Text img, float FadeTime)
        {
            while (img.color.a > 0)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, img.color.a - (Time.deltaTime * FadeTime));

                yield return null;
            }
        }
    }

    public enum WAIT_STATE
    {
        NOT_WAITING,
        CHARACTER_FADEIN,
        CHARACTER_FADEOUT,
        CHARACTER_FADEOUT_INDIVIDUAL,
        TRANSITION,
        WAIT_COMMAND,
        BACKGROUND_MOVING,
        VIDEO_PLAYING
    }

    public struct WaitObject
    {
        public WAIT_STATE type;
        public List<String> strings;
        public List<int> intParams;
        public WaitObject(WAIT_STATE _type, String[] _strings, int[] _intParams)
        {
            type = _type;
            strings = new List<string>();
            intParams = new List<int>();
            for (int i = 0; i < _strings.Length; i++)
                strings.Add(_strings[i]);
            for (int i = 0; i < _intParams.Length; i++)
                intParams.Add(_intParams[i]);
        }
        public WaitObject(WAIT_STATE _type)
        {
            type = _type;
            strings = null;
            intParams = null;
        }
        public WaitObject(WAIT_STATE _type, string _string)
        {
            type = _type;
            strings = new List<string>();
            strings.Add(_string);
            intParams = null;
        }
    }

    public class ScriptMaster : MonoBehaviour
    {
        [HideInInspector]
        public line_c CURRENT_LINE;

        public Image background;
        public Image backgroundTrans;
        private BackgroundScript backgroundScript;
        private BackgroundScript backgroundTransScript;

        private GameObject LoopedSFXPrefab;

        private M22.Script.Script currentScript_c = new M22.Script.Script();
        private int lineIndex = 0;

        private VNHandler VNHandlerScript;
        private AudioMaster AudioMasterScript;

        private Image TextboxIMG;

        public TypeWriterScript TEXT;

        //private WAIT_STATE WaitState = 0;
        private List<WaitObject> WaitQueue;

        public GameObject TransitionPrefab;
        private Dictionary<string, Sprite> TransitionEffects;

        public GameObject DecisionsPrefab;

        public GameObject VideoPlayerPrefab;
        private VideoPlayer VideoPlayerInstance;

        private bool inLineFunctionMode = false;
        private line_c CurrentInlineFunction;

        private List<Canvas> Canvases;
        public enum CANVAS_TYPES
        {
            BACKGROUND,
            PRECHARACTER,
            CHARACTER,
            POSTCHARACTER,
            TEXTBOX,
            EFFECTS,
            NUM_OF_CANVASES
        }

        public List<string> SCRIPT_FLAGS;

        static public Dictionary<string, VideoClip> loadedVideoClips;

        private float waitCommandTimer = -1.0f;

        static public ScriptCompiler.ANIMATION_TYPES ActiveAnimationType;

        static public bool LoadVideoFile(string _file)
        {
            if (loadedVideoClips.ContainsKey(_file))
                return true;

            VideoClip temp = Resources.Load<VideoClip>("Video/" + _file) as VideoClip;
            if (temp == null)
                return false;
            else
            {
                loadedVideoClips.Add(_file, temp);
                return true;
            }
        }

        public void SetCurrentInlineFunction(line_c _lineC) { CurrentInlineFunction = _lineC; }

        void Awake()
        {
            WaitQueue = new List<WaitObject>();
            Canvases = new List<Canvas>((int)CANVAS_TYPES.NUM_OF_CANVASES);
            loadedVideoClips = new Dictionary<string, VideoClip>();
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
            TextboxIMG = GameObject.Find("Textbox").GetComponent<Image>();
            if (TEXT == null)
            {
                Debug.Log("TEXT not found in ScriptMaster; falling back to searching...");
                TEXT = TextboxIMG.gameObject.GetComponentInChildren<TypeWriterScript>();
                if (TEXT == null)
                    Debug.Log("This also failed! :(");
                else
                    TEXT.SetParent(this);
            }
            else
                TEXT.SetParent(this);
            if (background == null)
            {
                Debug.Log("background not found in ScriptMaster; falling back to searching...");
                if (GameObject.Find("Background") != null)
                    background = GameObject.Find("Background").GetComponent<Image>();
                if (background == null)
                    Debug.Log("This also failed! :(");
                else
                    backgroundScript = background.gameObject.GetComponent<BackgroundScript>();
            }
            else
                backgroundScript = background.gameObject.GetComponent<BackgroundScript>();

            if (backgroundTrans == null)
            {
                Debug.Log("backgroundTrans not found in ScriptMaster; falling back to searching...");
                if (GameObject.Find("BackgroundTransition") != null)
                    backgroundTrans = GameObject.Find("BackgroundTransition").GetComponent<Image>();
                if (backgroundTrans == null)
                    Debug.Log("This also failed! :(");
                else
                    backgroundTransScript = backgroundTrans.gameObject.GetComponent<BackgroundScript>();
            }
            else
                backgroundTransScript = backgroundTrans.gameObject.GetComponent<BackgroundScript>();

            if (GameObject.Find("BackgroundCanvas") != null)
                Canvases.Add(GameObject.Find("BackgroundCanvas").GetComponent<Canvas>());
            if (GameObject.Find("PreCharacterEffectCanvas") != null)
                Canvases.Add(GameObject.Find("PreCharacterEffectCanvas").GetComponent<Canvas>());
            if (GameObject.Find("CharacterCanvas") != null)
                Canvases.Add(GameObject.Find("CharacterCanvas").GetComponent<Canvas>());
            if (GameObject.Find("PostCharacterEffectCanvas") != null)
                Canvases.Add(GameObject.Find("PostCharacterEffectCanvas").GetComponent<Canvas>());
            if (GameObject.Find("TextboxCanvas") != null)
                Canvases.Add(GameObject.Find("TextboxCanvas").GetComponent<Canvas>());
            if (GameObject.Find("EffectCanvas") != null)
                Canvases.Add(GameObject.Find("EffectCanvas").GetComponent<Canvas>());

            if (TransitionPrefab == null)
                Debug.LogError("TransitionPrefab not attached to ScriptMaster! Check this under Main Camera!");
            TransitionEffects = new Dictionary<string, Sprite>();
            TransitionEffects.Add("tr_eyes", Resources.Load<Sprite>("Transitions/tr_eyes") as Sprite);
            TransitionEffects.Add("default", Resources.Load<Sprite>("Images/white") as Sprite);
            TransitionEffects.Add("tr-pronoise", Resources.Load<Sprite>("Transitions/tr-pronoise") as Sprite);
            TransitionEffects.Add("tr-clockwipe", Resources.Load<Sprite>("Transitions/tr-clockwipe") as Sprite);
            TransitionEffects.Add("tr-softwipe", Resources.Load<Sprite>("Transitions/tr-softwipe") as Sprite);
            TransitionEffects.Add("tr-delayblinds", Resources.Load<Sprite>("Transitions/tr-delayblinds") as Sprite);
            TransitionEffects.Add("tr-flashback", Resources.Load<Sprite>("Transitions/tr-flashback") as Sprite);

            if (VideoPlayerPrefab == null)
                Debug.LogError("VideoPlayerPrefab not attached to ScriptMaster! Check this under Main Camera!");

            LoopedSFXPrefab = Resources.Load("Prefabs/SFXPrefab") as GameObject;
            if(LoopedSFXPrefab == null)
                Debug.LogError("Failed to load LoopedSFXPrefab! Check \"Resources/Prefabs\" for this!");

            if (TextboxIMG != null)
                HideText();
        }

        public delegate void VoidDelegate();
        public delegate void VoidDelegateWithBool(bool _bool = false);

        public void FinishBackgroundMovement()
        {
            if (WaitQueue.Count > 0 && WaitQueue[0].type == WAIT_STATE.BACKGROUND_MOVING)
            {
                if (WaitQueue.Count > 0)
                    WaitQueue.RemoveAt(0);
                NextLine();
            }
        }

        public void NextLine(bool _isInLine = false)
        {
            if(_isInLine == true || inLineFunctionMode == true)
            {
                TEXT.FinishedInlineFunction();
                inLineFunctionMode = false;
                return;
            }

            ++lineIndex;
            CURRENT_LINE = currentScript_c.GetLine(lineIndex);
            if (VNHandlerScript.VNMode == true && CURRENT_LINE.m_lineType != LINETYPE.MAKE_DECISION)
                TEXT.Reset(true);
            if (CURRENT_LINE.m_lineType != LINETYPE.MAKE_DECISION)
                TEXT.SetNewCurrentLine("");
            ExecuteFunction(CURRENT_LINE);
        }
        public void GotoLine(int lineNum)
        {
            lineIndex = lineNum - 1;
            NextLine();
        }

        public void ExecuteFunction(line_c _line, bool _isInLine = false)
        {
            if (_isInLine == true) inLineFunctionMode = true;

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
                        NextLine(_isInLine);
                    break;
                case LINETYPE.MOVEMENT_SPEED:
                    if (VNHandlerScript != null)
                        VNHandlerScript.SetMovementSpeed(float.Parse(_line.m_parameters_txt[0]));
                    NextLine(_isInLine);
                    break;
                case LINETYPE.TEXT_SPEED:
                    if (TEXT != null)
                        TEXT.SetTextSpeed(float.Parse(_line.m_parameters_txt[0]));
                    NextLine(_isInLine);
                    break;
                case LINETYPE.DRAW_BACKGROUND:
                    if (backgroundScript.IsMoving())
                        backgroundScript.SetIsMoving(false);
                    backgroundTrans.sprite = M22.BackgroundMaster.GetBackground(_line.m_parameters_txt[0]);
                    //RectTransform tempRT = backgroundTrans.gameObject.GetComponent<RectTransform>();
                    //tempRT.offsetMax = new Vector2(backgroundTrans.sprite.texture.height, 0);

                    if (backgroundTrans.sprite == background.sprite)
                    {
                        //WaitState = WAIT_STATE.BACKGROUND_MOVING;
                        WaitQueue.Add(new M22.WaitObject(WAIT_STATE.BACKGROUND_MOVING));
                        backgroundScript.UpdatePos(
                            _line.m_parameters[0],
                            _line.m_parameters[1]
                        );
                    }
                    else
                    {
                        backgroundTransScript.UpdateBackground(
                            _line.m_parameters[0],
                            _line.m_parameters[1],
                            float.Parse(_line.m_parameters_txt[1]),
                            float.Parse(_line.m_parameters_txt[2])
                        );
                        backgroundTrans.color = new Color(1, 1, 1, 0.001f);
                    }

                    if (_line.m_parameters[2] == 1)
                    {
                        //WaitState = WAIT_STATE.NOT_WAITING;
                        if(WaitQueue.Count > 0)
                            WaitQueue.RemoveAt(0);
                        NextLine(_isInLine);
                    }
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
                    if (!success)
                    {
                        Debug.LogError("Failed to find checkpoint: " + _line.m_parameters_txt[0]);
                        NextLine(_isInLine);
                    }
                    break;
                case LINETYPE.WAIT:
                    //WaitState = WAIT_STATE.WAIT_COMMAND;
                    WaitQueue.Add(new WaitObject(WAIT_STATE.WAIT_COMMAND));
                    waitCommandTimer = 0;
                    break;
                case LINETYPE.DRAW_CHARACTER:
                    VNHandlerScript.CreateCharacter(_line.m_parameters_txt[0], _line.m_parameters_txt[1], _line.m_parameters[0]);
                    if (_line.m_parameters[1] != 1)
                    {
                        HideText();
                        //WaitState = WAIT_STATE.CHARACTER_FADEIN;
                        WaitQueue.Add(new WaitObject(WAIT_STATE.CHARACTER_FADEIN));
                    }
                    else
                        NextLine(_isInLine);
                    break;
                case LINETYPE.PLAY_MUSIC:
                    M22.AudioMaster.ChangeTrack(_line.m_parameters_txt[0]);
                    NextLine(_isInLine);
                    break;
                case LINETYPE.ANIMATION_TYPE:
                    ActiveAnimationType = (ScriptCompiler.ANIMATION_TYPES)_line.m_parameters[0];
                    NextLine(_isInLine);
                    break;
                case LINETYPE.STOP_MUSIC:
                    if (_line.m_parameters_txt != null)
                        AudioMasterScript.StopMusic(_line.m_parameters_txt[0]);
                    else
                        AudioMasterScript.StopMusic("1.0");
                    NextLine(_isInLine);
                    break;
                case LINETYPE.PLAY_STING:
                    M22.AudioMaster.PlaySting(_line.m_parameters_txt[0]);
                    NextLine(_isInLine);
                    break;
                case LINETYPE.HIDE_WINDOW:
                    if (_line.m_parameters_txt != null && _line.m_parameters_txt.Count >= 1)
                        HideText(true, float.Parse(_line.m_parameters_txt[0]));
                    else
                        HideText();
                    NextLine(_isInLine);
                    break;
                case LINETYPE.SHOW_WINDOW:
                    if (_line.m_parameters_txt != null && _line.m_parameters_txt.Count >= 1)
                        ShowText(true, float.Parse(_line.m_parameters_txt[0]));
                    else
                        ShowText();
                    NextLine(_isInLine);
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
                    NextLine(_isInLine);
                    break;
                case LINETYPE.IF_STATEMENT:
                    // m22IF _flag_to_check_if_true Command [params]
                    if (SCRIPT_FLAGS.Contains(_line.m_parameters_txt[0]))
                    {
                        //Debug.Log("True!");
                        line_c tempCompiledLine = new line_c();
                        tempCompiledLine.m_lineType = _line.m_lineTypeSecondary;
                        tempCompiledLine.m_parameters_txt = new List<string>();
                        if (_line.m_parameters_txt != null && _line.m_parameters_txt.Count > 1)
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
                        NextLine(_isInLine);
                    }
                    break;
                case LINETYPE.NULL_OPERATOR:
                    NextLine(_isInLine);
                    break;
                case LINETYPE.MAKE_DECISION:
                    GameObject tempObj = GameObject.Instantiate<GameObject>(DecisionsPrefab, Canvases[(int)CANVAS_TYPES.EFFECTS].transform);
                    if (_line.m_parameters_txt.Count == 6)
                        tempObj.GetComponent<Decision>().Initialize(_line.m_parameters_txt[0], _line.m_parameters_txt[1], _line.m_parameters_txt[2], _line.m_parameters_txt[3], _line.m_parameters_txt[4], _line.m_parameters_txt[5]);
                    else
                        tempObj.GetComponent<Decision>().Initialize(_line.m_parameters_txt[0], _line.m_parameters_txt[1], _line.m_parameters_txt[2], _line.m_parameters_txt[3]);
                    break;
                case LINETYPE.ENABLE_NOVEL_MODE:
                    if (VNHandlerScript.VNMode == true)
                        VNHandlerScript.ToggleVNMode();
                    NextLine(_isInLine);
                    break;
                case LINETYPE.DISABLE_NOVEL_MODE:
                    if (VNHandlerScript.VNMode == false)
                        VNHandlerScript.ToggleVNMode();
                    NextLine(_isInLine);
                    break;
                case LINETYPE.PLAY_VIDEO:
                    VideoPlayerInstance = GameObject.Instantiate<GameObject>(VideoPlayerPrefab, Canvases[(int)CANVAS_TYPES.EFFECTS].transform).GetComponent<VideoPlayer>();
                    VideoPlayerInstance.targetCamera = Camera.main;
                    AudioMasterScript.StopMusic("100.0");
                    VideoClip tempVid;
                    loadedVideoClips.TryGetValue(_line.m_parameters_txt[0], out tempVid);
                    VideoPlayerInstance.clip = tempVid;
                    VideoPlayerInstance.SetTargetAudioSource(0, Camera.main.GetComponent<AudioSource>());
                    VideoPlayerInstance.Play();
                    //WaitState = WAIT_STATE.VIDEO_PLAYING;
                    WaitQueue.Add(new WaitObject(WAIT_STATE.VIDEO_PLAYING));
                    break;
                case LINETYPE.LOAD_SCRIPT:
                    LoadScript(_line.m_parameters_txt[0]);
                    break;
                case LINETYPE.PLAY_SFX_LOOPED:
                    SFXScript temploopSFX = GameObject.Instantiate<GameObject>(LoopedSFXPrefab, Camera.main.transform).GetComponent<SFXScript>();
                    temploopSFX.Init(AudioMaster.GetAudio(_line.m_parameters_txt[0]), _line.m_parameters_txt[1], _line.m_parameters_txt[2], true);
                    NextLine();
                    break;
                case LINETYPE.STOP_SFX_LOOPED:
                    GameObject loopSFXobj = GameObject.Find(_line.m_parameters_txt[0]);
                    if (loopSFXobj == null)
                    {
                        Debug.LogErrorFormat("Failed to stop looping SFX \"{0}\" at line {1}; is not currently playing!", _line.m_parameters_txt[0], _line.m_origScriptPos);
                        NextLine();
                        return;
                    }
                    SFXScript stopSFXloop = loopSFXobj.GetComponent<SFXScript>();
                    stopSFXloop.Stop(_line.m_parameters_txt[1]);
                    NextLine();
                    break;
                case LINETYPE.CLEAR_CHARACTER:
                    if (VNHandlerScript.ClearCharacter(_line.m_parameters_txt[0], _line.m_parameters[0] == 1 ? true : false) == false)
                    {
                        Debug.LogErrorFormat("Unable to clear character {0} at line {1}", _line.m_parameters_txt[0], _line.m_origScriptPos);
                        NextLine(_isInLine);
                        break;
                    }
                    if (_line.m_parameters[0] == 1)
                        NextLine(_isInLine);
                    else
                        WaitQueue.Add(new WaitObject(WAIT_STATE.CHARACTER_FADEOUT_INDIVIDUAL, _line.m_parameters_txt[0]));
                    break;
                case LINETYPE.CLEAR_CHARACTERS:
                    VNHandlerScript.ClearCharacters(_line.m_parameters[0] == 1 ? true : false );
                    HideText();
                    //WaitState = WAIT_STATE.CHARACTER_FADEOUT;
                    WaitQueue.Add(new WaitObject(WAIT_STATE.CHARACTER_FADEOUT));
                    break;
                case LINETYPE.EXECUTE_FUNCTION:
                    CustomFunctionHandler.CustomFunctionDelegate temp = CustomFunctionHandler.GetFunction(_line.m_parameters_txt[0]);
                    if (temp != null)
                        temp(_line.m_parameters_txt[1], _line.m_parameters_txt[2], _line.m_parameters_txt[3]);
                    else
                        Debug.LogError("Failed to find custom function: " + _line.m_parameters_txt[0]);
                    break;
                case LINETYPE.TRANSITION:
                    GameObject tempGO = GameObject.Instantiate<GameObject>(TransitionPrefab, Canvases[(int)CANVAS_TYPES.POSTCHARACTER].transform);
                    tempGO.GetComponent<Image>().material = Instantiate<Material>(tempGO.GetComponent<Image>().material) as Material;
                    BackgroundTransition TransitionObj = tempGO.GetComponent<BackgroundTransition>();
                    TransitionObj.callback = FadeToBlackCallback;
                    if(_line.m_parameters_txt.Count >= 4)
                    {
                        TransitionObj.Speed = float.Parse(_line.m_parameters_txt[3]);
                    }
                    //TransitionObj.srcSprite = background.sprite;
                    TransitionObj.srcSprite = Resources.Load<Sprite>("Images/empty") as Sprite;
                    TransitionEffects.TryGetValue(_line.m_parameters_txt[1], out TransitionObj.effect);
                    TransitionObj.destSprite = M22.BackgroundMaster.GetBackground(_line.m_parameters_txt[0]);
                    TransitionObj.inOrOut = (String.Equals(_line.m_parameters_txt[2], "in") ? BackgroundTransition.IN_OR_OUT.IN : BackgroundTransition.IN_OR_OUT.OUT);
                    //WaitState = WAIT_STATE.TRANSITION;
                    WaitQueue.Add(new WaitObject(WAIT_STATE.TRANSITION));
                    break;
                case LINETYPE.NUM_OF_LINETYPES:
                    // do nuzing.
                    Debug.Log("End of script! This shouldn't really happen; make sure your script ends properly!");
                    HideText();
                    break;
                default:
                    NextLine(_isInLine);
                    break;
            }
        }

        public Canvas GetCanvas(CANVAS_TYPES _type)
        {
            if (_type == CANVAS_TYPES.NUM_OF_CANVASES) return null;
            return Canvases[(int)_type];
        }

        public void FadeToBlackCallback()
        {
            BackgroundTransition temp = GameObject.FindGameObjectWithTag("Transition").GetComponent<BackgroundTransition>();
            background.sprite = temp.destSprite;
            backgroundScript.UpdateBackground(
                0,
                0,
                1.0f,
                1.0f
            );
            //WaitState = WAIT_STATE.NOT_WAITING;
            if (WaitQueue.Count > 0) WaitQueue.RemoveAt(0);
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

        void HideText(bool _fade = true, float _speed = 6.0f)
        {
            for (int i = 0; i < TextboxIMG.gameObject.transform.childCount; i++)
            {
                var obj = TextboxIMG.gameObject.transform.GetChild(i).gameObject.GetComponent<Text>();
                if (_fade == true)
                    StartCoroutine(FadeEffectClass.FadeOutTXT(obj, _speed));
                else
                    obj.color = new Color(255, 255, 255, 0);
            }

            if (_fade == true)
                StartCoroutine(FadeEffectClass.FadeOutIMG(TextboxIMG, _speed));
            else
                TextboxIMG.color = new Color(255, 255, 255, 0);
        }

        void ShowText(bool _fade = true, float _speed = 6.0f)
        {
            for (int i = 0; i < TextboxIMG.gameObject.transform.childCount; i++)
            {
                var obj = TextboxIMG.gameObject.transform.GetChild(i).gameObject.GetComponent<Text>();
                if (_fade == true)
                    StartCoroutine(FadeEffectClass.FadeInTXT(obj, _speed));
                else
                    obj.color = new Color(255, 255, 255, 255);
            }

            if (_fade == true)
                StartCoroutine(FadeEffectClass.FadeInIMG(TextboxIMG, _speed));
            else
                TextboxIMG.color = new Color(255, 255, 255, 255);
        }

        void Update()
        {
            if ((WaitQueue.Count == 0 && InputWrapper.NextLineButton()) || InputWrapper.SkipTextButton())
            {
                if (backgroundTrans != null && backgroundTrans.color.a != 0)
                {
                    //nullop
                }
                else
                    FireInput();
            }

            if (backgroundTrans != null && backgroundTrans.color.a != 0)
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
                    backgroundScript.UpdateBackground(
                        CURRENT_LINE.m_parameters[0],
                        CURRENT_LINE.m_parameters[1],
                        float.Parse(CURRENT_LINE.m_parameters_txt[1]),
                        float.Parse(CURRENT_LINE.m_parameters_txt[2])
                    );
                    NextLine();
                }
            }

            if (WaitQueue.Count > 0)
            {
                switch (WaitQueue[0].type)
                {
                    case WAIT_STATE.BACKGROUND_MOVING:
                        break;
                    case WAIT_STATE.VIDEO_PLAYING:
                        if (VideoPlayerInstance.isPlaying == false)
                        {
                            WaitQueue.RemoveAt(0);
                            Destroy(VideoPlayerInstance.gameObject);
                            NextLine();
                        }
                        break;
                    case WAIT_STATE.CHARACTER_FADEIN:
                        var characters = GameObject.FindGameObjectsWithTag("Character");
                        int size = characters.Length;
                        int count = 0;
                        foreach (var item in characters)
                        {
                            if (item.GetComponent<CharacterScript>().GetState() == CharacterScript.STATES.IDLE)
                            {
                                count++;
                            }
                        }

                        if (size == count)
                        {
                            WaitQueue.RemoveAt(0);
                            ShowText();
                            NextLine();
                        }
                        break;
                    case WAIT_STATE.CHARACTER_FADEOUT:
                        size = GameObject.FindGameObjectsWithTag("Character").Length;
                        if (size == 0)
                        {
                            WaitQueue.RemoveAt(0);
                            ShowText();
                            NextLine();
                        }
                        break;
                    case WAIT_STATE.CHARACTER_FADEOUT_INDIVIDUAL:
                        if(WaitQueue[0].strings.Count > 0)
                        {
                            GameObject find = GameObject.Find(WaitQueue[0].strings[0]);
                            if(find == null)
                            {
                                WaitQueue.RemoveAt(0);
                                ShowText();
                                NextLine();
                            }
                        }
                        break;
                    case WAIT_STATE.WAIT_COMMAND:
                        waitCommandTimer += Time.deltaTime;
                        int param;
                        if (inLineFunctionMode == true)
                            param = CurrentInlineFunction.m_parameters[0];
                        else
                            param = CURRENT_LINE.m_parameters[0];
                        if (waitCommandTimer >= (param * 0.001f))
                        {
                            WaitQueue.RemoveAt(0);
                            NextLine();
                            waitCommandTimer = -1;
                        }
                        break;
                }
            }

        }

        public void LoadScript(string _fname)
        {
            BackgroundMaster.UnloadBackgrounds();
            ScriptCompiler.UnloadCheckpoints();
            AudioMaster.UnloadAudio();
            VNHandler.UnloadCharacters();
            Resources.UnloadUnusedAssets();
            currentScript_c = M22.ScriptCompiler.CompileScript(_fname);
            //M22.ScriptCompiler.CompileScriptAsync(_fname, ref currentScript_c);
            lineIndex = 0;
            TEXT.Reset(true);
            CURRENT_LINE = currentScript_c.GetLine(lineIndex);
            TEXT.SetNewCurrentLine(CURRENT_LINE.m_lineContents);
            ExecuteFunction(CURRENT_LINE);
        }

        public void DialogueExampleFunction()
        {
            LoadScript("DialogueExample");
            ShowText();
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
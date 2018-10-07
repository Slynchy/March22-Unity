using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M22
{
    public class SceneManager : MonoBehaviour
    {

        AudioMaster audioMaster;
        ScriptMaster scriptMaster;
        VNHandler vnHandler;
        CustomFunctionHandler customFunctionality;
        AudioSource musicSource;

        public GameObject BackgroundCanvasPrefab;
        public GameObject PreCharacterEffectCanvasPrefab;
        public GameObject CharacterCanvasPrefab;
        public GameObject PostCharacterEffectCanvasPrefab;
        public GameObject TextboxCanvasPrefab;
        public GameObject EffectCanvasPrefab;

        public GameObject TransitionPrefab;
        public GameObject DecisionsPrefab;
        public GameObject VideoPlayerPrefab;
        public GameObject LoopedSFXPrefab;
        public GameObject CharacterPrefab;
        public UnityEngine.Audio.AudioMixerGroup outputAudioMixer;

        private GameObject BackgroundCanvas;
        private GameObject PreCharacterEffectCanvas;
        private GameObject CharacterCanvas;
        private GameObject PostCharacterEffectCanvas;
        private GameObject TextboxCanvas;
        private GameObject EffectCanvas;

        private Camera camera;

        void Awake()
        {
            if (!(camera = this.GetComponent<Camera>()))
            {
                throw new System.Exception("[SceneManager] Needs to be attached to a camera.");
            }

            this.AddPrefabs();

            musicSource = this.gameObject.AddComponent<AudioSource>();
            musicSource.bypassEffects = true;
            musicSource.bypassReverbZones = true;
            musicSource.loop = true;
            musicSource.priority = 64;
            musicSource.outputAudioMixerGroup = this.outputAudioMixer;
            audioMaster = new AudioMaster(musicSource, this);

            vnHandler = new VNHandler(CharacterPrefab);
            scriptMaster = new ScriptMaster(
                vnHandler,
                audioMaster,
                TextboxCanvas.transform.Find("Textbox").GetComponent<Image>(),
                DecisionsPrefab,
                TransitionPrefab,
                VideoPlayerPrefab,
                LoopedSFXPrefab
            );
            scriptMaster.background = BackgroundCanvas.gameObject.transform.Find("Background").gameObject.GetComponent<Image>();
            scriptMaster.backgroundTrans = BackgroundCanvas.gameObject.transform.Find("BackgroundTransition").gameObject.GetComponent<Image>();
            scriptMaster.TEXT = TextboxCanvas.gameObject.transform.GetComponentInChildren<TypeWriterScript>();
            scriptMaster.TransitionPrefab = this.TransitionPrefab;
            scriptMaster.DecisionsPrefab = this.DecisionsPrefab;
            scriptMaster.VideoPlayerPrefab = this.VideoPlayerPrefab;

            customFunctionality = new CustomFunctionHandler();
        }

        private void AddPrefabs()
        {
            BackgroundCanvas = GameObject.Instantiate(BackgroundCanvasPrefab, this.transform);
            PreCharacterEffectCanvas = GameObject.Instantiate(PreCharacterEffectCanvasPrefab, this.transform);
            CharacterCanvas = GameObject.Instantiate(CharacterCanvasPrefab, this.transform);
            PostCharacterEffectCanvas = GameObject.Instantiate(PostCharacterEffectCanvasPrefab, this.transform);
            TextboxCanvas = GameObject.Instantiate(TextboxCanvasPrefab, this.transform);
            EffectCanvas = GameObject.Instantiate(EffectCanvasPrefab, this.transform);

            BackgroundCanvas.name = "BackgroundCanvas";
            PreCharacterEffectCanvas.name = "PreCharacterEffectCanvas";
            CharacterCanvas.name = "CharacterCanvas";
            PostCharacterEffectCanvas.name = "PostCharacterEffectCanvas";
            TextboxCanvas.name = "TextboxCanvas";
            EffectCanvas.name = "EffectCanvas";

            BackgroundCanvas.GetComponent<Canvas>().worldCamera = this.camera;
            PreCharacterEffectCanvas.GetComponent<Canvas>().worldCamera = this.camera;
            CharacterCanvas.GetComponent<Canvas>().worldCamera = this.camera;
            PostCharacterEffectCanvas.GetComponent<Canvas>().worldCamera = this.camera;
            TextboxCanvas.GetComponent<Canvas>().worldCamera = this.camera;
            EffectCanvas.GetComponent<Canvas>().worldCamera = this.camera;

            BackgroundCanvas.GetComponent<Canvas>().planeDistance = 40;
            PreCharacterEffectCanvas.GetComponent<Canvas>().planeDistance = 35;
            CharacterCanvas.GetComponent<Canvas>().planeDistance = 30;
            PostCharacterEffectCanvas.GetComponent<Canvas>().planeDistance = 25;
            TextboxCanvas.GetComponent<Canvas>().planeDistance = 20;
            EffectCanvas.GetComponent<Canvas>().planeDistance = 10;

            BackgroundCanvas.GetComponent<Canvas>().sortingLayerName = "Backgrounds";
            PreCharacterEffectCanvas.GetComponent<Canvas>().sortingLayerName = "PreCharacter";
            CharacterCanvas.GetComponent<Canvas>().sortingLayerName = "Characters";
            PostCharacterEffectCanvas.GetComponent<Canvas>().sortingLayerName = "PostCharacter";
            TextboxCanvas.GetComponent<Canvas>().sortingLayerName = "Textbox";
            EffectCanvas.GetComponent<Canvas>().sortingLayerName = "Effects";
        }
        
        void Start()
        {

        }
        
        void Update()
        {
            scriptMaster.Update();
            vnHandler.Update();
        }

        public VNHandler VNHandler
        {
            get
            {
                return this.vnHandler;
            }
            set
            {
                throw new System.Exception("Do not overwrite!");
            }
        }

        public ScriptMaster ScriptMaster
        {
            get
            {
                return this.scriptMaster;
            }
            set
            {
                throw new System.Exception("Do not overwrite!");
            }
        }
    }
}

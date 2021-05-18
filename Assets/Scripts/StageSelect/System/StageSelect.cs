using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour {

    public static StageSelect instance;

    #region インスペクタ編集部
    public List<string> path;

    public List<Sprite> images;
    #endregion

    #region インスペクタ参照部
    [Disable]
    [SerializeField]
    private GameObject fadeImage;

    [Disable]
    [SerializeField]
    private List<Image> stageWindows;

    [Disable]
    [SerializeField]
    private List<Image> clearStars;
    #endregion

    #region データ部
    private int _stageIndex;
    private int stageIndex {
        get { return _stageIndex; }
        set {
            _stageIndex = value;
            _stageIndex = Mathf.Min(Mathf.Min(path.Count - 1, clearIndex), _stageIndex);
            _stageIndex = Mathf.Max(0, _stageIndex);
        }
    }

    public static int playingIndex;

    public static bool isStageSelect;
    public static string stagePath;

    public static int clearIndex;

    private float unlockTimer = 0;
    #endregion

    private void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        gameObject.AddComponent<InputManager>();
        if (InputManager.isInit == false) {
            InputManager.Init();
#if UNITY_EDITOR
            SystemSupporter.DebugInitInput();
#endif
        }

        isStageSelect = true;

        fadeImage.AddComponent<FadeIn>().timeScale = 2;

        clearIndex = PlayerPrefs.GetInt("ClearIndex", 0);

        if(path.Count != images.Count) {
            Debug.Log("ファイルパスと画像リストのサイズが違います");
        }
    }

    // Update is called once per frame
    void Update() {
        if (InputManager.GetKeyDown(Keys.LEFT)) {
            stageIndex--;
        }
        if (InputManager.GetKeyDown(Keys.RIGHT)) {
            stageIndex++;
        }
        if (InputManager.GetKeyDown(Keys.B)) {
            stagePath = path[stageIndex];
            var fade = fadeImage.AddComponent<FadeOut>();
            fade.nextStagePath = "Stage";
            fade.timeScale = 2;
            playingIndex = stageIndex;
        }
        if (InputManager.GetKeyDown(Keys.A)) {
            var fade = fadeImage.AddComponent<FadeOut>();
            fade.nextStagePath = "Title";
            fade.timeScale = 2;
        }
        stageWindows[0].gameObject.SetActive(stageIndex != 0);
        stageWindows[2].gameObject.SetActive(stageIndex < path.Count - 1);
        stageWindows[3].gameObject.SetActive(stageIndex < path.Count - 2);

        for(int i = 0; i < 3; i++) {
            clearStars[i + 1].enabled = clearIndex > stageIndex + i;
        }

        Debug.Log(clearIndex);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Text;
using UnityEngine.SceneManagement;

/// <summary>
/// 初期化とロード、ステージ生成のクラス
/// </summary>
[DisallowMultipleComponent]
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class Stage : MonoBehaviour {



    #region インスペクタ編集部
    public static Stage instance;

    [Tooltip("編集モード")]
    public bool isEditorMode = false;

    [Tooltip("オプションモード")]
    public bool isOptionMode = false;

    [Tooltip("Resources/StageDatas以下のパスです")]
    public string stagePath;

    public List<char> objectIndex;
    public List<GameObject> objectList;
    #endregion

    [Header("-------------")]


    #region インスペクタ参照部(いじれないようにDisable属性)
    [Disable]
    [SerializeField]
    private GameObject player;

    [Disable]
    [SerializeField]
    private GameObject stageEditor;

    [Disable]
    new public GameObject camera;

    [Disable]
    [SerializeField]
    private GameObject start;

    [Disable]
    [SerializeField]
    private GameObject ghostManager;

    [Disable]
    [SerializeField]
    private GameObject fade;

    [SerializeField]
    private GameObject[] particles;

    [HideInInspector]
    [SerializeField]
    public int turnMax;

    [Disable]
    [SerializeField]
    private GameObject pauseWindow;

    [Disable]
    [SerializeField]
    private RectTransform upSinema;

    [Disable]
    [SerializeField]
    private RectTransform downSinema;
    #endregion

    #region データ部
    private Renderer startObjRenderer;
    private Renderer goalObjRenderer;

    private bool isSetClearFadeOut;

    public GameObject stageParent { get; private set; }
    public List<List<char>> stageData { get; private set; }
    public Vector3 startPosition { get; private set; }

    public List<int> maxTimes { get; private set; } = new List<int>();
    public List<int> maxLoop { get; private set; } = new List<int>();
    public List<GameObject> startBlockList { get; private set; } = new List<GameObject>(7);
    public List<List<GameObject>> enemyList { get; private set; } = new List<List<GameObject>>(7);
    public List<List<Krawler>> enemyScriptList { get; private set; } = new List<List<Krawler>>(7);

    public int visualMode { get; private set; } = 0;


    private List<string> comments = new List<string>();

    private const string stageHeader = "#StageData";
    private const string objectHeader = "#ObjectData";
    private const string detailHeader = "#Detail";
    private const string commentHeader = "#Comment";

    public enum Mode {
        START,
        GAME,
        DEAD,
        CLEAR
    }
    [Disable]
    public Mode nowMode = Mode.START;

    [Disable]
    public CRT crt;

    private float startTimer = 0;

    #endregion

    private void Awake() {
        instance = this;

#if UNITY_EDITOR
        stageEditor = Instantiate(stageEditor, transform);
        stageEditor.SetActive(false);
#else
		isEditorMode = false;
#endif
        stageParent = new GameObject("StageParent");
        stageParent.transform.parent = transform;


        startBlockList = new List<GameObject>();
        for (int i = 0; i < 8; i++) {
            startBlockList.Add(null);
        }
        startBlockList.Capacity = 8;

        enemyList = new List<List<GameObject>>();
        for (int i = 0; i < 8; i++) {
            enemyList.Add(new List<GameObject>());
        }

        enemyScriptList = new List<List<Krawler>>();
        for (int i = 0; i < 8; i++) {
            enemyScriptList.Add(new List<Krawler>());
        }
    }

    void Start() {

        fade.AddComponent<FadeIn>().timeScale = 2;

        gameObject.AddComponent<InputManager>();
        gameObject.AddComponent<SwitchManager>();

        if (InputManager.isInit == false) {
            InputManager.Init();
            SystemSupporter.DebugInitInput();
        }

        player = Instantiate(player);
        ghostManager = Instantiate(ghostManager);

        if (StageSelect.isStageSelect == true) {
            stagePath = StageSelect.stagePath;
        }

        //ステージの生成
        if (ReadCSV(stagePath) == false) {
            Debug.LogError("ステージの読み込みで不具合が発生したため終了しました");
            SystemSupporter.ExitGame();
        }

        Instantiate(particles[visualMode], camera.transform);

        pauseWindow = Instantiate(pauseWindow);

        camera.transform.position = player.transform.position + new Vector3(0.5f, 10, -4.5f);
        pauseWindow.SetActive(false);
    }

    void Update() {
        SystemSupporter.PlaySupport();
        switch (nowMode) {
            case Mode.START: // スタート時の演出
                GameStart();
                break;
            case Mode.GAME:
                if (InputManager.GetKeyDown(Keys.SELECT)) {
                    if (SystemSupporter.IsUnityEditor() == true) {
                        isEditorMode = !isEditorMode;
                        player.transform.position = stageEditor.transform.position;
                    }
                }
                if (SystemSupporter.IsUnityEditor()) {
                    stageEditor.SetActive(isEditorMode);
                }

                if (InputManager.GetKeyDown(Keys.START)) {
                    isOptionMode = !isOptionMode;
                }
                pauseWindow.SetActive(isOptionMode);

                player.SetActive(!(isEditorMode || isOptionMode));
                break;
            case Mode.DEAD:

                break;
            case Mode.CLEAR:
                StageClear();
                break;
        }
    }



    /// <summary>
    /// ステージのCSVを読み込みます
    /// </summary>
    /// <param name="path">ファイルのパス</param>
    bool ReadCSV(string path) {
        if (path == "") {
            Debug.LogError("Stageのファイルパスがnullでした");
            return false;
        }

        try {
            TextAsset csv = Resources.Load("StageDatas/" + path) as TextAsset;
            StringReader reader = new StringReader(csv.text);

            reader.ReadLine(); // ステージ全体のヘッダ読み込み
            var line = reader.ReadLine();

            //ステージ配列を初期化
            stageData = new List<List<char>>();
            var sizeX = int.Parse(line.Split(':')[1].Split(',')[0]);
            var sizeY = int.Parse(line.Split(':')[1].Split(',')[1]);
            for (int i = 0; i < sizeY; i++) {
                stageData.Add(new List<char>());
                for (int j = 0; j < sizeX; j++) {
                    stageData[i].Add('0');
                }
            }

            visualMode = int.Parse(reader.ReadLine().Split(':')[1]);
            line = reader.ReadLine();
            var posX = int.Parse(line.Split(':')[1].Split(',')[0]);
            var posY = int.Parse(line.Split(':')[1].Split(',')[1]);

            //objectHeaderに当たるまでコメント読み込み
            bool isComment = false;
            while (line != objectHeader) {
                if (isComment == true) {
                    comments.Add(line);
                }
                if (line == commentHeader) {
                    isComment = true;
                }
                line = reader.ReadLine();
            }

            //ステージ部分読み込みと生成
            line = reader.ReadLine();
            var lineCount = 0;
            while (line != detailHeader && reader.Peek() > -1) {
                //line引数で関数化出来そう
                if (line == "") {
                    line = reader.ReadLine();
                    break;
                }
                var lineData = line.ToCharArray();
                for (int i = 0; i < line.Length; i++) {
                    var _code = lineData[i];
                    stageData[lineCount][i] = _code;
                    if (_code != '0') {
                        Instantiate(objectList[objectIndex.FindIndex(n => _code == n)], new Vector3(posX + i, 0, posY - lineCount), Quaternion.identity, stageParent.transform);

                    }
                }
                if (reader.Peek() <= -1) { //Detail終了時
                    Debug.Log("Detailなしで終了しました");
                    return true;
                }
                line = reader.ReadLine();
                lineCount++;
            }

            //詳細読み込み
            var code = '0';
            var information = "";
            DetailBase detailBase = null;
            while (reader.Peek() > -1) {
                line = reader.ReadLine();
                if (line == "") {
                    if (detailBase != null) {
                        detailBase.SetData(information);
                        if (code == 'I') {
                            var phase = ((StartBlock)detailBase).phaseCount;
                            if (phase == 0) {
                                Debug.LogError("スタートのフェーズが0でした。1以上に修正してください\n座標：" + detailBase.gameObject.transform.position);
                            }
                            var tmp = phase;
                            int count = 1;
                            while (tmp > 0) {
                                if ((tmp & 1) > 0) {
                                    if (startBlockList[count - 1] != null) {
                                        Debug.LogError("同じフェイズのスタートを検知しました。\n旧座標：" + startBlockList[count - 1].transform.position + "\n新座標：" + detailBase.transform.position);
                                    }
                                    startBlockList[count - 1] = detailBase.gameObject;
                                }
                                tmp >>= 1;
                                count++;
                            }

                        }
                        if (code == 'K') {
                            var phase = ((Krawler)detailBase).phaseCount;
                            if (phase == 0) {
                                Debug.LogError("スタートのフェーズが0でした。1以上に修正してください\n座標：" + detailBase.gameObject.transform.position);
                            }
                            var tmp = phase;
                            int count = 1;
                            while (tmp > 0) {
                                if ((tmp & 1) > 0) {
                                    enemyList[count - 1].Add(detailBase.gameObject);
                                    enemyScriptList[count - 1].Add(detailBase.gameObject.GetComponent<Krawler>());
                                }
                                tmp >>= 1;
                                count++;
                            }

                        }
                        detailBase = null;
                        information = "";
                    }
                    continue;
                }
                if (line.Contains("_")) {
                    code = line.Split('_')[0][0]; // ('_')
                } else if (line.Contains(":")) {
                    if (line.Contains("pos")) {
                        var posString = line.Split(':')[1];
                        var pos = new Vector3(float.Parse(posString.Split(',')[0]), 0, float.Parse(posString.Split(',')[1]));
                        var obj = GetStageObject(pos);
                        detailBase = obj.GetComponent<DetailBase>();
                        continue;
                    }
                    information += line + "\n";
                }
            }

        } catch (NullReferenceException e) {
            Debug.Log("ファイルが見つかりませんでした。新規作成モードにします");
            Debug.Log(e);
            StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Resources/StageDatas/" + stagePath + ".txt", false);
            streamWriter.Flush();
            streamWriter.Close();

            //最初のエリア
            Instantiate(objectList[0], new Vector3(-1, 0, 1), Quaternion.identity, stageParent.transform);
            Instantiate(objectList[0], new Vector3(0, 0, 1), Quaternion.identity, stageParent.transform);
            Instantiate(objectList[0], new Vector3(1, 0, 1), Quaternion.identity, stageParent.transform);
            Instantiate(objectList[0], new Vector3(-1, 0, 0), Quaternion.identity, stageParent.transform);
            Instantiate(objectList[0], new Vector3(1, 0, 0), Quaternion.identity, stageParent.transform);
            Instantiate(objectList[0], new Vector3(-1, 0, -1), Quaternion.identity, stageParent.transform);
            Instantiate(objectList[0], new Vector3(0, 0, -1), Quaternion.identity, stageParent.transform);
            Instantiate(objectList[0], new Vector3(1, 0, -1), Quaternion.identity, stageParent.transform);
            Instantiate(objectList[objectIndex.FindIndex(n => n == 'I')], stageParent.transform);
        } catch (Exception e) {
            Debug.LogError(e);
        }
        return true;
    }

    /// <summary>
    /// コンストラクタ(InitializeOnLoad属性によりエディター起動時に呼び出される)
    /// </summary>
    static Stage() {
        //Playmodeの状態が変わった時のイベントを登録
#if UNITY_EDITOR

        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

#if UNITY_EDITOR
    private static void OnPlayModeStateChanged(PlayModeStateChange state) {
        //再生終了ボタンを押した
        if (instance != null && !EditorApplication.isPlayingOrWillChangePlaymode) {
            instance.WriteCSV();
        }
        try {
        } catch (Exception e) {
            Debug.LogError("終了時にエラーが発生しました" + e);
        }
    }
#endif

    void WriteCSV() {

        float maxLeft = 0;
        float maxRight = 0;
        float maxUp = 0;
        float maxDown = 0;

        //マップ作成。全てのオブジェクトを調べ、全てのオブジェクトが入るList<List<char>>を作る
        for (int i = 0; i < stageParent.transform.childCount; i++) {
            var child = stageParent.transform.GetChild(i).transform;
            if (child.position.x < maxLeft) {
                maxLeft = child.position.x;
            }
            if (child.position.x > maxRight) {
                maxRight = child.position.x;
            }
            if (child.position.z < maxUp) {
                maxUp = child.position.z;
            }
            if (child.position.z > maxDown) {
                maxDown = child.position.z;
            }
        }

        var sizeX = maxRight - maxLeft + 1;
        var sizeY = maxDown - maxUp + 1;

        //stageDataを初期化
        stageData = new List<List<char>>();
        for (int y = 0; y < sizeY; y++) {
            stageData.Add(new List<char>());
            for (int x = 0; x < sizeX; x++) {
                stageData[y].Add('0');
            }
        }

        //ファイルを開いてバッファ準備
        StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Resources/StageDatas/" + stagePath + ".txt", false);
        StringBuilder sb = new StringBuilder();
        StringBuilder detailSb = new StringBuilder(); //詳細設定用StringBuilder

        //ステージヘッダーに書き込み
        sb.AppendLine(stageHeader);
        sb.AppendLine("size:" + sizeX.ToString() + "," + sizeY.ToString());
        sb.AppendLine("design:" + visualMode.ToString()); //ステージデザイン
        sb.AppendLine("pos:" + maxLeft + "," + maxDown);
        sb.AppendLine();
        sb.AppendLine();

        sb.AppendLine(commentHeader);
        for (int i = 0; i < comments.Count; i++) {
            sb.AppendLine(comments[i]);
        }
        if (comments.Count == 0) {
            sb.AppendLine();
        }

        sb.AppendLine(objectHeader);

        //詳細ヘッダーに書き込み
        detailSb.AppendLine("#Detail");

        //stageDataに現在の状況を反映
        for (int i = 0; i < stageParent.transform.childCount; i++) {
            var obj = stageParent.transform.GetChild(i);
            var posX = Mathf.CeilToInt(obj.position.x - maxLeft);
            var posZ = Mathf.CeilToInt(maxDown - obj.position.z);
            var code = GetObjectCode(obj.name);
            stageData[posZ][posX] = code;

            if (code >= 'A' && code <= 'Z') {
                detailSb.AppendLine(obj.GetComponent<DetailBase>().ToFileString());
            }
        }

        //バッファにステージデータを書き込んでいく
        for (int y = 0; y < stageData.Count; y++) {
            for (int x = 0; x < stageData[y].Count; x++) {
                var code = stageData[y][x];
                sb.Append(code);
            }
            sb.Append("\n");

        }

        //書きこみ
        streamWriter.WriteLine(sb.ToString());
        if (detailSb.Length > 10) { //何か書き込まれていたら
            streamWriter.Write(detailSb.ToString());
        }

        //ファイルを閉じる
        streamWriter.Flush();
        streamWriter.Close();
    }

    public void GenerateObject(Vector3 pos, GameObject obj) {
        if (pos == Vector3.zero) {
            //スタートエリアなのでスキップ
            return;
        }
        bool isExistObj = false;
        GameObject existObj = GetStageObject(pos);
        string objName = "";
        if (existObj != null) {
            isExistObj = true;
            objName = existObj.name;
        }
        if (isExistObj == false) {
            Instantiate(obj, pos, Quaternion.identity, stageParent.transform);
        } else if (objName != obj.name + "(Clone)") {
            Destroy(existObj);
            Instantiate(obj, pos, Quaternion.identity, stageParent.transform);
        }
    }

    /// <summary>
    /// その座標のギミックオブジェクトを取得します
    /// </summary>
    /// <param name="pos">座標</param>
    /// <returns>その座標のステージオブジェクト</returns>
    public GameObject GetStageObject(Vector3 pos) {
        for (int i = 0; i < stageParent.transform.childCount; i++) {
            var child = stageParent.transform.GetChild(i);
            if (child.transform.position == pos) {
                return child.gameObject;
            }
        }
        return null;
    }


    public char GetObjectCode(string name) {
        //命名規則は「コード_名前」なのでコード単独の抽出可能
        return name.Split('_')[0][0];
    }

    private int SortCehckpoint(Vector3 a, Vector3 b) {
        var aChannel = GetStageObject(a).GetComponent<CheckPoint>().channel;
        var bChannel = GetStageObject(b).GetComponent<CheckPoint>().channel;

        Debug.Log(GetStageObject(a).GetComponent<CheckPoint>());
        Debug.Log(bChannel);

        return (int)(bChannel - aChannel);
    }

    public bool DestroyEnemy() {
        int phase = Player.instance.phase;
        if (enemyList[phase] != null) {
            for (int i = 0; i < enemyList[phase].Count; ++i) {

                //表示されておる敵だけ倒せる
                if (enemyScriptList[phase][i].isDie == false) {

                    //倒す条件(プレイヤーの移動先の座標だったら)
                    Vector3 enemyPos = enemyList[phase][i].transform.GetChild(0).transform.position;
                    enemyPos = new Vector3(Mathf.Round(enemyPos.x), 0, Mathf.Round(enemyPos.z));

                    Vector3 playerPos = Player.instance.transform.position;
                    playerPos = new Vector3(Mathf.Round(playerPos.x), 0, Mathf.Round(playerPos.z));

                    if (enemyPos == Player.instance.newStepPos ||
                        enemyPos == Player.instance.oldStepPos) {
                        var krawler = enemyList[phase][i].GetComponent<Krawler>();
                        krawler.isDie = true;
                        krawler.destroyEffect.SetActive(true);
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public void ResetEnemy() {
        for (int i = 0; i < enemyList[Player.instance.phase].Count; i++) {
            enemyList[Player.instance.phase][i].GetComponent<Krawler>().isDie = false;
        }
    }

    public void Action() {
        GhostManager.instance.Action();
        for (int i = 0; i < enemyScriptList[Player.instance.phase].Count; i++) {
            enemyScriptList[Player.instance.phase][i].Action();
        }
    }

    public void GameStart() {
        startTimer += Time.deltaTime;
        player.SetActive(false);
        if (startTimer <= 0.5f) {
            AudioManeger.instance.Play("Start");
            //フェードイン、こちらでは何もしない
        } else if (startTimer <= 3f) {
            if (startObjRenderer == null) {
                startObjRenderer = startBlockList[0].transform.GetChild(1).GetChild(0).GetComponent<Renderer>();
                startObjRenderer.material.EnableKeyword("_EMISSION");
            }
            var colorChannel = Mathf.Pow(2, Mathf.Max(Mathf.Min(12 - startTimer * 6f, 7), 1));
            startObjRenderer.material.SetColor("_EmissionColor", new Color(colorChannel, colorChannel, colorChannel));
        } else if (startTimer <= 3.7f) {
            upSinema.anchoredPosition = new Vector2(0, 200 + (startTimer - 3f) * 100);
            downSinema.anchoredPosition = new Vector2(0, -200 - (startTimer - 3f) * 100);
        } else {
            nowMode = Mode.GAME;
            startTimer = 0;
            AudioManeger.instance.PlayBGM(visualMode);
        }
    }

    public void StageClear() {
        startTimer += Time.deltaTime;
        player.SetActive(false);
        if (startTimer <= 0.5f) {
            upSinema.anchoredPosition = new Vector2(0, 250 - startTimer * 100);
            downSinema.anchoredPosition = new Vector2(0, -250 + startTimer * 100);
        } else if (startTimer <= 2f) {
            if (goalObjRenderer == null) {
                goalObjRenderer = GetStageObject(Player.instance.newStepPos).transform.GetChild(1).GetChild(0).GetComponent<Renderer>();
                goalObjRenderer.material.EnableKeyword("_EMISSION");
            }
            var colorChannel = Mathf.Pow(2, Mathf.Max(2 + startTimer * 5f, 1));
            goalObjRenderer.material.SetColor("_EmissionColor", new Color(colorChannel, colorChannel, colorChannel));
        } else if (startTimer <= 4f) {
            var colorChannel = Mathf.Pow(2, Mathf.Max(12 - (startTimer - 2) * 5f, 1));
            goalObjRenderer.material.SetColor("_EmissionColor", new Color(colorChannel, colorChannel, colorChannel));
        } else if (startTimer <= 5f) {
            if (isSetClearFadeOut == false) {
                var fadeOut = fade.AddComponent<FadeOut>();
                fadeOut.nextStagePath = "StageSelect";
                isSetClearFadeOut = true;
            }
        } else {
            if (StageSelect.isStageSelect == true) {
                if(PlayerPrefs.GetInt("ClearIndex", 0) <= StageSelect.playingIndex) {
                    PlayerPrefs.SetInt("ClearIndex", StageSelect.playingIndex + 1);
                }
                PlayerPrefs.SetInt("IsUnlock", 1);
            }
            AudioManeger.instance.Play("SceneChange");
        }
    }
}
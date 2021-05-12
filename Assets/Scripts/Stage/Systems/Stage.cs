using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Text;

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
    private GameObject deathUI;

    [Disable]
    [SerializeField]
    private GameObject clearUI;

    [Disable]
    [SerializeField]
    private GameObject fade;

    [Disable]
    public GameObject lastMomentFilter; //制限時間が残りギリギリですよー

    [SerializeField]
    private GameObject[] particles;

    [HideInInspector]
    [SerializeField]
    public int turnMax;

    [SerializeField]
    private GameObject pauseWindow;
    #endregion


    #region データ部
    public GameObject stageParent { get; private set; }
    public List<List<char>> stageData { get; private set; }
    public Vector3 startPosition { get; private set; }
    public Vector3 goalPosition { get; private set; } = Vector3.one * -1;

    public List<Vector3> checkPoints { get; private set; } = new List<Vector3>();
    public List<int> maxTimes { get; private set; } = new List<int>();
    public List<int> maxLoop { get; private set; } = new List<int>();
    public List<GameObject> startBlockList { get; private set; } = new List<GameObject>(7);
    public List<List<GameObject>> enemyList { get; private set; } = new List<List<GameObject>>(7);
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

    BezierCurve bezie;
    float bezieTime;
    int bezieIndex = 1; //今目指している点

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
       // startBlockList.Capacity = 8;
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
        Instantiate(particles[visualMode], camera.transform);
        ghostManager = Instantiate(ghostManager);

        if (StageSelect.isStageSelect == true) {
            stagePath = StageSelect.stagePath;
        }



        //ステージの生成
        if (ReadCSV(stagePath) == false) {
            Debug.LogError("ステージの読み込みで不具合が発生したため終了しました");
            SystemSupporter.ExitGame();
        }

        pauseWindow = Instantiate(pauseWindow);

        //ーーーーーーーーーーー以下、開始時演出の処理ーーーーーーーーーー

        //チェックポイントをソートしてカメラ順にする
        checkPoints.Sort((a, b) => SortCehckpoint(a, b));
        checkPoints.Add(goalPosition);
        Vector3 centerPos = Vector3.zero;
        Vector3 endPos = Vector3.zero;

        if (checkPoints.Count == 1) { //startとゴールのみ
            centerPos = goalPosition / 2;
            endPos = goalPosition;
        } else if (checkPoints.Count >= 2) { //中間一個以上
            centerPos = checkPoints[0];
            endPos = checkPoints[1];
        }
        bezie = new BezierCurve(Vector3.zero, centerPos, endPos);
    }

    void Update() {


        SystemSupporter.PlaySupport();
        switch (nowMode) {
            case Mode.START: // スタート時の演出
                nowMode = Mode.GAME;
                player.SetActive(false);
                camera.transform.position = new Vector3(bezie.GetPoint(bezieTime).x, 10, bezie.GetPoint(bezieTime).z - 2);
                if (bezieIndex < checkPoints.Count) {
                    if (bezieIndex == checkPoints.Count - 1) {
                        if (bezieTime >= 1.0f) {
                            nowMode = Mode.GAME;
                        } else if (bezieTime >= 0.85f) {
                            bezieTime += Time.deltaTime * 0.1f; //減速
                        } else {
                            bezieTime += Time.deltaTime * 0.3f;
                        }
                    } else {
                        if (bezieTime >= 0.5f) {
                            bezieTime = 0.0f;
                            bezieIndex++;
                            bezie.SetPoint(camera.transform.position + Vector3.forward * 2, checkPoints[bezieIndex - 1], checkPoints[bezieIndex]);
                        }
                        bezieTime += Time.deltaTime * 0.3f;
                    }
                }
                if (goalPosition == Vector3.one * -1) {
                    nowMode = Mode.GAME;
                }
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
                player.SetActive(false);
                deathUI.SetActive(true);
                break;
            case Mode.CLEAR:
                player.SetActive(false);
                clearUI.SetActive(true);
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
                    if (_code == 'Y') {
                        checkPoints.Add(new Vector3(posX + i, 0, posY - lineCount));
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
                                    if (startBlockList[count - 1] != null) {
                                        Debug.LogError("同じフェイズのスタートを検知しました。\n旧座標：" + startBlockList[count - 1].transform.position + "\n新座標：" + detailBase.transform.position);
                                    }
                                    enemyList[phase - 1].Add(detailBase.gameObject);
                                }
                                tmp >>= 1;
                                count++;
                            }
                            
                        }

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

                        //スタートブロックを取得
                        if (code == 'I') {
                            //startBlockList.Add(obj);
                        }



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

    private static void OnPlayModeStateChanged(PlayModeStateChange state) {
#if UNITY_EDITOR
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


    public static char GetObjectCode(string name) {
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
                if (enemyList[phase][i].transform.GetChild(0).gameObject.activeSelf == true) {

                    //倒す条件(プレイヤーの移動先の座標だったら)
                    if (enemyList[phase][i].transform.position == Player.instance.newStepPos) {
                        enemyList[phase][i].GetComponent<Krawler>().isDie = true;
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public void ResetEnemy() {
        for(int i=0; i < enemyList[Player.instance.phase].Count; i++) {
            enemyList[Player.instance.phase][i].GetComponent<Krawler>().isDie = false;
        }
    }

    public void Action() {
        GhostManager.instance.Action();
        // Krawler.instance.Action();
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Text;
using UnityEngine.SceneManagement;

/// <summary>
/// �������ƃ��[�h�A�X�e�[�W�����̃N���X
/// </summary>
[DisallowMultipleComponent]
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class Stage : MonoBehaviour {



    #region �C���X�y�N�^�ҏW��
    public static Stage instance;

    [Tooltip("�ҏW���[�h")]
    public bool isEditorMode = false;

    [Tooltip("�I�v�V�������[�h")]
    public bool isOptionMode = false;

    [Tooltip("Resources/StageDatas�ȉ��̃p�X�ł�")]
    public string stagePath;

    public List<char> objectIndex;
    public List<GameObject> objectList;
    #endregion

    [Header("-------------")]


    #region �C���X�y�N�^�Q�ƕ�(������Ȃ��悤��Disable����)
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

    #region �f�[�^��
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

        //�X�e�[�W�̐���
        if (ReadCSV(stagePath) == false) {
            Debug.LogError("�X�e�[�W�̓ǂݍ��݂ŕs��������������ߏI�����܂���");
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
            case Mode.START: // �X�^�[�g���̉��o
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
    /// �X�e�[�W��CSV��ǂݍ��݂܂�
    /// </summary>
    /// <param name="path">�t�@�C���̃p�X</param>
    bool ReadCSV(string path) {
        if (path == "") {
            Debug.LogError("Stage�̃t�@�C���p�X��null�ł���");
            return false;
        }

        try {
            TextAsset csv = Resources.Load("StageDatas/" + path) as TextAsset;
            StringReader reader = new StringReader(csv.text);

            reader.ReadLine(); // �X�e�[�W�S�̂̃w�b�_�ǂݍ���
            var line = reader.ReadLine();

            //�X�e�[�W�z���������
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

            //objectHeader�ɓ�����܂ŃR�����g�ǂݍ���
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

            //�X�e�[�W�����ǂݍ��݂Ɛ���
            line = reader.ReadLine();
            var lineCount = 0;
            while (line != detailHeader && reader.Peek() > -1) {
                //line�����Ŋ֐����o������
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
                if (reader.Peek() <= -1) { //Detail�I����
                    Debug.Log("Detail�Ȃ��ŏI�����܂���");
                    return true;
                }
                line = reader.ReadLine();
                lineCount++;
            }

            //�ڍדǂݍ���
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
                                Debug.LogError("�X�^�[�g�̃t�F�[�Y��0�ł����B1�ȏ�ɏC�����Ă�������\n���W�F" + detailBase.gameObject.transform.position);
                            }
                            var tmp = phase;
                            int count = 1;
                            while (tmp > 0) {
                                if ((tmp & 1) > 0) {
                                    if (startBlockList[count - 1] != null) {
                                        Debug.LogError("�����t�F�C�Y�̃X�^�[�g�����m���܂����B\n�����W�F" + startBlockList[count - 1].transform.position + "\n�V���W�F" + detailBase.transform.position);
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
                                Debug.LogError("�X�^�[�g�̃t�F�[�Y��0�ł����B1�ȏ�ɏC�����Ă�������\n���W�F" + detailBase.gameObject.transform.position);
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
            Debug.Log("�t�@�C����������܂���ł����B�V�K�쐬���[�h�ɂ��܂�");
            Debug.Log(e);
            StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Resources/StageDatas/" + stagePath + ".txt", false);
            streamWriter.Flush();
            streamWriter.Close();

            //�ŏ��̃G���A
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
    /// �R���X�g���N�^(InitializeOnLoad�����ɂ��G�f�B�^�[�N�����ɌĂяo�����)
    /// </summary>
    static Stage() {
        //Playmode�̏�Ԃ��ς�������̃C�x���g��o�^
#if UNITY_EDITOR

        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

#if UNITY_EDITOR
    private static void OnPlayModeStateChanged(PlayModeStateChange state) {
        //�Đ��I���{�^����������
        if (instance != null && !EditorApplication.isPlayingOrWillChangePlaymode) {
            instance.WriteCSV();
        }
        try {
        } catch (Exception e) {
            Debug.LogError("�I�����ɃG���[���������܂���" + e);
        }
    }
#endif

    void WriteCSV() {

        float maxLeft = 0;
        float maxRight = 0;
        float maxUp = 0;
        float maxDown = 0;

        //�}�b�v�쐬�B�S�ẴI�u�W�F�N�g�𒲂ׁA�S�ẴI�u�W�F�N�g������List<List<char>>�����
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

        //stageData��������
        stageData = new List<List<char>>();
        for (int y = 0; y < sizeY; y++) {
            stageData.Add(new List<char>());
            for (int x = 0; x < sizeX; x++) {
                stageData[y].Add('0');
            }
        }

        //�t�@�C�����J���ăo�b�t�@����
        StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Resources/StageDatas/" + stagePath + ".txt", false);
        StringBuilder sb = new StringBuilder();
        StringBuilder detailSb = new StringBuilder(); //�ڍאݒ�pStringBuilder

        //�X�e�[�W�w�b�_�[�ɏ�������
        sb.AppendLine(stageHeader);
        sb.AppendLine("size:" + sizeX.ToString() + "," + sizeY.ToString());
        sb.AppendLine("design:" + visualMode.ToString()); //�X�e�[�W�f�U�C��
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

        //�ڍ׃w�b�_�[�ɏ�������
        detailSb.AppendLine("#Detail");

        //stageData�Ɍ��݂̏󋵂𔽉f
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

        //�o�b�t�@�ɃX�e�[�W�f�[�^����������ł���
        for (int y = 0; y < stageData.Count; y++) {
            for (int x = 0; x < stageData[y].Count; x++) {
                var code = stageData[y][x];
                sb.Append(code);
            }
            sb.Append("\n");

        }

        //��������
        streamWriter.WriteLine(sb.ToString());
        if (detailSb.Length > 10) { //�����������܂�Ă�����
            streamWriter.Write(detailSb.ToString());
        }

        //�t�@�C�������
        streamWriter.Flush();
        streamWriter.Close();
    }

    public void GenerateObject(Vector3 pos, GameObject obj) {
        if (pos == Vector3.zero) {
            //�X�^�[�g�G���A�Ȃ̂ŃX�L�b�v
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
    /// ���̍��W�̃M�~�b�N�I�u�W�F�N�g���擾���܂�
    /// </summary>
    /// <param name="pos">���W</param>
    /// <returns>���̍��W�̃X�e�[�W�I�u�W�F�N�g</returns>
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
        //�����K���́u�R�[�h_���O�v�Ȃ̂ŃR�[�h�P�Ƃ̒��o�\
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

                //�\������Ă���G�����|����
                if (enemyScriptList[phase][i].isDie == false) {

                    //�|������(�v���C���[�̈ړ���̍��W��������)
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
            //�t�F�[�h�C���A������ł͉������Ȃ�
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
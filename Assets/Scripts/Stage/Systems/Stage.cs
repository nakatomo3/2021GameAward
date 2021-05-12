using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Text;

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
    private GameObject deathUI;

    [Disable]
    [SerializeField]
    private GameObject clearUI;

    [Disable]
    [SerializeField]
    private GameObject fade;

    [Disable]
    public GameObject lastMomentFilter; //�������Ԃ��c��M���M���ł���[

    [SerializeField]
    private GameObject[] particles;

    [HideInInspector]
    [SerializeField]
    public int turnMax;

    [SerializeField]
    private GameObject pauseWindow;
    #endregion


    #region �f�[�^��
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
    int bezieIndex = 1; //���ڎw���Ă���_

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



        //�X�e�[�W�̐���
        if (ReadCSV(stagePath) == false) {
            Debug.LogError("�X�e�[�W�̓ǂݍ��݂ŕs��������������ߏI�����܂���");
            SystemSupporter.ExitGame();
        }

        pauseWindow = Instantiate(pauseWindow);

        //�[�[�[�[�[�[�[�[�[�[�[�ȉ��A�J�n�����o�̏����[�[�[�[�[�[�[�[�[�[

        //�`�F�b�N�|�C���g���\�[�g���ăJ�������ɂ���
        checkPoints.Sort((a, b) => SortCehckpoint(a, b));
        checkPoints.Add(goalPosition);
        Vector3 centerPos = Vector3.zero;
        Vector3 endPos = Vector3.zero;

        if (checkPoints.Count == 1) { //start�ƃS�[���̂�
            centerPos = goalPosition / 2;
            endPos = goalPosition;
        } else if (checkPoints.Count >= 2) { //���Ԉ�ȏ�
            centerPos = checkPoints[0];
            endPos = checkPoints[1];
        }
        bezie = new BezierCurve(Vector3.zero, centerPos, endPos);
    }

    void Update() {


        SystemSupporter.PlaySupport();
        switch (nowMode) {
            case Mode.START: // �X�^�[�g���̉��o
                nowMode = Mode.GAME;
                player.SetActive(false);
                camera.transform.position = new Vector3(bezie.GetPoint(bezieTime).x, 10, bezie.GetPoint(bezieTime).z - 2);
                if (bezieIndex < checkPoints.Count) {
                    if (bezieIndex == checkPoints.Count - 1) {
                        if (bezieTime >= 1.0f) {
                            nowMode = Mode.GAME;
                        } else if (bezieTime >= 0.85f) {
                            bezieTime += Time.deltaTime * 0.1f; //����
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
                    if (_code == 'Y') {
                        checkPoints.Add(new Vector3(posX + i, 0, posY - lineCount));
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
                                    if (startBlockList[count - 1] != null) {
                                        Debug.LogError("�����t�F�C�Y�̃X�^�[�g�����m���܂����B\n�����W�F" + startBlockList[count - 1].transform.position + "\n�V���W�F" + detailBase.transform.position);
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

                        //�X�^�[�g�u���b�N���擾
                        if (code == 'I') {
                            //startBlockList.Add(obj);
                        }



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

    private static void OnPlayModeStateChanged(PlayModeStateChange state) {
#if UNITY_EDITOR
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


    public static char GetObjectCode(string name) {
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
                if (enemyList[phase][i].transform.GetChild(0).gameObject.activeSelf == true) {

                    //�|������(�v���C���[�̈ړ���̍��W��������)
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

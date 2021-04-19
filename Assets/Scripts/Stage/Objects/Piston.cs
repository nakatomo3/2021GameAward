using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : MonoBehaviour {

    [SerializeField]
    private Transform piston;

    private float pistonTimer;
    private float _pistonInterval;
    public float pistonInterval { // ピストン間隔
        get { return _pistonInterval; }
        set {
            _pistonInterval = Mathf.Round(value * 10) / 10;
            if (value < 0.1f) {
                _pistonInterval = 0.1f;
            }
            if (value > 10) {
                _pistonInterval = 10;
            }
        }
    }
    private float _delay;
    public float deley { // 遅延
        get { return _delay; }
        set {
            _delay = Mathf.Round(value * 10) / 10;
            if (value < 0.0f) {
                _delay = 0.0f;
            }
            if (value > 10) {
                _delay = 10;
            }
        }
    }
    public enum Direction { // 向き
        Front,
        Back,
        Right,
        Left
    }
    public Direction direction;

    private float returnTimer;
    private float returnInterval = 1.0f;
    private Vector3 targetPosition;
    private Vector3 addForce;
    [SerializeField]
    public bool isPush { get; private set; }

    // Start is called before the first frame update
    void Start() {
        isPush = false;
        pistonTimer = -deley;
        if (pistonInterval == 0) {
            pistonInterval = 5.0f;
        }
    }

    // Update is called once per frame
    private void FixedUpdate() {
        if (isPush == true) {
            piston.transform.localPosition = new Vector3(1.0f, 0, 0);
        } else {
            piston.transform.localPosition = new Vector3(0.1f, 0, 0);
        }

        switch (direction) {
            case Direction.Front:
                targetPosition = transform.position + new Vector3(0, 0, -1);
                addForce = new Vector3(0, 0, -1);
                transform.eulerAngles = new Vector3(0, 90, 0);
                break;
            case Direction.Back:
                targetPosition = transform.position + new Vector3(0, 0, 1);
                addForce = new Vector3(0, 0, 1);
                transform.eulerAngles = new Vector3(0, 270, 0);
                break;
            case Direction.Right:
                targetPosition = transform.position + new Vector3(1, 0, 0);
                addForce = new Vector3(1, 0, 0);
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case Direction.Left:
                targetPosition = transform.position + new Vector3(-1, 0, 0);
                addForce = new Vector3(-1, 0, 0);
                transform.eulerAngles = new Vector3(0, 180, 0);
                break;
        }
        pistonTimer += Time.deltaTime;

        if (pistonTimer >= pistonInterval) {

            if (Player.instance.transform.position == targetPosition && isPush == false) {
                Player.instance.newStepPos += addForce; // プレイヤーを押し出す
                if (Stage.instance.GetStageObject(transform.position + addForce * 2) == null) { // 押し出した先が奈落なら死
                    Player.instance.Fall();
                }
            }
            isPush = true; // 射出
            returnTimer += Time.deltaTime;
            if (returnTimer >= returnInterval) {
                pistonTimer = 0;
                returnTimer = 0;
                isPush = false;
            }
        }
    }
}

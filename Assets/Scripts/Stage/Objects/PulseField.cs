using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseField : MonoBehaviour {

    [SerializeField]
    private Renderer render;

    private float pulseTimer;
    public float modeInterval = 2.0f; // ���[�h�ؑ֊Ԋu
    public float delay = 0;           // �x��
    private bool isPulse = false;

    private bool isDamage = true;
    private float timer = 0.0f;
    private float damageInterval = 1.0f;

    // Start is called before the first frame update
    void Start() {
        render = GetComponentInChildren<Renderer>();
        pulseTimer = -delay;
    }

    // Update is called once per frame
    void Update() {
        pulseTimer += Time.deltaTime;

        if (pulseTimer >= modeInterval) {
            pulseTimer = 0;
            isPulse = !isPulse; // ���[�h��؂�ւ���
        }

        if (isPulse == true) { // �p���XON�̎��̓_���[�W�u���b�N�Ɠ����̏���
            Color color = render.material.color;
            color.a = 1.0f;
            render.material.color = color;
            if (transform.position == Player.instance.transform.position) {
                if (isDamage == false) {
                    timer += Time.deltaTime;
                    if (timer >= damageInterval) { // �C���^�[�o���J�E���g
                        timer = 0.0f;
                        isDamage = true;
                        color = render.material.color;
                        color.g = 1.0f;
                        render.material.color = color;
                    }
                } else {
                    Player.instance.Damage(1); // �_���[�W����
                    isDamage = false;
                    color = render.material.color;
                    color.g = 0.0f;
                    render.material.color = color;
                }
            } else { // �v���C���[�����ꂽ�烊�Z�b�g
                timer = 0.0f;
                isDamage = true;
                color = render.material.color;
                color.g = 1.0f;
                render.material.color = color;
            }
        } else {
            Color color = render.material.color;
            color.a = 0.2f;
            render.material.color = color;
        }
    }
}

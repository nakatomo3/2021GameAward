using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailBase : MonoBehaviour {
	//�I�u�W�F�N�g�����t�@�C���������ݗp��string�ɕϊ�
	public virtual string ToFileString() { return ""; }

	//�I�u�W�F�N�g�����X�e�[�W�G�f�B�^�p��string�ɕϊ�
	public virtual string ToEditorString() { return ""; }
}

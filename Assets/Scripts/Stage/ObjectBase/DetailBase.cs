using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DetailBase : MonoBehaviour {
	//�I�u�W�F�N�g�����t�@�C���������ݗp��string�ɕϊ�
	public abstract string ToFileString();

	//�I�u�W�F�N�g�����X�e�[�W�G�f�B�^�p��string�ɕϊ�
	public abstract string ToEditorString();
}

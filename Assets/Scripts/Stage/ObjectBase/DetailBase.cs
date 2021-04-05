using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DetailBase : MonoBehaviour {

	/// <summary>
	/// �I�u�W�F�N�g�����t�@�C���������ݗp��string�ɕϊ�
	/// </summary>
	/// <returns>�t�@�C���������ݗp��string</returns>
	public abstract string ToFileString();

	/// <summary>
	/// �I�u�W�F�N�g�����X�e�[�W�G�f�B�^�p��string�ɕϊ�
	/// </summary>
	/// <returns>�X�e�[�W�G�f�B�^�p��string</returns>
	public abstract string ToEditorString();

	/// <summary>
	/// �I�u�W�F�N�g����string���I�u�W�F�N�g�̕ϐ��ɔ��f
	/// </summary>
	/// <param name="information">�p�����[�^�[�B�e�s���ƂɘA�����Ă���</param>
	public abstract void SetData(string information);

	protected string ConvertPos() {
		return Mathf.CeilToInt(transform.position.x) + "," + Mathf.CeilToInt(transform.position.z);
	}
}

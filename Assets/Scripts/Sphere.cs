using IroSphere;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static IroSphere.SphereManager;
using System.IO;

public class Sphere
{
	public GameObject Root { get; private set; }
	public GameObject initNodesParent { get; private set; }
	public GameObject additiveNodesParent { get; private set; }

	SphereManager manager;

	public Vector3 moveTargetPos { get; set; }
	Vector3 velocity;

	float SeparateMoveSpeed = 0.2f;

	public List<GameObject> InitNodes { get; private set; } = new List<GameObject>();
	public List<GameObject> AdditiveNodes { get; private set; } = new List<GameObject>();

	GameObject PreviewNode;
	Material previewMaterial;
	GameObject grid;

	public Sphere(Transform parent, int i, SphereManager sphereManager)
	{
		i++;
		this.manager = sphereManager;
		this.Root = new GameObject("Sphere" + i);
		this.Root.transform.parent = parent;
		this.Root.transform.localPosition = Vector3.zero;
		this.Root.transform.localRotation = Quaternion.identity; //Quaternion.AngleAxis(30.0f, Vector3.right);
		this.Root.transform.localScale = Vector3.one;

		initNodesParent = new GameObject("InitNodes" + i);
		initNodesParent.transform.parent = this.Root.transform;
		initNodesParent.transform.localPosition = Vector3.zero;
		initNodesParent.transform.localRotation = Quaternion.identity;
		initNodesParent.transform.localScale = Vector3.one;

		additiveNodesParent = new GameObject("AdditiveNodes" + i);
		additiveNodesParent.transform.parent = this.Root.transform;
		additiveNodesParent.transform.localPosition = Vector3.zero;
		additiveNodesParent.transform.localRotation = Quaternion.identity;
		additiveNodesParent.transform.localScale = Vector3.one;
		this.manager = sphereManager;

		grid = GameObject.Instantiate(sphereManager.Grid, Root.transform);
		grid.SetActive(true);
	}
	public void SetRotation(Quaternion rotation)
	{
		Root.transform.localRotation = rotation;
	}

	public void UpdateMove()
	{
		Root.transform.localPosition = Vector3.SmoothDamp(Root.transform.localPosition, moveTargetPos, ref velocity, SeparateMoveSpeed, float.MaxValue, Time.deltaTime);
	}



	/// <summary>
	/// �m�[�h�̍쐬
	/// </summary>
	/// <param name="position"></param>
	/// <param name="size"></param>
	/// <param name="nodeType"></param>
	/// <returns></returns>
	public GameObject CreateNode(Vector3 position, float size, NodeType nodeType, ShapeType shapeType, Material material)
	{
		var primitiveType = shapeType == ShapeType.SPHERE ? PrimitiveType.Sphere : PrimitiveType.Cube;
		GameObject nodeObj = GameObject.CreatePrimitive(primitiveType);

		nodeObj.GetComponent<MeshRenderer>().material = material;
		HSL hsl = HSL.PositionToHSL(position);
		Color color = hsl.ToRgb();

		var m = nodeObj.GetComponent<MeshRenderer>().material;
		m.color = color;

		switch (nodeType)
		{
			case NodeType.INIT:
				nodeObj.name = "Init" + nodeObj.name;
				nodeObj.transform.parent = initNodesParent.transform;
				InitNodes.Add(nodeObj);
				break;
			case NodeType.ADDITIVE:
				nodeObj.name = "Additive" + nodeObj.name;
				nodeObj.transform.parent = additiveNodesParent.transform;
				AdditiveNodes.Add(nodeObj);
				break;
			case NodeType.PREVIEW:
				nodeObj.name = "Preview" + nodeObj.name;
				nodeObj.transform.parent = Root.transform;
				PreviewNode = nodeObj;
				break;
		}

		nodeObj.transform.localPosition = position;

		nodeObj.transform.localRotation = PositionToRotation(position);
		nodeObj.transform.localScale = Vector3.one * size;


		return nodeObj;
	}

	public void CreatePreviewNode()
	{
		//����������u�Ԍ������Ⴄ�̂ŁA��U�J�����̌��ɉB�����ʒu�ɐ������Ă��܂�
		Transform cameraTrs = Camera.main.transform;
		GameObject obj = CreateNode(cameraTrs.position - cameraTrs.forward, manager.PreviewNodeSize, NodeType.PREVIEW, manager.PreviewNodeShapeType, manager.PreviewMaterial);
		previewMaterial = obj.GetComponent<Renderer>().material;
	}

	Quaternion PositionToRotation(Vector3 position)
	{
		//��]����B���S�c����LookRotation�o���Ȃ��̂ŏ��O
		Vector3 p = position;
		p.y = 0.0f;
		if (p.sqrMagnitude > float.Epsilon)
			return Quaternion.LookRotation(Vector3.zero - position, Vector3.up);
		return Quaternion.identity;
	}

	/// <summary>
	/// �ǉ�����m�[�h�쐬
	/// </summary>
	/// <returns>�m�[�h�쐬�����������ǂ���</returns>
	public bool CreateAdditiveNode()
	{
		if (manager.MaxAdditiveNodeNum <= AdditiveNodes.Count)
			return false;
		CreateNode(PreviewNode.transform.localPosition, manager.AdditiveNodeSize, NodeType.ADDITIVE, manager.AdditiveShapeType,manager.Material);
		return true;
	}

	/// <summary>
	/// �v���r���[�m�[�h�X�V
	/// </summary>
	/// <param name="color"></param>
	/// <param name="isInImage"></param>
	public void UpdatePreviewNode(Color color, bool isInImage)
	{

		if (!isInImage)
		{
			//�摜�̊O�Ƀ}�E�X�J�[�\�������鎞�́A�T�C�Y��0�ɂ��ĉB��
			PreviewNode.transform.localScale = Vector3.zero;
		}
		else
		{
			previewMaterial.color = color;
			HSL hsl = HSL.RGBToHSL(color);
			PreviewNode.transform.localPosition = hsl.ToPosition();
			PreviewNode.transform.localRotation = PositionToRotation(PreviewNode.transform.localPosition);
			PreviewNode.transform.localScale = Vector3.one * manager.PreviewNodeSize;

		}
	}
	


	public void DeletePreviewNode()
	{
		GameObject.Destroy(PreviewNode);
		PreviewNode = null;
		previewMaterial = null;
	}

	/// <summary>
	/// Undo�@�Ō�ɍ�����m�[�h���珇�Ԃɍ폜
	/// </summary>
	public void UndoAdditiveNode()
	{
		if (AdditiveNodes.Count == 0)
			return;
		GameObject.Destroy(AdditiveNodes[AdditiveNodes.Count - 1]);
		AdditiveNodes.RemoveAt(AdditiveNodes.Count - 1);
	}

	public void ClearAllAdditiveNode()
	{
		foreach(GameObject g in AdditiveNodes)
		{
			GameObject.Destroy(g);
		}
		AdditiveNodes.Clear();
	}


	public void SetAllAdditiveNodeSize(float size)
	{
		if (AdditiveNodes == null ||  AdditiveNodes.Count == 0)
			return;

		for (int i = 0; i < AdditiveNodes.Count; i++)
		{
			AdditiveNodes[i].transform.localScale = Vector3.one * size;
		}
	}
	public void ShowGrid()
	{
		grid.SetActive(!grid.activeSelf);
	}

	/// <summary>
	/// ���ݕ\�����̃m�[�h���t�@�C���ɕۑ�
	/// </summary>
	public void Save()
	{
		if (AdditiveNodes.Count == 0)
			return;

		//�t�@�C���p�X����
		var path = "Assets/SaveData/";

		DateTime dt = DateTime.Now;
		string now = dt.Year.ToString("d4") + dt.Month.ToString("d2") + dt.Day.ToString("d2") + dt.Hour.ToString("d2") + dt.Minute.ToString("d2") + dt.Second.ToString("d2");
		var fileName = manager.Picture.name + "_" + now;
		if (!Directory.Exists(path))
			Directory.CreateDirectory(path);

		//ScriptableObject�쐬
		var saveData = ScriptableObject.CreateInstance<SaveData>();

		//CSV�f�[�^�쐬
		StreamWriter csvSw;
		FileInfo csvFI = new FileInfo(path + fileName + ".csv");
		csvSw = csvFI.AppendText();
		csvSw.WriteLine("X,Y,Z,H,S,L,R,G,B");

		Vector3[] positions = new Vector3[AdditiveNodes.Count];
		for(int i = 0; i < positions.Length; i++)
		{
			positions[i] = AdditiveNodes[i].transform.localPosition;

			HSL hsl = HSL.PositionToHSL(positions[i]);
			Color rgb = hsl.ToRgb();
			csvSw.WriteLine(positions[i].x + "," + positions[i].y + "," + positions[i].z+","+
				hsl.h + "," + hsl.s + "," + hsl.l + "," +
				rgb.r + "," + rgb.g + "," + rgb.b);


		}

		//ScriptableObject�ۑ�
		saveData.Position = positions;
		AssetDatabase.CreateAsset(saveData, path + fileName + ".asset");

		//CSV�ۑ�
		csvSw.Flush();
		csvSw.Close();

	}
}
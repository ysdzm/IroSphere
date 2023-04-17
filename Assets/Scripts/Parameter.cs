using IroSphere;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using static IroSphere.SphereManager;

namespace IroSphere
{

	[CreateAssetMenu(menuName = "IroSphere/Parameter", fileName = "Parameter")]
	public class Parameter : ScriptableObject
	{


		[Header("スフィアの回転、移動、拡縮性能")]
		[SerializeField, Tooltip("角加速度")]
		float rotateSpeed = 200.0f;
		public float RotateSpeed { get { return rotateSpeed; } set { rotateSpeed = value; } }

		[SerializeField, Tooltip("移動速度")]
		float moveSpeed = 2.0f;
		public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }

		[SerializeField, Tooltip("拡縮速度")]
		float scaleSpeed = 2.0f;
		public float ScaleSpeed { get { return scaleSpeed; } set { scaleSpeed = value; } }

		[Header("ノード形状タイプ")]
		[SerializeField, Tooltip("形状タイプ")]
		ShapeType shapeType = ShapeType.SPHERE;
		public ShapeType ShapeType { get { return shapeType; } set { shapeType = value; } }

		[Header("初期配置ノードの個数（▲ゲーム実行中変更不可▲）")]

		[SerializeField, DisableEditOnPlay, Range(0, 32), Tooltip("色相方向のノードの数。動作が重たいと感じたらここを下げて下さい")]
		int initNodeNumH = 21;
		public int InitNodeNumH { get { return initNodeNumH; } set { initNodeNumH = value; } }


		[SerializeField, DisableEditOnPlay, Range(0, 10), Tooltip("彩度方向のノードの数。動作が重たいと感じたらここを下げて下さい")]
		int initNodeNumS = 7;
		public int InitNodeNumS { get { return initNodeNumS; } set { initNodeNumS = value; } }

		[SerializeField, DisableEditOnPlay, Range(0, 19), Tooltip("明度方向のノードの数。動作が重たいと感じたらここを下げて下さい")]
		int initNodeNumL = 15;
		public int InitNodeNumL { get { return initNodeNumL; } set { initNodeNumL= value; } }

		[Header("初期配置のノードのサイズ")]

		[SerializeField, Range(0.0f, 1.0f), Tooltip("初期配置ノードのサイズ")]
		float initNodeSize = 0.02f;
		public float InitNodeSize { get { return initNodeSize; } set { initNodeSize = value; } }

		[SerializeField, Range(0.0f, 1.0f), Tooltip("スフィアの中心方向に行くに従って小さくするかどうか")]
		float initNodeCenterSmall = 1;
		public float InitNodeCenterSmall { get { return initNodeCenterSmall; } set { initNodeCenterSmall = value; } }

		[Header("プレビュー用ノードのサイズ")]

		[SerializeField, Range(0.0f, 1.0f), Tooltip("プレビューノードのサイズ")]
		float previewNodeSize = 0.4f;
		public float PreviewNodeSize => previewNodeSize;


		[Header("クリックで追加するノードのサイズ")]

		[SerializeField, Range(0.0f, 1.0f), Tooltip("クリックして置ける球のサイズ")]
		float additiveNodeSize = 0.2f;
		public float AdditiveNodeSize => additiveNodeSize;

		[Header("クリックで追加するノードの最大数（▲ゲーム実行中変更不可▲）")]
		[SerializeField, DisableEditOnPlay, Range(1, 1000), Tooltip("クリックして置ける球の最大数")]
		int maxAdditiveNodeNum = 200;
		public int MaxAdditiveNodeNum => maxAdditiveNodeNum;



		public SphereManager manager { get; set; }


#if UNITY_EDITOR
		private void OnValidate()
		{
			if (!EditorApplication.isPlaying || manager == null)
				return;

			manager.ChangeNodeSize();
		}
#endif

	}
}
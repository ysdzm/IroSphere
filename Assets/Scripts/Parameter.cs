using IroSphere;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static IroSphere.SphereManager;

namespace IroSphere
{
	[CreateAssetMenu(menuName = "IroSphere/Parameter", fileName = "Parameter")]
	public class Parameter : ScriptableObject
	{

		[Header("カラーピックしたい画像をここへ")]
		[SerializeField, Tooltip("画像の設定をSprite(2D and UI)に変更するのをお忘れなく！")]
		Sprite picture;
		public Sprite Picture => picture;

		[Header("スフィアの回転、移動、拡縮性能")]
		[SerializeField, Tooltip("角加速度")]
		float rotateSpeed = 200.0f;
		public float RotateSpeed => rotateSpeed;
		
		[SerializeField, Tooltip("角速度ブレーキング性能")]
		float rotateBrake = 3.0f;
		public float RotateBrake => rotateBrake;
		
		[SerializeField, Tooltip("移動速度")]
		float moveSpeed = 2.0f;
		public float MoveSpeed => moveSpeed;

		[SerializeField, Tooltip("拡縮速度")]
		float scaleSpeed = 2.0f;
		public float ScaleSpeed => scaleSpeed;

		[SerializeField, Tooltip("形状タイプ")]
		ShapeType shapeType = ShapeType.SPHERE;
		public ShapeType ShapeType => shapeType;

		[Header("初期配置のノードのパラメーター（実行中変更不可）")]

		[SerializeField, Range(1, 32), Tooltip("色相方向のノードの数。動作が重たいと感じたらここを下げて下さい")]
		int initNodeNumH = 21;
		public int InitNodeNumH => initNodeNumH;


		[SerializeField, Range(1, 10), Tooltip("彩度方向のノードの数。動作が重たいと感じたらここを下げて下さい")]
		int initNodeNumS = 7;
		public int InitNodeNumS => initNodeNumS;

		[SerializeField, Range(1, 19), Tooltip("明度方向のノードの数。動作が重たいと感じたらここを下げて下さい")]
		int initNodeNumL = 15;
		public int InitNodeNumL => initNodeNumL;

		[SerializeField, Range(0.0f, 1.0f), Tooltip("中心方向に行くに従って小さくするかどうか")]
		float initNodeCenterSmall = 1;
		public float InitNodeCenterSmall => initNodeCenterSmall;

		[SerializeField, Range(0.01f, 1.0f), Tooltip("初期配置ノードのサイズ")]
		float initNodeSize = 0.02f;
		public float InitNodeSize => initNodeSize;

		[Header("プレビュー用のノードのパラメーター")]

		[SerializeField, Range(0.01f, 1.0f), Tooltip("マウスカーソルあてた時のノードのサイズ。このパラメーターは実行中にリアルタイムに変更可能です")]
		float previewNodeSize = 0.4f;
		public float PreviewNodeSize => previewNodeSize;


		[Header("クリックで追加するノードのパラメーター")]

		[SerializeField, Range(0.01f, 1.0f), Tooltip("クリックして置ける球のサイズ。このパラメーターは実行中にリアルタイムに変更可能です")]
		float additiveNodeSize = 0.2f;
		public float AdditiveNodeSize => additiveNodeSize;

		[SerializeField, Range(1, 500), Tooltip("クリックして置ける球の最大数")]
		int maxAdditiveNodeNum = 200;
		public int MaxAdditiveNodeNum => maxAdditiveNodeNum;

		[Header("ロードしたいファイルをここに入れてLキー")]
		[SerializeField]
		SaveData saveData;
		public SaveData SaveData => saveData;

		public SphereManager manager { get; set; }

		private void OnValidate()
		{
			if (!EditorApplication.isPlaying || manager == null)
				return;

			manager.ChangeNodeSize();
		}

	}
}
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// マネージャークラス
/// ここであらゆる球の生成や、球の回転などを行っています
/// </summary>
/// 
namespace IroSphere
{
	public class SphereManager : MonoBehaviour
	{
		public enum NodeType { INIT, ADDITIVE ,PREVIEW}
		public enum ShapeType { SPHERE, BOX}

		[Header("カラーピックしたい画像をここへ")]
		[SerializeField, Tooltip("画像の設定をSprite(2D and UI)に変更するのをお忘れなく！")]
		Sprite picture;
		public Sprite Picture => picture;

		[Header("スフィアの回転、移動、拡縮性能")]
		[SerializeField, Tooltip("角加速度")]
		float rotateSpeed = 200.0f;
		[SerializeField, Tooltip("角速度ブレーキング性能")]
		float rotateBrake = 3.0f;
		[SerializeField, Tooltip("移動速度")]
		float moveSpeed = 2.0f;
		[SerializeField, Tooltip("拡縮速度")]
		float scaleSpeed = 2.0f;

		[SerializeField, Tooltip("形状タイプ")]
		ShapeType shapeType = ShapeType.SPHERE;
		public ShapeType Shape => shapeType;

		[Header("初期配置のノードのパラメーター（実行中変更不可）")]

		[SerializeField, Range(1, 32), Tooltip("色相方向のノードの数。動作が重たいと感じたらここを下げて下さい")]
		int initNodeNumH = 21;
		[SerializeField, Range(1, 10), Tooltip("彩度方向のノードの数。動作が重たいと感じたらここを下げて下さい")]
		int initNodeNumS = 7;
		[SerializeField, Range(1, 19), Tooltip("明度方向のノードの数。動作が重たいと感じたらここを下げて下さい")]
		int initNodeNumL = 15;
		[SerializeField, Range(0.0f, 1.0f), Tooltip("中心方向に行くに従って小さくするかどうか")]
		float initNodeCenterSmall = 1;
		[SerializeField, Tooltip("初期配置ノードのサイズ")]
		float initNodeSize = 0.02f;

		[Header("プレビュー用のノードのパラメーター")]
		[SerializeField, Tooltip("マウスカーソルあてた時のノードのサイズ。このパラメーターは実行中にリアルタイムに変更可能です")]
		float previewNodeSize = 0.4f;
		public float PreviewNodeSize => previewNodeSize;



		[Header("クリックで追加するノードのパラメーター")]

		[SerializeField, Tooltip("クリックして置ける球のサイズ。このパラメーターは実行中にリアルタイムに変更可能です")]
		float additiveNodeSize = 0.2f;
		public float AdditiveNodeSize => additiveNodeSize;

		[SerializeField, Range(1, 500), Tooltip("クリックして置ける球の最大数")]
		int maxAdditiveNodeNum = 200;
		public int MaxAdditiveNodeNum => maxAdditiveNodeNum;

		[Header("ロードしたいファイルをここに入れてLキー")]
		[SerializeField]
		SaveData saveData;

		[Header("▲ここから↓は触らないで下さい▲")]
		[SerializeField]
		Mesh sphereMesh;

		[SerializeField]
		Mesh cubeMesh;

		[SerializeField]
		GameObject imageObj;

		[SerializeField]
		Material material;
		public Material Material => material;

		[SerializeField]
		Material previewMaterial;
		public Material PreviewMaterial => previewMaterial;

		[SerializeField]
		GameObject grid;
		public GameObject Grid => grid;

		[SerializeField]
		Text AdditiveNumText;

		[SerializeField]
		GameObject infoObjText;
		[SerializeField]
		GameObject infoObjTextRGB;
		[SerializeField]
		GameObject infoObjBG;
		[SerializeField]
		GameObject infoObjR;
		[SerializeField]
		GameObject infoObjG;
		[SerializeField]
		GameObject infoObjB;

		RectTransform infoTextRect;
		Text infoText;
		RectTransform infoTextRectRGB;
		Text infoTextRGB;
		RectTransform infoRectBG;
		Image infoImageBG;
		RectTransform infoRectR;
		Image infoImageR;
		RectTransform infoRectG;
		Image infoImageG;
		RectTransform infoRectB;
		Image infoImageB;



		//2つ目の球の配置座標
		float secondarySpherePosition = 2.25f;
		//プレビュースフィアの透過率
		float previewSphereAlpha = 0.2f;

		//横画像の時の横のピクセル数
		float imageMaxSizeHorizontal = 800.0f;
		//縦画像の時の縦のピクセル数
		float imageMaxSizeVertical = 1000.0f;

		//移動限界。あまりに画面外に行き過ぎない様に
		Rect moveSphereLimit = new Rect(-4, -2, 6, 4);


		public GetColor getColor { get; set; }

		int currentSphereID;
		Sphere[] spheres = new Sphere[2];


		Vector2 rotate;
		Vector2 rotateVelocity;
		float scale = 1.0f;

		Vector3 pastMousePos;

		//インフォメーション系の情報
		bool enableInfo = false;
		bool pastEnableInfo = false;
		Vector2 offsetInfoR = new Vector2(130.0f, 195.0f);
		Vector2 offsetInfoG = new Vector2(130.0f, 162.0f);
		Vector2 offsetInfoB = new Vector2(130.0f, 130.0f);
		float infoBarSize = 230.0f;

		ShapeType pastShapeType;

		private void OnValidate()
		{
			if (!EditorApplication.isPlaying)
				return;

			foreach(Sphere s in spheres)
			{
				if (s == null)
					continue;
				s.SetAllAdditiveNodeSize(AdditiveNodeSize);
			}

			SetImageSize();





		}

		private void Start()
		{
			CreateSphere();
			CreateInitNode();
			spheres[currentSphereID].CreatePreviewNode();

			rotate = Vector2.up * -30.0f;

			infoTextRect = infoObjText.GetComponent<RectTransform>();
			infoText = infoObjText.GetComponent<Text>();
			infoTextRectRGB = infoObjTextRGB.GetComponent<RectTransform>();
			infoTextRGB = infoObjTextRGB.GetComponent<Text>();
			infoRectBG = infoObjBG.GetComponent<RectTransform>();
			infoImageBG = infoObjBG.GetComponent<Image>();
			infoRectR = infoObjR.GetComponent<RectTransform>();
			infoImageR = infoObjR.GetComponent<Image>();
			infoRectG = infoObjG.GetComponent<RectTransform>();
			infoImageG = infoObjG.GetComponent<Image>();
			infoRectB = infoObjB.GetComponent<RectTransform>();
			infoImageB = infoObjB.GetComponent<Image>();

			pastShapeType = shapeType;

		}


		void Update()
		{
			
			//一個でもNULLが含まれていたら終了
			foreach (Sphere s in spheres)
			{
				if (s == null)
					return;
			}
			

			//クリックしたら球作成
			if (InputMouseButton() && getColor.isInImageRect)
			{
				CreateAdditiveNode();
			}

			SeparateSphere();

			foreach (Sphere s in spheres)
			{
				s.UpdateMove();
			}

			RotateSphere();
			MoveSphere();
			ScaleSphere();
			ShowGrid();
			CreateRandomNode();
			UndoAdditiveNode();
			ClearAllAdditiveNode();
			UpdateAdditiveNumText();
			Save();
			Load();
			EnableInfomation();
			ChangeShape();
		}

		public bool CreateAdditiveNode()
		{
			return spheres[currentSphereID].CreateAdditiveNode();
		}


		bool InputMouseButton()
		{
			Vector3 mousePos = Input.mousePosition;

			bool r;
			if(Input.GetMouseButtonDown(0))
			{	//マウスボタンを押した瞬間
				r = true;
				pastMousePos = mousePos;
			}
			else if (Input.GetMouseButton(0) && (mousePos - pastMousePos).sqrMagnitude > 1000)
			{	//マウスボタンを押しながらドラッグし、一定距離進んだ時
				r = true;
				pastMousePos = mousePos;
			}
			else
			{
				r = false;
			}
			return r;

		}

		/// <summary>
		/// 初期配置用のノード作成
		/// </summary>
		void CreateInitNode()
		{
			int parentID = 0;

			for (int k = 0; k < initNodeNumL; k++)
			{
				float elevation = initNodeNumL == 1 ? 0.0f : 180.0f / (initNodeNumL - 1) * k - 90.0f;

				for (int j = 1; j <= initNodeNumS; j++)
				{
					float radius = 1.0f / initNodeNumS * j;
					if (elevation >= 90.0f || elevation <= -90.0f)
					{
						Quaternion rot = Quaternion.AngleAxis(elevation, Vector3.right);
						Vector3 pos = rot * Vector3.forward * radius;
						spheres[parentID].CreateNode(pos, initNodeSize * radius, NodeType.INIT, material);

						parentID = parentID == 0 ? 1 : 0;
					}
					else
					{
						for (int i = 0; i < initNodeNumH; i++)
						{

							float azimuth = 360.0f / initNodeNumH * i;

							Quaternion rot = Quaternion.AngleAxis(azimuth, Vector3.up) * Quaternion.AngleAxis(elevation, Vector3.right);
							Vector3 pos = rot * Vector3.forward * radius;

							float size = Mathf.Lerp(initNodeSize, initNodeSize * radius, initNodeCenterSmall);

							spheres[parentID].CreateNode(pos, size, NodeType.INIT, material);

							parentID = parentID == 0 ? 1 : 0;

						}
					}
				}
			}
		}

		/// <summary>
		/// ゲーム起動時にスフィア達の親作成
		/// </summary>
		void CreateSphere()
		{
			for(int i = 0; i < spheres.Length; i++)
			{
				spheres[i] = new Sphere(transform, i, this);
			}
		}

		/// <summary>
		/// ランダムでノード作成
		/// </summary>
		void CreateRandomNode()
		{
			if (!Input.GetButtonDown("Random"))
				return;

			getColor.IsRandomRead = true;
		}

		/// <summary>
		/// プレビューノードアップデード
		/// </summary>
		/// <param name="color"></param>
		/// <param name="isInImage"></param>
		public void UpdatePreviewNode(Color color, bool isInImage)
		{
			color.a = previewSphereAlpha;
			spheres[currentSphereID].UpdatePreviewNode(color, isInImage);
		}

		/// <summary>
		/// Undo　最後に作ったノードから順番に削除
		/// </summary>
		void UndoAdditiveNode()
		{
			if (!Input.GetButtonDown("Undo"))
				return;
			spheres[currentSphereID].UndoAdditiveNode();
		}

		/// <summary>
		/// 全追加ノード消去
		/// </summary>
		void ClearAllAdditiveNode()
		{
			if (!Input.GetButtonDown("Clear"))
				return;

			spheres[currentSphereID].ClearAllAdditiveNode();
		}

		/// <summary>
		/// 球の回転操作
		/// </summary>
		void RotateSphere()
		{
			rotateVelocity += new Vector2(Input.GetAxis("RotateHorizontal"), -Input.GetAxis("RotateVertical")) * Time.deltaTime * rotateSpeed;
			rotateVelocity /= (1.0f + rotateBrake * Time.deltaTime);
			rotate += rotateVelocity * Time.deltaTime;
			if (rotate.x > 180.0f)
				rotate.x -= 360.0f;
			else if (rotate.x < -180.0f)
				rotate.x += 360.0f;

			if (rotate.y < -89.9f)
			{
				rotate.y = -89.9f;
				rotateVelocity.y = 0.0f;
			}
			if (rotate.y > 89.9f)
			{
				rotate.y = 89.9f;
				rotateVelocity.y = 0.0f;
			}

			Quaternion r = Quaternion.AngleAxis(rotate.y, Vector3.right) * Quaternion.AngleAxis(rotate.x, Vector3.up);

			for (int i = 0; i < spheres.Length; i++)
			{
				spheres[i].SetRotation(r);
			}
		}


		/// <summary>
		/// 球の移動
		/// </summary>
		void MoveSphere()
		{
			Vector3 p = Vector3.right * moveSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
			p += Vector3.up * moveSpeed * Time.deltaTime * Input.GetAxis("Vertical");
			transform.position += p;

			p = transform.position;

			if(p.x < moveSphereLimit.xMin)
				p.x = moveSphereLimit.xMin;
			else if(p.x > moveSphereLimit.xMax)
				p.x = moveSphereLimit.xMax;

			if(p.y < moveSphereLimit.yMin)
				p.y = moveSphereLimit.yMin;
			else if(p.y > moveSphereLimit.yMax)
				p.y = moveSphereLimit.yMax;
			transform.position = p;
		}


		/// <summary>
		/// 球のスケール
		/// </summary>
		void ScaleSphere()
		{
			scale += (Input.GetAxis("Scale")+Input.mouseScrollDelta.y*10.0f) * scaleSpeed * Time.deltaTime;

			scale = Mathf.Clamp(scale, 0.1f, 10.0f);

			transform.localScale = Vector3.one * scale;
		}

		/// <summary>
		/// 球の分割
		/// </summary>
		void SeparateSphere()
		{
			if (!Input.GetButtonDown("Separate"))
				return;

			spheres[currentSphereID].DeletePreviewNode();

			if (currentSphereID==0)
			{
				currentSphereID = 1;
				spheres[0].moveTargetPos = Vector3.left * secondarySpherePosition;
				spheres[1].moveTargetPos = Vector3.zero;
			}
			else
			{
				currentSphereID = 0;
				spheres[0].moveTargetPos = Vector3.zero;
				spheres[1].moveTargetPos = Vector3.left * secondarySpherePosition;
			}
			spheres[currentSphereID].CreatePreviewNode();

		}

		/// <summary>
		/// グリッドの表示、非表示
		/// </summary>
		void ShowGrid()
		{
			if (!Input.GetButtonDown("ShowGrid"))
				return;

			foreach (Sphere s in spheres)
			{
				s.ShowGrid();
			}
		}

		/// <summary>
		/// 右上のノード数カウンタ
		/// </summary>
		void UpdateAdditiveNumText()
		{
			AdditiveNumText.text = spheres[currentSphereID].AdditiveNodes.Count.ToString() + " / " + maxAdditiveNodeNum;

			
			if (maxAdditiveNodeNum == spheres[currentSphereID].AdditiveNodes.Count)
			{	//限界の時は赤色
				AdditiveNumText.color = Color.red;
			}
			else
			{	//灰色
				AdditiveNumText.color = Color.gray;
			}
		}

		/// <summary>
		/// 画像がドラッグ・アンド・ドロップされると画像のサイズを読み取って配置してくれる
		/// </summary>
		void SetImageSize()
		{
			var image = imageObj.GetComponent<Image>();
			var rect = imageObj.GetComponent<RectTransform>();

			if (picture == null)
			{	//画像が設定されていなかった時
				image.sprite = picture;
				rect.sizeDelta = Vector3.one * imageMaxSizeHorizontal;
			}
			else
			{
				image.sprite = picture;
				float width = picture.textureRect.width;
				float height = picture.textureRect.height;

				if (width >= height)
				{	//横長画像の時
					rect.sizeDelta = new Vector2(imageMaxSizeHorizontal, imageMaxSizeHorizontal * height / width);
				}
				else
				{	//縦長画像の時
					rect.sizeDelta = new Vector2(imageMaxSizeVertical * width / height, imageMaxSizeVertical);
				}
			}
		}

		/// <summary>
		/// 実行中のシェイプの変更受付
		/// </summary>
		void ChangeShape()
		{
			//本当はOnValidateで処理したいけど、Warningが出てしまうのでUpdateで。。。

			if (pastShapeType == shapeType)
				return;

			Mesh mesh = shapeType == ShapeType.BOX ? cubeMesh : sphereMesh;
			spheres[currentSphereID].ChangeSphapeType(mesh);

			pastShapeType = shapeType;
		}


		/// <summary>
		/// 現在表示中のノードをファイルに保存
		/// </summary>
		void Save()
		{
			if (!Input.GetButtonDown("Save"))
				return;
			spheres[currentSphereID].Save();
		}

		/// <summary>
		/// セーブデータからロード
		/// </summary>
		void Load()
		{
			if (!Input.GetButtonDown("Load") ||saveData == null)
				return;

			for (int i = 0; i < saveData.Position.Length; i++)
			{
				HSL hsl = HSL.PositionToHSL(saveData.Position[i]);
				Color color = hsl.ToRgb();
				UpdatePreviewNode(color, true);

				if (!CreateAdditiveNode())
					return;
			}
		}

		void EnableInfomation()
		{
			if (Input.GetButtonDown("Information"))
				enableInfo = !enableInfo;
		}




		public void UpdateInformation(bool isInImageRect, Vector2 mousePos, Color color,Vector2 onImagePosRatio)
		{
			if (enableInfo && !pastEnableInfo)
			{
				infoImageR.enabled = true;
				infoImageG.enabled = true;
				infoImageB.enabled = true;
				infoText.enabled = true;
				infoTextRGB.enabled = true;
				infoImageBG.enabled = true;
			}
			else if(!enableInfo && pastEnableInfo)
			{
				infoImageR.enabled = false;
				infoImageG.enabled = false;
				infoImageB.enabled = false;
				infoText.enabled = false;
				infoTextRGB.enabled = false;
				infoImageBG.enabled = false;
			}
			pastEnableInfo = enableInfo;

			if (!enableInfo)
				return;

			infoRectBG.position = mousePos;
			infoTextRectRGB.position = mousePos;
			infoTextRect.position = mousePos;
			infoRectR.position = mousePos + offsetInfoR;
			infoRectG.position = mousePos + offsetInfoG;
			infoRectB.position = mousePos + offsetInfoB;

			infoRectR.sizeDelta = new Vector2(color.r * infoBarSize, 5.0f);
			infoRectG.sizeDelta = new Vector2(color.g * infoBarSize, 5.0f);
			infoRectB.sizeDelta = new Vector2(color.b * infoBarSize, 5.0f);

			infoTextRGB.text = (int)(color.r * 255) + "\n";
			infoTextRGB.text += (int)(color.g * 255) + "\n";
			infoTextRGB.text += (int)(color.b * 255) + "\n";
			infoText.text = "POS : ( " + ((int)(onImagePosRatio.x * picture.rect.width)).ToString() + " , " + 
				((int)(onImagePosRatio.y * picture.rect.height)).ToString() + " )\n";
			HSL hsl = HSL.RGBToHSL(color);
			infoText.text += "HSL : ( " + hsl.h.ToString("f2") + " , " + hsl.s.ToString("f2") + " , " + hsl.l.ToString("f2") + " )\n";



		}

	}
}
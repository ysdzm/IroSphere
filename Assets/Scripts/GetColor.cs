using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UI;




/// <summary>
/// 色読み取り用のクラス
/// カメラにアタッチして使用します
/// </summary>

namespace IroSphere
{
	public class GetColor : MonoBehaviour
	{

		[SerializeField]
		SphereManager sphereManager;

		private Texture2D capture = null;

		[SerializeField]
		GameObject imageObj;

		RectTransform imageRectTrs;
		Image image;
		public bool isInImageRect { get; private set; } = false;

		Vector2 imageCornerBottomLeft;
		Vector2 imageCornerTopRight;

		public bool IsRandomRead { get; set; }

		private void Awake()
		{
			capture = new Texture2D(1, 1, TextureFormat.RGB24, false);
			imageRectTrs = imageObj.GetComponent<RectTransform>();
			image = imageObj.GetComponent<Image>();
			sphereManager.getColor = this;
		}

		void OnPostRender()
		{
			UpdateCorners();
			RandomRead();

			Vector2 mousePos = Input.mousePosition;

			isInImageRect = (mousePos.x >= imageCornerBottomLeft.x && imageCornerTopRight.x >= mousePos.x &&
							 mousePos.y >= imageCornerBottomLeft.y && imageCornerTopRight.y >= mousePos.y &&
							 image.enabled);



			Color color = isInImageRect ? ReadPixels(mousePos) : Color.white;
			sphereManager.UpdatePreviewNode(color, isInImageRect);

		}

			private void Update()
		{
			ShowImage();
		}


		void ShowImage()
		{
			if (!Input.GetButtonDown("ShowImage"))
				return;

			image.enabled = !image.enabled;
		}


		/// <summary>
		/// 画面を読み取って色を取得
		/// </summary>
		/// <param name="pos"></param>
		public Color ReadPixels(Vector2 pos)
		{
			capture.ReadPixels(new Rect(pos.x, Screen.height - pos.y, 1, 1), 0, 0);
			return capture.GetPixel((int)pos.x, (int)pos.y);
		}

		
		/// <summary>
		/// 起動直後に画像を読み取ってランダムで球を配置
		/// </summary>
		public void RandomRead()
		{
			if (!IsRandomRead)
				return;

			IsRandomRead = false;

			if (!image.enabled)
				return;

			while (true)
			{
				Vector2 pos = new Vector2(
					Random.Range(imageCornerBottomLeft.x, imageCornerTopRight.x),
					Random.Range(imageCornerBottomLeft.y, imageCornerTopRight.y));
				Color color = ReadPixels(pos);
				sphereManager.UpdatePreviewNode(color, true);

				if (!sphereManager.CreateAdditiveNode())
					return;
			}
		}

			public void UpdateCorners()
		{
			var corners = new Vector3[4];
			imageRectTrs.GetWorldCorners(corners);
			imageCornerBottomLeft = RectTransformUtility.WorldToScreenPoint(Camera.main, corners[0]);
			imageCornerTopRight = RectTransformUtility.WorldToScreenPoint(Camera.main, corners[2]);
		}
	}
}
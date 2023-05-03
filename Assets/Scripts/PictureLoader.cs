using System;
using UnityEngine;

namespace IroSphere
{
	public class PictureLoader : MonoBehaviour
	{
		[Serializable]
		class PictureProperty
		{
			public string name;
			public int width;
			public int height;
		}

		[SerializeField]
		SphereManager manager;

		Texture2D texture;
		Sprite sprite;

		void OnDestroy()
		{
			ReleaseResources();
		}

		/// <summary>
		/// ネイティブからテクスチャを設定する
		/// DragAndDropした時にSendMessageで呼び出す
		/// </summary>
		void SetPicture(string propJson)
		{
			manager.Picture = null;
			ReleaseResources();
			
			// Texture作成
			var prop = JsonUtility.FromJson<PictureProperty>(propJson);
			texture = new Texture2D(prop.width, prop.height, TextureFormat.RGBA32, false);
			texture.name = prop.name;

			NativeUtils.BindTexture(texture);
			texture.wrapMode = TextureWrapMode.Clamp;
			texture.filterMode = FilterMode.Bilinear;

			// TextureからSpriteを作成して設定
			sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
			sprite.name = prop.name;
			manager.Picture = sprite;
		}

		void ReleaseResources()
		{
			if (texture != null)
			{
				Destroy(texture);
			}
			if (sprite != null)
			{
				Destroy(sprite);
			}
		}

		/// <summary>
		/// ローカルデバッグ用メソッド
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public void DebugLoadFromPath(string path)
		{
			var rawData = System.IO.File.ReadAllBytes(path);
			Texture2D texture2D = new Texture2D(0, 0);
			texture2D.LoadImage(rawData);
			sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height),
					new Vector2(0.5f, 0.5f), 100f);
			manager.Picture = sprite;
		}
	}
}
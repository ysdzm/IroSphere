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
	}
}
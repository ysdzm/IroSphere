using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace IroSphere
{
	public static class NativeUtils
	{
#if !UNITY_EDITOR && UNITY_WEBGL
		[DllImport("__Internal")]
		private static extern void bindTexture(IntPtr texture);

		[DllImport("__Internal")]
		private static extern void exportFile(string fileName, string mimeType, byte[] data, int size);

		[DllImport("__Internal")]
		private static extern void copyToClipboard(string text);
#endif

		public static void BindTexture(Texture texture)
		{
#if !UNITY_EDITOR && UNITY_WEBGL
			bindTexture(texture.GetNativeTexturePtr());
#endif
		}

		public static void ExportFile(string fileName, string mimeType, byte[] data)
		{
#if !UNITY_EDITOR && UNITY_WEBGL
			exportFile(fileName, mimeType, data, data.Length);
#endif
		}

		public static void CopyToClipboard(string text)
		{
#if !UNITY_EDITOR && UNITY_WEBGL
			copyToClipboard(text);
#endif
		}
	}
}
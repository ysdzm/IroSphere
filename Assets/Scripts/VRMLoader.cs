using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using System;
using UniVRM10;
using System.Threading.Tasks;

public class VRMLoader : MonoBehaviour
{
    private int length;
    private int index;
    private List<byte> byteContent = new List<byte>();

    public void ByteLength(int length)
    {
        this.length = length;
        index = 0;
        byteContent.Clear();
    }

    public void AddRange(string byteString)
    {
        var decode = Convert.FromBase64String(byteString);
        byteContent.AddRange(decode);
        index += 1;
        if (length != index) return;
        Spawn();
    }

    public void Test(byte[] hoge)
    {
        Debug.Log(hoge);
    }

    public async void Spawn()
    {
        var vrmBytes = byteContent.ToArray();
        // URP 対応 ロード
        var instance = await Vrm10.LoadBytesAsync(vrmBytes, awaitCaller: new VRMShaders.RuntimeOnlyNoThreadAwaitCaller(),materialGenerator: new UrpVrm10MaterialDescriptorGenerator());
        var root = instance.gameObject;
        root.transform.position = new Vector3(-2, -1f, 0);
        root.transform.rotation = Quaternion.Euler(0,180,0);
    }

    private IEnumerator Load(string url)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();
        if (webRequest.isNetworkError)
        {
            // エラー処理
            yield break;
        }
        Debug.Log(webRequest.responseCode);
        LoadVRM(webRequest.downloadHandler.data);
    }

    public async void LoadVRM(Byte[] url)
    {
        var path = url;
        var instance = await LoadAsync(path, new VRMShaders.RuntimeOnlyNoThreadAwaitCaller());
        var root = instance.gameObject;
        root.transform.position = new Vector3(0, 0, 0);
        root.transform.rotation = Quaternion.identity;
    }

    async Task<Vrm10Instance> LoadAsync(Byte[] path, VRMShaders.IAwaitCaller awaitCaller)
    {
        var instance = await Vrm10.LoadBytesAsync(path, awaitCaller: awaitCaller);
        return instance;
    }
}

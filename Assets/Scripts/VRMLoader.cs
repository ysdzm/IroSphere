using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using System;
using UniVRM10;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class VRMLoader : MonoBehaviour
{
    [SerializeField] Vector3 avatar_pos;
    [SerializeField] Quaternion avatar_rot;

    /// <summary>
    /// javascript側からurl呼ばれる、アップロードされたvrmのurlからvrmをロードするメソッド
    /// </summary>
    /// <param name="url">js側で生成されたblobオブジェクトを指すurl</param>
    public async void LoadFromURL(string url){

        Vrm10Instance instance = await LoadVRM(url);

        if(instance == null){
            // null チェック
        }

        var root = instance.gameObject;
        root.transform.position = avatar_pos;
        root.transform.rotation = avatar_rot;
    }

    /// <summary>
    /// urlからVrm10Instanceをロード
    /// </summary>
    /// <param name="url"></param>
    /// <returns>js側で生成されたblobオブジェクトを指すurl</returns>
    private async Task<Vrm10Instance> LoadVRM(string url)
    {
        var uwr = UnityWebRequest.Get(url);
        await uwr.SendWebRequest();
        if(uwr.isHttpError || uwr.isNetworkError) throw new Exception(uwr.error);
        byte[] data = uwr.downloadHandler.data;
        var instance = await Vrm10.LoadBytesAsync(data, awaitCaller: new VRMShaders.RuntimeOnlyNoThreadAwaitCaller(), materialGenerator: new UrpVrm10MaterialDescriptorGenerator());
        return instance;
    }

}

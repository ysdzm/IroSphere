using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using System;
using UniVRM10;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class VRMLoader : MonoBehaviour
{
    [SerializeField] Vector3 avatar_pos;
    [SerializeField] Quaternion avatar_rot;
    [SerializeField] GameObject obj;
    [SerializeField] GameObject root;
    [SerializeField] Slider slider;

    /// <summary>
    /// javascript側からurl呼ばれる、アップロードされたvrmのurlからvrmをロードするメソッド
    /// </summary>
    /// <param name="url">js側で生成されたblobオブジェクトを指すurl</param>
    public async void LoadFromURL(string url){

        ReleaseResources();
        Vrm10Instance instance = await LoadVRM(url);

        if(instance == null){
            // null チェック
        }

        obj = instance.gameObject;
        obj.transform.parent = root.transform;
        obj.transform.localPosition = avatar_pos;
        obj.transform.localRotation = avatar_rot;
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

    /// <summary>
    /// ローカルデバッグ用メソッド
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public async void DebugLoadFromPath(string path)
    {
        ReleaseResources();
        Vrm10Instance instance = await Vrm10.LoadPathAsync(path,materialGenerator: new UrpVrm10MaterialDescriptorGenerator());

        if(instance == null){
            // null チェック
        }

        obj = instance.gameObject;
        obj.transform.parent = root.transform;
        obj.transform.localPosition = avatar_pos;
        obj.transform.localRotation = avatar_rot;

        //obj.transform.position = avatar_pos;
        //obj.transform.rotation = avatar_rot;
    }

    void ReleaseResources()
	{
        if(obj != null)
        {
            Destroy(obj);
        }
    }

    public void Start() {
        string sample_path = Application.streamingAssetsPath + "/" + "AvatarSample_A.vrm";
        DebugLoadFromPath(sample_path);
    }

    public void Update()
    {
        root.transform.rotation = Quaternion.Euler(0,slider.value * 360f - 180f, 0);
    }
}

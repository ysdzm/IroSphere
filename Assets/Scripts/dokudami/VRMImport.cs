using System.Collections;
using UnityEngine;
using SFB;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System;

using System.Runtime.InteropServices;

namespace UniVRM10.VRM10Viewer
{/*=== VRM10Viewer ===*/

    public class VRMImport : MonoBehaviour, IPointerDownHandler
    {

        [SerializeField]
        public Button LoadButton;
        public Button StartButton;
        public GameObject AvatarPos;
        public GameObject ReAvatar;
        public AudioClip SE;

        public bool VrmLoadFlag = false;

        void Start()
        {

#if !(UNITY_WEBGL && !UNITY_EDITOR)
            LoadButton.onClick.AddListener(On_LoadFileSelect);
#endif
            
            VrmLoadFlag = true;

        }

#if UNITY_WEBGL && !UNITY_EDITOR

        //
        // WebGL
        //

        // StandaloneFileBrowserのブラウザスクリプトプラグインから呼び出す
        [DllImport("__Internal")]
        private static extern void UploadFile(string gameObjectName, string methodName,string   filter, bool multiple);

        // ファイルを開く
        public void OnPointerDown(PointerEventData eventData) {
            VrmLoadFlag = false;
            UploadFile(gameObject.name, "OnFileUpload", ".vrm", false);
            VrmLoadFlag = true;
        }

        // ファイルアップロード後の処理
        public void OnFileUpload(string url) {
            StartCoroutine(Load(url));
            VrmLoadFlag = true;
        }
        
#else
        //
        // OSビルド & Unity editor上
        //
        public void OnPointerDown(PointerEventData eventData) { }

        // ファイルを開く
        public void On_LoadFileSelect()
        {

            VrmLoadFlag = false;

            // 拡張子フィルタ
            var extensions = new[] {
                new ExtensionFilter("Uni-VRM v0.108.0", "vrm" ),
            };
    
            // ファイルダイアログを開く
            var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions,    false);
            if (paths.Length > 0 && paths[0].Length > 0)
            {
    
                StartCoroutine(Load(new System.Uri(paths[0]).AbsoluteUri));
                VrmLoadFlag = true;
            }
        }
    
#endif
        // ファイル読み込み
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
            LoadVRMClicked(webRequest.downloadHandler.data);
    
        }
    
        public async void LoadVRMClicked(Byte[] url)
        {
    
            var path = url;

            var instance = await LoadAsync(path, new VRMShaders.RuntimeOnlyNoThreadAwaitCaller());
    
            Transform AvatarCount = AvatarPos.GetComponentInChildren<Transform>();

            //子要素を全て消去
            if (AvatarCount.childCount != 0)
            {
                //子オブジェクトを一つずつ取得
                foreach (Transform child in AvatarCount.transform)
                {
                    //削除する
                    Destroy(child.gameObject);
                }
    
            }

            //アバターのゲームオブジェクトを取得
            var root = instance.gameObject;
            //アバターの位置を移動
            root.transform.position = new Vector3(0, 0, 0);
            root.transform.rotation = Quaternion.identity;
            //Avatarの子にVRM1(アバター本体を入れる)
            root.gameObject.transform.parent = AvatarPos.transform;

        }

        async Task<Vrm10Instance> LoadAsync(Byte[] path, VRMShaders.IAwaitCaller awaitCaller)
        {

            // var instance = await Vrm10.LoadBytesAsync(path, awaitCaller: awaitCaller);
            var instance = await Vrm10.LoadBytesAsync(path, awaitCaller: awaitCaller, materialGenerator: new UrpVrm10MaterialDescriptorGenerator());
    
            // VR用 FirstPerson 設定
            await instance.Vrm.FirstPerson.SetupAsync(instance.gameObject, awaitCaller);
    
            return instance;
        }


    }/*=== END_ViewImport ===*/

}/*=== END_VRM10Viewer ===*/
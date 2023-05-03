using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using IroSphere;

public class DebugButton : MonoBehaviour
{
    [SerializeField] private VRMLoader vRMLoader;
    [SerializeField] private PictureLoader pictureLoader;
    [SerializeField] private UISwitcher uISwitcher;

    #if UNITY_EDITOR

    public void Test()
    {
        //パスの取得
        var path = EditorUtility.OpenFilePanelWithFilters("Open vrm", "", new[]{
            "Image files", "png,jpg",
            "VRM files", "vrm",
        });
        if (string.IsNullOrEmpty(path))
            return;
        
        Debug.Log(path);

        if(path.EndsWith("png") || path.EndsWith("jpg"))
        {
            uISwitcher.Switch("image");
            pictureLoader.DebugLoadFromPath(path);
        }

        if(path.EndsWith("vrm"))
        {
            uISwitcher.Switch("vrm");
            vRMLoader.DebugLoadFromPath(path);
        }
    }

    #endif
}

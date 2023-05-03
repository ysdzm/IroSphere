using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IroSphere;

public class UISwitcher : MonoBehaviour
{
    [SerializeField] private GameObject image_mode;
    [SerializeField] private GameObject vrm_mode;
    [SerializeField] private GetColor getColor;

    public void Switch(string filemode)
    {
        Debug.Log(filemode);

        if(filemode == "vrm")
        {
            image_mode.SetActive(false);
            vrm_mode.SetActive(true);
            getColor.SetIsStaticImageCorner(true);
        }

        if(filemode == "image")
        {
            image_mode.SetActive(true);
            vrm_mode.SetActive(false);
            getColor.SetIsStaticImageCorner(false);
        }

    }

    void Start()
    {
        Switch("vrm");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInteraction : MonoBehaviour
{
    public GameObject popupHTP;  // Referensi ke UI Popup
    public GameObject popupHTP1;
    public GameObject popupHTP2;

    public GameObject popupSetting;  // Referensi ke UI Popup
    public GameObject popupSetting1;
    public GameObject popupSetting2;

    // Fungsi untuk menampilkan popup
    public void ShowPopup()
    {
        popupHTP.SetActive(true);
        popupHTP1.SetActive(true);
        popupHTP2.SetActive(true);
        
    }

    public void ShowPopupSetting()
    {
        popupSetting.SetActive(true);
        popupSetting1.SetActive(true);
        popupSetting2.SetActive(true);
    }

    // Fungsi untuk menyembunyikan popup
    public void HidePopup()
    {
        popupHTP.SetActive(false);
        popupHTP1.SetActive(false);
        popupHTP2.SetActive(false);
        popupSetting.SetActive(false);
        popupSetting1.SetActive(false);
        popupSetting2.SetActive(false);
    }

    // Fungsi untuk toggle popup
    public void TogglePopup()
    {
        popupHTP.SetActive(!popupHTP.activeSelf);
    }
}

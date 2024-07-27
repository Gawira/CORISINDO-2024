using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInteraction : MonoBehaviour
{
    public GameObject popupSetting;  // Referensi ke UI Popup
    public GameObject popupSetting1;
    public GameObject popupSetting2;

    // Fungsi untuk menampilkan popup
    public void ShowPopup()
    {
        popupSetting.SetActive(true);
        popupSetting1.SetActive(true);
        popupSetting2.SetActive(true);
    }

    // Fungsi untuk menyembunyikan popup
    public void HidePopup()
    {
        popupSetting.SetActive(false);
        popupSetting1.SetActive(false);
        popupSetting2.SetActive(false);
    }

    // Fungsi untuk toggle popup
    public void TogglePopup()
    {
        popupSetting.SetActive(!popupSetting.activeSelf);
    }
}

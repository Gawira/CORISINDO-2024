using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInteraction : MonoBehaviour
{
    public GameObject popupSetting;  // Referensi ke UI Popup

    // Fungsi untuk menampilkan popup
    public void ShowPopup()
    {
        popupSetting.SetActive(true);
    }

    // Fungsi untuk menyembunyikan popup
    public void HidePopup()
    {
        popupSetting.SetActive(false);
    }

    // Fungsi untuk toggle popup
    public void TogglePopup()
    {
        popupSetting.SetActive(!popupSetting.activeSelf);
    }
}

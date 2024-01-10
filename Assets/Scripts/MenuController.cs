using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown languagesDD;
    [SerializeField] private LocalizationSettings localizationsSettings;
    [SerializeField] private Locale tr;
    [SerializeField] private Locale en;
    // Start is called before the first frame update
    void Start()
    {
        if(localizationsSettings.GetSelectedLocale() == tr)
        {
            languagesDD.value = 1;
        }
        else
        {
            languagesDD.value = 0;
            localizationsSettings.SetSelectedLocale(en);
        }
       
   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetLanguage()
    {
        if(languagesDD.value == 0)
        {
            localizationsSettings.SetSelectedLocale(en);
        }
        else
        {
            localizationsSettings.SetSelectedLocale(tr);

        }
    }
}

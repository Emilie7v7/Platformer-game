using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateUI : MonoBehaviour
{
    [SerializeField] private GameObject ObjectPrefab;
    private TextMeshProUGUI UIText;
    private string ObjectID;


    //for the uitext taking the component textmeshpro
    //for objectID which is "cherry" taking the cherryprefab and taking the component of the cherry ?
    private void Awake()
    {
        UIText = GetComponent<TextMeshProUGUI>();
        ObjectID = ObjectPrefab.GetComponent<Object>().ID;
    }
    
    private void LateUpdate()
    {
        UIText.text = PlayerPrefs.GetInt(ObjectID).ToString();
    }

}

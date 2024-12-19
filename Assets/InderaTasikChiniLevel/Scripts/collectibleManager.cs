using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class collectibleManager : MonoBehaviour
{
    public int collectibleCounter;
    public TextMeshProUGUI collectibleText;

    // Update is called once per frame
    void Update()
    {
        collectibleText.text = collectibleCounter.ToString();
    }
}

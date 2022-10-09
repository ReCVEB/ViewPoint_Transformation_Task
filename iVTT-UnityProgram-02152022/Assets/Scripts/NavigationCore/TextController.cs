using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextController : MonoBehaviour
{
    private static TextController _instance;
    public static TextController Instance {
        get { return _instance; }
    }

    [SerializeField] private TextMeshPro[] textMeshPros;


    private void Awake() {
        if (_instance ==null) {
            _instance = this;
            //textMeshPros = new List<TextMeshPro>();
        }
    }

    public void setText(int index, string text) {
        textMeshPros[index].text = text;
    }

    public string getText(int index) {
        return textMeshPros[index].text;
    }

    public void setColor(int index, Color color) {
        textMeshPros[index].color = color;
    }
    // Update is called once per frame

    //public void setText(string text) {
    //    textMeshPro.SetText(text);
    //}

    //public string getText() {
    //    return textMeshPro.text;
    //}
}

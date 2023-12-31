using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTextMesh : MonoBehaviour
{
    public static WorldTextMesh Instance { get; private set; }

    private void Awake()
    {
        CreateSingleton();
    }
    void CreateSingleton()
    {
        if (Instance == null)
            Instance = this;
        //else
            //Destroy(gameObject);
    }
    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
}

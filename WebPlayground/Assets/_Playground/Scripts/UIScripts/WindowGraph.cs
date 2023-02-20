using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowGraph : MonoBehaviour
{
    [SerializeField] private RectTransform graphContainer;
    [SerializeField] private Sprite circleSprite;

    private void Awake()
    {
        List<int> valueList = new List<int>() { 5, 89, 23, 54, 45, 33, 85, 2, 44, 24, 74, 50, 20, 13 };
        ShowGraph(valueList);
    }

    private void CreateCircle(Vector2 anchoredPosition)
    {
        GameObject gObject = new GameObject("circle", typeof(Image));
        gObject.transform.SetParent(graphContainer, false);
        gObject.GetComponent<Image>().sprite = circleSprite;

        RectTransform rectTransform = gObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
    }

    private void ShowGraph(List<int> valueList)
    {
        float graphHeight = graphContainer.sizeDelta.y;
        float yMax = 10000000000f; // top of graph
        float xSize = 50f; // space between in graph
        for(int i = 0; i < valueList.Count; ++i)
        {
            float xPos = i * xSize;
            float yPos = (valueList[i] / yMax) * graphHeight;
            CreateCircle(new Vector2(xPos, yPos));
        }
    }
}

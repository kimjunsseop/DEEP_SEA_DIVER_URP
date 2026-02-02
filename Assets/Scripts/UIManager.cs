using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public UnityEngine.UI.Image O2Gage;
    public static UIManager instance;
    public UnityEngine.UI.Image[] items;
    public Dictionary<int, GameObject> itemss = new Dictionary<int, GameObject>();
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void SetPercent(float a)
    {
        O2Gage.fillAmount = a / 100;
    }

    public void Initialized()
    {
        for(int i = 0; i < itemss.Count; i++)
        {
            items[i].sprite = itemss[i].GetComponent<Item>().sprite;
            items[i].color = new Color(items[i].color.r, items[i].color.g, items[i].color.b, 0.5f);
        }
    }

    public void setImage(GameObject item, int index)
    {
        Item item1 = item.GetComponent<Item>();
        items[index].sprite = item1.sprite;
        items[index].color = new Color(items[index].color.r, items[index].color.g, items[index].color.b, 0.5f);
    }

    public void changeAlpha(int type)
    {
        for(int i = 0; i < items.Length; i++)
        {
            if(itemss[type].GetComponent<Item>().sprite == items[i].sprite)
            {
                items[i].color = new Color(items[i].color.r, items[i].color.g, items[i].color.b, 1f);
                GameManager.instance.Change(i);
            }
        }
    }
}

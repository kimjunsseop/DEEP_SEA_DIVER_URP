using UnityEngine;

public class Item : MonoBehaviour
{
    public Sprite sprite;
    public int itemType;
    public bool isPickUped;
    void Start()
    {
        isPickUped = false;
    }

    void Update()
    {
        
    }

    public void Pickuped()
    {
        isPickUped = true;
        UIManager.instance.changeAlpha(this.itemType);
        Destroy(gameObject);
    }
}

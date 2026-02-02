using UnityEngine;

public class Item : MonoBehaviour
{
    public Sprite sprite;
    public int itemType;
    void Start()
    {
    }

    void Update()
    {
        
    }

    public void Pickuped()
    {
        UIManager.instance.changeAlpha(this.itemType);
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Unity.VisualStudio.Editor;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Animator anim;
    [field:SerializeField] public float Speed {get;set;}
    private bool isBreathing = false;
    private Item nearBy;
    [Header("O2")]
    [field:SerializeField] public float O2Dwoun {get;set;}
    [field:SerializeField] public float O2Up {get;set;}
    public float O2;
    public float MaxO2 = 100f;
    public Light2D globallight;
    public Light2D spotLight;
    private bool isItem;
    public UnityEngine.UI.Image itemGage;
    public float maxItemDuration = 5f;
    float currentDuration;
    private BoxCollider2D col;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
        O2 = MaxO2;
        itemGage.enabled = false;
        currentDuration = maxItemDuration;
        //UIManager.instance.Initialized();
    }
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        SetAnim(h,v);
        transform.Translate(new Vector3(h,v,0) * Speed * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(nearBy != null)
            {
                if(UIManager.instance.itemss.ContainsKey(nearBy.itemType))
                {
                    nearBy.Pickuped();
                    nearBy = null;
                }
            }
        }
        if(isBreathing)
        {
            if(O2 >= MaxO2)
            {
                O2 = MaxO2;
            }
            else
            {
                O2 += O2Up * Time.deltaTime;
            }
        }
        else
        {
            O2 -= O2Dwoun * Time.deltaTime;
        }
        GameManager.instance.endTime += Time.deltaTime;
        UIManager.instance.SetPercent(O2);
        UIManager.instance.SetDepth(transform);
        if(!isBreathing)
        {
            globallight.intensity = 0.1f + (transform.position.y / 200);
        }
        if(isItem)
        {
            currentDuration -= Time.deltaTime;
            itemGage.fillAmount = currentDuration / maxItemDuration;
        }
        if(O2 <= 0)
        {
            SceneController.instance.EndGame();
            GameManager.instance.result = false;
        }
    }
    void FixedUpdate()
    {
        Vector3 newOffset = Camera.main.WorldToViewportPoint(transform.position);
        newOffset.x = Mathf.Clamp01(newOffset.x);
        newOffset.y = Mathf.Clamp01(newOffset.y);
        Vector3 world = Camera.main.ViewportToWorldPoint(newOffset);
        transform.position = world;
    }
    void SetAnim(float h, float v)
    {
        if(v > 0 && !isBreathing)
        {
            anim.SetBool("Up", true);
            if(!anim.GetBool("Right") && !anim.GetBool("Left"))
            {
                colRig();   
            }
        }
        else if(v < 0)
        {
            anim.SetBool("Down", true);
            if(!anim.GetBool("Right") && !anim.GetBool("Left"))
            {
                colRig();   
            }
        }
        else
        {
            anim.SetBool("Up", false);
            anim.SetBool("Down", false);
            if(!anim.GetBool("Down") && !anim.GetBool("Up"))
            {
                rawRig();
            }
        }

        if(h > 0 && !isBreathing)
        {
            anim.SetBool("Right", true);
            if(!anim.GetBool("Down") && !anim.GetBool("Up"))
            {
                rawRig();   
            }
        }
        else if(h < 0 && !isBreathing)
        {
            anim.SetBool("Left", true);
            if(!anim.GetBool("Down") && !anim.GetBool("Up"))
            {
                rawRig();   
            }
        }
        else
        {
            anim.SetBool("Right", false);
            anim.SetBool("Left", false);
            if(!anim.GetBool("Right") || !anim.GetBool("Left"))
            {
                colRig();
            }
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("SaftyZone"))
        {
            globallight.intensity = 1f;
            isBreathing = true;
            spotLight.enabled = false;
        }
        if(collision.CompareTag("Item"))
        {
            // ui 활성화 SetActive(true)
            nearBy = collision.GetComponent<Item>();
        }
        if(collision.CompareTag("Monster"))
        {
            O2 -= 30f;
            StartCoroutine(Shake());
        }
        if(collision.CompareTag("Finish"))
        {
            if(GameManager.instance.Check())
            {
                SceneController.instance.EndGame();
                GameManager.instance.result = true;
            }
        }
        if(collision.CompareTag("PlayerItem"))
        {
            PlayerItem playerItem = collision.GetComponent<PlayerItem>();
            switch(playerItem.itemType)
            {
                case type.Speed:
                    StartCoroutine(SpeedUp(maxItemDuration));
                    break;
                case type.Light:
                    StartCoroutine(ExpandLight(maxItemDuration));
                    break;
                case type.Oxygen:
                    RechargeOx(playerItem);
                    break;
            }
            Destroy(collision.gameObject);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("SaftyZone"))
        {
            globallight.intensity = 0.1f;
            spotLight.enabled = true;
            isBreathing = false;
            
        }
        if(collision.CompareTag("Item"))
        {
            nearBy = null;
        }
    }
    IEnumerator Shake()
    {
        int count = 5;
        while(count > 0)
        {
            count--;
            CameraImpulse.instance.Shake();
            yield return new WaitForSeconds(0.1f);
        }
    }
    IEnumerator SpeedUp(float duration)
    {
        isItem = true;
        itemGage.enabled = true;
        float original = Speed;
        Speed *= 2;
        yield return new WaitForSeconds(duration);
        Speed = original;
        itemGage.enabled = false;
        currentDuration = maxItemDuration;
        isItem = false;
    }
    IEnumerator ExpandLight(float duration)
    {
        isItem = true;
        itemGage.enabled = true;
        float original = spotLight.pointLightOuterRadius;
        spotLight.pointLightOuterRadius *= 2; 
        yield return new WaitForSeconds(duration);
        spotLight.pointLightOuterRadius = original;
        itemGage.enabled = false;
        currentDuration = maxItemDuration;
        isItem = false;
    }
    public void RechargeOx(PlayerItem item)
    {
        if(O2 + item.plusOx >= MaxO2)
        {
            O2 = MaxO2;
        }
        else
        {
            O2 += item.plusOx;
        }
    }

    public void rawRig()
    {
        col.offset = new Vector2(0.01130903f, -0.0947918f);
        col.size = new Vector2(1.799296f, 0.6757025f);
    }
    public void colRig()
    {
        col.offset = new Vector2(0, 0.05065355f);
        col.size = new Vector2(0.5890918f, 1.430633f);
    }
}

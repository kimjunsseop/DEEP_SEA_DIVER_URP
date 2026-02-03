using System.Collections;
using System.Collections.Generic;
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
    private float O2;
    private float MaxO2 = 100f;
    public Light2D globallight;
    public Light2D spotLight;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        O2 = MaxO2;
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
        if(h > 0 && !isBreathing)
        {
            anim.SetBool("Right", true);
        }
        else if(h < 0 && !isBreathing)
        {
            anim.SetBool("Left", true);
        }
        else
        {
            anim.SetBool("Right", false);
            anim.SetBool("Left", false);
        }

        if(v > 0 && !isBreathing)
        {
            anim.SetBool("Up", true);
        }
        else if(v < 0)
        {
            anim.SetBool("Down", true);
        }
        else
        {
            anim.SetBool("Up", false);
            anim.SetBool("Down", false);
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
            }
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
}

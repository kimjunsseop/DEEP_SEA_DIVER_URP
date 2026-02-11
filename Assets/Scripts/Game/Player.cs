using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UI;

enum PlayerState
{
    UP = 270,
    Down = 92,
    Left = 0,
    Right = 180
}

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
    public ParticleSystem bubble;
    private PlayerState ps;
    public Volume volume;
    private UnityEngine.Rendering.Universal.Bloom bloom;
    public UnityEngine.UI.Image arrow;
    public GameObject joyStick;
    private DynamicJoystick dj;
    private bool playerDeath;
    public bool isPlaying = false;
    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
        O2 = MaxO2;
        itemGage.enabled = false;
        currentDuration = maxItemDuration;
        bubble.gameObject.SetActive(false);
        volume.profile.TryGet(out bloom);
        arrow.gameObject.SetActive(false);
        UIManager.instance.StartMessage();
        playerDeath = false;
        UIManager.instance.deathText.gameObject.SetActive(false);
        UIManager.instance.succesText.gameObject.SetActive(false);
        dj = joyStick.GetComponent<DynamicJoystick>();
        //UIManager.instance.Initialized();
    }
    void Update()
    {
        if(isPlaying)
        {
            if(!playerDeath)
            {
                float h = 0f;
                float v = 0f;
                if (dj.Horizontal != 0 || dj.Vertical != 0)
                {
                    h = dj.Horizontal;
                    v = dj.Vertical;
                }
                
                SetAnim(h,v);
                transform.Translate(new Vector3(h,v,0) * Speed * Time.deltaTime);
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
            playerDeath = true;
            // 모션
            StartCoroutine(LoseEnding());
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
    public void PickupItem()
    {
        if (nearBy != null)
        {
            // 기존 아이템 획득 로직
            if (UIManager.instance.itemss.ContainsKey(nearBy.itemType))
            {
                nearBy.Pickuped();
                nearBy = null;
            }
        }
    }
    void SetAnim(float h, float v)
    {
        if(v > 0 && !isBreathing)
        {
            anim.SetBool("Up", true);
            if(!anim.GetBool("Right") && !anim.GetBool("Left"))
            {
                colRig();
                ps = PlayerState.UP;  
                if(bubble.gameObject.activeSelf)
                {
                    bubble.transform.rotation = Quaternion.Euler(0,0,(float)ps);
                } 
            }
        }
        else if(v < 0)
        {
            anim.SetBool("Down", true);
            if(!anim.GetBool("Right") && !anim.GetBool("Left"))
            {
                colRig();
                
                ps = PlayerState.Down; 
                if(bubble.gameObject.activeSelf)
                {
                    bubble.transform.rotation = Quaternion.Euler(0,0,(float)ps);
                }   
            }
        }
        else
        {
            anim.SetBool("Up", false);
            anim.SetBool("Down", false);
            if(!anim.GetBool("Down") && !anim.GetBool("Up"))
            {
                rawRig();
                ps = PlayerState.UP;
                if(bubble.gameObject.activeSelf)
                {
                    bubble.transform.rotation = Quaternion.Euler(0,0,(float)ps);
                } 
            }
        }

        if(h > 0 && !isBreathing)
        {
            anim.SetBool("Right", true);
            if(!anim.GetBool("Down") && !anim.GetBool("Up"))
            {
                rawRig();   
                ps = PlayerState.Right;
                if(bubble.gameObject.activeSelf)
                {
                    bubble.transform.rotation = Quaternion.Euler(0,0,(float)ps);
                } 
            }
        }
        else if(h < 0 && !isBreathing)
        {
            anim.SetBool("Left", true);
            if(!anim.GetBool("Down") && !anim.GetBool("Up"))
            {
                rawRig();   
                ps = PlayerState.Left;
                if(bubble.gameObject.activeSelf)
                {
                    bubble.transform.rotation = Quaternion.Euler(0,0,(float)ps);
                } 
            }
        }
        else
        {
            anim.SetBool("Right", false);
            anim.SetBool("Left", false);
            if(!anim.GetBool("Right") || !anim.GetBool("Left"))
            {
                colRig();
                if(v > 0)
                {
                    ps = PlayerState.UP;    
                }
                else
                {
                    ps = PlayerState.Down;
                }
                if(bubble.gameObject.activeSelf)
                {
                    bubble.transform.rotation = Quaternion.Euler(0,0,(float)ps);
                } 
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
            UIManager.instance.ButtonAnimT();
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
                StartCoroutine(Ending());
            }
        }
        if(collision.CompareTag("PlayerItem"))
        {
            PlayerItem playerItem = collision.GetComponent<PlayerItem>();
            switch(playerItem.itemType)
            {
                case type.Speed:
                    if(!isItem) StartCoroutine(SpeedUp(maxItemDuration));
                    break;
                case type.Light:
                    if(!isItem) StartCoroutine(ExpandLight(maxItemDuration));
                    break;
                case type.Oxygen:
                    if(!isItem)
                    {
                        RechargeOx(playerItem);
                        StartCoroutine(OXcharge(3f));
                    }
                    break;
                case type.Compass:
                    if(!isItem) StartCoroutine(OnCompass(maxItemDuration));
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
            UIManager.instance.ButtonAnimF();
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
        bubble.gameObject.SetActive(true);
        isItem = true;
        itemGage.enabled = true;
        float original = Speed;
        Speed *= 2;
        yield return new WaitForSeconds(duration);
        Speed = original;
        itemGage.enabled = false;
        bubble.gameObject.SetActive(false);
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
    IEnumerator OXcharge(float duration)
    {
        int totalCount = 2;
        float timePerCount = totalCount / duration;
        for(int i = 0; i < totalCount; i++)
        {
            float escape = 0f;
            float halftime = timePerCount / 2f;
            while(escape < halftime)
            {
                escape += Time.deltaTime;
                bloom.intensity.value = Mathf.Lerp(1f, 10f, escape / halftime);
                yield return null;
            }
            escape = 0;
            while(escape < halftime)
            {
                escape += Time.deltaTime;
                bloom.intensity.value = Mathf.Lerp(10f, 1f, escape / halftime);
                yield return null;
            }
        }
        bloom.intensity.value = 0f;
    }

    IEnumerator OnCompass(float duraiton)
    {
        Transform targetObj = null;
        float minDistance = Mathf.Infinity;
        for(int i = 0; i < ItemSpawner.instance.itemSize; i++) // items가 List라고 가정
        {
            Transform currentItem = ItemSpawner.instance.location[i];
            if(currentItem != null)
            {
                Item itemScript = currentItem.GetComponent<Item>();
                if(itemScript != null && !itemScript.isPickUped)
                {
                    float dist = Vector2.Distance(transform.position, currentItem.transform.position);
                    if(dist < minDistance)
                    {
                        minDistance = dist;
                        targetObj = currentItem;
                    }
                }
            }
        }
        if(targetObj != null)
        {
            arrow.gameObject.SetActive(true);
            itemGage.enabled = true;
            float currentTime = 0f;
            while(currentTime < duraiton)
            {
                if(targetObj == null) break;
                currentTime += Time.deltaTime;
                Vector2 direction = targetObj.transform.position - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                arrow.rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
                yield return null;
            }
            itemGage.enabled = false;
            arrow.gameObject.SetActive(false);
        }
    }
    IEnumerator Ending()
    {
        yield return new WaitForSeconds(0.2f);
        UIManager.instance.succesText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        SceneController.instance.result = true;
        SceneController.instance.EndGame();
    }
    IEnumerator LoseEnding()
    {
        yield return new WaitForSeconds(1f);
        while(spotLight.intensity >= 0)
        {
            spotLight.intensity -= Time.deltaTime;
            yield return null;   
        }
        UIManager.instance.deathText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneController.instance.result = false;
        SceneController.instance.EndGame();
    }
}

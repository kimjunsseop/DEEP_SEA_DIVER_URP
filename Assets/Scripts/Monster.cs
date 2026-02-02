using System;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public bool isRight;
    public float attackPower;
    public int flag;
    public float moveSpeed = 2f;
    Animator anim;

    void Start()
    {
        if(transform.position.x < 0)
        {
            isRight = false;
            flag = 1;
        }
        else
        {
            isRight = true;
            flag = -1;
        }
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if(isRight == false)
        {
            if(transform.position.x > -2.5)
            {
                flag = -1;
                transform.Translate(new Vector3(flag, 0, 0) * moveSpeed * Time.deltaTime);
            }
            else if(transform.position.x < -8.5)
            {
                flag = 1;
                transform.Translate(new Vector3(flag, 0, 0) * moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(new Vector3(flag, 0, 0) * moveSpeed * Time.deltaTime);
            }
        }
        else
        {
            if(transform.position.x > 8.5)
            {
                flag = -1;
                transform.Translate(new Vector3(flag, 0, 0) * moveSpeed * Time.deltaTime);
            }
            else if(transform.position.x < 2.5)
            {
                flag = 1;
                transform.Translate(new Vector3(flag, 0, 0) * moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(new Vector3(flag, 0, 0) * moveSpeed * Time.deltaTime);
            }
        }
    }
    void SetAnim(int flag)
    {
        if(flag == 1)
        {
            anim.SetBool("flag", true);
        }
        else
        {
            anim.SetBool("flag", false);
        }
    }

    public void SetRight(bool input)
    {
        isRight = input;
        if(input == false)
        {
            flag = 1;
        }
        else
        {
            flag = -1;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}

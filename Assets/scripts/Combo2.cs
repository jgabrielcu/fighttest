using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combo2 : MonoBehaviour
{
    private Animator anim;

    private int cantClick;
    private bool canIClick;

    private void Start()
    {
        anim = GetComponent<Animator>();
        cantClick = 0;
        canIClick = true;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            IniciarCombo();
        }
    }

    private void IniciarCombo()
    {
        if (canIClick)
        {
            cantClick++;
        }

        if (cantClick == 1)
        {
            anim.SetInteger("Attack", 1);
        }
    }

    public void VerificarCombo()
    {
        canIClick = false;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1") && cantClick == 1)
        {
            anim.SetInteger("Attack", 0);
            canIClick = true;
            cantClick = 0;
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1") && cantClick >= 2)
        {
            anim.SetInteger("Attack", 2);
            canIClick = true;
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2") && cantClick == 2)
        {
            anim.SetInteger("Attack", 0);
            canIClick = true;
            cantClick = 0;
        }
    }
}

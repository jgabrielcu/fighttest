using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class Personaje : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float rotationSpeed = 100f;
    private Rigidbody rb;

    [Header("Animaciones")]
    public Animator anima;

    [Header("Bloqueo")]
    public KeyCode teclaBloqueo = KeyCode.LeftAlt;

    private bool isMoving = false;
    private bool isRunning = false;
    private bool isBlocking = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        //    return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        isRunning = Input.GetKey(KeyCode.LeftShift);
        isBlocking = Input.GetKey(teclaBloqueo);

        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        //Bloqueo y movimiento

        if (!isBlocking)
        {
            Vector3 movement = transform.forward * currentSpeed * verticalInput;
            Quaternion rotation = Quaternion.Euler(0f, horizontalInput * rotationSpeed * Time.deltaTime, 0f);

            rb.MovePosition(rb.position + movement * Time.deltaTime);
            rb.MoveRotation(rb.rotation * rotation);
        }

        // --- Animaciones ---
        // --- Bloqueo ---

        anima.SetBool("block", isBlocking);

        if (!isBlocking)
        {
            if (Mathf.Abs(verticalInput) > 0.1f)
            {
                anima.SetBool("walk", !isRunning);
                anima.SetBool("run", isRunning);
                isMoving = true;
            }
            else
            {
                if (isMoving)
                {
                    int randomIdle = Random.Range(1, 4);
                    anima.Play("idle" + randomIdle);
                }
                anima.SetBool("walk", false);
                anima.SetBool("run", false);
                isMoving = false;
            }
        }
        else
        {
            // cuando bloquea, no camina ni corre
            anima.SetBool("walk", false);
            anima.SetBool("run", false);
        }
    }

    public bool EstaBloqueando()
    {
        return isBlocking;
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Comport
{
    IDDLE,
    AIR,
    MOVE,
    CARGANDO

}


public class Player : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("Velocidad caminar")]
    public float walkSpeed;
    [Tooltip("Velocidad Correr")]
    public float runSpeed;
    [Tooltip("Velocidad de Rotacion")]
    public float RotationVelocity = 1f;
    [Tooltip("Fuerza de Salto")]
    public float jumpForce = 5;

    public Comport actions;

    private Rigidbody rb;
    public float currentSpeed;

    public float desiredRotationSpeed = 0.1f;

    public float rotationDegreePerSecond = 200f;

    public float life = 10;

    private Animator animator;
    Transform cameraT;
    public Vector3 inputDir;
    public Vector3 desiredMoveDirection;

    public bool blSoltarCaja = false;

    public GameObject boxPlace;
    

    // Start is called before the first frame update
    void Awake()
    {
        rb = transform.GetComponent<Rigidbody>();
        animator = transform.GetComponent<Animator>();
        cameraT = Camera.main.transform;
        actions = Comport.IDDLE;
    }

    // Update is called once per frame


    #region movement
    public virtual void Jump()
    {

        if (actions == Comport.AIR) return;
        actions = Comport.AIR;
        rb.velocity = Vector3.zero;
        rb.AddForce((transform.up) * jumpForce, ForceMode.Impulse);

    }

    internal void SoltarCaja()
    {
        actions = Comport.MOVE;
        boxPlace.transform.GetChild(0).transform.GetComponent<Rigidbody>().isKinematic = false;
        boxPlace.transform.GetChild(0).SetParent(null);
    }

    public virtual void Move(Vector3 dir)
    {




        inputDir = dir.normalized;

        bool running = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.z;

        currentSpeed = targetSpeed;

        Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0f, rotationDegreePerSecond * (dir.x < 0f ? -1f : 1f), 0f), Mathf.Abs(dir.x * RotationVelocity));
        Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
        transform.rotation = (transform.rotation * deltaRotation);

        //transform.position += transform.forward * currentSpeed * Time.deltaTime;

        animator.SetFloat("speed", currentSpeed);


    }

    internal void Activate(RaycastHit hit)
    {
        if(hit.transform.gameObject.layer == 9)
        {
            hit.transform.position = boxPlace.transform.position;
            hit.transform.SetParent(boxPlace.transform);
            actions = Comport.CARGANDO;
            hit.transform.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    public void getDamage(float count)
    {

        life -= count;
    }


    public virtual void Move(Vector3 dir, Transform cam)
    {


        float targetSpeed = 0f;
        inputDir = dir.normalized;

        bool running = Input.GetKey(KeyCode.LeftShift);

        Vector3 dirCamForward = new Vector3(cam.forward.x, 0, cam.forward.z);
        Vector3 dirCamRigth = new Vector3(cam.right.x, 0, cam.right.z);
        Vector3 sumNorm = Vector3.zero;




        Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0f, rotationDegreePerSecond * (dir.x < 0f ? -1f : 1f), 0f), Mathf.Abs(dir.x * RotationVelocity));
        Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);

        if (dir.normalized.z != 0)
        {

            targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.z;
            sumNorm += dirCamForward.normalized * dir.normalized.z;
        }

        if (dir.normalized.x != 0)
        {

            sumNorm += dirCamRigth.normalized * dir.normalized.x;
            targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.x;
        }

        if (dir != Vector3.zero)
        {
            Vector3 newDir = Vector3.RotateTowards(transform.forward, sumNorm, desiredRotationSpeed * Time.deltaTime, 0.0f);
            //transform.rotation = Quaternion.LookRotation(newDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(sumNorm), desiredRotationSpeed);
            //transform.transform.rotation = Quaternion.RotateTowards(transform.transform.rotation, Quaternion.LookRotation(dir), desiredRotationSpeed * Time.deltaTime);
        }
        currentSpeed = Mathf.Abs(targetSpeed) * Time.deltaTime;


        rb.MovePosition(transform.position + transform.forward * currentSpeed);

        animator.SetFloat("speed", Mathf.Abs(targetSpeed));


    }
    #endregion
    private void OnCollisionEnter(Collision collision)
    {
        if(actions == Comport.AIR) { 
           actions = Comport.IDDLE;
        }

    }

}
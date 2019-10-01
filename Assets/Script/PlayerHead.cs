using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;


public class PlayerHead : MonoBehaviour
{
    [Header("Control")]


    //public Text txtClick;
    
    public Camera cam;
    private Player hb;

    // Start is called before the first frame update
    void Start()
    {
        //txtClick.gameObject.SetActive(false);

        hb = transform.GetComponent<Player>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float left;
        float up;
        Vector3 vtr3 = Vector3.zero;


        left = Input.GetAxis("Horizontal");
        up = Input.GetAxis("Vertical");
        vtr3 = new Vector3(left, 0, up);

        hb.Move(vtr3, cam.transform);
        PressKey();


    }

    #region Buttons
    private void PressKey()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hb.Jump();

        }
    }




    #endregion




}
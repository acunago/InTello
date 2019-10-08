using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlatformState
{
    ACTIVE,
    DISABLED
}

public class PlatformScript : MonoBehaviour,IMechanic
{
    public List<GameObject> interact = new List<GameObject>();
    public PlatformState state;
    public int objects;
    public float objectsToChange = 1;
    public Color keepColour;
    //public SoundBag snd;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (objects < objectsToChange)
        {

            DisableElements();

        }
        else
        {
            ActiveElements();

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != 20)
        {
            objects = objects + 1;
        }

    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer != 20)
        {
            objects = objects - 1;

        }
    }


    public void SetActive()
    {
        if (state == PlatformState.ACTIVE)
        {
            state = PlatformState.DISABLED;


            DisableElements();


        }
        else
        {
            state = PlatformState.ACTIVE;


            ActiveElements();
        }

    }


    public void ActiveElements()
    {
        for (int i = 0; i < interact.Count; i++)
        {
            if (interact[i].GetComponent<IMechanic>()!=null)
            {
                interact[i].GetComponent<IMechanic>().ActiveElements();
            }
            else
            {
                interact[i].SetActive(false);
            }
        }


    }
    public void DisableElements()
    {
        for (int i = 0; i < interact.Count; i++)
        {
            if (interact[i].GetComponent<IMechanic>() != null)
            {
                interact[i].GetComponent<IMechanic>().DisableElements();
            }
            else
            {
                interact[i].SetActive(true);
            }
        }

    }
}

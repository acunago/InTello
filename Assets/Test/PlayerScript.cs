using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    public void Comer()
    {
        Debug.Log("Estoy llamando a la funcion Comer");
    }
    public void Cagar()
    {
        Debug.Log("Estoy llamando a la funcion Cagar");
    }
    public void Caminar()
    {
        Debug.Log("Estoy llamando a la funcion Caminar");
    }
    public void Fumar()
    {
        Debug.Log("Estoy llamando a la funcion Fumar");
    }
    public bool EstaComiendo()
    {
        Debug.Log("EstaComiendo");
        return true;
    }
    public bool EstaCaminando()
    {
        Debug.Log("EstaCaminando");
        return true;
    }
    public bool EstaFumando()
    {
        Debug.Log("EstaFumando");
        return true;
    }
}

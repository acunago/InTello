using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    Transform _previous;
    Transform _next;

    /// <summary>
    /// Define el transform del punto previo
    /// </summary>
    /// <param name="tr"></param>
    public void SetPrevious(Transform tr)
    {
        _previous = tr;
    }

    /// <summary>
    /// Define el transform del punto siguiente
    /// </summary>
    /// <param name="tr"></param>
    public void SetNext(Transform tr)
    {
        _next = tr;
    }

    /// <summary>
    /// Obtiene el transform del punto previo
    /// </summary>
    /// <returns></returns>
    public Transform GetPrevious()
    {
        return _previous;
    }

    /// <summary>
    /// Obtiene el transform del punto siguiente
    /// </summary>
    /// <returns></returns>
    public Transform GetNext()
    {
        return _next;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class CollisionDetecter : MonoBehaviour
{
    [SerializeField] private TriggerEvent onTriggerStay = new TriggerEvent();
    [SerializeField] private TriggerEvent onTriggerEnter = new TriggerEvent();

    private void OnTriggerStay2D(Collider2D collision)
    {
        onTriggerStay.Invoke(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        onTriggerEnter.Invoke(collision);
    }

    [Serializable]
    public class TriggerEvent: UnityEvent<Collider2D>
    {

    }
}

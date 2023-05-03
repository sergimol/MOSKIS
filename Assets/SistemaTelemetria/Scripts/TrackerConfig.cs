using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerConfig : MonoBehaviour
{
    // Configuración del tracker específica para el juego
    // Struct y array para elegir desde el editor que eventos se trackean
    [Serializable]
    public struct EventConfig
    {
        public string eventName;
        public bool isTracked;
    }
    [SerializeField]
    EventConfig[] eventConfig;

    // Diccionario para comprobar rápidamente si debe trackearse un evento durante ejecución
    public Dictionary<string, bool> eventsTracked = new Dictionary<string, bool>();

    private void Awake()
    {
        foreach(var config in eventConfig)
        {
            eventsTracked.Add(config.eventName, config.isTracked);
        }
    }
}

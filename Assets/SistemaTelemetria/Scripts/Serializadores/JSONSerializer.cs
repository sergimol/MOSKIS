using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public class JSONSerializer : ISerializer
{
    public override string Serialize(TrackerEvent e)
    {
        string cadena = e.toJSON();

        return cadena;
    }
}

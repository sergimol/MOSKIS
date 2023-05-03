using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

public class CSVSerializer : ISerializer
{
    public override string Serialize(TrackerEvent e)
    {
        string cadena = e.toCSV();

        // La devolvemos como string
        return cadena;
    }
}

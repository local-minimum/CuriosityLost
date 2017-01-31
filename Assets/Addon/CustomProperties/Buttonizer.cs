using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttonizer : System.Attribute {

    public string name;

    public Buttonizer(string name)
    {
        this.name = name;
    }

    public Buttonizer()
    {
        name = null;
    }
}

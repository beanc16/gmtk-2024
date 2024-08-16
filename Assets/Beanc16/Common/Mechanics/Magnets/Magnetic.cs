using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnetic : MonoBehaviour
{
    [SerializeField]
    private Polarity polarity = Polarity.Any;

    public Polarity Polarity { get => this.polarity; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    //All abilities share this behavior
    void Activate(int duplicateCount, Bank workBench);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkBenchSale
{
    //Purely to hold the info on a workbenchsale for the analytics manager
    //To be used with jsonutility.tojson so I can send the information in a post request
    public List<string> soldCardNames;
    public int totalPoints;
    public string timestamp;

    public WorkBenchSale(List<string> soldCardNames, int totalPoints)
    {
        this.soldCardNames = soldCardNames;
        this.totalPoints = totalPoints;
        this.timestamp = System.DateTime.UtcNow.ToString();
    }
}

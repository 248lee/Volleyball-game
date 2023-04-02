using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GluedBool : MonoBehaviour
{
    private bool v;
    private bool isGlued = false;
    public bool value()
    {
        return this.v;
    }
    public void ChangeValue(bool value)
    {
        if (!this.isGlued)
        {
            this.v = value;
        }
        else
            Debug.Log("This variable is glued now! Please double click this message and check why you attempt to change the glued bool!");
    }
    public void GluedChangeValue(bool value, float gluedTime)
    {
        if (!this.isGlued)
        {
            this.v = value;
            StartCoroutine(Glue(gluedTime));
        }
        else
            Debug.Log("This variable is glued now! Please double click this message and check why you attempt to change the glued bool!");

    }
    IEnumerator Glue(float gluedTime)
    {
        this.isGlued = true;
        yield return new WaitForSeconds(gluedTime);
        this.isGlued = false;
    }
}

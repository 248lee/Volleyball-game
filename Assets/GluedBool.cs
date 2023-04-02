using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

public class GluedBool
{
    private bool v;
    private bool isGlued = false;
    public bool value()
    {
        return this.v;
    }
    public void ChangeValue(bool value)
    {
        if (!isGlued)
        {
            this.v = value;
        }
        else
            Debug.Log("This variable is glued now! Please double click this message and check why you attempt to change the glued bool!");
    }
    public async Task GluedChangeValue(bool value, float time)
    {
        if (!isGlued)
        {
            isGlued = true;
            this.v = value;
            await Task.Delay((int)(time * 1000));
            isGlued = false;
        }
        else
            Debug.Log("This variable is glued now! Please double click this message and check why you attempt to change the glued bool!");
    }
}

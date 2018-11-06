using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplerInput : MonoBehaviour {

    [HideInInspector]
    public int playerNum;

    public virtual float GetHorizontalInput()
    {
        return 0f;
    }

    public virtual float GetVerticalInput()
    {
        return 0f;
    }

    public virtual bool GetUseButtonDown()
    {
        return false;
    }

    public virtual bool GetUseButtonUp()
    {
        return false;
    }

    public virtual bool ShouldLockRotationToCar()
    {
        return false;
    }

    public virtual CarAttachment GetAttachmentToUse()
    {
        return null;
    }
}

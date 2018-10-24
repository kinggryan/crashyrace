using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAttachment : MonoBehaviour {

    public bool continuous;

    private new Renderer renderer;
    protected Color unhighlightedColor = Color.yellow;
    protected Color highlightedColor = Color.cyan;
    protected Color usedColor = Color.green;

    private bool highlighted = false;
    public bool beingUsed { get; private set; }

	// Use this for initialization
	void Awake () {
        renderer = GetComponent<Renderer>();
        beingUsed = false;
    }

    private void Start()
    {
        renderer.material.color = unhighlightedColor;
    }

    public virtual bool IsUseable()
    {
        return true;
    }
    
    public virtual void Use(GrapplerCharacterController character)
    {
        renderer.material.color = usedColor;
        beingUsed = true;
        if(!continuous)
            StartCoroutine(EndUseInSeconds(0.3f));
    }

    public void Highlight()
    {
        highlighted = true;
        if (beingUsed)
            return;

        renderer.material.color = highlightedColor;
    }

    public void Unhighlight()
    {
        highlighted = false;
        if (beingUsed)
        {
            if (continuous)
                EndUse();
            return;
        }

        renderer.material.color = unhighlightedColor;
    }

    public void EndUseManual()
    {
        if (continuous)
            EndUse();
    }

    IEnumerator EndUseInSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        EndUse();
    }

    void EndUse()
    {
        beingUsed = false;
        if (highlighted)
        {
            Highlight();
        }
        else
        {
            Unhighlight();
        }
    }
}

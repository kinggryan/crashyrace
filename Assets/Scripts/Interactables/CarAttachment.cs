using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAttachment : MonoBehaviour {

    private new Renderer renderer;
    protected Color unhighlightedColor = Color.yellow;
    protected Color highlightedColor = Color.cyan;
    protected Color usedColor = Color.green;

    private bool highlighted = false;
    private bool beingUsed = false;

	// Use this for initialization
	void Awake () {
        renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        renderer.material.color = unhighlightedColor;
    }

    public virtual bool IsUseable()
    {
        return true;
    }
    
    public virtual void Use()
    {
        renderer.material.color = usedColor;
        beingUsed = true;
        StartCoroutine(EndUse());
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
            return;

        renderer.material.color = unhighlightedColor;
    }

    IEnumerator EndUse()
    {
        yield return new WaitForSeconds(0.3f);

        beingUsed = false;
        if(highlighted)
        {
            Highlight();
        } else
        {
            Unhighlight();
        }
    }
}

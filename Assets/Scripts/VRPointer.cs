/*using UnityEngine;
using Valve.VR.Extras;

// This is throwing a lot of spooky errors

public class VRPointer : SteamVR_LaserPointer
{
    public GameObject collider_prefab;
    ViewerRig parent;
    GameObject visible_collider;

    void Awake(){
        parent = GetComponentInParent(typeof(ViewerRig)) as ViewerRig;
        if(parent == null)
            Debug.LogError("Laser pointer needs to be on the hands, where the parent gameobject has a ViewerRig");
    }
	
    public override void OnPointerClick(PointerEventArgs e)
    {
        if(e.target == null) return;
        
        if(e.target.GetComponent(typeof(ViewerPose)) is ViewerPose vp){
            if(vp != null){
                Debug.Log("Clicked a pose!");
                parent.ChangePose(vp);
            }
        }
        else{
            Debug.Log("Clicked something else...");
        }
    }

    public override void OnPointerOut(PointerEventArgs e)
    {
        if(visible_collider != null){
            Debug.Log("Exited");
            GameObject temp = visible_collider;
            visible_collider = null;
            Destroy( temp );
        }
    }

    public override void OnPointerIn(PointerEventArgs e)
    {
        if(e.target == null) return;

        if(e.target.GetComponent(typeof(ViewerPose)) is ViewerPose vp){
            if(vp != null){
                Debug.Log("Entered pose");
                Collider c = e.target.GetComponent<Collider>();
                visible_collider = Instantiate(collider_prefab);
                visible_collider.transform.parent = e.target;
                visible_collider.transform.position = c.bounds.center;
                visible_collider.transform.localScale = c.bounds.size;
            }
        }
    }
 }*/

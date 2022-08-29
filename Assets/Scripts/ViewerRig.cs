using UnityEngine;

public class ViewerRig : MonoBehaviour
{
    SceneLoader loader;

    public ViewerAvatar current_avatar;

    // Start is called before the first frame update
    void Awake(){
        loader = new SceneLoader(this);
        DontDestroyOnLoad(this.gameObject);
    }

    public bool LoadWorldAsync(string uri){
		StartCoroutine( loader.DownloadAndLoad( uri ) );
		return true;
	}

    public void SetTransform(Transform t){
        transform.position = t.transform.position;
        transform.rotation = t.transform.rotation;
        transform.localScale = t.transform.localScale;

        if( current_avatar != null ){
            current_avatar.transform.position = t.transform.position;
            current_avatar.transform.rotation = t.transform.rotation;
            current_avatar.transform.localScale = t.transform.localScale;
        }
    }

    public void ChangePose(ViewerPose pose){
        SetTransform( pose.transform );
        current_avatar.SetController( pose.pose_controller );
    }
}
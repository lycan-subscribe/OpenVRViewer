using UnityEngine;
using UnityEngine.SceneManagement;

public class ViewerRig : MonoBehaviour
{
    SceneLoader loader;
    Camera head;

    ViewerAvatar current_avatar;
    bool world_avatar = true;
    ViewerPose current_pose;
    GameObject pose_viewpoint; // If current_pose is not null this shouldn't be either

    // Hacky stuff for changing the viewpoint on load
    private bool pose_changed = false;
    private int frame_count_on_scene_load = -10;


    // Awake is called before the first frame update
    void Awake(){
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        loader = new SceneLoader(this);
        head = GetComponentInChildren(typeof(Camera)) as Camera;
        

        // Set a default avatar?
    }


    // More hacky stuff for changing the viewpoint on load

    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        frame_count_on_scene_load = Time.frameCount;
    }

    void Update(){
        if( pose_changed ){
            ResetCameraToPose();
            if( Time.frameCount - frame_count_on_scene_load > 0 ){ // Load on animation frame 2
                pose_changed = false;

                // Then activate IK?
            }
        }
    }


    // World stuff

    public bool LoadWorldAsync(string uri){
		StartCoroutine( loader.DownloadAndLoad( uri ) );
		return true;
	}


    // Pose stuff, moving around

    public void SetTransform(Transform t, bool move_camera = false){
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
        current_pose = pose;
        pose_viewpoint = new GameObject("VIEWPOINT");
        pose_viewpoint.transform.parent = current_avatar.transform;
        pose_viewpoint.transform.localPosition = current_avatar.viewpoint;
        Transform head_bone = current_avatar.animator.GetBoneTransform(HumanBodyBones.Head);
        pose_viewpoint.transform.parent = head_bone;
        // Hacky solution for hiding the head bone (BREAKS DPS PARENT CONSTRAINTS)
        head_bone.localScale = new Vector3(0,0,0);

        current_avatar.SetController( pose.pose_controller );
        SetTransform(current_pose.transform); // Only need to set avatar pos
        pose_changed = true;
    }

    public void ResetCameraToPose(){
        // Move the camera to the pose viewpoint
        // So, move the origin to viewpoint + (origin - head)
        Debug.Log(head.transform.localPosition);
        transform.position = pose_viewpoint.transform.position - head.transform.localPosition;
        Quaternion target_rotation = pose_viewpoint.transform.rotation * Quaternion.Inverse( head.transform.localRotation );
        transform.rotation = Quaternion.LookRotation( target_rotation * Vector3.forward, Vector3.up );
    }


    // Avatar stuff

    // Pass with true if the user is changing to a custom avatar
    // False if the world is changing the avatar
    public void ChangeAvatar(ViewerAvatar a, bool force = false){
        if( world_avatar || force ){
            current_avatar = a;
            world_avatar = !force;
        }
    }
}
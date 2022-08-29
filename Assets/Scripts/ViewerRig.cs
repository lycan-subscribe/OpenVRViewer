using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

public class ViewerRig : MonoBehaviour
{
    SceneLoader loader;
    Camera head;

    ViewerAvatar current_avatar;
    bool world_avatar = true;
    ViewerPose current_pose;
    ConstraintSource head_source;
    GameObject head_constraint;
    GameObject avatar_viewpoint;

    // Hacky stuff for changing the viewpoint on load
    private bool pose_changed = false;
    private int frame_count_on_pose_change = -10;


    // Awake is called before the first frame update
    void Awake(){
        DontDestroyOnLoad(this.gameObject);
        //SceneManager.sceneLoaded += OnSceneLoaded;

        loader = new SceneLoader(this);
        head = GetComponentInChildren(typeof(Camera)) as Camera;

        // Set a default avatar?
    }


    // More hacky stuff for changing the viewpoint on load

    /*void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        frame_count_on_scene_load = Time.frameCount;
    }*/

    void Update(){
        if( pose_changed ){
            if( Time.frameCount - frame_count_on_pose_change > 1 ){ // Load on animation frame 2
                ResetCameraToPose();
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

        current_avatar.SetController( pose.pose_controller );
        SetTransform(current_pose.transform); // Only need to set avatar pos
        pose_changed = true;
        frame_count_on_pose_change = Time.frameCount;
    }

    public void ResetCameraToPose(){
        // Move the camera to the pose viewpoint
        // So, move the origin to viewpoint + (origin - head)
        Debug.Log(head.transform.localPosition);
        transform.position = avatar_viewpoint.transform.position - head.transform.localPosition;
        Quaternion target_rotation = avatar_viewpoint.transform.rotation * head.transform.localRotation;
        //transform.rotation = Quaternion.Euler( 0, target_rotation.eulerAngles.y, 0 );
        //transform.rotation = Quaternion.LookRotation( target_rotation * Vector3.forward, Vector3.up );
        transform.rotation = Quaternion.Euler(0, avatar_viewpoint.transform.rotation.eulerAngles.y - head.transform.localRotation.eulerAngles.y, 0 );
    }


    // Avatar stuff

    // Pass with true if the user is changing to a custom avatar
    // False if the world is changing the avatar
    public void ChangeAvatar(ViewerAvatar a, bool force = false){
        if( world_avatar || force ){
            current_avatar = a;
            world_avatar = !force;

            avatar_viewpoint = new GameObject("_VIEWPOINT");
            avatar_viewpoint.transform.parent = current_avatar.transform;
            avatar_viewpoint.transform.localPosition = current_avatar.viewpoint;
            Transform head_bone = current_avatar.animator.GetBoneTransform(HumanBodyBones.Head);
            
            // Hacky solution for hiding the head bone (BREAKS DPS PARENT CONSTRAINTS)
            head_constraint = new GameObject("_HEADBONE");
            PositionConstraint pc = head_constraint.AddComponent<PositionConstraint>() as PositionConstraint;
            RotationConstraint rc = head_constraint.AddComponent<RotationConstraint>() as RotationConstraint;
            head_source.sourceTransform = head_bone;
            head_source.weight = 1;
            pc.AddSource(head_source);
            head_constraint.transform.position = head_bone.position;
            pc.constraintActive = true;
            rc.AddSource(head_source);
            head_constraint.transform.rotation = head_bone.rotation;
            rc.constraintActive = true;

            avatar_viewpoint.transform.parent = head_constraint.transform;
            head_bone.localScale = new Vector3(0,0,0);
        }
    }
}
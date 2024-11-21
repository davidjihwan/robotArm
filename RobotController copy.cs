using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Axis{
    x=0,
    y=1,
    z=2
}

[System.Serializable]
public struct EnumFloatPair{
    
    public Axis axis;
    public float theta;

    public EnumFloatPair(Axis a, float t)
    {
        axis = a;
        theta = t;
    }
}

public class RobotController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] joints;
    [SerializeField]
    private Slider slider1;
    [SerializeField]
    private Slider slider2;
    [SerializeField]
    private Slider slider3;
    [SerializeField]
    private Slider slider4;
    [SerializeField]
    private int[] jointsIdx;
    [SerializeField]
    private SceneController sc;
    
    [SerializeField]
    public EnumFloatPair[] thetas;

    [SerializeField]
    private Vector3[] translations;
    
    private Matrix4x4[] frames;
    private int numTrans;
    private int numJoints;

    private bool framesUpdated = false;

    // Start is called before the first frame update
    void Start()
    {
        // if (joints.Length != thetas.Length)
        //     Debug.Log("joints length != thetas length");
        numTrans = thetas.Length;
        numJoints = joints.Length;
        frames = new Matrix4x4[numTrans];
        slider1.value = thetas[jointsIdx[0]].theta; 
        slider2.value = thetas[jointsIdx[1]].theta; 
        if (numJoints == 4){
            slider3.value = thetas[jointsIdx[2]].theta; 
            slider4.value = thetas[jointsIdx[3]].theta; 
        } else {
            slider3.gameObject.SetActive(false);
            slider4.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFrames(true);
    }

    void UpdateFrames(bool force_recompute=false){
        Debug.Assert(numTrans > 0);
        if (!framesUpdated || force_recompute){
            Matrix4x4[] transformations = new Matrix4x4[numTrans];
            for (int i = 0; i < numTrans; ++i){
                transformations[i] = RotationM(thetas[i])*TranslationM(translations[i]);
            }
            Matrix4x4 prev = Matrix4x4.identity;
            for (int i = 0; i < numTrans; ++i){
                prev = prev*transformations[i];
                frames[i] = prev;
            }
            SetJoints();
            framesUpdated = true;
        }
    }

    void SetJoints(){
        for (int i = 0; i < numJoints; ++i){
            Matrix4x4 frame = frames[jointsIdx[i]];
            Transform t = joints[i].transform;
            SetTransformFromMatrix(t, ref frame);
        }
    }

    Matrix4x4 RotationM(EnumFloatPair pair){
        Matrix4x4 m = Matrix4x4.identity;
        Axis axis = pair.axis;
        float theta = pair.theta;

        switch(axis){
            case Axis.x:
                m.SetRow(0, new Vector4(1f,0f,0f,0f));
                m.SetRow(1, new Vector4(0f,Mathf.Cos(theta),-Mathf.Sin(theta),0f));
                m.SetRow(2, new Vector4(0f,Mathf.Sin(theta),Mathf.Cos(theta),0f));
                m.SetRow(3, new Vector4(0f,0f,0f,1f));
                break;
            case Axis.y:
                m.SetRow(0, new Vector4(Mathf.Cos(theta),0f,Mathf.Sin(theta),0f));
                m.SetRow(1, new Vector4(0f,1f,0f,0f));
                m.SetRow(2, new Vector4(-Mathf.Sin(theta),0f,Mathf.Cos(theta),0f));
                m.SetRow(3, new Vector4(0f,0f,0f,1f));
                break;
            case Axis.z:
                m.SetRow(0, new Vector4(Mathf.Cos(theta),-Mathf.Sin(theta),0f,0f));
                m.SetRow(1, new Vector4(Mathf.Sin(theta),Mathf.Cos(theta),0f,0f));
                m.SetRow(2, new Vector4(0f,0f,1f,0f));
                m.SetRow(3, new Vector4(0f,0f,0f,1f));
                break;
            default:
                break;
        }

        return m;
    }

    Matrix4x4 TranslationM(Vector3 offset){
        Matrix4x4 m = Matrix4x4.identity;
        m.m03 = offset.x;
        m.m13 = offset.y;
        m.m23 = offset.z;
        return m;
    }

    public void UpdateThetas(){
        if (numJoints == 2){
            ChangeTheta(jointsIdx[0], new EnumFloatPair(thetas[jointsIdx[0]].axis, slider1.value));
            ChangeTheta(jointsIdx[1], new EnumFloatPair(thetas[jointsIdx[1]].axis, slider2.value));
        } else {
            ChangeTheta(jointsIdx[0], new EnumFloatPair(thetas[jointsIdx[0]].axis, slider1.value));
            ChangeTheta(jointsIdx[1], new EnumFloatPair(thetas[jointsIdx[1]].axis, slider2.value));
            ChangeTheta(jointsIdx[2], new EnumFloatPair(thetas[jointsIdx[2]].axis, slider3.value));
            ChangeTheta(jointsIdx[3], new EnumFloatPair(thetas[jointsIdx[3]].axis, slider4.value));
        }
    }

    private void ChangeTheta(int jointIdx, EnumFloatPair pair){
        thetas[jointIdx] = pair;
        framesUpdated = false;
    }


    // Code from numberkruncher: https://forum.unity.com/threads/how-to-assign-matrix4x4-to-transform.121966/
    /// <summary>
    /// Extract translation from transform matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <returns>
    /// Translation offset.
    /// </returns>
    public static Vector3 ExtractTranslationFromMatrix(ref Matrix4x4 matrix) {
        Vector3 translate;
        translate.x = matrix.m03;
        translate.y = matrix.m13;
        translate.z = matrix.m23;
        return translate;
    }
     
    /// <summary>
    /// Extract rotation quaternion from transform matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <returns>
    /// Quaternion representation of rotation transform.
    /// </returns>
    public static Quaternion ExtractRotationFromMatrix(ref Matrix4x4 matrix) {
        Vector3 forward;
        forward.x = matrix.m02;
        forward.y = matrix.m12;
        forward.z = matrix.m22;
     
        Vector3 upwards;
        upwards.x = matrix.m01;
        upwards.y = matrix.m11;
        upwards.z = matrix.m21;
     
        return Quaternion.LookRotation(forward, upwards);
    }
     
    /// <summary>
    /// Extract scale from transform matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <returns>
    /// Scale vector.
    /// </returns>
    public static Vector3 ExtractScaleFromMatrix(ref Matrix4x4 matrix) {
        Vector3 scale;
        scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
        scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
        scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
        return scale;
    }
     
    /// <summary>
    /// Extract position, rotation and scale from TRS matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <param name="localPosition">Output position.</param>
    /// <param name="localRotation">Output rotation.</param>
    /// <param name="localScale">Output scale.</param>
    public static void DecomposeMatrix(ref Matrix4x4 matrix, out Vector3 localPosition, out Quaternion localRotation, out Vector3 localScale) {
        localPosition = ExtractTranslationFromMatrix(ref matrix);
        localRotation = ExtractRotationFromMatrix(ref matrix);
        localScale = ExtractScaleFromMatrix(ref matrix);
    }
     
    /// <summary>
    /// Set transform component from TRS matrix.
    /// </summary>
    /// <param name="transform">Transform component.</param>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    public static void SetTransformFromMatrix(Transform transform, ref Matrix4x4 matrix) {
        transform.localPosition = ExtractTranslationFromMatrix(ref matrix);
        transform.localRotation = ExtractRotationFromMatrix(ref matrix);
        // transform.localScale = ExtractScaleFromMatrix(ref matrix);
    }
     
    // public static Matrix4x4 TranslationMatrix(Vector3 offset) {
    //     Matrix4x4 matrix = IdentityMatrix;
    //     matrix.m03 = offset.x;
    //     matrix.m13 = offset.y;
    //     matrix.m23 = offset.z;
    //     return matrix;
    // }

    // End of borrowed code

    

    
}

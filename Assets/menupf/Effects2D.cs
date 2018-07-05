using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Effects2D : MonoBehaviour
{

    /// <summary>
    /// 变大
    /// </summary>
    /// <param name="tr"></param>
    /// <param name="value"></param>
    public  void SizeUp( float value)
    {
        transform.localScale += new Vector3(value,value,0f);
    }

    /// <summary>
    /// 变小
    /// </summary>
    /// <param name="tr"></param>
    /// <param name="value"></param>
    public  void SizeDown( float value)
    {
        transform.localScale -= new Vector3(value, value, 0f);
    }
}

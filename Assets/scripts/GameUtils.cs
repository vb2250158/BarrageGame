using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 游戏工具类
/// </summary>
public class GameUtils
{
    /// <summary>
    /// 设置实例的个数,多出的会Destroy掉
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="prefab"></param>
    /// <param name="number"></param>
    /// <param name="parent"></param>
    public static void SetInstancesNumber<T>(List<T> list, T prefab, int number, Transform parent = null) where T : Component
    {
        if (number < 0)
        {
            number = 0;
        }

        while (list.Count > number)
        {
            GameObject.DestroyImmediate(list[list.Count - 1].gameObject);
            list.RemoveAt(list.Count - 1);
        }
        while (list.Count < number)
        {
            if (parent)
            {
                T instance = GameObject.Instantiate(prefab, parent);
                instance.name = prefab.name + " id:" + list.Count;
                list.Add(instance);
            }
            else
            {
                list.Add(GameObject.Instantiate(prefab));
            }
        }

    }

    /// <summary>
    /// 网络资源加载
    /// </summary>
    public class WWWLoad
    {
        /// <summary>
        /// 读取一张图片(协程)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onLoadEnd"></param>
        /// <returns></returns>
        public static IEnumerator LoadWWWImage(string path, Action<Texture2D, string> onLoadEnd)
        {
            WWW w = new WWW(path);
            yield return w;
            if (w.texture != null)
            {
                string[] lines = System.IO.File.ReadAllLines(path);
                string data = lines[lines.Length - 1];
                Texture2D texture = w.texture;// Sprite.Create(w.texture, new Rect(0, 0, w.texture.width, w.texture.height), new Vector2(0.5f, 0.5f));
                onLoadEnd(texture, data);
            }
            else
            {
                Debug.LogError("图片不存在");
            }
        }
    }

    /// <summary>
    /// 本地资源读写工具
    /// Android : 请确保File => PlayerSettings => OtherSettings => Write Permission : External
    /// </summary>
    public class IO
    {
        public class ImageIOEventData : EventArgs
        {
            public string path;
            public string data;
            public byte[] bytes;
        }

        /// <summary>
        /// 截屏,并且保存数据
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="path"></param>
        /// <param name="extendedData"></param>
        /// <param name="size">品质大小</param>
        /// <returns></returns>
        public static void SaveCaptureImageAsyn(Camera camera, string path, string extendedData, Action<ImageIOEventData> OnSaveEnd = null, float size = 1f)
        {
            int width = Screen.width;
            int height = Screen.height;

            width = (int)(width * size);
            height = (int)(height * size);

            RenderTexture rt = new RenderTexture(width, height, 0);

            camera.targetTexture = rt;
            camera.Render();

            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenShot.Apply();

            camera.targetTexture = null;
            RenderTexture.active = null;
            GameObject.Destroy(rt);

            byte[] bytes = Texture2DToBytes(screenShot);

            //创建线程
            System.Threading.Thread save = new System.Threading.Thread(() =>
            {
                List<byte> bytesEX = new List<byte>(bytes);
                bytesEX.AddRange(System.Text.Encoding.UTF8.GetBytes(extendedData));
                bytes = bytesEX.ToArray();
                string filename = path;
                System.IO.File.WriteAllBytes(filename, bytes);

                if (OnSaveEnd != null)
                {
                    OnSaveEnd(new ImageIOEventData() { bytes = bytes, data = extendedData, path = path });
                }
            });

            save.Start();
        }

        /// <summary>
        /// 异步读取图片(多线程)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="size"></param>
        /// <param name="onLoadEnd"></param>
        public static void LoadCaptureImageAsyn(string path, Action<ImageIOEventData> onLoadEnd)
        {
            System.Threading.Thread load = new System.Threading.Thread(() =>
            {
                byte[] datas = System.IO.File.ReadAllBytes(path);
                if (datas.Length > 0)
                {
                    string[] lines = System.IO.File.ReadAllLines(path);
                    string data = lines[lines.Length - 1];
                    onLoadEnd(new ImageIOEventData() { bytes = datas, data = data, path = path });
                }
                else
                {
                    Debug.LogError("图片不存在");
                }

            });
            load.Start();

        }

        public delegate void FileCheckCallback(string path);

        /// <summary>
        /// 文件校验,校验某个文件夹下的文件
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="paths"></param>
        /// <param name="onLack">相比实际情况，传进来的目录缺少的</param>
        /// <param name="onSurplus"></param>
        public static void FileCheck(string directory, List<string> paths, FileCheckCallback onLack, FileCheckCallback onSurplus)
        {
            List<string> thePaths = new List<string>(System.IO.Directory.GetFiles(directory));
            List<string> inPaths = new List<string>(paths);
            //查看缺少的
            foreach (var item in thePaths)
            {
                int index = inPaths.FindIndex((data) => { return data == item; });
                //如果在传进来的路径里面找不到实际路径就是缺少
                if (index == -1)
                {
                    if (onLack != null)
                    {
                        onLack(item);
                    }
                }
                //找到了，就在容器中移除，这样容器最后剩下的都是多出来的
                else
                {
                    inPaths.RemoveAt(index);
                }
            }
            //遍历剩余多出来的
            foreach (var item in inPaths)
            {
                if (onSurplus != null)
                {
                    onSurplus(item);
                }
            }
        }

        /// <summary>
        /// 读取一个目录下的所有图片(多线程)
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="onLoadEnd"></param>
        public static void LoadCaptureAllImageAsyn(string directory, Action<ImageIOEventData> onLoadEnd)
        {
            string[] directorys = System.IO.Directory.GetFiles(directory);
            Debug.Log(directorys.Length + ";" + directory);
            foreach (var item in directorys)
            {
                Debug.Log("LoadImage:" + item);
                LoadCaptureImageAsyn(item, onLoadEnd);
            }
        }


        /// <summary>
        /// byte转图片
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Texture2D BytesToTexture2D(byte[] data, int width, int height)
        {
            Texture2D texture2D = new Texture2D(width, height);
            texture2D.LoadImage(data);
            return texture2D;
        }

        /// <summary>
        /// 图片转byte
        /// </summary>
        /// <param name="texture2D"></param>
        /// <returns></returns>
        private static byte[] Texture2DToBytes(Texture2D texture2D)
        {
            return texture2D.EncodeToPNG();
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        public static void Delete(string path)
        {
            System.IO.File.Delete(path);
        }
    }

    /// <summary>
    /// 获取循环的索引
    /// 将值限制在0~max(不到)之间,以循环形式限定值，与求余不同，即负数会反向循环
    /// </summary>
    /// <param name="i"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    internal static int LoopIndex(int i, int max)
    {
        if (max <= 0)
        {
            return 0;
        }
        //为负数时
        if (i < 0)
        {
            return LoopIndex(i + max, max);
        }
        //为正数时
        else if (i > max - 1)
        {
            return LoopIndex(i - max, max);
        }

        return i;
    }
    /// <summary>
    /// 将值限制在min~max(不到)之间,以循环形式限定值
    /// </summary>
    /// <param name="i"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    internal static int LoopIndex(int i, int min, int max)
    {
        //为负数时
        if (i < min)
        {
            return LoopIndex(i + max, min, max);
        }
        //为正数时
        else if (i > max - 1)
        {
            return LoopIndex(i - max, min, max);
        }

        return i;
    }
    /// <summary>
    /// 获取列表内随机的一个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T GetRandomItem<T>(List<T> list)
    {
        if (list.Count == 0)
        {
            return default(T);
        }
        int min = 0;
        int max = list.Count;
        return list[Random.Range(min, max)];
    }

    /// <summary>
    /// 随机
    /// </summary>
    public class Random
    {
        public static int Range(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static float Range(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }
    }

}

#if UNITY_EDITOR
namespace UnityEditor
{
    public class GameUtilsEditor
    {
        /// <summary>
        /// 获取对象某个属性的值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="name">属性名</param>
        /// <returns>值</returns>
        public static object GetField(object obj, string name)
        {
            Type type = obj.GetType();
            return type.GetField(name);
        }
    }

    public static class UnityEditorEx
    {
        /// <summary>
        /// 查找属性
        /// </summary>
        /// <param name="property"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static SerializedProperty FindBaseOrSiblingProperty(this SerializedProperty property, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return null;

            SerializedProperty relativeProperty = property.serializedObject.FindProperty(propertyName); // baseProperty

            // If base property is not found, look for the sibling property.
            if (relativeProperty == null)
            {
                string propertyPath = property.propertyPath;
                int localPathLength = property.name.Length;

                string newPropertyPath = propertyPath.Remove(propertyPath.Length - localPathLength, localPathLength) + propertyName;
                relativeProperty = property.serializedObject.FindProperty(newPropertyPath);

                // If a direct sibling property was not found, try to find the sibling of the array.
                if (relativeProperty == null && property.isArray)
                {
                    int propertyPathLength = propertyPath.Length;

                    int dotCount = 0;
                    const int SiblingOfListDotCount = 3;
                    for (int i = 1; i < propertyPathLength; i++)
                    {
                        if (propertyPath[propertyPathLength - i] == '.')
                        {
                            dotCount++;
                            if (dotCount >= SiblingOfListDotCount)
                            {
                                localPathLength = i - 1;
                                break;
                            }
                        }
                    }

                    newPropertyPath = propertyPath.Remove(propertyPath.Length - localPathLength, localPathLength) + propertyName;
                    relativeProperty = property.serializedObject.FindProperty(newPropertyPath);
                }
            }

            return relativeProperty;
        }
    }


}
#endif

/// <summary>
/// List方法扩展
/// </summary>
public static class ListEx
{
    /// <summary>
    /// 获取随机的一个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T GetRandomItem<T>(this List<T> list)
    {
        if (list.Count == 0)
        {
            return default(T);
        }
        int min = 0;
        int max = list.Count;
        return list[UnityEngine.Random.Range(min, max)];
    }
}


/// <summary>
/// 实例容器，继承时手动序列化
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class InstanceContainer<T, D> : BaseContainer<T> where T : Component
{
    [SerializeField]
    private List<T> objects = new List<T>();

    public List<T> Objects
    {
        get
        {
            return objects;
        }
        set
        {
            objects = value;
        }
    }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="datas"></param>
    public void SetDatas(List<D> datas)
    {
        GameUtils.SetInstancesNumber(objects, prefab, datas.Count, parent);
        for (int i = 0; i < objects.Count; i++)
        {
            OnSetData(objects[i], datas[i]);
        }
    }

    /// <summary>
    /// 添加一个数据
    /// </summary>
    /// <param name="data"></param>
    public virtual T AddData(D data)
    {
        T item = CreateInstance();
        Objects.Add(item);
        OnSetData(item, data);
        return item;
    }

    public T GetRandomItem()
    {
        return Objects.GetRandomItem();
    }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="datas"></param>
    protected abstract void OnSetData(T item, D data);

    /// <summary>
    /// 清空对象
    /// </summary>
    public void Clear()
    {
        foreach (var item in Objects)
        {
            GameObject.Destroy(item.gameObject);
        }
        Objects.Clear();
    }

    public int Cout { get { return Objects.Count; } }

    public void RemoveObject(Predicate<T> match)
    {
        Objects.RemoveAll((item) =>
        {
            bool isRemove = match(item);
            if (isRemove)
            {
                GameObject.Destroy(item.gameObject);
            }
            return isRemove;
        });
    }
}


public abstract class BaseContainer<T> where T : Component
{
    public T prefab;

    public Transform parent;

    public T CreateInstance()
    {
        return GameObject.Instantiate(prefab, parent);
    }

    /// <summary>
    /// 存在为真
    /// </summary>
    /// <param name="baseContainer"></param>
    /// <returns></returns>
    public static bool operator true(BaseContainer<T> baseContainer)
    {
        return baseContainer != null;
    }

    /// <summary>
    /// 存在为真
    /// </summary>
    /// <param name="baseContainer"></param>
    /// <returns></returns>
    public static bool operator false(BaseContainer<T> baseContainer)
    {
        return baseContainer == null;
    }
}


/// <summary>
/// 循环链表(查找时永远不会超出索引)
/// </summary>
/// <typeparam name="T"></typeparam>
public class ListLoop<T> : IEnumerable<T>
{
    [SerializeField]
    private int index;

    public int Index { get { return index; } set { index = value; } }

    [SerializeField]
    private List<T> list = new List<T>();

    /// <summary>
    /// 获得当前对象
    /// </summary>
    public T Current
    {
        get
        {
            return this[Index];
        }
        set
        {
            this[Index] = value;
        }
    }

    /// <summary>
    /// 下一个对象
    /// </summary>
    /// <returns></returns>
    public T Next()
    {
        Index++;
        return this[Index];
    }

    /// <summary>
    /// 是否是最后一个
    /// </summary>
    public bool IsEndCount { get { return Index == list.Count - 1; } }

    public T this[int i]
    {
        get
        {
            Index = i;
            LoopIndex();
            return list[Index];
        }
        set
        {
            Index = i;
            LoopIndex();
            list[Index] = value;
        }
    }
    private void LoopIndex()
    {
        Index = GameUtils.LoopIndex(Index, list.Count);
    }
    public void Add(T obj)
    {
        list.Add(obj);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return list.GetEnumerator();
    }
}


/// <summary>
/// 滚动对象(待扩展，可以垂直或水平的循环滚动)
/// </summary>
public class RollObject : MonoBehaviour
{
    /// <summary>
    /// 滚动事件的类型
    /// </summary>
    public enum RollObjectEventType
    {
        Last, Next, End,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum RollObjectState
    {
        Max, Min, Normal
    }

    /// <summary>
    /// 滚动时的Unity事件
    /// </summary>
    [System.Serializable]
    public class RollObjectEvent : UnityEvent<RollObjectEventType>
    {

    }

    /// <summary>
    /// 当前是第几个在中间
    /// </summary>
    public int index = 0;

    /// <summary>
    /// 间隔
    /// </summary>
    public float spacing;

    /// <summary>
    /// 背景
    /// </summary>
    public Transform bg;

    /// <summary>
    /// 背景每移动该距离就会重置位置
    /// </summary>
    public int bgReSetSize;

    public Transform digiParent;

    private List<Transform> digiList = new List<Transform>();
    private Vector3 moveToPosi;

    public delegate void OnRollObject(RollObjectEventType eventType);

    public event OnRollObject OnRollObjectEvent;
    public RollObjectEvent rollObjectEvent;

    private RollObjectState rollObjectState;



    public RollObjectState RollObjectStateing { get { return rollObjectState; } }

    public void SetIndex(int index)
    {
        this.index = index;

        moveToPosi = digiParent.localPosition = spacing * Vector3.down * (index - 2);

        UpdateItemPosition();

    }

    public void OnEnd()
    {
        float offset = moveToPosi.y % spacing;

        if (offset < 0)
        {

            if (offset < -spacing / 2)
            {

                moveToPosi.y += -spacing - offset;
            }
            else
            {
                moveToPosi.y -= offset;
            }
        }
        else
        {
            if (offset < spacing / 2)
            {
                moveToPosi.y -= offset;
            }
            else
            {
                moveToPosi.y += spacing - offset;
            }
        }
        UpdateItemPosition();
        if (OnRollObjectEvent != null)
        {
            OnRollObjectEvent.Invoke(RollObjectEventType.End);
        }

    }

    public void AddSize(Vector3 addSize)
    {
        moveToPosi += addSize;
    }
    public void AddSize(Vector2 addSize)
    {
        moveToPosi += (Vector3)addSize;
    }
    public void AddYSize(float addSize)
    {
        moveToPosi.y += addSize;
    }
    /// <summary>
    /// 刷新内元素位置
    /// </summary>
    private void UpdateItemPosition()
    {
        foreach (var item in digiList)
        {
            if (item.localPosition.y + digiParent.localPosition.y < -spacing * digiList.Count / 2)
            {
                Vector3 posi = item.localPosition;
                posi.y += spacing * digiList.Count;
                item.localPosition = posi;
            }
            else if (item.localPosition.y + digiParent.localPosition.y > spacing * digiList.Count / 2)
            {
                Vector3 posi = item.localPosition;
                posi.y -= spacing * digiList.Count;
                item.localPosition = posi;
            }
        }
        int i = (int)((-moveToPosi.y / spacing) + 2);

        SetIndexData(i);
    }

    public void Last()
    {
        moveToPosi.y += spacing;
    }

    public void Next()
    {
        moveToPosi.y -= spacing;
    }

    /// <summary>
    /// 设置索引数据
    /// </summary>
    /// <param name="i"></param>
    private void SetIndexData(int i)
    {
        //循环索引
        int index = GameUtils.LoopIndex(i, digiList.Count);
        //限制1：索引限制，一样不需要赋值
        if (index == this.index)
        {
            return;
        }
        //限制2：位置限制，超过回弹点后进行赋值
        RollObjectEventType eventTyp;

        if (index == digiList.Count - 1 && this.index == 0)
        {
            eventTyp = RollObjectEventType.Last;
        }
        else if (index == 0 && this.index == digiList.Count - 1)
        {
            eventTyp = RollObjectEventType.Next;
        }
        else
        {
            eventTyp = this.index < index ? RollObjectEventType.Next : RollObjectEventType.Last;
        }
        if (OnRollObjectEvent != null)
        {
            OnRollObjectEvent.Invoke(eventTyp);
        }

        if (index == digiList.Count - 1)
        {
            rollObjectState = RollObjectState.Max;
        }
        else if (index == 0)
        {
            rollObjectState = RollObjectState.Min;
        }
        else
        {
            rollObjectState = RollObjectState.Normal;
        }




        this.index = index;
    }

    private void Update()
    {
        digiParent.localPosition = Vector3.Lerp(digiParent.localPosition, moveToPosi, Time.deltaTime * 10);
        if (bg)
        {
            float ySize = digiParent.localPosition.y;
            ySize %= bgReSetSize;
            bg.transform.localPosition = new Vector3(0, ySize, 0.05f);
        }
        UpdateItemPosition();
    }

    private void Awake()
    {
        digiParent.localPosition = Vector3.zero;
        moveToPosi = digiParent.localPosition;
        for (int i = 0; i < digiParent.childCount; i++)
        {
            Transform item = digiParent.GetChild(i);
            digiList.Add(item);
            item.localPosition = new Vector3(-0.08f, i * spacing - spacing, 0);
        }
        SetIndex(index);
    }

    private void OnEnable()
    {
        OnRollObjectEvent += RollObject_OnRollObjectEvent;
    }

    private void OnDisable()
    {
        OnRollObjectEvent -= RollObject_OnRollObjectEvent;
    }

    private void RollObject_OnRollObjectEvent(RollObjectEventType eventType)
    {
        rollObjectEvent.Invoke(eventType);
    }

}

/// <summary>
/// 该工具类取自迷失岛2
/// </summary>
namespace Isoland2
{
    public class Drawing
    {
        public enum Samples
        {
            None,
            Samples2,
            Samples4,
            Samples8,
            Samples16,
            Samples32,
            RotatedDisc
        }

        public static Samples NumSamples = Samples.Samples4;

        public static Texture2D DrawLine(Vector2 from, Vector2 to, float w, Color col, Texture2D tex)
        {
            return DrawLine(from, to, w, col, tex, false, Color.black, 0);
        }

        public static Texture2D DrawLine(Vector2 from, Vector2 to, float w, Color col, Texture2D tex, bool stroke, Color strokeCol, float strokeWidth)
        {
            w = Mathf.Round(w);//It is important to round the numbers otherwise it will mess up with the texture width
            strokeWidth = Mathf.Round(strokeWidth);

            var extent = w + strokeWidth;
            var stY = Mathf.Clamp(Mathf.Min(from.y, to.y) - extent, 0, tex.height);//This is the topmost Y value
            var stX = Mathf.Clamp(Mathf.Min(from.x, to.x) - extent, 0, tex.width);
            var endY = Mathf.Clamp(Mathf.Max(from.y, to.y) + extent, 0, tex.height);
            var endX = Mathf.Clamp(Mathf.Max(from.x, to.x) + extent, 0, tex.width);//This is the rightmost Y value

            strokeWidth = strokeWidth / 2;
            var strokeInner = (w - strokeWidth) * (w - strokeWidth);
            var strokeOuter = (w + strokeWidth) * (w + strokeWidth);
            var strokeOuter2 = (w + strokeWidth + 1) * (w + strokeWidth + 1);
            var sqrW = w * w;//It is much faster to calculate with squared values

            int lengthX = (int)(endX - stX);
            int lengthY = (int)(endY - stY);
            var start = new Vector2(stX, stY);
            Color[] pixels = tex.GetPixels((int)stX, (int)stY, (int)lengthX, (int)lengthY, 0);//Get all pixels

            for (var y = 0; y < lengthY; y++)
            {
                for (var x = 0; x < lengthX; x++)
                {//Loop through the pixels
                    var p = new Vector2(x, y) + start;
                    var center = p + new Vector2(0.5f, 0.5f);
                    float dist = (center - Mathfx.NearestPointStrict(from, to, center)).sqrMagnitude;//The squared distance from the center of the pixels to the nearest point on the line
                    if (dist <= strokeOuter2)
                    {
                        var samples = Sample(p);
                        var c = Color.black;
                        var pc = pixels[y * lengthX + x];
                        for (var i = 0; i < samples.Length; i++)
                        {//Loop through the samples
                            dist = (samples[i] - Mathfx.NearestPointStrict(from, to, samples[i])).sqrMagnitude;//The squared distance from the sample to the line
                            if (stroke)
                            {
                                if (dist <= strokeOuter && dist >= strokeInner)
                                {
                                    c += strokeCol;
                                }
                                else if (dist < sqrW)
                                {
                                    c += col;
                                }
                                else
                                {
                                    c += pc;
                                }
                            }
                            else
                            {
                                if (dist < sqrW)
                                {//Is the distance smaller than the width of the line
                                    c += col;
                                }
                                else
                                {
                                    c += pc;//No it wasn't, set it to be the original colour
                                }
                            }
                        }
                        c /= samples.Length;//Get the avarage colour
                        pixels[y * lengthX + x] = c;
                    }
                }
            }
            tex.SetPixels((int)stX, (int)stY, (int)lengthX, (int)lengthY, pixels, 0);
            tex.Apply();
            return tex;
        }


        public static Texture2D Paint(Vector2 pos, float rad, Color col, float hardness, Texture2D tex)
        {
            var start = new Vector2(Mathf.Clamp(pos.x - rad, 0, tex.width), Mathf.Clamp(pos.y - rad, 0, tex.height));
            var end = new Vector2(Mathf.Clamp(pos.x + rad, 0, tex.width), Mathf.Clamp(pos.y + rad, 0, tex.height));
            var widthX = Mathf.Round(end.x - start.x);
            var widthY = Mathf.Round(end.y - start.y);
            var sqrRad2 = (rad + 1) * (rad + 1);
            Color[] pixels = tex.GetPixels((int)start.x, (int)start.y, (int)widthX, (int)widthY, 0);

            for (var y = 0; y < widthY; y++)
            {
                for (var x = 0; x < widthX; x++)
                {
                    var p = new Vector2(x, y) + start;
                    var center = p + new Vector2(0.5f, 0.5f);
                    float dist = (center - pos).sqrMagnitude;
                    if (dist > sqrRad2)
                    {
                        continue;
                    }
                    var samples = Sample(p);
                    var c = Color.black;
                    for (var i = 0; i < samples.Length; i++)
                    {
                        dist = Mathfx.GaussFalloff(Vector2.Distance(samples[i], pos), rad) * hardness;
                        if (dist > 0)
                        {
                            c += Color.Lerp(pixels[(int)((float)y * widthX + x)], col, dist);
                        }
                        else
                        {
                            c += pixels[(int)((float)y * widthX + x)];
                        }
                    }
                    c /= samples.Length;

                    pixels[(int)((float)y * widthX + x)] = c;
                }
            }

            tex.SetPixels((int)start.x, (int)start.y, (int)widthX, (int)widthY, pixels, 0);
            return tex;
        }
        /// <summary>
        /// 画线
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="rad"></param>
        /// <param name="col"></param>
        /// <param name="hardness">值越小边缘越柔和,取值0~1</param>
        /// <param name="tex"></param>
        /// <returns></returns>
        public static bool PaintLine(Vector2 from, Vector2 to, float rad, Color col, float hardness, Texture2D tex)
        {
            var extent = rad;
            var stY = Mathf.Clamp(Mathf.Min(from.y, to.y) - extent, 0, tex.height);
            var stX = Mathf.Clamp(Mathf.Min(from.x, to.x) - extent, 0, tex.width);
            var endY = Mathf.Clamp(Mathf.Max(from.y, to.y) + extent, 0, tex.height);
            var endX = Mathf.Clamp(Mathf.Max(from.x, to.x) + extent, 0, tex.width);

            int lengthX = (int)(endX - stX);
            int lengthY = (int)(endY - stY);

            bool changed = false;
            var sqrRad2 = (rad + 1) * (rad + 1);
            Color[] pixels = tex.GetPixels((int)stX, (int)stY, (int)lengthX, (int)lengthY, 0);
            var start = new Vector2(stX, stY);
            //		Debug.Log (lengthX + "   " + lengthY + "   " + lengthX * lengthY);
            for (int y = 0; y < lengthY; y++)
            {
                for (int x = 0; x < lengthX; x++)
                {
                    var p = new Vector2(x, y) + start;
                    var center = p + new Vector2(0.5f, 0.5f);
                    float dist = (center - Mathfx.NearestPointStrict(from, to, center)).sqrMagnitude;
                    if (dist > sqrRad2)
                    {
                        continue;
                    }
                    //获得颜色的混合值,用于柔化边缘
                    dist = Mathfx.GaussFalloff(Mathf.Sqrt(dist), rad) * hardness;
                    Color c;
                    int pixelA = y * lengthX + x;
                    Color tempColor = pixels[pixelA];
                    if (dist > 0)
                    {
                        c = Color.Lerp(tempColor, col, dist);
                    }
                    else
                    {
                        c = tempColor;
                    }
                    if (!changed)
                    {
                        if (tempColor != c)
                        {
                            changed = true;
                        }
                    }
                    pixels[pixelA] = c;
                }
            }
            if (changed)
            {
                tex.SetPixels((int)start.x, (int)start.y, (int)lengthX, (int)lengthY, pixels, 0);
            }
            return changed;
        }

        public static void DrawBezier(BezierPoint[] points, float rad, Color col, Texture2D tex)
        {
            rad = Mathf.Round(rad);//It is important to round the numbers otherwise it will mess up with the texture width

            if (points.Length <= 1)
            {
                return;
            }

            Vector2 topleft = new Vector2(Mathf.Infinity, Mathf.Infinity);
            Vector2 bottomright = new Vector2(0, 0);

            for (var i = 0; i < points.Length - 1; i++)
            {
                BezierCurve curve = new BezierCurve(points[i].main, points[i].control2, points[i + 1].control1, points[i + 1].main);
                points[i].curve2 = curve;
                points[i + 1].curve1 = curve;

                topleft.x = Mathf.Min(topleft.x, curve.rect.x);

                topleft.y = Mathf.Min(topleft.y, curve.rect.y);

                bottomright.x = Mathf.Max(bottomright.x, curve.rect.x + curve.rect.width);

                bottomright.y = Mathf.Max(bottomright.y, curve.rect.y + curve.rect.height);
            }

            topleft -= new Vector2(rad, rad);
            bottomright += new Vector2(rad, rad);

            var start = new Vector2(Mathf.Clamp(topleft.x, 0, tex.width), Mathf.Clamp(topleft.y, 0, tex.height));
            var width = new Vector2(Mathf.Clamp(bottomright.x - topleft.x, 0, tex.width - start.x), Mathf.Clamp(bottomright.y - topleft.y, 0, tex.height - start.y));

            Color[] pixels = tex.GetPixels((int)start.x, (int)start.y, (int)width.x, (int)width.y, 0);

            for (int y = 0; y < width.y; y++)
            {
                for (int x = 0; x < width.x; x++)
                {
                    var p = new Vector2(x + start.x, y + start.y);
                    if (!Mathfx.IsNearBeziers(p, points, rad + 2))
                    {
                        continue;
                    }

                    var samples = Sample(p);
                    var c = Color.black;
                    var pc = pixels[(int)((float)y * width.x + x)];//Previous pixel color
                    for (var i = 0; i < samples.Length; i++)
                    {
                        if (Mathfx.IsNearBeziers(samples[i], points, rad))
                        {
                            c += col;
                        }
                        else
                        {
                            c += pc;
                        }
                    }

                    c /= samples.Length;

                    pixels[(int)((float)y * width.x + x)] = c;

                }
            }

            tex.SetPixels((int)start.x, (int)start.y, (int)width.x, (int)width.y, pixels, 0);
            tex.Apply();
        }

        public static Vector2[] Sample(Vector2 p)
        {
            switch (NumSamples)
            {
                case Samples.None:
                    return new Vector2[] { p + new Vector2(0.5f, 0.5f) };
                case Samples.Samples2:
                    return new Vector2[] { p + new Vector2(0.25f, 0.5f), p + new Vector2(0.75f, 0.5f) };
                case Samples.Samples4:
                    return new Vector2[] {
                p + new Vector2 (0.25f, 0.5f),
                p + new Vector2 (0.75f, 0.5f),
                p + new Vector2 (0.5f, 0.25f),
                p + new Vector2 (0.5f, 0.75f)
            };
                case Samples.Samples8:
                    return new Vector2[] {
                p + new Vector2 (0.25f, 0.5f),
                p + new Vector2 (0.75f, 0.5f),
                p + new Vector2 (0.5f, 0.25f),
                p + new Vector2 (0.5f, 0.75f),

                p + new Vector2 (0.25f, 0.25f),
                p + new Vector2 (0.75f, 0.25f),
                p + new Vector2 (0.25f, 0.75f),
                p + new Vector2 (0.75f, 0.75f)
            };
                case Samples.Samples16:
                    return new Vector2[] {
                p + new Vector2 (0, 0),
                p + new Vector2 (0.3f, 0),
                p + new Vector2 (0.7f, 0),
                p + new Vector2 (1, 0),

                p + new Vector2 (0, 0.3f),
                p + new Vector2 (0.3f, 0.3f),
                p + new Vector2 (0.7f, 0.3f),
                p + new Vector2 (1, 0.3f),

                p + new Vector2 (0, 0.7f),
                p + new Vector2 (0.3f, 0.7f),
                p + new Vector2 (0.7f, 0.7f),
                p + new Vector2 (1, 0.7f),

                p + new Vector2 (0, 1),
                p + new Vector2 (0.3f, 1),
                p + new Vector2 (0.7f, 1),
                p + new Vector2 (1, 1)
            };
                case Samples.Samples32:
                    return new Vector2[] {
                p + new Vector2 (0, 0),
                p + new Vector2 (1, 0),
                p + new Vector2 (0, 1),
                p + new Vector2 (1, 1),

                p + new Vector2 (0.2f, 0.2f),
                p + new Vector2 (0.4f, 0.2f),
                p + new Vector2 (0.6f, 0.2f),
                p + new Vector2 (0.8f, 0.2f),

                p + new Vector2 (0.2f, 0.4f),
                p + new Vector2 (0.4f, 0.4f),
                p + new Vector2 (0.6f, 0.4f),
                p + new Vector2 (0.8f, 0.4f),

                p + new Vector2 (0.2f, 0.6f),
                p + new Vector2 (0.4f, 0.6f),
                p + new Vector2 (0.6f, 0.6f),
                p + new Vector2 (0.8f, 0.6f),

                p + new Vector2 (0.2f, 0.8f),
                p + new Vector2 (0.4f, 0.8f),
                p + new Vector2 (0.6f, 0.8f),
                p + new Vector2 (0.8f, 0.8f),

                p + new Vector2 (0.5f, 0),
                p + new Vector2 (0.5f, 1),
                p + new Vector2 (0, 0.5f),
                p + new Vector2 (1, 0.5f),

                p + new Vector2 (0.5f, 0.5f)
            };
                case Samples.RotatedDisc:
                    return new Vector2[] {
                p + new Vector2 (0, 0),
                p + new Vector2 (1, 0),
                p + new Vector2 (0, 1),
                p + new Vector2 (1, 1),

                p + new Vector2 (0.5f, 0.5f) + new Vector2 (0.258f, 0.965f),//Sin (75°) && Cos (75°)
				p + new Vector2 (0.5f, 0.5f) + new Vector2 (-0.965f, -0.258f),
                p + new Vector2 (0.5f, 0.5f) + new Vector2 (0.965f, 0.258f),
                p + new Vector2 (0.5f, 0.5f) + new Vector2 (0.258f, -0.965f)
            };
                default:
                    return null;
            }
        }
    }


    public class BezierPoint
    {
        public Vector2 main;
        public Vector2 control1;
        //Think of as left
        public Vector2 control2;
        //Right
        //Rect rect;
        public BezierCurve curve1;
        //Left
        public BezierCurve curve2;
        //Right

        public BezierPoint(Vector2 m, Vector2 l, Vector2 r)
        {
            main = m;
            control1 = l;
            control2 = r;
        }
    }

    public class BezierCurve
    {
        public Vector2[] points;
        public float aproxLength;
        public Rect rect;

        public Vector2 Get(float t)
        {
            int t2 = (int)Mathf.Round(t * (points.Length - 1));
            return points[t2];
        }

        void Init(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {

            Vector2 topleft = new Vector2(Mathf.Infinity, Mathf.Infinity);
            Vector2 bottomright = new Vector2(Mathf.NegativeInfinity, Mathf.NegativeInfinity);

            topleft.x = Mathf.Min(topleft.x, p0.x);
            topleft.x = Mathf.Min(topleft.x, p1.x);
            topleft.x = Mathf.Min(topleft.x, p2.x);
            topleft.x = Mathf.Min(topleft.x, p3.x);

            topleft.y = Mathf.Min(topleft.y, p0.y);
            topleft.y = Mathf.Min(topleft.y, p1.y);
            topleft.y = Mathf.Min(topleft.y, p2.y);
            topleft.y = Mathf.Min(topleft.y, p3.y);

            bottomright.x = Mathf.Max(bottomright.x, p0.x);
            bottomright.x = Mathf.Max(bottomright.x, p1.x);
            bottomright.x = Mathf.Max(bottomright.x, p2.x);
            bottomright.x = Mathf.Max(bottomright.x, p3.x);

            bottomright.y = Mathf.Max(bottomright.y, p0.y);
            bottomright.y = Mathf.Max(bottomright.y, p1.y);
            bottomright.y = Mathf.Max(bottomright.y, p2.y);
            bottomright.y = Mathf.Max(bottomright.y, p3.y);

            rect = new Rect(topleft.x, topleft.y, bottomright.x - topleft.x, bottomright.y - topleft.y);


            List<Vector2> ps = new List<Vector2>();

            var point1 = Mathfx.CubicBezier(0, p0, p1, p2, p3);
            var point2 = Mathfx.CubicBezier(0.05f, p0, p1, p2, p3);
            var point3 = Mathfx.CubicBezier(0.1f, p0, p1, p2, p3);
            var point4 = Mathfx.CubicBezier(0.15f, p0, p1, p2, p3);

            var point5 = Mathfx.CubicBezier(0.5f, p0, p1, p2, p3);
            var point6 = Mathfx.CubicBezier(0.55f, p0, p1, p2, p3);
            var point7 = Mathfx.CubicBezier(0.6f, p0, p1, p2, p3);

            aproxLength = Vector2.Distance(point1, point2) + Vector2.Distance(point2, point3) + Vector2.Distance(point3, point4) + Vector2.Distance(point5, point6) + Vector2.Distance(point6, point7);

            Debug.Log(Vector2.Distance(point1, point2) + "     " + Vector2.Distance(point3, point4) + "   " + Vector2.Distance(point6, point7));
            aproxLength *= 4;

            float a2 = 0.5f / aproxLength;//Double the amount of points since the aproximation is quite bad
            for (float i = 0; i < 1; i += a2)
            {
                ps.Add(Mathfx.CubicBezier(i, p0, p1, p2, p3));
            }

            points = ps.ToArray();
        }

        public BezierCurve(Vector2 main, Vector2 control1, Vector2 control2, Vector2 end)
        {
            Init(main, control1, control2, end);
        }
    }



    public class Mathfx
    {

        public static float Hermite(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));
        }

        public static float Sinerp(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
        }

        public static float Coserp(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f));
        }

        public static float Berp(float start, float end, float value)
        {
            value = Mathf.Clamp01(value);
            value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) *
                Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
            return start + (end - start) * value;
        }

        public static float SmoothStep(float x, float min, float max)
        {
            x = Mathf.Clamp(x, min, max);
            var v1 = (x - min) / (max - min);
            var v2 = (x - min) / (max - min);
            return -2f * v1 * v1 * v1 + 3f * v2 * v2;
        }

        public static float Lerp(float start, float end, float value)
        {
            return ((1.0f - value) * start) + (value * end);
        }

        public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            var lineDirection = Normalize(lineEnd - lineStart);
            var closestPoint = Vector3.Dot((point - lineStart), lineDirection) / Vector3.Dot(lineDirection, lineDirection);
            return (Vector3)lineStart + (Vector3)(closestPoint * lineDirection);
        }

        public static Vector3 NearestPointStrict(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            var fullDirection = lineEnd - lineStart;
            var lineDirection = Normalize(fullDirection);
            var closestPoint = Vector3.Dot((point - lineStart), lineDirection) / Vector3.Dot(lineDirection, lineDirection);
            return (Vector3)lineStart + (Vector3)(Mathf.Clamp(closestPoint, 0.0f, Vector3.Magnitude(fullDirection)) * lineDirection);
        }

        public static Vector2 NearestPointStrict(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
        {
            var fullDirection = lineEnd - lineStart;
            var lineDirection = Normalize(fullDirection);
            var closestPoint = Vector2.Dot((point - lineStart), lineDirection) / Vector2.Dot(lineDirection, lineDirection);
            return lineStart + (Mathf.Clamp(closestPoint, 0.0f, fullDirection.magnitude) * lineDirection);
        }

        public static float Bounce(float x)
        {
            return Mathf.Abs(Mathf.Sin(6.28f * (x + 1f) * (x + 1f)) * (1f - x));
        }

        // test for value that is near specified float (due to floating point inprecision)
        // all thanks to Opless for this!
        public static bool Approx(float val, float about, float range)
        {
            return ((Mathf.Abs(val - about) < range));
        }

        // test if a Vector3 is close to another Vector3 (due to floating point inprecision)
        // compares the square of the distance to the square of the range as this 
        // avoids calculating a square root which is much slower than squaring the range
        public static bool Approx(Vector3 val, Vector3 about, float range)
        {
            return ((val - about).sqrMagnitude < range * range);
        }
        public static float GaussFalloff(float distance, float inRadius)
        {
            return Mathf.Clamp01(Mathf.Pow(360.0f, -Mathf.Pow(distance / inRadius, 2.5f) - 0.01f));
        }
        // CLerp - Circular Lerp - is like lerp but handles the wraparound from 0 to 360.
        // This is useful when interpolating eulerAngles and the object
        // crosses the 0/360 boundary.  The standard Lerp function causes the object
        // to rotate in the wrong direction and looks stupid. Clerp fixes that.
        public static float Clerp(float start, float end, float value)
        {
            var min = 0.0f;
            var max = 360.0f;
            var half = Mathf.Abs((max - min) / 2.0f);//half the distance between min and max
            var retval = 0.0f;
            var diff = 0.0f;

            if ((end - start) < -half)
            {
                diff = ((max - start) + end) * value;
                retval = start + diff;
            }
            else if ((end - start) > half)
            {
                diff = -((max - end) + start) * value;
                retval = start + diff;
            }
            else retval = start + (end - start) * value;

            return retval;
        }


        //======= NEW =========//


        public static Vector2 RotateVector(Vector2 vector, float rad)
        {
            rad *= Mathf.Deg2Rad;
            return new Vector2((vector.x * Mathf.Cos(rad)) - (vector.y * Mathf.Sin(rad)), (vector.x * Mathf.Sin(rad)) + (vector.y * Mathf.Cos(rad)));
        }

        public static Vector2 IntersectPoint(Vector2 start1, Vector2 start2, Vector2 dir1, Vector2 dir2)
        {
            if (dir1.x == dir2.x)
            {
                return Vector2.zero;
            }

            var h1 = dir1.y / dir1.x;
            var h2 = dir2.y / dir2.x;

            if (h1 == h2)
            {
                return Vector2.zero;
            }

            var line1 = new Vector2(h1, start1.y - start1.x * h1);
            var line2 = new Vector2(h2, start2.y - start2.x * h2);

            var y1 = line2.y - line1.y;
            var x1 = line1.x - line2.x;

            var x2 = y1 / x1;

            var y2 = line1.x * x2 + line1.y;
            return new Vector2(x2, y2);
        }

        public static Vector2 ThreePointCircle(Vector2 a1, Vector2 a2, Vector2 a3)
        {
            var dir = a2 - a1;
            dir /= 2;
            var b1 = a1 + dir;
            dir = RotateVector(dir, 90);
            var l1 = dir;

            dir = a3 - a2;
            dir /= 2;
            var b2 = a2 + dir;
            dir = RotateVector(dir, 90);
            var l2 = dir;
            var p = IntersectPoint(b1, b2, l1, l2);
            return p;
        }

        //===== Bezier ====== //

        public static Vector2 CubicBezier(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            t = Mathf.Clamp01(t);
            var t2 = 1 - t;
            return Mathf.Pow(t2, 3) * p0 + 3 * Mathf.Pow(t2, 2) * t * p1 + 3 * t2 * Mathf.Pow(t, 2) * p2 + Mathf.Pow(t, 3) * p3;
        }

        public static Vector2 NearestPointOnBezier(Vector2 p, BezierCurve c, float accuracy, bool doubleAc)
        {
            float minDist = Mathf.Infinity;
            float minT = 0;
            Vector2 minP = Vector2.zero;
            for (float i = 0; i < 1; i += accuracy)
            {
                var point = c.Get(i);
                float d = (p - point).sqrMagnitude;
                if (d < minDist)
                {
                    minDist = d;
                    minT = i;
                    minP = point;
                }
            }

            if (!doubleAc)
            {
                return minP;
            }

            float st = Mathf.Clamp01(minT - accuracy);
            float en = Mathf.Clamp01(minT + accuracy);


            for (var i = st; i < en; i += accuracy / 10)
            {
                var point = c.Get(i);
                var d = (p - point).sqrMagnitude;
                if (d < minDist)
                {
                    minDist = d;
                    minT = i;
                    minP = point;
                }
            }


            return minP;
        }

        public static bool IsNearBezierTest(Vector2 p, BezierCurve c, float accuracy, float maxDist)
        {
            Vector2 prepoint = c.Get(0);
            for (float i = accuracy; i < 1; i += accuracy)
            {
                var point = c.Get(i);
                float d = (p - point).sqrMagnitude;
                float d2 = (prepoint - point + new Vector2(maxDist, maxDist)).sqrMagnitude;
                if (d <= d2 * 2)
                {
                    return true;
                }
            }

            return false;
        }

        public static Vector2 NearestPointOnBezier(Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            var minDist = Mathf.Infinity;
            var minT = 0f;
            Vector2 minP = Vector2.zero;
            for (float i = 0f; i < 1f; i += 0.01f)
            {
                var point = CubicBezier(i, p0, p1, p2, p3);
                float d = (p - point).sqrMagnitude;
                if (d < minDist)
                {
                    minDist = d;
                    minT = i;
                    minP = point;
                }
            }

            float st = Mathf.Clamp01(minT - 0.01f);
            float en = Mathf.Clamp01(minT + 0.01f);

            for (float i = st; i < en; i += 0.001f)
            {
                var point = CubicBezier(i, p0, p1, p2, p3);
                var d = (p - point).sqrMagnitude;
                if (d < minDist)
                {
                    minDist = d;
                    minT = i;
                    minP = point;
                }
            }

            return minP;

        }

        public static bool IsNearBezier(Vector2 p, BezierPoint point1, BezierPoint point2, float rad)
        {
            if (point1.curve2 != point2.curve1)
            {
                Debug.LogError("Curves Not The Same");
                return false;
            }

            BezierCurve curve = point1.curve2;

            var r = curve.rect;
            r.x -= rad;
            r.y -= rad;
            r.width += rad * 2;
            r.height += rad * 2;

            if (!r.Contains(p))
            {
                return false;
            }

            var nearest = NearestPointOnBezier(p, curve, 0.1f, false);

            var sec = point1.curve2.aproxLength / 10;

            if ((nearest - p).sqrMagnitude >= (sec * 3) * (sec * 3))
            {
                return false;
            }

            nearest = NearestPointOnBezier(p, curve, 0.01f, true);

            if ((nearest - p).sqrMagnitude <= rad * rad)
            {
                return true;
            }

            return false;
        }

        public static bool IsNearBeziers(Vector2 p, BezierPoint[] points, float rad)
        {
            for (var i = 0; i < points.Length - 1; i++)
            {
                if (IsNearBezier(p, points[i], points[i + 1], rad))
                {
                    return true;
                }
            }
            return false;
        }

        //====== End Bezier ========//

        public static Vector2 NearestPointOnCircle(Vector2 p, Vector2 center, float w)
        {
            var dir = p - center;
            dir = Normalize(dir);
            dir *= w;
            return center + dir;
        }
        public static Vector2 Normalize(Vector2 p)
        {
            float mag = p.magnitude;
            return p / mag;
        }
    }
}

/// <summary>
/// GUI工具组件扩展
/// </summary>
public class GUILayoutEX
{
    private static readonly int lineSize = 4;
    public static List<bool> opens = new List<bool>();

    public static int PopupIndex { get; set; }

    /// <summary>
    /// 选择框组件
    /// </summary>
    /// <param name="conditionIndex">当前选择的索引</param>
    /// <param name="v">所有选项</param>
    /// <returns></returns>
    internal static int Popup(int conditionIndex, string[] v)
    {

        if (opens.Count <= PopupIndex)
        {
            opens.Add(false);
        }

        int index = conditionIndex;

        GUILayout.BeginVertical();

        //如果是打开状态在原点显示数据
        if (opens[PopupIndex])
        {
            for (int i = 0; i < v.Length; i++)
            {
                if (i == index)
                {
                    GUILayout.Label('[' + v[i] + ']');
                }

            }
        }
        else
        {

        }

        for (int i = 0; i < v.Length; i++)
        {

            if (i % lineSize == 0)
            {
                GUILayout.BeginHorizontal();
            }

            if (opens[PopupIndex])
            {
                if (GUILayout.Button(v[i]))
                {
                    opens[PopupIndex] = false;
                    index = i;
                }

            }
            else if (i == index && !opens[PopupIndex])
            {

                if (GUILayout.Button(v[i]))
                {
                    opens[PopupIndex] = true;
                }
            }

            //分行列末
            if (i % lineSize == lineSize - 1)
            {
                GUILayout.EndHorizontal();
            }
        }

        if (v.Length % lineSize != 0)
        {
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();

        PopupIndex++;

        return index;
    }
}

/// <summary>
/// GUI内容组
/// </summary>
[System.Serializable]
public class GUIContentGroup
{
    public List<GUIContentItem> contentItems;

    public void OnGUI()
    {
        foreach (var item in contentItems)
        {
            if (GUILayout.Button(item.name))
            {
                item.isOpen = !item.isOpen;
            }
            if (item.isOpen)
            {
                item.OnGUI();
            }
        }
    }

    internal void AddItem(string name, Action onGUI)
    {
        contentItems.Add(new GUIContentItem(name, onGUI));
    }
}

/// <summary>
/// GUI内容项
/// </summary>
[System.Serializable]
public class GUIContentItem
{
    public string name;
    private Action onGUI;
    public bool isOpen;
    public GUIContentItem(string name, Action onGUI)
    {
        this.name = name;
        this.onGUI = onGUI;
    }

    internal void OnGUI()
    {
        if (onGUI != null)
        {

            GUILayout.BeginHorizontal();
            onGUI();
            GUILayout.EndHorizontal();
        }
    }
}
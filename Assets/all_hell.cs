using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
//[assembly: CLSCompliant(true)]
public class all_hell : MonoBehaviour {
    GameObject Camera_1;
    public GameObject GameObject;
    public int size;
    public int type;
    float Cam_po_y;
    public int speedper;
    public float speed;
    public int direction;
    public float perdis;
    public float weight;
    public int bri;
    public int sat;
    public int con;
    //[Range(0.0f, 3.0f)]
    static public float brightness;

    //[Range(0.0f, 3.0f)]
    static public float saturation;

    //[Range(0.0f, 3.0f)]
    static public float contrast;
    // Use this for initialization
    protected void CheckResources()
    {
        bool isSupported = CheckSupport();

        if (isSupported == false)
        {
            NotSupported();
        }
    }

    protected bool CheckSupport()
    {
        if (SystemInfo.supportsImageEffects == false)
        {
            Debug.LogWarning("This platform does not support image effects or render textures.");
            return false;
        }
        return true;
    }

    protected void NotSupported()
    {
        enabled = false;
    }
    void Start()
    {
        speedper = 5;
        size = 2;
        direction = 0;
        type = 3;
        bri = 75;
        sat = 25;
        con = 25;
        brigh();
        cacul();
        copydata();
        CheckResources();
        Camera_1 = GameObject.Find("Main Camera");
        GameObject = GameObject.Find("GameObject");
        movepic();
        changetype();
        //Screen.fullScreen = true;
        //Display.displays[1].Activate();
        //Display.displays[0].Deactivate();
        HookLoad();//安装钩子
    }
    void OnApplicationQuit()
    {
        HookClosing();//移除钩子
    }
    protected Material CheckShaderAndCreateMaterial(Shader shader, Material material)
    {
        if (shader == null)
        {
            return null;
        }

        if (shader.isSupported && material && material.shader == shader)
        {
            return material;
        }

        if (!shader.isSupported)
        {
            return null;
        }
        else
        {
            material = new Material(shader);
            material.hideFlags = HideFlags.DontSave;
            if (material)
                return material;
            else
                return null;
        }
    }

    public Shader briSatConShader;
    private Material briSatConMaterial;
    public Material material
    {
        get
        {
            briSatConMaterial = CheckShaderAndCreateMaterial(briSatConShader, briSatConMaterial);
            return briSatConMaterial;
        }
    }



    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null)
        {

            material.SetFloat("_Brightness", brightness);
            material.SetFloat("_Saturation", saturation);
            material.SetFloat("_Contrast", contrast);
            Graphics.Blit(src, dest, material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
    //钩子接收消息的结构  
    public struct CWPSTRUCT { public int lparam; public int wparam; public uint message; public IntPtr hwnd; }

    //建立钩子  
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, uint dwThreadId);

    //移除钩子  
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern bool UnhookWindowsHookEx(int idHook);

    //把信息传递到下一个监听  
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern int CallNextHookEx(int idHook, int nCode, int wParam, int lParam);
    //回调委托  
    private delegate int HookProc(int nCode, int wParam, int lParam);
    //钩子  
    int idHook = 0;
    //是否安装了钩子  
    bool isHook = false;
    GCHandle gc;
    private const int WH_CALLWNDPROC = 4;  //钩子类型 全局钩子
    public struct COPYDATASTRUCT
    {
        public int dwData;    //not used  
        public int cbData;    //长度  
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpData;
    }
    void OnDestroy()
    {
        //关闭钩子  
        HookClosing();
    }
    private void HookLoad()
    {
        //Debug.Log("开始运行");
        //安装钩子  
        {
            //钩子委托  
            HookProc lpfn = new HookProc(Hook);
            //关联进程的主模块  
            IntPtr hInstance = IntPtr.Zero;// GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);  
            idHook = SetWindowsHookEx(WH_CALLWNDPROC, lpfn, hInstance, (uint)AppDomain.GetCurrentThreadId());
            if (idHook > 0)
            {
                Debug.Log("钩子[" + idHook + "]安装成功");
                isHook = true;
                //保持活动 避免 回调过程 被垃圾回收  
                gc = GCHandle.Alloc(lpfn);
                //Debug.Log("ccccccccccccc"+gc);
            }
            else
            {
                Debug.Log("钩子安装失败");
                isHook = false;
                UnhookWindowsHookEx(idHook);
            }
        }
    }
    //卸载钩子  
    private void HookClosing()
    {
        if (isHook)
        {
            UnhookWindowsHookEx(idHook);
        }
    }

    private bool _bCallNext;
    public bool CallNextProc
    {
        get { return _bCallNext; }
        set { _bCallNext = value; }
    }
    //jison格式//属性的名字，必须与json格式字符串中的"key"值一样。
    public struct getdate
    {
        public int direction { get; set; }  
        public int size { get; set; }
        public int speed { get; set; }
        public int type { get; set; }
        public int bri { get; set; }
        public int sat { get; set; }
        public int con { get; set; }
    }
    private unsafe int Hook(int nCode, int wParam, int lParam)
    {

        try
        {
            IntPtr p = new IntPtr(lParam);
            CWPSTRUCT m = (CWPSTRUCT)Marshal.PtrToStructure(p, typeof(CWPSTRUCT));

            if (m.message == 74)
            {
                COPYDATASTRUCT entries = (COPYDATASTRUCT)Marshal.PtrToStructure((IntPtr)m.lparam, typeof(COPYDATASTRUCT));
                string str = entries.lpData;
                getdate list = JsonConvert.DeserializeObject<getdate>(str);
                direction = list.direction;
                size = list.size;
                speedper = list.speed;
                type = list.type;
                bri = list.bri;
                sat = list.sat;
                con = list.con;
                brigh();
                copydata();
                cacul();
                movepic();
                changetype();
                Debug.Log("direction:   " + direction + "  size:  " + size + "  speed:  " + speedper + "  type:  " + type);
                Debug.Log("sssss: " + str);

            }

            if (CallNextProc)
            {
                return CallNextHookEx(idHook, nCode, wParam, lParam);
            }
            else
            {
                return CallNextHookEx(idHook, nCode, wParam, lParam);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            return 0;
        }

    }
    public class ImportFromDLL
    {
        public const int WM_COPYDATA = 0x004A;

        //启用非托管代码    
        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public int dwData;    //not used    
            public int cbData;    //长度    
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        [DllImport("User32.dll")]
        public static extern int SendMessage(
            IntPtr hWnd,     // handle to destination window     
            int Msg,         // message    
            IntPtr wParam,    // first message parameter     
            ref COPYDATASTRUCT pcd // second message parameter     
        );

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("Kernel32.dll", EntryPoint = "GetConsoleWindow")]
        public static extern IntPtr GetConsoleWindow();
    }
    void copydata()
    {
        string strDlgTitle = "Experiment_Control";
        //Debug.Log("direction: " + direction);
        //接收端的窗口句柄    
        IntPtr hwndRecvWindow = ImportFromDLL.FindWindow(null, strDlgTitle);
        if (hwndRecvWindow == IntPtr.Zero)
        {
            Debug.Log("请先启动接收消息程序");
            return;
        }
        Debug.Log(strDlgTitle);
        //自己的窗口句柄    
        IntPtr hwndSendWindow = ImportFromDLL.GetConsoleWindow();
       var data = new getdate
        {
            direction = direction,
            size = size,
            speed = speedper,
            type = type,
            bri = bri,
            sat = sat,
            con = con
       };
        var serializedData = JsonConvert.SerializeObject(data);
        string strText = serializedData;
        //string strText = "1234";
        //填充COPYDATA结构    
        ImportFromDLL.COPYDATASTRUCT copydata = new ImportFromDLL.COPYDATASTRUCT();
        copydata.cbData = Encoding.Default.GetBytes(strText).Length; //长度 注意不要用strText.Length;    
        copydata.lpData = strText;                                   //内容    

        ImportFromDLL.SendMessage(hwndRecvWindow, ImportFromDLL.WM_COPYDATA, hwndSendWindow, ref copydata);

        Debug.Log(strText);
        Thread.Sleep(1000);
        
    }
    void cacul()
    {
        //Debug.Log("movepic");
        Cam_po_y = size*2 - 21.0f;
        weight = 1+0.0625f * (Cam_po_y - 1) / 2;
        perdis = -(1- weight)/(Cam_po_y-1);//1 - weight;
        //Debug.Log("weight: "+ weight+ "perdis: "+ perdis);
        //speed = speedper * 0.0005f*(22-2*size);//速度矫正
        speed = speedper * 0.625f/ 344.16f * (22 - 2 * size);//每秒多少厘米
        //0.0625一条对应344.16mm/n

    }
    void brigh()
    {
        brightness = bri/25;
        saturation = sat/25;
        contrast = con/25;
    }
    void movepic() { 
        Camera_1.GetComponent<Transform>().position = new Vector3(-3 * perdis, 0, Cam_po_y);

    }
    void changetype()
    {
        string path1 = @"file://D:\\test\\" + type + ".png";      //需要替换上去的资源路径

        GameObject.gameObject.GetComponent<MeshRenderer>().material.mainTexture = new WWW(path1).texture;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (direction == 1)
        {
            if (Camera_1.GetComponent<Transform>().position.x <= weight)
            {
                Camera_1.GetComponent<Transform>().Translate(speed *  Time.deltaTime, 0, 0);
                //Debug.Log(Camera_1.GetComponent<Transform>().position);
            }
            else
            {
                Camera_1.GetComponent<Transform>().position = new Vector3(-1 * weight, 0, Cam_po_y);
            }
    

        }
        else if (direction == 0)
        {
            if (Camera_1.GetComponent<Transform>().position.x >= -1 * weight)
            {
                Camera_1.GetComponent<Transform>().Translate(-speed *  Time.deltaTime, 0, 0);

            }
            else
            {
                Camera_1.GetComponent<Transform>().position = new Vector3(weight, 0, Cam_po_y);
            }
      

        }
    }
}

using System.IO;
using UnityEngine;
using Game;
using Game.Modding;
using ModCommon;

namespace GridOverlay {
  [System.Serializable] public class Settings {
    public bool enabled=true; public float spacing=100f; public float opacity=0.4f;
    public bool showOnStart=false; public float winX=20f, winY=80f;
  }
  static class P { public static string Dir=>Path.Combine(Application.persistentDataPath,"Mods","GridOverlay");
                   public static string Json=>Path.Combine(Dir,"settings.json"); }

  public class Mod : IMod {
    GameObject go;
    public void OnLoad(UpdateSystem us){ if(go==null){ go=new GameObject("GridOverlayUI"); Object.DontDestroyOnLoad(go); go.AddComponent<UI>(); } }
    public void OnDispose(){ if(go!=null){ Object.Destroy(go); go=null; } }
  }

  public class UI : MonoBehaviour {
    static UI inst; static bool latchedSet=false, latched=false;
    public static bool Visible{ get=> inst?inst._vis:(latchedSet?latched:false);
                                set{ if(inst) inst._vis=value; else { latched=value; latchedSet=true; } } }

    Settings s = new Settings();
    Rect win = new Rect(20,80,300,180);
    Texture2D line; bool _vis=false;

    void Awake(){
      inst=this; Directory.CreateDirectory(P.Dir); s=Load();
      _vis = latchedSet? latched : s.showOnStart;
      win.x=s.winX; win.y=s.winY;
      line=new Texture2D(1,1); line.SetPixel(0,0,Color.white); line.Apply();
    }
    void OnDestroy(){ if(inst==this) inst=null; }
    void Update(){ if (Input.GetKeyDown(KeyCode.F6)) _vis=!_vis; }

    void OnGUI(){
      if(s.enabled){
        var prev=GUI.color; GUI.color = new Color(1,1,1,Mathf.Clamp01(s.opacity));
        float sp = Mathf.Max(8f, s.spacing);
        int cx = Mathf.Min(500, Mathf.CeilToInt(Screen.width / sp));
        int cy = Mathf.Min(500, Mathf.CeilToInt(Screen.height/ sp));
        for(int i=0;i<=cx;i++){ float x = Mathf.Round(i*sp); GUI.DrawTexture(new Rect(x,0,1,Screen.height), line); }
        for(int j=0;j<=cy;j++){ float y = Mathf.Round(j*sp); GUI.DrawTexture(new Rect(0,y,Screen.width,1), line); }
        GUI.color = prev;
      }

      if(!_vis){ /* hosted-inline */ /* ClickBlocker.SetZone("GridOverlay", Rect.zero, false); */ return; }
// [hosted] ClickBlocker.SetZone("GridOverlay", win, true);

      win = GUI.Window(56001, win, id=>{
        s.enabled = GUILayout.Toggle(s.enabled, " Enabled");
ModCommon.ImGuiBlockerShim.Mark("GridOverlay", win); // [CB-INJECTED:GridOverlay]
ModCommon.ImGuiBlockerShim.Mark("GridOverlay", win);
        s.showOnStart = GUILayout.Toggle(s.showOnStart, " Show on start");
        GUILayout.Label($"Spacing: {s.spacing:0} px"); s.spacing = GUILayout.HorizontalSlider(s.spacing, 8f, 240f);
        GUILayout.Label($"Opacity: {s.opacity:0.00}");  s.opacity  = GUILayout.HorizontalSlider(s.opacity, 0.05f, 0.9f);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save")) Save(s);
        if (GUILayout.Button("Close (F6)")) _vis=false;
        GUILayout.EndHorizontal();
        GUI.DragWindow();
      }, "GridOverlay (F6)");

      s.winX = win.x; s.winY = win.y;
    }

    Settings Load(){ try{ if(File.Exists(P.Json)) return JsonUtility.FromJson<Settings>(File.ReadAllText(P.Json)) ?? new Settings(); }catch{} return new Settings(); }
    void Save(Settings t){ try{ Directory.CreateDirectory(P.Dir); File.WriteAllText(P.Json, JsonUtility.ToJson(t,true)); }catch{} }
  }
}




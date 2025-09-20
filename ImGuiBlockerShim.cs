using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace ModCommon {
  public static class ImGuiBlockerShim {
    static bool ready=false;
    static readonly Dictionary<string, Rect> rectByKey=new Dictionary<string, Rect>();
    static readonly HashSet<string> seenThisLayout=new HashSet<string>();
    static MethodInfo miSetZone; static object[] args=new object[3];
    class Runner:MonoBehaviour{
      void OnGUI(){
        if (Event.current!=null && Event.current.type==EventType.Layout){
          foreach(var key in new List<string>(rectByKey.Keys)){
            bool on=seenThisLayout.Contains(key);
            CallSetZone(key, rectByKey[key], on);
            if(!on) rectByKey.Remove(key);
          }
          seenThisLayout.Clear();
        }
      }
    }
    static void Ensure(){
      if(ready) return;
      var go=new GameObject("ImGuiBlockerShimRunner");
      UnityEngine.Object.DontDestroyOnLoad(go);
      go.hideFlags=HideFlags.HideAndDontSave;
      go.AddComponent<Runner>(); ready=true;
    }
    static Type ResolveHost(){
      // AppDomain全体から ModCommon.ClickBlocker を探す（1つ＝HelperDock版だけにする）
      foreach(var asm in AppDomain.CurrentDomain.GetAssemblies()){
        var t=asm.GetType("ModCommon.ClickBlocker", false);
        if(t!=null) return t;
      }
      return null;
    }
    static void CallSetZone(string key, Rect rect, bool on){
      if(miSetZone==null){ var t=ResolveHost(); if(t!=null) miSetZone=t.GetMethod("SetZone", BindingFlags.Public|BindingFlags.Static); }
      if(miSetZone!=null){ args[0]=key; args[1]=rect; args[2]=on; miSetZone.Invoke(null,args); }
    }
    public static void Mark(string key, Rect rect){ Ensure(); rectByKey[key]=rect; seenThisLayout.Add(key); }
  }
}

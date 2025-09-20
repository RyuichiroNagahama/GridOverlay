using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ModCommon;

namespace ModCommon {
  public static class ClickShield {
    static GameObject _root;
    static Canvas _canvas;
    static readonly List<Image> _panels = new List<Image>(); // 4 * holes 枚を動的確保

    static Image NewPanel() {
      var go = new GameObject("Block");
      go.transform.SetParent(_canvas.transform, false);
      var img = go.AddComponent<Image>();
      img.color = new Color(0,0,0,0);
      img.raycastTarget = true; // これでuGUIのクリックを吸う
      return img;
    }
    static void SetRect(RectTransform rt, float x, float y, float w, float h){
      rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.zero; rt.pivot = Vector2.zero;
      rt.anchoredPosition = new Vector2(Mathf.Max(0,x), Mathf.Max(0,y));
      rt.sizeDelta = new Vector2(Mathf.Max(0,w), Mathf.Max(0,h));
    }

    public static void Set(bool on){
      if(on){
        if(_root==null){
          _root = new GameObject("IMGUI_ClickShield");
          _canvas = _root.AddComponent<Canvas>();
          _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
          _canvas.sortingOrder = 32760; // ほぼ最前面
          _root.AddComponent<GraphicRaycaster>();
          Object.DontDestroyOnLoad(_root);
        }
        if(!_root.activeSelf) _root.SetActive(true);
      } else {
        if(_root!=null && _root.activeSelf) _root.SetActive(false);
      }
    }

    // 可視穴：holes（画面座標 IMGUI の Rect）
    public static void UpdateHoles(Rect[] holes){
      if(_root==null) return;
      if(!_root.activeSelf) _root.SetActive(true);

      int need = Mathf.Max(0, holes?.Length ?? 0) * 4;
      while(_panels.Count < need) _panels.Add(NewPanel());
      for(int i=0;i<_panels.Count;i++) _panels[i].gameObject.SetActive(i < need);

      int k=0;
      int sw = Screen.width; int sh = Screen.height;
      for(int i=0;i<(holes?.Length ?? 0); i++){
        var r = holes[i];
        // 画面内にクランプ
        float x = Mathf.Clamp(r.x, 0, sw);
        float y = Mathf.Clamp(r.y, 0, sh);
        float w = Mathf.Clamp(r.width,  0, sw - x);
        float h = Mathf.Clamp(r.height, 0, sh - y);

        // 左
        SetRect(_panels[k++].rectTransform, 0, 0, x, sh);
        // 右
        SetRect(_panels[k++].rectTransform, x + w, 0, sw - (x + w), sh);
        // 上
        SetRect(_panels[k++].rectTransform, x, 0, w, y);
        // 下
        SetRect(_panels[k++].rectTransform, x, y + h, w, sh - (y + h));
      }
    }
  }
}


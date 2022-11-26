using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpriteAnimation : MonoBehaviour {
  private RectTransform rt;
  private Image img;
  private Material mat;

  private void Awake() {
    rt = GetComponent<RectTransform>();
    img = GetComponent<Image>();
    mat = img.material;
  }

  public void UpdateOffset(Vector2Int coord) {
    mat.SetTextureOffset("_MainTex", coord);
  }

  public void UpdateDivX(float x) {
    mat.SetFloat("_DivX", x);
  }

  public void UpdateDivY(float y) {
    mat.SetFloat("_DivY", y);
  }

  public void UpdateSpriteSize(Vector2Int div) {
    var w = img.sprite.texture.width / div.x;
    var h = img.sprite.texture.height / div.y;
    rt.sizeDelta = new Vector2(w, h);
  }

  public void ChangeSprite(string path, Vector2Int div) {
    var tex = LoadTexture(path);
    if (tex == null) {
      return;
    }
    img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
    rt.sizeDelta = new Vector2(tex.width / div.x, tex.height / div.y);
  }

  private Texture2D LoadTexture(string path) {
    var fi = new FileInfo(path);
    var ext = fi.Extension.ToLower();
    if (ext != ".png" && ext != ".jpg") {
      return null;
    }
    var data = File.ReadAllBytes(path);
    var tex = new Texture2D(1,1);
    tex.filterMode = FilterMode.Point;
    tex.wrapMode = TextureWrapMode.Repeat;
    tex.LoadImage(data);
    return tex;
  }
}

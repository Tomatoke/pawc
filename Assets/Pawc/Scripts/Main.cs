using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using B83.Win32;

public class Main : MonoBehaviour {
  private string SettingFilePath { get => "settings.json"; }
  private string SettingDefaultLabel { get => "Default"; }
  private Dictionary<string, Setting> settings;
  [SerializeField] private Dropdown loadUi;
  [SerializeField] private TMP_InputField labelUi;
  [SerializeField] private TMP_InputField divUiX;
  [SerializeField] private TMP_InputField divUiY;
  [SerializeField] private TMP_InputField patternUi;
  private SpriteAnimation[] sas;
  private Vector2Int div;
  private List<int> pattern;
  private float speed;
  private int sequence;
  private int count;

  private void Awake() {
    sas = GetComponentsInChildren<SpriteAnimation>();
    Application.targetFrameRate = 60;
    div = new Vector2Int(3, 4);
    pattern = new List<int>() { 10, 9, 10, 11 };
    speed = 1.0f;
    sequence = 0;
    count = 0;
  }

  private void Start() {
    if (ExistSettings()) {
      ReadSettings();
    } else {
      DefaultSettings();
    }
    UpdatePattern();
    UpdateDivX();
    UpdateDivY();
  }

  private void Update() {
    if (++count >= Application.targetFrameRate / speed) {
      sequence = (sequence + 1) % pattern.Count;
      UpdatePattern();
      count = 0;
    }
  }

  private void OnEnable () {
    UnityDragAndDropHook.InstallHook();
    UnityDragAndDropHook.OnDroppedFiles += OnFiles;
  }

  private void OnDisable() {
    UnityDragAndDropHook.UninstallHook();
  }

  private void OnFiles(List<string> paths, POINT _) {
    var path = paths.First();
    var ped = new PointerEventData(EventSystem.current);
    ped.position = Input.mousePosition;
    var results = new List<RaycastResult>();
    EventSystem.current.RaycastAll(ped, results);
    foreach(var result in results) {
      var sa = result.gameObject.GetComponent<SpriteAnimation>();
      if (sa != null) {
        sa.ChangeSprite(path, div);
      }
    }
  }

  private void OnGUI() {
    loadUi.value = 0;
  }

  public void OnChangeLoad(Dropdown change) {
    if (change.value == 0) {
      return;
    }
    labelUi.text = loadUi.options[change.value].text;
    div = settings[labelUi.text].div;
    divUiX.text = $"{div.x}";
    divUiY.text = $"{div.y}";
    pattern = settings[labelUi.text].pattern;
    patternUi.text = pattern.Select(n => $"{n}").Aggregate((a, b) => $"{a},{b}");
    UpdatePattern();
    UpdateDivX();
    UpdateDivY();
  }

  public void OnClickSave(TMP_InputField label) {
    if (label.text != string.Empty && label.text != SettingDefaultLabel) {
      settings[label.text] = new Setting(div, pattern);
      WriteSettings();
    }
  }

  public void OnClickDelete(TMP_InputField label) {
    if (label.text != string.Empty && label.text != SettingDefaultLabel) {
      if (settings.ContainsKey(label.text)) {
        settings.Remove(label.text);
        WriteSettings();
      }
    }
  }

  public bool ExistSettings() {
    return File.Exists(SettingFilePath);
  }

  public void DefaultSettings() {
    settings = new Dictionary<string, Setting>();
    settings.Add(SettingDefaultLabel, new Setting(div, pattern));
    UpdateLoadList();
  }

  public void ReadSettings() {
    var text = File.ReadAllText(SettingFilePath);
    var data = JsonUtility.FromJson<JsonDictionary<string, Setting>>(text);
    settings = data.Dictionary;
    UpdateLoadList();
  }

  public void WriteSettings() {
    var data = new JsonDictionary<string, Setting>(settings);
    var text = JsonUtility.ToJson(data);
    File.WriteAllText(SettingFilePath, text);
    UpdateLoadList();
  }

  public void UpdateLoadList() {
    loadUi.options.RemoveAll(o=>o.text != "Load");
    foreach(var p in settings) {
      if (loadUi.options.All(o=>o.text != p.Key)) {
        loadUi.options.Add(new Dropdown.OptionData(p.Key));
      }
    }
  }

  public void OnChangeScale(Dropdown change) {
    float[] scales = { 2.0f, 1.0f, 0.5f };
    foreach(var sa in sas) {
      sa.ChangeScale(scales[change.value]);
    }
  }

  public void OnChangeSpeed(Dropdown change) {
    float[] speeds = { 4.0f, 2.0f, 0.0f, 1.0f, 0.5f, 0.25f };
    speed = speeds[change.value];
  }

  public void OnEndEditDivX(TMP_InputField input) {
    if (input.text == string.Empty) {
      return;
    }
    div.x = int.Parse(input.text);
    UpdateDivX();
  }

  public void OnEndEditDivY(TMP_InputField input) {
    if (input.text == string.Empty) {
      return;
    }
    div.y = int.Parse(input.text);
    UpdateDivY();
  }

  public void OnEndEditPattern(TMP_InputField input) {
    if (input.text == string.Empty) {
      return;
    }
    pattern = input.text.Split(',').Select(c => int.Parse(c)).ToList();
    sequence = 0;
    count = 0;
    UpdatePattern();
  }

  public void OnChangeBackGroundColor(Dropdown change) {
    Color[] colors = { Color.white, Color.grey, Color.black };
    Camera.main.backgroundColor = colors[change.value];
  }

  private void UpdatePattern() {
    foreach(var sa in sas) {
      sa.UpdateOffset(IndexToCoord(pattern[sequence], div));
    }
  }

  private void UpdateDivX() {
    foreach(var sa in sas) {
      sa.UpdateDivX(div.x);
      sa.UpdateSpriteSize(div);
    }
  }

  private void UpdateDivY() {
    foreach(var sa in sas) {
      sa.UpdateDivY(div.y);
      sa.UpdateSpriteSize(div);
    }
  }

  private Vector2Int IndexToCoord(int index, Vector2Int div) {
    return new Vector2Int(index % div.x, index / div.x);
  }

  private int CoordToIndex(Vector2Int coord, Vector2Int div) {
    return coord.y * div.x + coord.x;
  }
}

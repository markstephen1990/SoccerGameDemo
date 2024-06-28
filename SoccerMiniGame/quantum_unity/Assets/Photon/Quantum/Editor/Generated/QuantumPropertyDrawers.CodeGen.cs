// <auto-generated>
// This code was auto-generated by a tool, every time
// the tool executes this code will be reset.
// </auto-generated>

namespace Quantum.Editor {
  using Quantum;
  using UnityEngine;
  using UnityEditor;

  [CustomPropertyDrawer(typeof(AssetRefGameSpec))]
  public class AssetRefGameSpecPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(GameSpecAsset));
    }
  }

  [CustomPropertyDrawer(typeof(Quantum.Prototypes.GameState_Prototype))]
  [CustomPropertyDrawer(typeof(Quantum.Prototypes.InputButtons_Prototype))]
  partial class PrototypeDrawer {}
}

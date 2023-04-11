using UnityEngine;
using UnityEditor;

public class MeterTool : EditorWindow
{
    [MenuItem("Quickit/Meters")]
    public static void ShowWindow()
    {
        thisWindow = GetWindow(typeof(MeterTool), false, "Meters");
        windowRect = thisWindow.position;
    }

    private static Rect windowRect;
    private static EditorWindow thisWindow;

    private Vector2 windowScrollPos;


    private void OnGUI()
    {
        if (thisWindow == null)
            thisWindow = GetWindow(typeof(MeterTool));
        if (windowRect.size != thisWindow.position.size)
        {
            windowRect = thisWindow.position;
        }

        windowScrollPos = EditorGUILayout.BeginScrollView(windowScrollPos, GUILayout.MaxWidth(windowRect.width));

        EditorGUILayout.LabelField("Options",EditorStyles.boldLabel);
    //  MeterDrawer.colorType = (Meter.ColorType)EditorGUILayout.EnumPopup("Meter color type",MeterDrawer.colorType);
    //  switch (MeterDrawer.colorType)
    //  {
    //      case Meter.ColorType.single:
    //          MeterDrawer.meterCol = EditorGUILayout.ColorField("Meter color", MeterDrawer.meterCol);
    //          break;
    //      case Meter.ColorType.gradient:
    //          MeterDrawer.meterGrad = EditorGUILayout.GradientField("Meter gradient", MeterDrawer.meterGrad);
    //          break;
    //      default:
    //          Debug.LogError("Invalid color type selection, check MeterDrawer.colorType is valid");
    //          break;
    //  }
    //  MeterDrawer.meterBgCol = EditorGUILayout.ColorField("Meter background color", MeterDrawer.meterBgCol);
    //  if (GUILayout.Button("Default values"))
    //  {
    //      MeterDrawer.colorType = Meter.ColorType.single;
    //      MeterDrawer.meterCol = MeterDrawer.defaultMeterCol;
    //      MeterDrawer.meterBgCol = MeterDrawer.defaultMeterBgCol;
    //      MeterDrawer.meterGrad = new Gradient();
    //  }
        EditorGUILayout.EndScrollView();
    }

}

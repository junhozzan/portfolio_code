#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
public class DrawController : MonoBehaviour
{
    public enum DrawType
    {
        TARGET,
        SKILL_RANGE,
        UNIT_COLLIDER,
        Count
    }

    private bool[] drawables = Enumerable.Repeat(true, (int)DrawType.Count).ToArray();
    public static DrawController instance = null;

    public static void Create()
    {
        if (instance != null)
        {
            return;
        }

        instance = new GameObject("draw_controller").AddComponent<DrawController>();
        DontDestroyOnLoad(instance);
    }

    private void Awake()
    {
        drawables[(int)DrawType.UNIT_COLLIDER] = false;
    }

    private void DrawSHOW_TARGET()
    {
        if (!IsDrawable(DrawType.TARGET))
        {
            return;
        }

        var myUnit = MyUnit.Instance;
        if (!UnitRule.IsValid(myUnit))
        {
            return;
        }

        Gizmos.color = Color.red;
        foreach (var target in myUnit.core.target._targets)
        {
            if (!UnitRule.IsValid(target))
            {
                return;
            }

            Gizmos.DrawWireSphere(target.core.transform.GetPosition(), 0.3f);
        }
    }

    private static IEnumerable<SkillType> skillTypes = System.Enum.GetValues(typeof(SkillType)).Cast<SkillType>();
    private void DrawSKILL_RANGE()
    {
        if (!IsDrawable(DrawType.SKILL_RANGE))
        {
            return;
        }

        var myUnit = MyUnit.Instance;
        if (!UnitRule.IsValid(myUnit))
        {
            return;
        }


        var aiScript = myUnit.core.ai._behaviorTree.root as AIScript;
        if (aiScript == null)
        {
            return;
        }

        var pos = myUnit.core.transform.GetPosition();
        foreach (var skillType in skillTypes)
        {
            if (skillType == SkillType.NONE
                || skillType == SkillType.DEFAULT_ATTACK)
            {
                continue;
            }

            var skillID = myUnit.core.skill.enroll.GetSkillID(skillType);
            if (!myUnit.core.skill.TryGetSkill(skillID, out var skill))
            {
                continue;
            }

            var color = Color.white;
            switch (skillType)
            {
                case SkillType.SKILL_ATTACK:
                    color = ColorHex.FromRgb(0xFF7500);
                    break;

                case SkillType.SKILL_BUFF:
                    color = ColorHex.FromRgb(0xE2FF60);
                    break;

                case SkillType.SKILL_TELEPORT:
                    color = ColorHex.FromRgb(0x7AFF00);
                    break;
            }

            DrawWireSphere(pos, myUnit.core.skill.GetSkillRange(skill.resSkill), color);
        }
    }

    private void DrawUNIT_COLLIDER()
    {
        if (!IsDrawable(DrawType.UNIT_COLLIDER))
        {
            return;
        }

        foreach (var unit in UnitManager.Instance._targets)
        {
            if (!UnitRule.IsValid(unit))
            {
                continue;
            }

            var myColider = unit.core.transform.GetMyCollider();
            if (myColider != null)
            {
                DrawWireSphere(myColider.collInfo.currPos, myColider.collInfo.volume, Color.green);
            }
        }
    }

    private void OnDrawGizmos()
    {
        DrawSHOW_TARGET();
        DrawSKILL_RANGE();
        DrawUNIT_COLLIDER();
    }

    public bool IsDrawable(DrawType type)
    {
        if (!Application.isPlaying)
        {
            return false;
        }

        if (type < 0 || type >= DrawType.Count)
        {
            return false;
        }

        return drawables[(int)type];
    }

    public void SetDrawType(DrawType type, bool use)
    {
        if (type < 0 || type >= DrawType.Count)
        {
            return;
        }

        drawables[(int)type] = use;
    }

    /// <summary>
    /// Gizmos.DrawSphere의 십자 형태의 줄 제거 기능
    /// </summary>
    public static void DrawWireSphere(Vector2 center, float radius, Color color)
    {
        if (radius <= 0f)
        {
            return;
        }

        Gizmos.color = color;

        var length = 2 * Mathf.PI * radius;
        var count = Mathf.FloorToInt(length / (radius * 0.2f));
        float deg = 360f / count;

        for (int i = 0; i < count; ++i)
        {
            var aRad = i * deg * Mathf.Deg2Rad;
            var bRad = (i + 1) * deg * Mathf.Deg2Rad;

            var aSpot = center + new Vector2(Mathf.Cos(aRad), Mathf.Sin(aRad)) * radius;
            var bSpot = center + new Vector2(Mathf.Cos(bRad), Mathf.Sin(bRad)) * radius;

            Gizmos.DrawLine(aSpot, bSpot);
        }
    }

    public static void ExternalGizmoDrawLines(Color color, bool loop, params Vector2[] aPos)
    {
        Gizmos.color = color;
        for (int i = 0, len = loop ? aPos.Length : aPos.Length - 1; i < len; ++i)
        {
            Gizmos.DrawLine(aPos[i], aPos[(i + 1) % len]);
        }
    }
}
public class DrawDebugger : EditorWindow
{
    [MenuItem("MyCustom/DrawDebugger")]
    public static void ShowWindow()
    {
        DrawDebugger window = (DrawDebugger)GetWindow(typeof(DrawDebugger), false, "Draw Debugger");
        window.Show();
    }

    private void OnGUI()
    {
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Cannot use this utility in Editor Mode", MessageType.Info);
            return;
        }
        DrawController ctrl = DrawController.instance;
        if (ctrl == null)
        {
            EditorGUILayout.HelpBox("draw controller is null", MessageType.Info);
            return;
        }
        GUILayout.Toggle(false, "Draw Debugger", GUI.skin.FindStyle("LODLevelNotifyText"));
        GUIStyle guiStyle = new GUIStyle(GUI.skin.button);
        for (DrawController.DrawType type = 0; type < DrawController.DrawType.Count; ++type)
        {
            var drawable = ctrl.IsDrawable(type);
            guiStyle.normal.textColor = drawable ? Color.green : Color.red;
            if (GUILayout.Button($"[{type}]", guiStyle))
            {
                ctrl.SetDrawType(type, !drawable);
            }
        }
    }
}
#endif
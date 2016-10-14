using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public static class InputExt
{
    // [ControlScheme][ControlType] => InputExt.Entry
    private static Dictionary<IConvertible, Dictionary<IConvertible, List<Entry>>> controls;

    // [player_id] => ControlScheme
    private static IConvertible[] player_control_schemes;

    public static bool DebugMode { get; set; }
    public static float RepeatDelay { get; set; }


    static InputExt()
    {
        controls = new Dictionary<IConvertible, Dictionary<IConvertible, List<Entry>>>();
        DebugMode = false;
        RepeatDelay = 0.25f;
    }

    public static void ClearControls(IConvertible control_scheme, IConvertible control)
    {
        List<Entry> entries = TryGetEntryList(control_scheme, control);
        if (entries != null) entries.Clear();
    }
    public static void AddAxis(IConvertible control_scheme, IConvertible control, KeyCode neg, KeyCode pos)
    {
        AxisEntry entry = new AxisEntry(
            () => (Input.GetKey(neg) ? -1 : 0) + (Input.GetKey(pos) ? 1 : 0));
        GetOrAddEntryList(control_scheme, control).Add(entry);
    }
    public static void AddAxis(IConvertible control_scheme, IConvertible control, Func<bool> neg, Func<bool> pos)
    {
        AxisEntry entry = new AxisEntry(
            () => (neg() ? -1 : 0) + (pos() ? 1 : 0));
        GetOrAddEntryList(control_scheme, control).Add(entry);
    }
    public static void AddAxis(IConvertible control_scheme, IConvertible control, string im_name)
    {
        AxisEntry entry = new AxisEntry(() => Input.GetAxis(im_name));
        GetOrAddEntryList(control_scheme, control).Add(entry);
    }
    public static void AddKey(IConvertible control_scheme, IConvertible control, KeyCode keycode)
    {
        GetOrAddEntryList(control_scheme, control).Add(new KeyCodeEntry(keycode));
    }
    public static void AddKey(IConvertible control_scheme, IConvertible control, Func<bool> is_down)
    {
        GetOrAddEntryList(control_scheme, control).Add(new PseudoKeyEntry(is_down));
    }

    public static void RegisterPlayers(int num_players, IConvertible default_scheme)
    {
        player_control_schemes = new IConvertible[num_players];
        for (int i = 0; i < num_players; ++i)
        {
            player_control_schemes[i] = default_scheme;
        }
    }
    public static void SetPlayerControlScheme(int id, IConvertible scheme)
    {
        if (!CheckPlayerIdValid(id)) return;
        player_control_schemes[id] = scheme;
    }

    public static bool GetKey(int id, IConvertible control)
    {
        if (!CheckPlayerIdValid(id)) return false;
        return GetKeyCS(player_control_schemes[id], control);
    }
    public static bool GetKeyDown(int id, IConvertible control)
    {
        if (!CheckPlayerIdValid(id)) return false;
        return GetKeyDownCS(player_control_schemes[id], control);
    }
    public static bool GetKeyUp(int id, IConvertible control)
    {
        if (!CheckPlayerIdValid(id)) return false;
        return GetKeyUpCS(player_control_schemes[id], control);
    }
    public static float GetAxis(int id, IConvertible control, bool repeat=false)
    {
        if (!CheckPlayerIdValid(id)) return 0;
        return GetAxisCS(player_control_schemes[id], control, repeat);
    }
    public static int GetAxisInt(int id, IConvertible control, bool repeat=false)
    {
        if (!CheckPlayerIdValid(id)) return 0;
        return GetAxisIntCS(player_control_schemes[id], control, repeat);
    }

    public static bool GetKeyCS(IConvertible control_scheme, IConvertible control)
    {
        List<Entry> entries = TryGetEntryList(control_scheme, control);
        if (entries == null) return false;

        bool answer = false;
        foreach (Entry e in entries)
        {
            KeyEntry ke = e as KeyEntry;
            if (ke != null)
            {
                answer = answer || ke.GetKey();
            }
        }
        return answer;
    }
    public static bool GetKeyDownCS(IConvertible control_scheme, IConvertible control)
    {
        List<Entry> entries = TryGetEntryList(control_scheme, control);
        if (entries == null) return false;

        bool answer = false;
        foreach (Entry e in entries)
        {
            KeyEntry ke = e as KeyEntry;
            if (ke != null)
            {
                answer = answer || ke.GetKeyDown();
            }
        }
        return answer;
    }
    public static bool GetKeyUpCS(IConvertible control_scheme, IConvertible control)
    {
        List<Entry> entries = TryGetEntryList(control_scheme, control);
        if (entries == null) return false;

        bool answer = false;
        foreach (Entry e in entries)
        {
            KeyEntry ke = e as KeyEntry;
            if (ke != null)
            {
                answer = answer || ke.GetKeyUp();
            }
        }
        return answer;
    }
    public static float GetAxisCS(IConvertible control_scheme, IConvertible control, bool repeat=false)
    {
        List<Entry> entries = TryGetEntryList(control_scheme, control);
        if (entries == null) return 0;

        float answer = 0;
        foreach (Entry e in entries)
        {
            AxisEntry ae = e as AxisEntry;
            if (ae != null)
            {
                answer += ae.GetAxis(repeat);
            }
        }
        return Mathf.Clamp(answer, -1, 1);
    }
    public static int GetAxisIntCS(IConvertible control_scheme, IConvertible control, bool repeat=false)
    {
        float f = GetAxisCS(control_scheme, control, repeat);
        return f < 0 ? -1 : f > 0 ? 1 : 0;
    }

    //public static string GetKeyName(KeyCode key)
    //{

    //    return key.ToString();
    //}
    //public static KeyCode GetJoystickKeyCode(int joystick_num, int btn_num)
    //{
    //    return KeyCode.None;
    //}
    public static IConvertible GetPlayerScheme(int id)
    {
        CheckPlayerIdValid(id);
        return player_control_schemes[id];
    }


    private static bool CheckPlayerIdValid(int id)
    {
        if (id < 0 || id > player_control_schemes.Length)
        {
            if (DebugMode) Debug.LogWarning("Invalid player id - insure to RegisterPlayers with the correct num_players");
            return false;
        }
        return true;
    }
    private static List<Entry> TryGetEntryList(IConvertible control_scheme, IConvertible control)
    {
        Dictionary<IConvertible, List<Entry>> d;
        if (!controls.TryGetValue(control_scheme, out d))
        {
            if (DebugMode) Debug.LogWarning(string.Format("Control Scheme {0} not found", control_scheme));
            return null;
        }
        List<Entry> list;
        if (!d.TryGetValue(control, out list))
        {
            if (DebugMode) Debug.LogWarning(string.Format("Control {0} not found for scheme {1}", control, control_scheme));
            return null;
        }
        return list;
    }
    private static List<Entry> GetOrAddEntryList(IConvertible control_scheme, IConvertible control)
    {
        Dictionary<IConvertible, List<Entry>> d;
        if (!controls.TryGetValue(control_scheme, out d))
        {
            d = new Dictionary<IConvertible, List<Entry>>();
            controls.Add(control_scheme, d);
        }
        List<Entry> list;
        if (!d.TryGetValue(control, out list))
        {
            list = new List<Entry>();
            d.Add(control, list);
        }
        return list;
    }


    public class Entry
    {

    }
    public class KeyEntry : Entry
    {
        public virtual bool GetKey()
        {
            return false;
        }
        public virtual bool GetKeyDown()
        {
            return false;
        }
        public virtual bool GetKeyUp()
        {
            return false;
        }
    }
    public class KeyCodeEntry : KeyEntry
    {
        private KeyCode keycode;

        public KeyCodeEntry(KeyCode keycode)
        {
            this.keycode = keycode;
        }

        public override bool GetKey()
        {
            return Input.GetKey(keycode);
        }
        public override bool GetKeyDown()
        {
            return Input.GetKeyDown(keycode);
        }
        public override bool GetKeyUp()
        {
            return Input.GetKeyUp(keycode);
        }
    }
    public class PseudoKeyEntry : KeyEntry
    {
        private Func<bool> is_down;
        private bool down_last_frame = false;

        public PseudoKeyEntry(Func<bool> is_down)
        {
            this.is_down = is_down;
        }

        public override bool GetKey()
        {
            bool down = is_down();
            down_last_frame = down;
            return down;
        }
        public override bool GetKeyDown()
        {
            bool down = is_down();
            bool b = !down_last_frame && down;
            down_last_frame = down;
            return b;
        }
        public override bool GetKeyUp()
        {
            bool down = is_down();
            bool b = down_last_frame && !down;
            down_last_frame = down;
            return b;
        }
    }
    public class AxisEntry : Entry
    {
        private float repeat_timestamp = -100;
        private Func<float> get_func;

        public AxisEntry(Func<float> get_func)
        {
            this.get_func = get_func;
        }

        public float GetAxis(bool repeat=false)
        {
            float answer = get_func();

            if (repeat)
            {
                if (answer == 0)
                {
                    repeat_timestamp = -100;
                }
                else if (Time.time - repeat_timestamp >= RepeatDelay)
                {
                    repeat_timestamp = Time.time;
                }
                else answer = 0;
            }

            return answer;
        }
    }
}




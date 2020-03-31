using EXILED;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoundMods
{

    public static class ConfigExtentions
    {
        public static bool GetBool(this IDictionary<object, object> data2, string key, bool defaultstr)
        {
            bool ret = defaultstr;
            if (!bool.TryParse(GetString(data2, key, defaultstr.ToString()), out ret))
            {
                Log.Error("Config " + key + " is the wrong type (expected type: bool).");
                return defaultstr;
            }
            return ret;
        }

        public static int GetInt(this IDictionary<object, object> data2, string key, int defaultstr)
        {
            int ret = defaultstr;
            if (!int.TryParse(GetString(data2, key, defaultstr.ToString()), out ret))
            {
                Log.Error("Config " + key + " is the wrong type (expected type: int).");
                return defaultstr;
            }
            return ret;
        }

        public static float GetFloat(this IDictionary<object, object> data2, string key, float defaultstr)
        {
            float ret = defaultstr;
            if (!float.TryParse(GetString(data2, key, defaultstr.ToString()), out ret))
            {
                Log.Error("Config " + key + " is the wrong type (expected type: int).");
                return defaultstr;
            }
            return ret;
        }

        public static string GetString(this IDictionary<object, object> data2, string key, string defaultstr)
        {

            object ret2 = GetObject(data2, key, defaultstr);
            if (ret2.GetType() != typeof(string))
            {
                Log.Error("Config " + key + " is the wrong type (expected type: string).");
                return defaultstr;
            }
            return (string)ret2;

            /*if (data2 == null)
            {
                Log.Error("Config data was not found. Did you make a config?");
                return defaultstr;
            }
            string ret = defaultstr;
            try
            {
                ret = (string)data2[key];
            }
            catch (KeyNotFoundException)
            {
                Log.Debug("Config " + key + " was not found.");
                return defaultstr;
            }
            catch (InvalidCastException)
            {
                Log.Error("Config " + key + " is invalid.");
                return defaultstr;
            }
            return ret;*/
        }

        public static object GetObject(this IDictionary<object, object> data2, string key, object defaultstr, string key3)
        {
            if (data2 == null)
            {
                Log.Error("Config data was not found. Did you make a config?");
                return defaultstr;
            }
            object ret = defaultstr;
            try
            {
                List<string> arr = new List<string>(key.Split('.'));
                string key2 = arr[0];
                arr.RemoveAt(0);
                if (arr.Count > 0)
                {
                    ret = GetObject((IDictionary<object, object>)data2[key2], arr.Join(null, "."), defaultstr, key3);
                }
                else
                {
                    ret = data2[key];
                }
            }
            catch (KeyNotFoundException)
            {
                Log.Debug("Config " + key3 + " was not found.");
                return defaultstr;
            }
            catch (InvalidCastException)
            {
                Log.Error("Config " + key + " is invalid.");
                return defaultstr;
            }
            return ret;
        }

        public static object GetObject(this IDictionary<object, object> data2, string key, object defaultstr)
        {
            if (data2 == null)
            {
                Log.Error("Config data was not found. Did you make a config?");
                return defaultstr;
            }
            object ret = defaultstr;
            try
            {
                List<string> arr = new List<string>(key.Split('.'));
                string key2 = arr[0];
                arr.RemoveAt(0);
                if (arr.Count > 0)
                {
                    ret = GetObject((IDictionary<object, object>)data2[key2], arr.Join(null, "."), defaultstr, key);
                }
                else
                {
                    ret = data2[key];
                }
            }
            catch (KeyNotFoundException)
            {
                Log.Debug("Config " + key + " was not found.");
                return defaultstr;
            }
            catch (InvalidCastException)
            {
                Log.Error("Config " + key + " is invalid.");
                return defaultstr;
            }
            return ret;
        }

        public static List<object> GetList(this IDictionary<object, object> data2, string key, List<object> defaultstr)
        {
            object ret2 = GetObject(data2, key, defaultstr);
            if (ret2.GetType() != typeof(List<object>))
            {
                Log.Error("Config " + key + " is the wrong type (expected type: list).");
                return defaultstr;
            }
            return (List<object>)ret2;
        }
    }
}

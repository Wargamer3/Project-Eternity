using System.Collections.Generic;
using System.IO;

namespace ProjectEternity.Core.Online
{
    public abstract class OnlineScriptHolder : Holder<OnlineScript>
    {
        public static Dictionary<string, OnlineScript> LoadAllScripts(params object[] Args)
        {
            Dictionary<string, List<OnlineScript>> DicAllScript = GetAllScriptsByCategory(Args);
            Dictionary<string, OnlineScript> ReturnValue = new Dictionary<string, OnlineScript>();
            foreach (KeyValuePair<string, List<OnlineScript>> ActiveListScript in DicAllScript)
            {
                for (int S = ActiveListScript.Value.Count - 1; S >= 0; --S)
                    ReturnValue.Add(ActiveListScript.Value[S].Name, ActiveListScript.Value[S]);
            }

            return ReturnValue;
        }

        public static Dictionary<string, List<OnlineScript>> GetAllScriptsByCategory(params object[] Args)
        {
            return ReflectionHelper.GetContentByName<OnlineScript>(Directory.GetFiles("Online", "*.dll"), Args);
        }
    }
}

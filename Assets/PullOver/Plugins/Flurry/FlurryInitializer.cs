using FlurrySDK;
using UnityEngine;

namespace PullOver.Plugins
{
    public class FlurryInitializer : MonoBehaviour
    {
        private const string PluginKey = "*****";

        private void Awake()
        {
            new Flurry.Builder()
                  .WithCrashReporting(true)
                  .WithLogEnabled(true)
                  .WithLogLevel(Flurry.LogLevel.VERBOSE)
                  .WithMessaging(true)
                  .Build(PluginKey);
        }
    }
}
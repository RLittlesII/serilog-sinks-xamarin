using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Foundation;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using UIKit;

namespace Serilog.Sinks.Xamarin
{
    class OSLogSink : ILogEventSink
    {
        readonly ITextFormatter _textFormatter;

        public OSLogSink(ITextFormatter textFormatter)
        {
            if (textFormatter == null)
            {
                throw new ArgumentNullException(nameof(textFormatter));
            }

            _textFormatter = textFormatter;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            var renderSpace = new StringWriter();
            _textFormatter.Format(logEvent, renderSpace);

            OSLogHelper.OSLog(renderSpace.ToString());
        }
    }

    // In most extension contexts, System.Console.WriteLine is not useful, as it is not readable.
    // Invoking NSLog directly will allow the message to appear directly in the System Log found in
    // the "Console" application.
    /// <summary>
    /// https://github.com/xamarin/mac-samples/blob/master/ExtensionSamples/Utilities/NSLogHelper.cs
    /// https://stackoverflow.com/questions/52429007/how-to-use-ios-oslog-with-xamarin
    /// </summary>
    public static class OSLogHelper
    {
        [DllImport("__Internal", EntryPoint = "os_log_create")]
        public static extern IntPtr os_log_create(string subsystem, string category);

        [DllImport("/System/Library/Frameworks/Foundation.framework/Foundation")]
        static extern void OSLog(IntPtr format, [MarshalAs(UnmanagedType.LPStr)] string s);

        public static void OSLog(string format, params object[] args)
        {
            var fmt = NSString.CreateNative("%s");
            var val = (args == null || args.Length == 0) ? format : string.Format(format, args);

            os_log_create("hello", "world");
            OSLog(fmt, val);
            NSString.ReleaseNative(fmt);
        }
    }
}
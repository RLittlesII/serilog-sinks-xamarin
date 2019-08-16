// Copyright 2015 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;
using System.Runtime.InteropServices;
using Foundation;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.Xamarin
{
	class NSLogSink : ILogEventSink
	{
		readonly ITextFormatter _textFormatter;

		public NSLogSink(ITextFormatter textFormatter)
		{
			if (textFormatter == null)
            {
                throw new ArgumentNullException("textFormatter");
            }

            _textFormatter = textFormatter;
		}

		public void Emit(LogEvent logEvent)
		{
			if (logEvent == null) throw new ArgumentNullException("logEvent");
			var renderSpace = new StringWriter();
			_textFormatter.Format(logEvent, renderSpace);
            NSLogHelper.NSLog(renderSpace.ToString ());
		}
    }

    // In most extension contexts, System.Console.WriteLine is not useful, as it is not readable.
    // Invoking NSLog directly will allow the message to appear directly in the System Log found in
    // the "Console" application.
    /// <summary>
    /// https://github.com/xamarin/mac-samples/blob/master/ExtensionSamples/Utilities/NSLogHelper.cs
    /// </summary>
    public static class NSLogHelper
    {
        [DllImport("/System/Library/Frameworks/Foundation.framework/Foundation")]
        extern static void NSLog(IntPtr format, [MarshalAs(UnmanagedType.LPStr)] string s);

        public static void NSLog(string format, params object[] args)
        {
            var fmt = NSString.CreateNative("%s");
            var val = (args == null || args.Length == 0) ? format : string.Format(format, args);

            NSLog(fmt, val);
            NSString.ReleaseNative(fmt);
        }
    }
}
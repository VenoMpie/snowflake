﻿using System;
using System.Reflection;

namespace Snowflake.Ajax
{
    internal sealed class JSMethod : IJSMethod
    {
        public MethodInfo MethodInfo { get; }
        public Func<IJSRequest, IJSResponse> Method { get; }
        public JSMethod(MethodInfo methodInfo, Func<IJSRequest, IJSResponse> method)
        {
            this.Method = method;
            this.MethodInfo = methodInfo;
        }
    }
}

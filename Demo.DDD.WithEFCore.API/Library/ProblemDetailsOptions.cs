using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System;

namespace Demo.DDD.WithEFCore.API.Library
{
    public class ProblemDetailsOptions
    {
        public int SourceCodeLineCount { get; set; }
        public IFileProvider FileProvider { get; set; }
        public Func<HttpContext, string> GetTraceId { get; set; }
        public Func<HttpContext, Exception, bool> IncludeExceptionDetails { get; set; }
        public Func<HttpContext, bool> IsProblem { get; set; }
        public Func<HttpContext, ProblemDetails> MapStatusCode { get; set; }
        public Action<HttpContext, ProblemDetails> OnBeforeWriteDetails { get; set; }
        public Func<HttpContext, Exception, ProblemDetails, bool> ShouldLogUnhandledException { get; set; }
    }
}

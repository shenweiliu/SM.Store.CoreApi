using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SM.Store.Api.Common
{
    public static class ExceptionHandlerFactory
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            //Enable request buffering.
            app.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                return next();
            });

            //Handle global error.
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (errorFeature != null)
                    {
                        //Serilog.
                        //Log.Error($"Error: {errorFeature.Error}");

                        //await context.Response.WriteAsync(new ErrorDetails()
                        //{
                        //    StatusCode = context.Response.StatusCode,
                        //    Message = errorFeature.Error.Message
                        //}.ToString());

                        await ProcessError.LogAndReport(context, errorFeature.Error);
                    }
                });
            });
        }
    }

}

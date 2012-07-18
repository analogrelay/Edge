using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gate;
using Owin;

namespace Edge
{
    public class ErrorPageMiddleware
    {
        public string ErrorCssFile { get; set; }

        public AppDelegate Start(AppDelegate next)
        {
            return async call =>
            {
                try
                {
                    return await next(call);
                }
                catch (Exception ex)
                {
                    return GenerateErrorPage(ex).GetResultAsync();
                }
            };
        }

        private Response GenerateErrorPage(Exception ex)
        {
            // Start the response
            Response resp = new Response();
            resp.Start();

            // Check for IHttpException or HttpException
            ProcessHttpException(resp, ex as IHttpException);

            // Write the content type
            resp.ContentType = "text/html";

            // Write the response body
            resp.Write(String.Format(
                Strings.ErrorPage_Template,
                GetErrorCss(),
                ex.GetType().Name,
                ex.Message,
                GetAdditionalTemplate(ex)));
        }

        private string GetAdditionalTemplate(Exception ex)
        {
            IMultiErrorException mex = ex as IMultiErrorException;
            return mex == null ? String.Empty :
                String.Format(
                    Strings.ErrorPage_ListComponent,
                    String.Concat(
                        mex.ErrorMessages.Select(message =>
                            String.Format(Strings.ErrorPage_ListItemComponent, message))));
        }

        private string GetErrorCss()
        {
            return String.IsNullOrEmpty(ErrorCssFile) ? String.Empty :
                   String.Format(Strings.ErrorPage_LinkComponent, ErrorCssFile);
        }

        private void ProcessHttpException(Response resp, IHttpException httpException)
        {
            if (httpException != null)
            {
                resp.StatusCode = httpException.StatusCode;
                resp.ReasonPhrase = httpException.ReasonPhrase;
            }
        }
    }
}

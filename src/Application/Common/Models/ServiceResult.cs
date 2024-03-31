using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ImageBrowser.Application.Common.Models;
public class ServiceResult
{
    public bool IsSuccess { get; set; }
    public bool Redirecting { get; set; }
    public string Error { get; set; }
    public string Link { get; set; }
    public int Key { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    //public string KeyStr { get; set; }


    public ServiceResult(bool succeeded, string errors)
    {
        IsSuccess = succeeded;
        Error = errors;
    }
    public ServiceResult()
    {
    }

    public static ServiceResult Failure(string error)
    {
        return new ServiceResult()
        {
            Error = error,
            IsSuccess = false,
        };
    }

    public static ServiceResult Redirect(string link)
    {
        return new ServiceResult()
        {
            Link = link,
            IsSuccess = false,
            Redirecting = true
        };
    }

    public static ServiceResult Success()
    {
        return new ServiceResult()
        {
            IsSuccess = true
        };
    }

    public static ServiceResult Success(int key)
    {
        return new ServiceResult()
        {
            IsSuccess = true,
            Key = key
        };
    }
    //public static ServiceResult Success(string key)
    //{
    //    return new ServiceResult()
    //    {
    //        IsSuccess = true,
    //        KeyStr = key
    //    };
    //}

    public static ServiceResult<T> Success<T>(T value) where T : class
    {
        return new ServiceResult<T>
        {
            IsSuccess = true,
            Value = value
        };
    }



    public static ServiceResult<T> Failure<T>(string error) where T : class
    {
        return new ServiceResult<T>
        {
            Error = error,
            IsSuccess = false
        };
    }

    public static ServiceResult<T> Redirect<T>(string link) where T : class
    {
        return new ServiceResult<T>
        {
            Link = link,
            IsSuccess = false,
            Redirecting = true
        };
    }

}
public class ServiceResult<T> : ServiceResult where T : class
{
    public T Value { get; set; }
}

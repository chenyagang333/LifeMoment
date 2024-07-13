using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.Commons.ApiResult.Generic;

public class ApiResult<T>
{
    public static ApiResult<T> Succeess => new ApiResult<T> { Code = 200 };
    public static ApiResult<T> Error => new ApiResult<T> { Code = 400 };
    
    public int Code { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public static ApiResult<T> Succeeded(T data)
    {
        return new ApiResult<T> { Code = 200, Data = data };
    }

    public static ApiResult<T> Succeeded(T data,string message)
    {
        return new ApiResult<T> { Code = 200, Data = data,Message = message };
    }

    public static ApiResult<T> Failed(string message)
    {
        return new ApiResult<T> { Code = 400, Message = message };
    }
}

public class ApiListResult<T> : ApiResult<T>
{
    public long Total { get; set; }
    public static new ApiListResult<T> Error => new ApiListResult<T> { Code = 400 };


    public static ApiListResult<T> Succeeded(T data, long total)
    {
        return new ApiListResult<T> { Code = 200, Data = data, Total = total };
    }

    public static ApiListResult<T> Succeeded(T data, long total, string message)
    {
        return new ApiListResult<T> { Code = 200, Data = data, Total = total, Message = message };
    }


}

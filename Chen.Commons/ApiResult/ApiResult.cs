using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.Commons.ApiResult;

public class ApiResult
{
    public static ApiResult Succeess => new ApiResult { Code = 200 };
    public static ApiResult Error => new ApiResult { Code = 400 };

    public int Code { get; set; }
    public string? Message { get; set; }
    public dynamic? Data { get; set; }

    public static ApiResult Succeeded<T>(T data)
    {
        return new ApiResult { Code = 200, Data = data };
    }

    public static ApiResult Succeeded<T>(T data, string message)
    {
        return new ApiResult { Code = 200, Data = data, Message = message };
    }

    public static ApiResult Failed(string message)
    {
        return new ApiResult { Code = 400, Message = message };
    }
}

public class ApiListResult : ApiResult
{
    public long Total { get; set; }
    public static new ApiListResult Error => new ApiListResult { Code = 400 };


    public static ApiListResult Succeeded<T>(T data, long total)
    {
        return new ApiListResult { Code = 200, Data = data, Total = total };
    }

    public static ApiListResult Succeeded<T>(T data, long total, string message)
    {
        return new ApiListResult { Code = 200, Data = data, Total = total, Message = message };
    }


}

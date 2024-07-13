using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.Commons.FunResult
{
    public class FunResult
    {
        public string Description { get; set; }
        public bool Succeeded { get; set; }
        public static FunResult Success => new FunResult { Succeeded = true };
        public static FunResult Fail => new FunResult { Succeeded = false };

        public static FunResult Succeed(string description)
        {
            return new FunResult
            {
                Succeeded = true,
                Description = description
            };

        }


        public static FunResult Failed(string description)
        {
            return new FunResult
            {
                Succeeded = false,
                Description = description
            };
        }
    }

}

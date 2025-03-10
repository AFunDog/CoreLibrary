using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Core.Structs;

namespace CoreLibrary.Toolkit.Extensions;

public static class ResultExtension
{
    public static Result<T, E> Handle<T, E>(this Result<T, E> result, Action<T> successAction, Action<E> errorAction)
    {
        switch (result)
        {
            case Success<T, E> success:
                successAction(success.Value);
                break;
            case Error<T, E> error:
                errorAction(error.Value);
                break;
        }
        return result;
    }
}

//internal sealed class Test
//{
//    public Result<int, string> Calc(int a, int b)
//    {
//        if (a <= 0 || b <= 0)
//            return new Error<int, string>("Invalid value");
//        return new Success<int, string>(a + b);
//    }

//    public void Run()
//    {
//        Calc(1, 2).Handle(value => Console.WriteLine(value), error => Console.WriteLine(error));
//    }
//}

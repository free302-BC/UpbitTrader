using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Universe.AppBase
{
    public static class Extensions
    {
        /// <summary>
        /// 멤버의 getter 'Func'과 setter 'Action'을 리턴
        /// </summary>
        /// <typeparam name="P">멤버의 데이터 타입</typeparam>
        /// <param name="selector">instance를 포함한 member access func</param>
        /// <returns></returns>
        public static (Func<P> getter, Action<P> setter)
            ToDelegate<P>(this Expression<Func<P>> selector)//P=Property
        {
            //selector.Body := member access expression
            //var v0 = selector.Compile()();//현재값
            var paramMember = Expression.Parameter(selector.Body.Type);//== P
            var assignExp = Expression.Lambda<Action<P>>(Expression.Assign(selector.Body, paramMember), paramMember);
            var setter = assignExp.Compile();

            return (selector.Compile(), setter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="instance"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static (P currentValue, Action<P> setter)
            ToDelegate<I, P>(this I instance, Expression<Func<I, P>> selector)//I=Instance,P=Property
        {
            var v0 = selector.Compile()(instance);

            var paramInstance = selector.Parameters[0];//== I
            var paramMember = Expression.Parameter(selector.Body.Type);//parameter expression == P
            var assignExp = Expression.Lambda<Action<I, P>>(
                Expression.Assign(selector.Body,paramMember),paramInstance, paramMember);
            var setter = assignExp.Compile();

            Action<P> setterWithoutInstance = p => setter(instance, p);

            return (v0, setterWithoutInstance);
        }

        //ct.Services.Configure<S>(s => ct.Config.GetSection(key).Bind(s));//이렇게 하면 change event 발생 안함
        public static IServiceCollection AddOptions<S>(this IServiceCollection sc,
            IConfiguration configSection) where S : class
        {
            return sc.Configure<S>(configSection);
        }
        public static IServiceCollection AddOptions<S>(this IServiceCollection sc,
            string name, IConfiguration configSection) where S : class
        {
            return sc.Configure<S>(name, configSection);
        }

        public static IServiceCollection AddSimpleConsole(this IServiceCollection sc)
        {
            return sc.AddLogging(b => b.AddSimpleConsole(
                opt =>
                {
                    opt.IncludeScopes = false;
                    opt.TimestampFormat = "[yyMMdd.HHmmss.fff] ";
                }));
        }

        //public static IServiceCollection AddJsonFile(
        //    this IServiceCollection sc,
        //    string jsonFile = "appsettings.json")
        //{
        //    return sc.AddSingleton(
        //                   new ConfigurationBuilder()
        //                   .SetBasePath(Environment.CurrentDirectory)
        //                   .AddJsonFile(jsonFile, false, true)
        //                   .Build());
        //}

    }//class
}

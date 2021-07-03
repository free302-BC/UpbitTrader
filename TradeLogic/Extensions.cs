using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic
{
    public static class Extensions
    {
        public static string Print(this IEnumerable<IViewModel> models) => IViewModel.Print(models);
        public static string Print(this IEnumerable<IViewModel> models, int offset, int count)
            => IViewModel.Print(models, offset, count);


        #region ---- move to Universe.Utility ----

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

        public static (Func<I, P> getter, Action<I, P> setter)
            ToDelegate<I, P>(this Expression<Func<I, P>> selector)//I=Instance,P=Property
        {
            var paramInstance = selector.Parameters[0];//== I
            var paramMember = Expression.Parameter(selector.Body.Type);//parameter expression == P
            var assignExp = Expression.Lambda<Action<I, P>>(
                Expression.Assign(selector.Body, paramMember), paramInstance, paramMember);
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
                Expression.Assign(selector.Body, paramMember), paramInstance, paramMember);
            var setter = assignExp.Compile();

            Action<P> setterWithoutInstance = p => setter(instance, p);

            return (v0, setterWithoutInstance);
        } 
        #endregion


        public static void LogWebException(this ILogger logger, HttpRequestException ex)
        {
            var nl = Environment.NewLine;
            var msg = $"ex.StatusCode={ex.StatusCode}{nl}ex.Message= {ex.Message}";

            //if (ex. != null)
            //{
            //    var res = (HttpWebResponse)ex.Response;
            //    Span<byte> buffer = stackalloc byte[1024];
            //    res.GetResponseStream().Read(buffer);
            //    var text = Encoding.ASCII.GetString(buffer);
            //    msg = $"{msg}{nl}res.StatusCode= {res.StatusCode}{nl}res.Content= {text}";
            //}
            logger.LogError(msg);
        }

    }
}

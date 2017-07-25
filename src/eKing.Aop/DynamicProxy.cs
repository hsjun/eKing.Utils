using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace eKing.Aop.DynamicProxy
{
    /// <summary>
    /// 动态代理类
    /// </summary>
    public class ProxyGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <param name="interceptMethod"></param>
        /// <returns></returns>
        public static T CreateClassProxy<T, I>(string interceptMethod) 
            where T : class, new() 
            where I : class, new()
        {
            string nameOfAssembly = typeof(T).Name + "ProxyAssembly";
            string nameOfModule = typeof(T).Name + "ProxyModule";
            string nameOfType = typeof(T).Name + "Proxy";

            var assemblyName = new AssemblyName(nameOfAssembly);
            var assembly = AppDomain.CurrentDomain
                .DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

            var moduleBuilder = assembly.DefineDynamicModule(nameOfModule);

            var typeBuilder = moduleBuilder.DefineType(nameOfType, TypeAttributes.Public, typeof(T));

            //拦截注入
            InjectInterceptor<T, I>(typeBuilder, interceptMethod);

            var t = typeBuilder.CreateType();

            return Activator.CreateInstance(t) as T;
        }

        private static void InjectInterceptor<T, I>(TypeBuilder typeBuilder, string interceptMethod)
        {
            Type interceptor = typeof(I);

            // ---- define fields ----
            var fieldInterceptor = typeBuilder.DefineField("_interceptor", interceptor, FieldAttributes.Private);

            // ---- define costructors ----
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);
            var ilOfCtor = constructorBuilder.GetILGenerator();

            ilOfCtor.Emit(OpCodes.Ldarg_0);
            ilOfCtor.Emit(OpCodes.Newobj, interceptor.GetConstructor(new Type[0]));
            ilOfCtor.Emit(OpCodes.Stfld, fieldInterceptor);
            ilOfCtor.Emit(OpCodes.Ret);

            // ---- define methods ----
            var methodsOfType = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance);

            for (var i = 0; i < methodsOfType.Length; i++)
            {
                var method = methodsOfType[i];
                var methodParameterTypes =
                  method.GetParameters().Select(p => p.ParameterType).ToArray();

                var methodBuilder = typeBuilder.DefineMethod(
                    method.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    CallingConventions.Standard,
                    method.ReturnType,
                    methodParameterTypes);

                var ilOfMethod = methodBuilder.GetILGenerator();
                ilOfMethod.Emit(OpCodes.Ldarg_0);
                ilOfMethod.Emit(OpCodes.Ldfld, fieldInterceptor);

                // create instance of T
                ilOfMethod.Emit(OpCodes.Newobj, typeof(T).GetConstructor(new Type[0]));
                ilOfMethod.Emit(OpCodes.Ldstr, method.Name);

                // build the method parameters
                if (methodParameterTypes == null)
                {
                    ilOfMethod.Emit(OpCodes.Ldnull);
                }
                else
                {
                    var parameters = ilOfMethod.DeclareLocal(typeof(object[]));
                    ilOfMethod.Emit(OpCodes.Ldc_I4, methodParameterTypes.Length);
                    ilOfMethod.Emit(OpCodes.Newarr, typeof(object));
                    ilOfMethod.Emit(OpCodes.Stloc, parameters);

                    for (var j = 0; j < methodParameterTypes.Length; j++)
                    {
                        ilOfMethod.Emit(OpCodes.Ldloc, parameters);
                        ilOfMethod.Emit(OpCodes.Ldc_I4, j);
                        ilOfMethod.Emit(OpCodes.Ldarg, j + 1);
                        ilOfMethod.Emit(OpCodes.Stelem_Ref);
                    }
                    ilOfMethod.Emit(OpCodes.Ldloc, parameters);
                }

                // call method of Interceptor
                ilOfMethod.Emit(OpCodes.Callvirt, interceptor.GetMethod(interceptMethod));

                // pop the stack if return void
                if (method.ReturnType == typeof(void))
                {
                    ilOfMethod.Emit(OpCodes.Pop);
                }

                // complete
                ilOfMethod.Emit(OpCodes.Ret);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ArkProjects.Wireguard.Mesh.Misc
{
    public class ConfigPath
    {
        private string _path = "";

        public ConfigPath Name(string name)
        {
            var bakingCopy = new ConfigPath
            {
                _path = string.IsNullOrEmpty(_path) ? name : _path + '.' + name
            };
            return bakingCopy;
        }

        public ConfigPath Index(int idx)
        {
            var bakingCopy = new ConfigPath
            {
                _path = string.IsNullOrEmpty(_path) ? $"[{idx}]" : _path + $"[{idx}]"
            };
            return bakingCopy;
        }

        public override string ToString()
        {
            return _path;
        }

        public static implicit operator string(ConfigPath d) => d.ToString();
    }

    public class ConfigPath<T>
    {
        private readonly string _path;

        public ConfigPath(string basePath = "")
        {
            _path = basePath;
        }

        public ConfigPath<TProperty> Name<TProperty>(Expression<Func<T, TProperty>> propertyLambda)
        {
            var name = GetPropertyInfo(propertyLambda, typeof(TProperty)).Name;
            var bakingCopy = new ConfigPath<TProperty>(string.IsNullOrEmpty(_path) ? name : _path + '.' + name);
            return bakingCopy;
        }

        public ConfigPathEnumerable<IEnumerable<TProperty>, TProperty> Name<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> propertyLambda)
        {
            var name = GetPropertyInfo(propertyLambda, typeof(TProperty)).Name;
            var bakingCopy = new ConfigPathEnumerable<IEnumerable<TProperty>, TProperty>(string.IsNullOrEmpty(_path) ? name : _path + '.' + name);
            return bakingCopy;
        }

        public ConfigPathEnumerable<IEnumerable<TProperty>, TProperty> Name<TProperty>(Expression<Func<T, IReadOnlyList<TProperty>>> propertyLambda)
        {
            var name = GetPropertyInfo(propertyLambda, typeof(TProperty)).Name;
            var bakingCopy = new ConfigPathEnumerable<IEnumerable<TProperty>, TProperty>(string.IsNullOrEmpty(_path) ? name : _path + '.' + name);
            return bakingCopy;
        }

        public ConfigPathEnumerable<IEnumerable<TProperty>, TProperty> Name<TProperty>(Expression<Func<T, TProperty[]>> propertyLambda)
        {
            var name = GetPropertyInfo(propertyLambda, typeof(TProperty)).Name;
            var bakingCopy = new ConfigPathEnumerable<IEnumerable<TProperty>, TProperty>(string.IsNullOrEmpty(_path) ? name : _path + '.' + name);
            return bakingCopy;
        }

        private PropertyInfo GetPropertyInfo<TProperty>(Expression<Func<T, TProperty>> propertyLambda, Type targetType)
        {
            var type = typeof(T);
            var body = propertyLambda.Body;

            if (body is MethodCallExpression call)
            {
                var callMethod = call.Method;

                var callAsEnumerable = typeof(Enumerable).GetMethod(nameof(Enumerable.AsEnumerable))!.MakeGenericMethod(targetType);
                var callToArray = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray))!.MakeGenericMethod(targetType);
                var callToList = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList))!.MakeGenericMethod(targetType);

                var callFirst = typeof(Enumerable)
                    .GetMethods()
                    .Single(x => x.Name == nameof(Enumerable.First) && x.IsGenericMethodDefinition && x.GetParameters().Length == 1)
                    .MakeGenericMethod(targetType);
                var callFirstOrDefault = typeof(Enumerable)
                    .GetMethods()
                    .Single(x => x.Name == nameof(Enumerable.FirstOrDefault) && x.IsGenericMethodDefinition && x.GetParameters().Length == 1)
                    .MakeGenericMethod(targetType);

                if (callMethod == callAsEnumerable)
                    body = call.Arguments[0];
                else if (callMethod == callToArray)
                    body = call.Arguments[0];
                else if (callMethod == callToList) 
                    body = call.Arguments[0];
                else if (callMethod == callFirstOrDefault)
                    body = call.Arguments[0];
                else if (callMethod == callFirst)
                    body = call.Arguments[0];
            }

            if (body is not MemberExpression member)
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");
            }

            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a property that is not from type {type}.");
            }

            return propInfo;
        }

        public override string ToString()
        {
            return _path;
        }

        public static implicit operator string(ConfigPath<T> d) => d.ToString();
    }
}
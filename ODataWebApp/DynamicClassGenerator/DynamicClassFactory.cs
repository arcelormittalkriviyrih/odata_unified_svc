﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Services.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicClassGenerator
{
    public class DynamicClassFactory
    {
        private AppDomain _appDomain;
        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;
        private TypeBuilder _typeBuilder;
        private string _assemblyName;

        public DynamicClassFactory()
            : this("Dynamic.Objects")
        {
        }

        public DynamicClassFactory(string assemblyName)
        {
            _appDomain = Thread.GetDomain();
            _assemblyName = assemblyName;
        }

        /// <summary>
        /// This is the normal entry point and just return the Type generated at runtime
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Type CreateDynamicType<T>(string name, Dictionary<string, Type> properties) where T : DynamicEntity
        {
            var tb = CreateDynamicTypeBuilder<T>(name, properties);
            return tb.CreateType();
        }

        /// <summary>
        /// Exposes a TypeBuilder that can be returned and created outside of the class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public TypeBuilder CreateDynamicTypeBuilder<T>(string name, Dictionary<string, Type> properties)
            where T : DynamicEntity
        {
            if (_assemblyBuilder == null)
                _assemblyBuilder = _appDomain.DefineDynamicAssembly(new AssemblyName(_assemblyName),
                    AssemblyBuilderAccess.RunAndSave);
            //vital to ensure the namespace of the assembly is the same as the module name, else IL inspectors will fail
            if (_moduleBuilder == null)
                _moduleBuilder = _assemblyBuilder.DefineDynamicModule(_assemblyName + ".dll");

            //typeof(T) is for the base class, can be omitted if not needed
            _typeBuilder = _moduleBuilder.DefineType(_assemblyName + "." + name, TypeAttributes.Public
                                                            | TypeAttributes.Class
                                                            | TypeAttributes.AutoClass
                                                            | TypeAttributes.AnsiClass
                                                            | TypeAttributes.Serializable
                                                            | TypeAttributes.BeforeFieldInit, typeof(T));

            //various class based attributes for WCF and EF
            AddDataContractAttribute();
            AddTableAttribute(name);
            AddDataServiceKeyAttribute();

            //if there is a property on the base class and also in the dictionary, remove them from the dictionary
            var pis = typeof(T).GetProperties();
            foreach (var pi in pis)
            {
                properties.Remove(pi.Name);
            }

            //get the OnPropertyChanged method from the base class
            var propertyChangedMethod = typeof(T).GetMethod("OnPropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic);

            CreateProperties(_typeBuilder, properties, propertyChangedMethod);

            return _typeBuilder;
        }

        public void AddDataContractAttribute()
        {
            Type attrType = typeof(DataContractAttribute);
            _typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(attrType.GetConstructor(Type.EmptyTypes),
                new object[] { }));
        }

        public void AddTableAttribute(string name)
        {
            Type attrType = typeof(TableAttribute);
            _typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(attrType.GetConstructor(new[] { typeof(string) }),
                new object[] { name }));
        }

        public void AddDataServiceKeyAttribute()
        {
            Type attrType = typeof(DataServiceKeyAttribute);
            _typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(attrType.GetConstructor(new[] { typeof(string) }),
                new object[] { "Id" }));
        }

        public void CreateProperties(TypeBuilder typeBuilder, Dictionary<string, Type> properties, MethodInfo raisePropertyChanged)
        {
            properties.ToList().ForEach(p => CreateFieldForType(p.Value, p.Key, raisePropertyChanged));
        }

        private void CreateFieldForType(Type type, String name, MethodInfo raisePropertyChanged)
        {
            FieldBuilder fieldBuilder = _typeBuilder.DefineField("_" + name.ToLowerInvariant(), type, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = _typeBuilder.DefineProperty(name, PropertyAttributes.HasDefault, type, null);

            //add the various WCF and EF attributes to the property
            AddDataMemberAttribute(propertyBuilder);
            AddColumnAttribute(propertyBuilder);

            MethodAttributes getterAndSetterAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;// | MethodAttributes.Virtual;

            //creates the Get Method for the property
            propertyBuilder.SetGetMethod(CreateGetMethod(getterAndSetterAttributes, name, type, fieldBuilder));
            //creates the Set Method for the property and also adds the invocation of the property change
            propertyBuilder.SetSetMethod(CreateSetMethod(getterAndSetterAttributes, name, type, fieldBuilder, raisePropertyChanged));
        }

        private void AddDataMemberAttribute(PropertyBuilder propertyBuilder)
        {
            Type attrType = typeof(DataMemberAttribute);
            var attr = new CustomAttributeBuilder(attrType.GetConstructor(Type.EmptyTypes), new object[] { });
            propertyBuilder.SetCustomAttribute(attr);
        }

        private void AddColumnAttribute(PropertyBuilder propertyBuilder)
        {
            Type attrType = typeof(ColumnAttribute);
            var attr = new CustomAttributeBuilder(attrType.GetConstructor(Type.EmptyTypes), new object[] { });
            propertyBuilder.SetCustomAttribute(attr);
        }

        private MethodBuilder CreateGetMethod(MethodAttributes attr, string name, Type type, FieldBuilder fieldBuilder)
        {
            var getMethodBuilder = _typeBuilder.DefineMethod("get_" + name, attr, type, Type.EmptyTypes);

            var getMethodILGenerator = getMethodBuilder.GetILGenerator();
            getMethodILGenerator.Emit(OpCodes.Ldarg_0);
            getMethodILGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            getMethodILGenerator.Emit(OpCodes.Ret);

            return getMethodBuilder;
        }

        private MethodBuilder CreateSetMethod(MethodAttributes attr, string name, Type type, FieldBuilder fieldBuilder, MethodInfo raisePropertyChanged)
        {
            var setMethodBuilder = _typeBuilder.DefineMethod("set_" + name, attr, null, new Type[] { type });

            var setMethodILGenerator = setMethodBuilder.GetILGenerator();
            setMethodILGenerator.Emit(OpCodes.Ldarg_0);
            setMethodILGenerator.Emit(OpCodes.Ldarg_1);
            setMethodILGenerator.Emit(OpCodes.Stfld, fieldBuilder);

            if (raisePropertyChanged != null)
            {
                setMethodILGenerator.Emit(OpCodes.Ldarg_0);
                setMethodILGenerator.Emit(OpCodes.Ldstr, name);
                setMethodILGenerator.EmitCall(OpCodes.Call, raisePropertyChanged, null);
            }

            setMethodILGenerator.Emit(OpCodes.Ret);

            return setMethodBuilder;
        }

        public void SaveAssembly()
        {
            _assemblyBuilder.Save(_assemblyBuilder.GetName().Name + ".dll");
        }
    }
}

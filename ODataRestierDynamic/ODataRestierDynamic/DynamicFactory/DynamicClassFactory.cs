using Microsoft.Restier.Core.Model;
using ODataRestierDynamic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Web;

namespace ODataRestierDynamic.DynamicFactory
{
	public class DynamicClassFactory
	{
		private AppDomain _appDomain;
		private AssemblyBuilder _assemblyBuilder;
		private ModuleBuilder _moduleBuilder;
		private TypeBuilder _typeBuilder;
		private string _assemblyName;

		public const string cDefaultNamespace = "ODataRestierDynamic.Models";

		public DynamicClassFactory()
			: this(cDefaultNamespace)
		{
		}

		public DynamicClassFactory(string assemblyName)
		{
			_appDomain = System.Threading.Thread.GetDomain();
			_assemblyName = assemblyName;
		}

		/// <summary>
		/// This is the normal entry point and just return the Type generated at runtime
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="properties"></param>
		/// <returns></returns>
		public Type CreateDynamicType<T>(string name, Dictionary<string, DynamicPropertyData> properties) where T : DynamicEntity
		{
			var tb = CreateDynamicTypeBuilder<T>(name, properties);
			return tb.CreateType();
		}

		/// <summary>
		/// This is the normal entry point and just return the Type generated at runtime
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="properties"></param>
		/// <returns></returns>
		public Type CreateDynamicTypeAction<T>(string name, Dictionary<string, DynamicMethodData> methods) where T : DynamicAction
		{
			var tb = CreateDynamicTypeActionBuilder<T>(name, methods);
			return tb.CreateType();
		}

		/// <summary>
		/// Exposes a TypeBuilder that can be returned and created outside of the class
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="properties"></param>
		/// <returns></returns>
		public TypeBuilder CreateDynamicTypeActionBuilder<T>(string name, Dictionary<string, DynamicMethodData> methods)
			where T : DynamicAction
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

			CreateMethods(_typeBuilder, methods);

			return _typeBuilder;
		}

		public void CreateMethods(TypeBuilder typeBuilder, Dictionary<string, DynamicMethodData> methods)
		{
			methods.ToList().ForEach(p => AddMethodDynamically(typeBuilder, p.Key, p.Value));
		}

		private void AddMethodDynamically(TypeBuilder typeBuilder, string methodName, DynamicMethodData dynamicMethodData)
		{
			MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodName,
												 MethodAttributes.Public |
												 MethodAttributes.Static,
												 dynamicMethodData.ReturnType,
												 dynamicMethodData.Params);

			ILGenerator ILout = methodBuilder.GetILGenerator();

			int numParams = dynamicMethodData.Params == null ? 0 : dynamicMethodData.Params.Length;

			for (int i = 0; i < numParams; i++)
			{
				methodBuilder.DefineParameter(i + 1, ParameterAttributes.None, dynamicMethodData.ParamNames[i].Replace("@", string.Empty));
			}

			for (byte x = 0; x < numParams; x++)
			{
				ILout.Emit(OpCodes.Ldarg_S, x);
			}

			string methodAction = string.Empty;

			if (numParams > 1)
			{
				for (int y = 0; y < (numParams - 1); y++)
				{
					switch (methodAction)
					{
						case "A": ILout.Emit(OpCodes.Add);
							break;
						case "M": ILout.Emit(OpCodes.Mul);
							break;
						default: ILout.Emit(OpCodes.Add);
							break;
					}
				}
			}
			ILout.Emit(OpCodes.Ret);

			AddActionAttribute(methodBuilder);
			AddFunctionAttribute(methodBuilder, methodName, dynamicMethodData);
		}

		private void AddActionAttribute(MethodBuilder methodBuilder)
		{
			Type attrType = typeof(ActionAttribute);
			var attr = new CustomAttributeBuilder(attrType.GetConstructor(Type.EmptyTypes), new object[] { });
			methodBuilder.SetCustomAttribute(attr);
		}

		private void AddFunctionAttribute(MethodBuilder methodBuilder, string methodName, DynamicMethodData dynamicMethodData)
		{
			//Type attrType = typeof(EntityFramework.Functions.ModelDefinedFunctionAttribute);
			//var attr = new CustomAttributeBuilder(attrType.GetConstructor(
			//	new[] { typeof(EntityFramework.Functions.FunctionType), typeof(string), typeof(string) }),
			//	new object[] { dynamicMethodData.FunctionType, methodName, dynamicMethodData.Schema });
			//methodBuilder.SetCustomAttribute(attr);

			Type attrType = typeof(System.Data.Entity.DbFunctionAttribute);
			var attr = new CustomAttributeBuilder(attrType.GetConstructor(
				new[] { typeof(string), typeof(string) }),
				new object[] { dynamicMethodData.Schema, methodName });
			methodBuilder.SetCustomAttribute(attr);
		}

		/// <summary>
		/// Exposes a TypeBuilder that can be returned and created outside of the class
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="properties"></param>
		/// <returns></returns>
		public TypeBuilder CreateDynamicTypeBuilder<T>(string name, Dictionary<string, DynamicPropertyData> properties)
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
			//AddDataContractAttribute();
			AddTableAttribute(name);
			//AddDataServiceKeyAttribute();

			//if there is a property on the base class and also in the dictionary, remove them from the dictionary
			var pis = typeof(T).GetProperties();
			foreach (var pi in pis)
			{
				properties.Remove(pi.Name);
			}

			//get the OnPropertyChanged method from the base class
			//var propertyChangedMethod = typeof(T).GetMethod("OnPropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic);

			CreateProperties(_typeBuilder, properties, null);

			return _typeBuilder;
		}

		public void AddAttribute(Type attrType)
		{
			_typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(attrType.GetConstructor(Type.EmptyTypes), new object[] { }));
		}

		public void AddTableAttribute(string name)
		{
			Type attrType = typeof(TableAttribute);
			_typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(attrType.GetConstructor(new[] { typeof(string) }),
				new object[] { name }));
		}

		public void CreateProperties(TypeBuilder typeBuilder, Dictionary<string, DynamicPropertyData> properties, MethodInfo raisePropertyChanged)
		{
			properties.ToList().ForEach(p => CreateFieldForType(p.Value, p.Key, raisePropertyChanged));
		}

		private void CreateFieldForType(DynamicPropertyData propData, string name, MethodInfo raisePropertyChanged)
		{
			string propertyName = name == _typeBuilder.Name ? name + "1" : name;

			FieldBuilder fieldBuilder = _typeBuilder.DefineField("_" + propertyName.ToLowerInvariant(), propData.Type, FieldAttributes.Private);

			PropertyBuilder propertyBuilder = _typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propData.Type, null);

			//add the various WCF and EF attributes to the property
			//AddDataMemberAttribute(propertyBuilder);
			if (propData.IsPrimaryKey || propData.IsForeignKey)
				AddColumnKeyAttribute(propertyBuilder, typeof(KeyAttribute));
			//if (propData.IsForeignKey)
			//	AddColumnKeyAttribute(propertyBuilder, typeof(ForeignKeyAttribute));

			if (propData.MaxLength.HasValue && propData.MaxLength.Value != -1)
				AddMaxLengthAttribute(propertyBuilder, propData.MaxLength.Value);

			//if(propData.Nullable)
			//	AddRequiredAttribute(propertyBuilder);

			AddColumnAttribute(propertyBuilder, name, propData.Order);

			MethodAttributes getterAndSetterAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;// | MethodAttributes.Virtual;

			//creates the Get Method for the property
			propertyBuilder.SetGetMethod(CreateGetMethod(getterAndSetterAttributes, propertyName, propData.Type, fieldBuilder));
			//creates the Set Method for the property and also adds the invocation of the property change
			propertyBuilder.SetSetMethod(CreateSetMethod(getterAndSetterAttributes, propertyName, propData.Type, fieldBuilder, raisePropertyChanged));
		}

		private void AddColumnKeyAttribute(PropertyBuilder propertyBuilder, Type attrType)
		{
			var attr = new CustomAttributeBuilder(attrType.GetConstructor(Type.EmptyTypes), new object[] { });
			propertyBuilder.SetCustomAttribute(attr);
		}

		private void AddColumnAttribute(PropertyBuilder propertyBuilder, string name, int order)
		{
			Type attrType = typeof(ColumnAttribute);
			var attr = new CustomAttributeBuilder(attrType.GetConstructor(
				new[] { typeof(string) }),
				new object[] { name },
				new PropertyInfo[] { attrType.GetProperty("Order") },
				new object[] { order });
			propertyBuilder.SetCustomAttribute(attr);
		}

		private void AddRequiredAttribute(PropertyBuilder propertyBuilder)
		{
			Type attrType = typeof(RequiredAttribute);
			var attr = new CustomAttributeBuilder(attrType.GetConstructor(Type.EmptyTypes),
				new object[]{},
				new PropertyInfo[] { attrType.GetProperty("AllowEmptyStrings") },
				new object[] { true });
			propertyBuilder.SetCustomAttribute(attr);
		}

		private void AddMaxLengthAttribute(PropertyBuilder propertyBuilder, int maxLength)
		{
			Type attrType = typeof(MaxLengthAttribute);
			var attr = new CustomAttributeBuilder(attrType.GetConstructor(
				new[] { typeof(int) }),
				new object[] { maxLength });
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
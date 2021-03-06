﻿using Microsoft.Restier.Core.Model;
using ODataRestierDynamic.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ODataRestierDynamic.DynamicFactory
{
    /// <summary>	A dynamic class factory. </summary>
    public class DynamicClassFactory
	{
		/// <summary>	The application domain. </summary>
		private AppDomain _appDomain;
		/// <summary>	The assembly builder. </summary>
		private AssemblyBuilder _assemblyBuilder;
		/// <summary>	The module builder. </summary>
		private ModuleBuilder _moduleBuilder;
		/// <summary>	Name of the assembly. </summary>
		private readonly string _assemblyName;

		/// <summary>	The default namespace. </summary>
		public const string cDefaultNamespace = "ODataRestierDynamic.Models";

		/// <summary>	Default constructor. </summary>
		public DynamicClassFactory()
			: this(cDefaultNamespace)
		{
		}

		/// <summary>	Constructor. </summary>
		///
		/// <param name="assemblyName">	Name of the assembly. </param>
		public DynamicClassFactory(string assemblyName)
		{
			_appDomain = System.Threading.Thread.GetDomain();
			_assemblyName = assemblyName;
		}

		/// <summary>
		/// This is the normal entry point and just return the Type generated at runtime.
		/// </summary>
		///
		/// <typeparam name="T">	. </typeparam>
		/// <param name="name">		 	. </param>
		/// <param name="properties">	. </param>
		///
		/// <returns>	The new dynamic type. </returns>
		public Type CreateDynamicType<T>(string schema, string name, Dictionary<string, DynamicPropertyData> properties) where T : DynamicEntity
		{
			var tb = CreateDynamicTypeBuilder<T>(schema, name, properties);
			return tb.CreateType();
		}

		/// <summary>
		/// This is the normal entry point and just return the Type generated at runtime.
		/// </summary>
		///
		/// <param name="name">   	. </param>
		/// <param name="methods">	The methods. </param>
		///
		/// <returns>	The new dynamic type action. </returns>
		public Type CreateDynamicTypeAction<T>(string name, Dictionary<string, DynamicMethodData> methods) where T : DynamicAction
		{
			var tb = CreateDynamicTypeActionBuilder<T>(name, methods);
			return tb.CreateType();
		}

		/// <summary>
		/// Exposes a TypeBuilder that can be returned and created outside of the class.
		/// </summary>
		///
		/// <param name="name">   	. </param>
		/// <param name="methods">	The methods. </param>
		///
		/// <returns>	The new dynamic type action builder. </returns>
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
			var typeBuilder = _moduleBuilder.DefineType(_assemblyName + "." + name, TypeAttributes.Public
															| TypeAttributes.Class
															| TypeAttributes.AutoClass
															| TypeAttributes.AnsiClass
															| TypeAttributes.Serializable
															| TypeAttributes.BeforeFieldInit, typeof(T));

			CreateMethods(typeBuilder, methods);

			return typeBuilder;
		}

		/// <summary>	Creates the methods. </summary>
		///
		/// <param name="typeBuilder">	The type builder. </param>
		/// <param name="methods">	  	The methods. </param>
		public void CreateMethods(TypeBuilder typeBuilder, Dictionary<string, DynamicMethodData> methods)
		{
			methods.ToList().ForEach(p => AddMethodDynamically(typeBuilder, p.Key, p.Value));
		}

		/// <summary>	Adds a method dynamically. </summary>
		///
		/// <param name="typeBuilder">			The type builder. </param>
		/// <param name="methodName">			Name of the method. </param>
		/// <param name="dynamicMethodData">	Information describing the dynamic method. </param>
		private void AddMethodDynamically(TypeBuilder typeBuilder, string methodName, DynamicMethodData dynamicMethodData)
		{
			Type[] paramTypes = (dynamicMethodData.Params == null || dynamicMethodData.Params.Length == 0) ? null : dynamicMethodData.Params.Select(t => t.Type).ToArray();
			MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodName,
												 MethodAttributes.Public |
												 MethodAttributes.Static,
												 dynamicMethodData.ReturnType,
												 paramTypes);

			ILGenerator ILout = methodBuilder.GetILGenerator();

			int numParams = dynamicMethodData.Params == null ? 0 : dynamicMethodData.Params.Length;

			for (int i = 0; i < numParams; i++)
			{
				var paramAttribute = ParameterAttributes.None;
				if (dynamicMethodData.Params[i].isIn)
					paramAttribute |= ParameterAttributes.In;
				if (dynamicMethodData.Params[i].isOut)
					paramAttribute |= ParameterAttributes.Out;

				methodBuilder.DefineParameter(i + 1, paramAttribute, dynamicMethodData.Params[i].Name.Replace("@", string.Empty));
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

		/// <summary>	Adds an action attribute. </summary>
		///
		/// <param name="methodBuilder">	The method builder. </param>
		private void AddActionAttribute(MethodBuilder methodBuilder)
		{
			Type attrType = typeof(ActionAttribute);
			var attr = new CustomAttributeBuilder(attrType.GetConstructor(Type.EmptyTypes), new object[] { });
			methodBuilder.SetCustomAttribute(attr);
		}

		/// <summary>	Adds a function attribute. </summary>
		///
		/// <param name="methodBuilder">		The method builder. </param>
		/// <param name="methodName">			Name of the method. </param>
		/// <param name="dynamicMethodData">	Information describing the dynamic method. </param>
		private void AddFunctionAttribute(MethodBuilder methodBuilder, string methodName, DynamicMethodData dynamicMethodData)
		{
			Type attrType = typeof(System.Data.Entity.DbFunctionAttribute);
			var attr = new CustomAttributeBuilder(attrType.GetConstructor(
				new[] { typeof(string), typeof(string) }),
				new object[] { dynamicMethodData.Schema, methodName });
			methodBuilder.SetCustomAttribute(attr);
		}

		/// <summary>
		/// Exposes a TypeBuilder that can be returned and created outside of the class.
		/// </summary>
		///
		/// <param name="name">		 	. </param>
		/// <param name="properties">	. </param>
		///
		/// <returns>	The new dynamic type builder. </returns>
		public TypeBuilder CreateDynamicTypeBuilder<T>(string schema, string name, Dictionary<string, DynamicPropertyData> properties)
			where T : DynamicEntity
		{
			if (_assemblyBuilder == null)
				_assemblyBuilder = _appDomain.DefineDynamicAssembly(new AssemblyName(_assemblyName),
					AssemblyBuilderAccess.RunAndSave);
			//vital to ensure the namespace of the assembly is the same as the module name, else IL inspectors will fail
			if (_moduleBuilder == null)
				_moduleBuilder = _assemblyBuilder.DefineDynamicModule(_assemblyName + ".dll");

			//typeof(T) is for the base class, can be omitted if not needed
			var typeBuilder = _moduleBuilder.DefineType(_assemblyName + "." + name, TypeAttributes.Public
															| TypeAttributes.Class
															| TypeAttributes.AutoClass
															| TypeAttributes.AnsiClass
															| TypeAttributes.Serializable
															| TypeAttributes.BeforeFieldInit, typeof(T));

			AddTableAttribute(typeBuilder, schema, name);

			if (properties != null && properties.Count > 0)
			{
				//if there is a property on the base class and also in the dictionary, remove them from the dictionary
				var pis = typeof(T).GetProperties();
				foreach (var pi in pis)
				{
					properties.Remove(pi.Name);
				}

				CreateProperties(typeBuilder, properties, null);
			}

			return typeBuilder;
		}

		/// <summary>	Adds an attribute. </summary>
		///
		/// <param name="attrType">	Type of the attribute. </param>
		public void AddAttribute(TypeBuilder typeBuilder, Type attrType)
		{
			typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(attrType.GetConstructor(Type.EmptyTypes), new object[] { }));
		}

		/// <summary>	Adds a table attribute. </summary>
		///
		/// <param name="name">	. </param>
		public void AddTableAttribute(TypeBuilder typeBuilder, string schema, string name)
		{
			Type attrType = typeof(TableAttribute);
			typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(attrType.GetConstructor(
				new[] { typeof(string) }),
				new object[] { name },
				new PropertyInfo[] { attrType.GetProperty("Schema") },
				new object[] { schema }));
		}

		/// <summary>	Creates the properties. </summary>
		///
		/// <param name="typeBuilder">		   	The type builder. </param>
		/// <param name="properties">		   	. </param>
		/// <param name="raisePropertyChanged">	The raise property changed. </param>
		public void CreateProperties(TypeBuilder typeBuilder, Dictionary<string, DynamicPropertyData> properties, MethodInfo raisePropertyChanged)
		{
			properties.ToList().ForEach(p => CreateFieldForType(typeBuilder, p.Value, p.Key, raisePropertyChanged));
		}

		/// <summary>	Creates the properties. </summary>
		///
		/// <param name="typeBuilder">		   	The type builder. </param>
		/// <param name="properties">		   	. </param>
		/// <param name="raisePropertyChanged">	The raise property changed. </param>
		public void CreateProperty(TypeBuilder typeBuilder, KeyValuePair<string, DynamicPropertyData> property, MethodInfo raisePropertyChanged)
		{
			CreateFieldForType(typeBuilder, property.Value, property.Key, raisePropertyChanged);
		}

		/// <summary>	Creates field for type. </summary>
		///
		/// <param name="propData">			   	Information describing the property. </param>
		/// <param name="name">				   	. </param>
		/// <param name="raisePropertyChanged">	The raise property changed. </param>
		private void CreateFieldForType(TypeBuilder typeBuilder, DynamicPropertyData propData, string name, MethodInfo raisePropertyChanged)
		{
			FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + name.ToLowerInvariant(), propData.Type, FieldAttributes.Private);

			PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.HasDefault, propData.Type, null);

			if (propData is FieldPropertyData)
			{
				var fieldPropData = propData as FieldPropertyData;

				if (fieldPropData.IsPrimaryKey)
					AddColumnKeyAttribute(propertyBuilder, typeof(KeyAttribute));

				if (fieldPropData.MaxLength.HasValue && fieldPropData.MaxLength.Value != -1)
					AddMaxLengthAttribute(propertyBuilder, fieldPropData.MaxLength.Value);

				if (fieldPropData.IsComputedID)
					AddDatabaseGeneratedAttribute(propertyBuilder, DatabaseGeneratedOption.None);

				if (!string.IsNullOrEmpty(fieldPropData.SequenceScript))
					AddSequenceAttribute(propertyBuilder, fieldPropData.SequenceScript);

				AddColumnAttribute(propertyBuilder, fieldPropData.ColumnName, fieldPropData.Order);
			}
			else if (propData is ForeignKeyPropertyData)
			{
				var foreignKeyPropData = propData as ForeignKeyPropertyData;
				AddColumnForeignKeyAttribute(propertyBuilder, foreignKeyPropData.ColumnName);
			}
			else if (propData is InversePropertyData)
			{
				var inversePropData = propData as InversePropertyData;
				AddColumnInversePropertyAttribute(propertyBuilder, inversePropData.ColumnName);
			}

			MethodAttributes getterAndSetterAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;// | MethodAttributes.Virtual;

			//creates the Get Method for the property
			propertyBuilder.SetGetMethod(CreateGetMethod(typeBuilder, getterAndSetterAttributes, name, propData.Type, fieldBuilder));
			//creates the Set Method for the property and also adds the invocation of the property change
			propertyBuilder.SetSetMethod(CreateSetMethod(typeBuilder, getterAndSetterAttributes, name, propData.Type, fieldBuilder, raisePropertyChanged));
		}

		/// <summary>	Adds a column key attribute to 'attrType'. </summary>
		///
		/// <param name="propertyBuilder">	The property builder. </param>
		/// <param name="attrType">		  	Type of the attribute. </param>
		private void AddColumnKeyAttribute(PropertyBuilder propertyBuilder, Type attrType)
		{
			var attr = new CustomAttributeBuilder(attrType.GetConstructor(Type.EmptyTypes), new object[] { });
			propertyBuilder.SetCustomAttribute(attr);
		}

		/// <summary>	Adds a column foreign key attribute to 'colName'. </summary>
		///
		/// <param name="propertyBuilder">	The property builder. </param>
		/// <param name="colName">		  	Name of the col. </param>
		private void AddColumnForeignKeyAttribute(PropertyBuilder propertyBuilder, string colName)
		{
			Type attrType = typeof(ForeignKeyAttribute);
			var attr = new CustomAttributeBuilder(attrType.GetConstructor(
				new[] { typeof(string) }),
				new object[] { colName },
				new PropertyInfo[] { },
				new object[] { });
			propertyBuilder.SetCustomAttribute(attr);
		}

		/// <summary>	Adds a column inverse property attribute to 'propName'. </summary>
		///
		/// <param name="propertyBuilder">	The property builder. </param>
		/// <param name="propName">		  	Name of the property. </param>
		private void AddColumnInversePropertyAttribute(PropertyBuilder propertyBuilder, string propName)
		{
			Type attrType = typeof(InversePropertyAttribute);
			var attr = new CustomAttributeBuilder(attrType.GetConstructor(
				new[] { typeof(string) }),
				new object[] { propName },
				new PropertyInfo[] { },
				new object[] { });
			propertyBuilder.SetCustomAttribute(attr);
		}

		/// <summary>	Adds a database generated attribute to 'option'. </summary>
		///
		/// <param name="propertyBuilder">	The property builder. </param>
		/// <param name="option">		  	The option. </param>
		private void AddDatabaseGeneratedAttribute(PropertyBuilder propertyBuilder, DatabaseGeneratedOption option)
		{
			Type attrType = typeof(DatabaseGeneratedAttribute);
			var attr = new CustomAttributeBuilder(attrType.GetConstructor(
				new[] { typeof(DatabaseGeneratedOption) }),
				new object[] { option },
				new PropertyInfo[] { },
				new object[] { });
			propertyBuilder.SetCustomAttribute(attr);
		}

		/// <summary>	Adds a sequence attribute to 'script'. </summary>
		///
		/// <param name="propertyBuilder">	The property builder. </param>
		/// <param name="script">		  	The script. </param>
		private void AddSequenceAttribute(PropertyBuilder propertyBuilder, string script)
		{
			Type attrType = typeof(SequenceAttribute);
			var attr = new CustomAttributeBuilder(attrType.GetConstructor(
				new[] { typeof(string) }),
				new object[] { script },
				new PropertyInfo[] { },
				new object[] { });
			propertyBuilder.SetCustomAttribute(attr);
		}

		/// <summary>	Adds a column attribute. </summary>
		///
		/// <param name="propertyBuilder">	The property builder. </param>
		/// <param name="name">			  	. </param>
		/// <param name="order">		  	The order. </param>
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

		/// <summary>	Adds a required attribute. </summary>
		///
		/// <param name="propertyBuilder">	The property builder. </param>
		private void AddRequiredAttribute(PropertyBuilder propertyBuilder)
		{
			Type attrType = typeof(RequiredAttribute);
			var attr = new CustomAttributeBuilder(attrType.GetConstructor(Type.EmptyTypes),
				new object[] { },
				new PropertyInfo[] { attrType.GetProperty("AllowEmptyStrings") },
				new object[] { true });
			propertyBuilder.SetCustomAttribute(attr);
		}

		/// <summary>	Adds a maximum length attribute to 'maxLength'. </summary>
		///
		/// <param name="propertyBuilder">	The property builder. </param>
		/// <param name="maxLength">	  	The maximum length. </param>
		private void AddMaxLengthAttribute(PropertyBuilder propertyBuilder, int maxLength)
		{
			Type attrType = typeof(MaxLengthAttribute);
			var attr = new CustomAttributeBuilder(attrType.GetConstructor(
				new[] { typeof(int) }),
				new object[] { maxLength });
			propertyBuilder.SetCustomAttribute(attr);
		}

		/// <summary>	Creates get method. </summary>
		///
		/// <param name="attr">		   	The attribute. </param>
		/// <param name="name">		   	. </param>
		/// <param name="type">		   	The type. </param>
		/// <param name="fieldBuilder">	The field builder. </param>
		///
		/// <returns>	The new get method. </returns>
		private MethodBuilder CreateGetMethod(TypeBuilder typeBuilder, MethodAttributes attr, string name, Type type, FieldBuilder fieldBuilder)
		{
			var getMethodBuilder = typeBuilder.DefineMethod("get_" + name, attr, type, Type.EmptyTypes);

			var getMethodILGenerator = getMethodBuilder.GetILGenerator();
			getMethodILGenerator.Emit(OpCodes.Ldarg_0);
			getMethodILGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
			getMethodILGenerator.Emit(OpCodes.Ret);

			return getMethodBuilder;
		}

		/// <summary>	Creates set method. </summary>
		///
		/// <param name="attr">				   	The attribute. </param>
		/// <param name="name">				   	. </param>
		/// <param name="type">				   	The type. </param>
		/// <param name="fieldBuilder">		   	The field builder. </param>
		/// <param name="raisePropertyChanged">	The raise property changed. </param>
		///
		/// <returns>	The new set method. </returns>
		private MethodBuilder CreateSetMethod(TypeBuilder typeBuilder, MethodAttributes attr, string name, Type type, FieldBuilder fieldBuilder, MethodInfo raisePropertyChanged)
		{
			var setMethodBuilder = typeBuilder.DefineMethod("set_" + name, attr, null, new Type[] { type });

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

		/// <summary>	Saves the assembly. </summary>
		public void SaveAssembly()
		{
			_assemblyBuilder.Save(_assemblyBuilder.GetName().Name + ".dll");
		}
	}
}
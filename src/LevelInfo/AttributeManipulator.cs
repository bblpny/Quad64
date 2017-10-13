
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace Quad64.Internal
{
	internal sealed class TypeManipulator
	{
		public readonly System.Type Observed;
		public readonly PropertyDescriptorCollection Properties;
		public readonly AttributeManipulator DisplayNameField, DescriptionField, BrowsableField;

		private readonly Dictionary<string, PropertyManipulator> dict = new Dictionary<string, PropertyManipulator>();
		private object dict_lock = new object();


		public PropertyManipulator this[string property]
		{
			get
			{
				PropertyManipulator found;
				{
					bool did_find;
					// don't want to do anything but lookup or define when the thread lock is acquired.
					// who knows what the constructor will awake.
					lock (dict_lock)
						did_find = dict.TryGetValue(property, out found);

					if (!did_find)
					{
						// make the lookup, and bind it if there wasn't a challenge previously entered
						// whilst we left the lock unlocked.
						PropertyManipulator made = new PropertyManipulator(this, property);

						lock (dict_lock)
							if (!dict.TryGetValue(property, out found))
								dict[property] = (found = made);
					}
				}
				return found;
			}
		}

		public TypeManipulator(System.Type Observed)
		{
			this.Observed = Observed;
			this.Properties = TypeDescriptor.GetProperties(Observed);
			this.DisplayNameField = AttributeManipulator.For<DisplayNameAttribute>(this, "_displayName");
			this.DescriptionField = AttributeManipulator.For<DescriptionAttribute>(this, "description");
			this.BrowsableField = AttributeManipulator.For<BrowsableAttribute>(this,"browsable");
		}
		public void HideShowProperty(PropertyManipulator property, bool show)
		{
			if (property.Manipulator != this) throw new System.ArgumentException("Not made by this manipulator!", "property");

			if (!property.SetBrowsable(show))
				Console.Error.WriteLine(
					string.Format(show ? "Failed to show property \"{0}\"" : "Failed to hide property \"{0}\"", PropLookupString(property)));
		}
		public void HideShowProperty(string property, bool show)
		{
			HideShowProperty(this[property], show);
		}
		public static string PropLookupString(PropertyManipulator lookup)
		{
			//if (lookup.Manipulator != this) throw new System.ArgumentException("Not made by this manipulator!", "property");

			return null == (object)lookup ? "NULL" : lookup.PropertyName ?? "NULL";
		}
		public void ChangePropertyName(PropertyManipulator property, string name)
		{
			if (property.Manipulator != this) throw new System.ArgumentException("Not made by this manipulator!", "property");

			if (!property.SetDisplayName(name))
				Console.Error.WriteLine(
					string.Format("Failed to set property \"{0}\" display name (\"{1}\")",
					PropLookupString(property), name ?? "NULL"));
		}

		public void ChangePropertyName(string property, string name)
		{
			ChangePropertyName(this[property], name);
		}
		public void ChangePropertyDescription(PropertyManipulator property, string description)
		{
			if (property.Manipulator != this) throw new System.ArgumentException("Not made by this manipulator!", "property");

			if (!property.SetDescription(description))
				Console.Error.WriteLine(
					string.Format("Failed to set property \"{0}\" description (\"{1}\")", PropLookupString(property), description ?? "NULL"));
		}
		public void ChangePropertyDescription(string property, string description)
		{
			ChangePropertyName(this[property], description);
		}

		public void UpdatePropertyName(PropertyManipulator property, string oce_name, string otherName)
		{
			if (property.Manipulator != this) throw new System.ArgumentException("Not made by this manipulator!", "property");

			if ((object)oce_name != null && 0!=oce_name.Length)
				ChangePropertyName(property, oce_name);
			else
				ChangePropertyName(property, otherName);
		}
		public void UpdatePropertyName(string property, string oce_name, string otherName)
		{
			UpdatePropertyName(this[property], oce_name, otherName);
		}

		public void UpdatePropertyDescription(PropertyManipulator property, string oce_desc)
		{
			if (property.Manipulator != this) throw new System.ArgumentException("Not made by this manipulator!", "property");

			if ((object)oce_desc != null && 0 != oce_desc.Length)
				ChangePropertyDescription(property, oce_desc);
			else
				ChangePropertyDescription(property, "");
		}
		public void UpdatePropertyDescription(string property, string oce_desc)
		{
			UpdatePropertyDescription(this[property], oce_desc);
		}
	}
	internal sealed class PropertyManipulator
	{
		public readonly TypeManipulator Manipulator;
		public readonly string PropertyName;
		public readonly PropertyDescriptor PropertyDescriptor;
		public readonly DisplayNameAttribute DisplayName;
		public readonly BrowsableAttribute Browsable;
		public readonly DescriptionAttribute Description;
		private PropertyManipulator ReadonlyPropLookup;
		public PropertyManipulator GetReadonlyPropLookup()
		{
			return ReadonlyPropLookup ?? Interlocked.CompareExchange(
				ref ReadonlyPropLookup, PropertyName.EndsWith("_ReadOnly") ? this : 
				Manipulator[PropertyName + "_ReadOnly"], null) ?? ReadonlyPropLookup;
		}
		
		public bool SetDisplayName(string value)
		{
			return Manipulator.DisplayNameField.Set(DisplayName, value);
		}
		public bool SetDescription(string value)
		{
			return Manipulator.DescriptionField.Set(Description, value);
		}
		public bool SetBrowsable(bool value)
		{
			return Manipulator.BrowsableField.Set(Browsable, value);
		}
		
		internal PropertyManipulator(TypeManipulator Manipulator, string property)
		{
			this.Manipulator = Manipulator;
			PropertyDescriptor = Manipulator.Properties[(this.PropertyName = property)];
			Manipulator.DisplayNameField.Get(PropertyDescriptor, out DisplayName);
			Manipulator.BrowsableField.Get(PropertyDescriptor, out Browsable);
			Manipulator.DescriptionField.Get(PropertyDescriptor, out Description);
		}

	}
	internal sealed class AttributeManipulator
	{
		public readonly TypeManipulator Manipulator;
		public readonly Type Type;
		public readonly FieldInfo Field;
		public readonly bool Exists;

		public static AttributeManipulator For<T>(TypeManipulator Manipulator, string FieldName, BindingFlags BindingFlags = BindingFlags.NonPublic | BindingFlags.Instance)
			where T : Attribute
		{
			return new AttributeManipulator(Manipulator, typeof(T), FieldName, BindingFlags);
		}
		private AttributeManipulator(TypeManipulator Manipulator, Type Type, string FieldName, BindingFlags BindingFlags)
		{
			this.Manipulator = Manipulator;
			Exists = null != (object)(this.Field = (this.Type = Type).GetField(FieldName, BindingFlags));
		}
		public bool Set(Attribute attribute, object Value)
		{
			if (Exists && null != attribute)
			{
				Field.SetValue(attribute, Value);
				return true;
			}
			return false;
		}
		public bool Set(Attribute attribute, string Value) { return Set(attribute, (object)Value); }
		private static readonly object TRUE = true, FALSE = false;
		public bool Set(System.Attribute attribute, bool Value) { return Set(attribute, Value ? TRUE : FALSE); }

		public void Get<T>(PropertyDescriptor Instance, out T Member) where T : Attribute
		{
			if (!typeof(T).IsAssignableFrom(Type)) throw new System.InvalidOperationException();
			Attribute Found;
			try
			{
				Found = Instance.Attributes[Type];
			}
			catch (OutOfMemoryException) { throw; }
			catch (ThreadAbortException) { throw; }
			catch (ThreadInterruptedException) { throw; }
			catch (Exception e)
			{
				Found = null;
				System.Console.Error.WriteLine(e);
			}
			Member = (T)Found;
		}
	}
}

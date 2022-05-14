using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace WpfCore.ViewModel;

/// <summary>
/// Represents a view model type (from MVVM pattern), and provides helper methods while implementing <see cref="INotifyPropertyChanged"/>.
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public static readonly string ALL_PROPS_CHANGED = string.Empty;

	public void Set<T>(ref T property,
					T value,
					[CallerMemberName] string? propertyName = null)
	{
		if (!EqualityComparer<T>.Default.Equals(property, value))
		{
			property = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public void SetBacking<TBacking, TProperty>(TBacking backingObject,
											 Expression<Func<TBacking, TProperty>> propertyPath,
											 TProperty value,
											 [CallerMemberName] string? propertyName = null)
	{
		MemberExpression expr = (MemberExpression)propertyPath.Body;
		PropertyInfo prop = (PropertyInfo)expr.Member;

		TProperty? propValue = (TProperty?)prop.GetValue(backingObject);
		if (!EqualityComparer<TProperty>.Default.Equals(propValue, value))
		{
			prop.SetValue(backingObject, value, null);
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}

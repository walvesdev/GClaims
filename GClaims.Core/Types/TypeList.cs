using System.Collections;
using System.Reflection;
using GClaims.Core.Filters.CustomExceptions;

namespace GClaims.Core.Types;

/// <summary>
/// Estende <see cref="T:System.Collections.Generic.List`1" /> para adicionar restrição a um tipo base específico.
/// </summary>
/// <typeparam name="TBaseType">Tipo base de <see cref="T:System.Type" />s nesta lista</typeparam>
public class TypeList<TBaseType> : ITypeList<TBaseType>
{
    private readonly List<Type> _typeList;

    /// <summary>
    /// Cria um novo objeto TypeList" />.
    /// </summary>
    public TypeList()
    {
        _typeList = new List<Type>();
    }

    /// <summary>
    /// Obtém a contagem.
    /// </summary>
    /// <value>A contagem.</value>
    public int Count => _typeList.Count;

    /// <summary>
    /// Obtém um valor que indica se esta instância é somente leitura.
    /// </summary>
    /// <value><c>true</c> se esta instância for somente leitura; caso contrário, <c>falso</c>.</value>
    public bool IsReadOnly => false;

    /// <summary>
    /// Obtém ou define o <see cref="T:System.Type" /> no índice especificado.
    /// </summary>
    /// <param name="index">Index.</param>
    public Type this[int index]
    {
        get => _typeList[index];
        set
        {
            CheckType(value);
            _typeList[index] = value;
        }
    }

    /// <inheritdoc />
    public void Add<T>() where T : TBaseType
    {
        _typeList.Add(typeof(T));
    }

    public bool TryAdd<T>() where T : TBaseType
    {
        if (Contains<T>())
        {
            return false;
        }

        Add<T>();
        return true;
    }

    /// <inheritdoc />
    public void Add(Type item)
    {
        CheckType(item);
        _typeList.Add(item);
    }

    /// <inheritdoc />
    public void Insert(int index, Type item)
    {
        CheckType(item);
        _typeList.Insert(index, item);
    }

    /// <inheritdoc />
    public int IndexOf(Type item)
    {
        return _typeList.IndexOf(item);
    }

    /// <inheritdoc />
    public bool Contains<T>() where T : TBaseType
    {
        return Contains(typeof(T));
    }

    /// <inheritdoc />
    public bool Contains(Type item)
    {
        return _typeList.Contains(item);
    }

    /// <inheritdoc />
    public void Remove<T>() where T : TBaseType
    {
        _typeList.Remove(typeof(T));
    }

    /// <inheritdoc />
    public bool Remove(Type item)
    {
        return _typeList.Remove(item);
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        _typeList.RemoveAt(index);
    }

    /// <inheritdoc />
    public void Clear()
    {
        _typeList.Clear();
    }

    /// <inheritdoc />
    public void CopyTo(Type[] array, int arrayIndex)
    {
        _typeList.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    public IEnumerator<Type> GetEnumerator()
    {
        return _typeList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _typeList.GetEnumerator();
    }

    private static void CheckType(Type item)
    {
        if (!typeof(TBaseType).GetTypeInfo().IsAssignableFrom(item))
        {
            throw new CustomException("Given type (" + item.AssemblyQualifiedName + ") should be instance of " +
                                      typeof(TBaseType).AssemblyQualifiedName + " " + "item");
        }
    }
}
namespace com.etsoo.Utils
{
    /// <summary>
    /// Enumeration
    /// 枚举
    /// https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types
    /// https://lostechies.com/jimmybogard/2008/08/12/enumeration-classes/
    /// </summary>
    /// <typeparam name="T">Id generic type</typeparam>
    public abstract class Enumeration<T> : IComparable where T : IComparable
    {
        public static readonly List<Enumeration<T>> Items = new();

        /// <summary>
        /// Value
        /// 值
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Name
        /// 名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="name">Name</param>
        protected Enumeration(T value, string name)
        {
            Value = value;
            Name = name;

            Items.Add(this);
        }

        /// <summary>
        /// Compare
        /// 比较
        /// </summary>
        /// <param name="obj">Target obj</param>
        /// <returns>Result</returns>
        public int CompareTo(object? obj)
        {
            if (obj == null) return 0;
            return Value.CompareTo(((Enumeration<T>)obj).Value);
        }

        /// <summary>
        /// Override Equal
        /// 重写 Equal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj is not Enumeration<T> e)
            {
                return false;
            }

            // Value and type the same
            return Value.Equals(e.Value) && GetType().Equals(obj.GetType());
        }

        /// <summary>
        /// Override GetHashCode
        /// 重写 GetHashCode
        /// </summary>
        /// <returns>Result</returns>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Override ToString to return name
        /// 重写 ToString 返回名称
        /// </summary>
        /// <returns>Result</returns>
        public override string ToString() => Name;
    }
}

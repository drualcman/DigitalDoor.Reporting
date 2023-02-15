using DigitalDoor.Reporting.Entities.Helpers;
using System.Linq.Expressions;

namespace DigitalDoor.Reporting.Entities.ValueObjects
{
    public class Item : IEquatable<Item>
    {
        public string ObjectName { get { return ObjectNameBK; } set { ObjectNameBK = value; } }
        private string ObjectNameBK;
        public string PropertyName { get { return PropertyNameBK; } set { PropertyNameBK = value; } }
        private string PropertyNameBK;

        public static Item SetItem<T>(Expression<Func<T, object>> property) where T : new() =>
            GetItem<T>(property);

        public static Item GetItem<T>(Expression property)
        {
            Item item = new Item();
            Type type = typeof(T);
            item.ObjectName = type.Name;
            try
            {
                MemberExpression member = ExpressionsTools.GetMemberInfo(property);
                item.PropertyName = member.Member.Name;
            }
            catch(Exception ex)
            {
                item.PropertyName = ex.Message;
            }
            return item;
        }


        public Item()
        {
            ObjectNameBK = "FreeText";
            PropertyNameBK = "";
        }
        public Item(string propertyName) : this("FreeText", propertyName) { }
        public Item(string objectName, string propertyName)
        {
            ObjectNameBK = objectName;
            PropertyNameBK = propertyName;
        }

        public bool Equals(Item other)
        {
            return ObjectNameBK == other.ObjectName && PropertyNameBK == other.PropertyName;
        }

    }
}

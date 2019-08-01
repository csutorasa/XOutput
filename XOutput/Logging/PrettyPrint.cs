using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace XOutput.Logging
{
    public static class PrettyPrint
    {
        public static string ToString(object obj)
        {
            StringBuilder sb = new StringBuilder();

            if (obj == null)
            {
                sb.AppendLine("null");
            }
            else if (obj is IEnumerable && !(obj is string))
            {
                sb.AppendLine("[");
                foreach (var v in (IEnumerable)obj)
                {
                    ToString(v, sb, "  ");
                    sb.AppendLine("  ,");
                }
                sb.AppendLine("]");
            }
            else if (obj.GetType().IsClass && !(obj is string))
            {
                sb.AppendLine("{");
                ToString(obj, sb, "");
                sb.AppendLine("}");
            }
            else
            {
                sb.AppendLine(obj.ToString());
            }
            return sb.ToString().Trim();
        }

        private static void ToString(object obj, StringBuilder sb, string intend)
        {
            foreach (var property in obj.GetType().GetProperties().Where(p => p.CanRead))
            {
                object value;
                try
                {
                    value = property.GetValue(obj, null);
                }
                catch (Exception e)
                {
                    value = e.Message;
                }
                if (value == null)
                {
                    sb.AppendLine(intend + property.Name + ": null");
                }
                else if (value is IEnumerable && !(value is string))
                {
                    sb.AppendLine(intend + property.Name + ": [");
                    foreach (var v in (IEnumerable)value)
                    {
                        ToString(v, sb, intend + "  ");
                        sb.AppendLine(intend + ",");
                    }
                    sb.AppendLine("]");
                }
                else if (value.GetType().IsClass && !(value is string))
                {
                    sb.AppendLine(intend + property.Name + ": {");
                    ToString(value, sb, intend + "  ");
                    sb.AppendLine(intend + "}");
                }
                else
                {
                    sb.AppendLine(intend + property.Name + ": " + value);
                }
            }
            foreach (var field in obj.GetType().GetFields().Where(f => f.IsPublic))
            {
                object value;
                try
                {
                    value = field.GetValue(obj);
                }
                catch (Exception e)
                {
                    value = e.Message;
                }
                if (value == null)
                {
                    sb.AppendLine(intend + field.Name + ": null");
                }
                else if (value is IEnumerable && !(value is string))
                {
                    sb.AppendLine(intend + field.Name + ": [");
                    foreach (var v in (IEnumerable)value)
                    {
                        ToString(v, sb, intend + "  ");
                        sb.AppendLine(intend + ",");
                    }
                    sb.AppendLine("]");
                }
                else if (value.GetType().IsClass && !(value is string))
                {
                    sb.AppendLine(intend + field.Name + ": {");
                    ToString(value, sb, intend + "  ");
                    sb.AppendLine(intend + "}");
                }
                else
                {
                    sb.AppendLine(intend + field.Name + ": " + value);
                }
            }
        }
    }
}
